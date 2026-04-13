using System;
using System.Data;
using Library_DataAccess;
using Library_Common; // استدعاء نظام التسجيل الموحد

namespace Library_Business
{
    public class clsMember
    {
        // 1. الأيفنتات (الحدث) لتبليغ الواجهات بتغير البيانات أو حدوث خطأ
        public static event Action<string, int> OnDataChanged;
        public static event Action<string> OnError;

        // 2. الربط التلقائي مع طبقة البيانات (Static Constructor)
        static clsMember()
        {
            // ربط أحداث طبقة الداتا بأحداث طبقة البيزنس وتوثيق أخطاء الداتا لير
            clsMemberData.OnDataChanged += (action, id) => OnDataChanged?.Invoke(action, id);
            
            clsMemberData.OnError += (msg) => 
            {
                EventLogger.Log($"Data Layer Error in clsMember: {msg}", "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke(msg);
            };
        }

        // 3. الأوضاع (Modes)
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        // 4. الخصائص (Properties)
        public int MemberID { get; set; }
        public int PersonID { get; set; }
        public clsPerson PersonInfo; // تكوين (Composition) לגلب بيانات الشخص المرتبط بالعضو

        public string FullName { get; set; }
        public string LibraryCardNumber { get; set; }
        public DateTime MembershipDate { get; set; }
        public bool IsActive { get; set; }

        // 5. المشيد الافتراضي
        public clsMember()
        {
            this.MemberID = -1;
            this.PersonID = -1;
            this.PersonInfo = new clsPerson();
            this.LibraryCardNumber = "";
            this.MembershipDate = DateTime.Now;
            this.IsActive = true;
            Mode = enMode.AddNew;
        }

        // 6. المشيد الخاص (للاستخدام الداخلي عند جلب البيانات)
        private clsMember(int MemberID, int PersonID, string FullName, string LibraryCardNumber,
                          DateTime MembershipDate, bool IsActive)
        {
            this.MemberID = MemberID;
            this.PersonID = PersonID;
            this.PersonInfo = clsPerson.Find(PersonID);
            this.FullName = FullName;
            this.LibraryCardNumber = LibraryCardNumber;
            this.MembershipDate = MembershipDate;
            this.IsActive = IsActive;
            Mode = enMode.Update;
        }

        // --- [ الدوال الاستاتيكية للبحث والجلب ] ---

        public static clsMember Find(int MemberID)
        {
            try
            {
                int PersonID = -1; string LibraryCardNumber = "", FullName = "";
                DateTime MembershipDate = DateTime.Now; bool IsActive = false;

                if (clsMemberData.GetMemberFullInfoByID(MemberID, ref PersonID, ref FullName,
                    ref LibraryCardNumber, ref MembershipDate, ref IsActive))
                {
                    return new clsMember(MemberID, PersonID, FullName, LibraryCardNumber, MembershipDate, IsActive);
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Business Layer Find Member Error (ID:{MemberID}): {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
            }
            return null;
        }

        public static clsMember FindByCard(string LibraryCardNumber)
        {
            try
            {
                int PersonID = -1; int MemberID = -1; string FullName = "";
                DateTime MembershipDate = DateTime.Now; bool IsActive = false;

                if (clsMemberData.GetMemberInfoByLibraryCardNumber(LibraryCardNumber, ref MemberID, ref PersonID,
                    ref FullName, ref MembershipDate, ref IsActive))
                {
                    return new clsMember(MemberID, PersonID, FullName, LibraryCardNumber, MembershipDate, IsActive);
                }
                else
                {
                    EventLogger.Log($"Search Attempt: Library Card Number '{LibraryCardNumber}' not found.", "WARN", EventLogger.LogTarget.TextFile);
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Business Layer FindByCard Error: {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
            }
            return null;
        }

        // --- [ منطق الحفظ والتعديل ] ---

        private bool _AddNew()
        {
            this.MemberID = clsMemberData.AddNewMember(this.PersonID, this.LibraryCardNumber, this.MembershipDate, this.IsActive);
            return (this.MemberID != -1);
        }

        private bool _Update()
        {
            return clsMemberData.UpdateMember(this.MemberID, this.PersonID, this.LibraryCardNumber, this.IsActive);
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
                            EventLogger.Log($"New Member Registered: {this.FullName} (Card: {this.LibraryCardNumber})", "INFO", EventLogger.LogTarget.TextFile);
                            Mode = enMode.Update;
                            return true;
                        }
                        break;
                    case enMode.Update:
                        if (_Update())
                        {
                            EventLogger.Log($"Member Data Updated: {this.FullName} (ID: {this.MemberID})", "INFO", EventLogger.LogTarget.TextFile);
                            return true;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Business Layer Save Member Error: {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
            }
            return false;
        }

        // --- [ العمليات العامة ] ---

        public static bool Delete(int MemberID)
        {
            try
            {
                bool result = clsMemberData.DeleteMember(MemberID);
                if (result)
                {
                    EventLogger.Log($"Member Deleted - ID: {MemberID}", "INFO", EventLogger.LogTarget.TextFile);
                }
                return result;
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Business Layer Delete Member Error (ID:{MemberID}): {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
                return false;
            }
        }

        public static DataTable GetAllMembers() => clsMemberData.GetAllMembers();

        public static bool IsCardExist(string Card) => clsMemberData.IsLibraryCardExist(Card);

        public static bool IsPersonMember(int PersonID) => clsMemberData.IsPersonAlreadyMember(PersonID);

        public static int GetActiveCount() => clsMemberData.GetActiveMembersCount();

        public bool SetActive(bool Active)
        {
            try
            {
                if (clsMemberData.SetMemberActiveStatus(this.MemberID, Active))
                {
                    this.IsActive = Active;
                    EventLogger.Log($"Member Status Changed to {(Active ? "Active" : "Inactive")} - ID: {this.MemberID}", "INFO", EventLogger.LogTarget.TextFile);
                    return true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Error Changing Member Status (ID:{this.MemberID}): {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
            }
            return false;
        }
    }
}