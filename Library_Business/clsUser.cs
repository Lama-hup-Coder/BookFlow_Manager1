using System;
using System.Data;
using Library_DataAccess;
using Library_Common; // استدعاء نظام التوثيق المشترك

namespace Library_Business
{
    public class clsUser
    {
        // مفتاح خاص لتشفير بيانات الريجستري
        private static string _RegistryEncryptionKey = "MyRegistryKey@123";

        // --- [ معالجة الأيفنت والديليجيت ] ---
        public static event Action<string> OnError;

        static clsUser()
        {
            // ربط إيفنت الداتا لير بالبيزنس لير وتوثيق أخطاء الداتا لير
            clsUserData.OnError += (errorMessage) =>
            {
                EventLogger.Log($"Data Layer Error in clsUser: {errorMessage}", "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke(errorMessage);
            };
        }

        // --- [ الخصائص والحالات ] ---
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int UserID { get; set; }
        public int PersonID { get; set; }
        public clsPerson PersonInfo { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }

        // --- [ المشيدات ] ---
        public clsUser()
        {
            this.UserID = -1;
            this.PersonID = -1;
            this.UserName = "";
            this.Password = "";
            this.IsActive = true;
            Mode = enMode.AddNew;
        }

        private clsUser(int UserID, int PersonID, string UserName, string Password, bool IsActive)
        {
            this.UserID = UserID;
            this.PersonID = PersonID;
            this.PersonInfo = clsPerson.Find(PersonID);
            this.UserName = UserName;
            this.Password = Password;
            this.IsActive = IsActive;
            Mode = enMode.Update;
        }

        // --- [ الدوال والمطابقة ] ---
        public static clsUser FindByUserID(int UserID)
        {
            try
            {
                int PersonID = -1;
                string UserName = "", Password = "";
                bool IsActive = false;

                if (clsUserData.GetUserInfoByID(UserID, ref PersonID, ref UserName, ref Password, ref IsActive))
                {
                    return new clsUser(UserID, PersonID, UserName, Password, IsActive);
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Error Finding User (ID: {UserID}): {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
            }
            return null;
        }

        public static clsUser FindByUsernameAndPassword(string UserName, string Password)
        {
            try
            {
                int UserID = -1, PersonID = -1;
                bool IsActive = false;
                string PasswordHash = clsSecurityUtils.ComputeHash(Password);

                if (clsUserData.GetUserInfoByUsernameAndPassword(UserName, PasswordHash, ref UserID, ref PersonID, ref IsActive))
                {
                    // تسجيل محاولة دخول ناجحة
                    EventLogger.Log($"User Logged In: {UserName}", "INFO", EventLogger.LogTarget.TextFile);
                    return new clsUser(UserID, PersonID, UserName, PasswordHash, IsActive);
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Login Error for User {UserName}: {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
            }
            return null;
        }

        public static DataTable GetAllUsers() => clsUserData.GetAllUsers();

        public static bool Delete(int UserID)
        {
            try
            {
                bool result = clsUserData.DeleteUser(UserID);
                if (result) EventLogger.Log($"User Deleted - ID: {UserID}", "INFO", EventLogger.LogTarget.TextFile);
                return result;
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Error Deleting User (ID: {UserID}): {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
                return false;
            }
        }

        public static bool IsExist(string UserName) => clsUserData.IsUserExist(UserName);

        // --- [ منطق الحفظ الذكي ] ---
        private bool _AddNew()
        {
            string PasswordHash = clsSecurityUtils.ComputeHash(this.Password);
            this.UserID = clsUserData.AddNewUser(this.PersonID, this.UserName, PasswordHash, this.IsActive);
            return (this.UserID != -1);
        }

        private bool _Update()
        {
            string PasswordHash = clsSecurityUtils.ComputeHash(this.Password);
            return clsUserData.UpdateUser(this.UserID, this.PersonID, this.UserName, PasswordHash, this.IsActive);
        }

        public bool Save()
        {
            try
            {
                switch (Mode)
                {
                    case enMode.AddNew:
                        if (_AddNew())
                        {
                            EventLogger.Log($"New User Created: {this.UserName}", "INFO", EventLogger.LogTarget.TextFile);
                            Mode = enMode.Update;
                            return true;
                        }
                        return false;
                    case enMode.Update:
                        if (_Update())
                        {
                            EventLogger.Log($"User Updated: {this.UserName}", "INFO", EventLogger.LogTarget.TextFile);
                            return true;
                        }
                        return false;
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Save User Error: {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
            }
            return false;
        }

        // --- [ إدارة بيانات الدخول المحفوظة (Registry Logic) ] ---

        public static bool SaveRememberMeCredentials(string UserName, string Password)
        {
            try
            {
                string EncryptedPassword = clsSecurityUtils.EncryptSymmetric(Password, _RegistryEncryptionKey);
                bool r1 = clsRegistryUtils.SaveValue("UserName", UserName);
                bool r2 = clsRegistryUtils.SaveValue("Password", EncryptedPassword);

                if (r1 && r2)
                    EventLogger.Log($"Remember Me credentials saved for: {UserName}", "INFO", EventLogger.LogTarget.TextFile);

                return r1 && r2;
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Error Saving Remember Me: {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
                return false;
            }
        }

        public static bool GetRememberMeCredentials(ref string UserName, ref string Password)
        {
            try
            {
                UserName = clsRegistryUtils.ReadValue("UserName");
                string EncryptedPassword = clsRegistryUtils.ReadValue("Password");

                if (!string.IsNullOrEmpty(EncryptedPassword))
                {
                    Password = clsSecurityUtils.DecryptSymmetric(EncryptedPassword, _RegistryEncryptionKey);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Error Retrieving Remember Me: {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
                return false;
            }
        }

        public static bool ClearRememberMeCredentials()
        {
            try
            {
                bool r1 = clsRegistryUtils.DeleteValue("UserName");
                bool r2 = clsRegistryUtils.DeleteValue("Password");

                if (r1 && r2)
                    EventLogger.Log("Remember Me credentials cleared.", "INFO", EventLogger.LogTarget.TextFile);

                return r1 && r2;
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Error Clearing Remember Me: {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
                return false;
            }
        }
    }
}