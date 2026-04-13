using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Library_Common; // استدعاء نظام التسجيل الموحد

namespace Library_DataAccess
{
    // 1. الأتربيوت: تحديد اسم الجدول في قاعدة البيانات
    [DataTableName("Users")]
    public static class clsUserData
    {
        // 2. الأحداث (Events): لتبليغ طبقات البزنس بالأخطاء أو التغييرات
        public static event Action<string> OnError;
        public static event Action<string, int> OnDataChanged;

        // 3. الريفليكشن: جلب اسم الجدول ديناميكياً
        private static string _GetTableName()
        {
            var attribute = typeof(clsUserData).GetCustomAttribute<DataTableNameAttribute>();
            return attribute?.TableName ?? "Users";
        }

        // 4. دالة مساعدة لتسجيل النجاح في ملف النص
        private static void _LogInfo(string action, int id)
        {
            EventLogger.Log($"User {action} - ID: {id}", "INFO", EventLogger.LogTarget.TextFile);
        }

        // --- الدوال الأساسية (CRUD) ---

        public static int AddNewUser(int PersonID, string UserName, string Password, bool IsActive)
        {
            int UserID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $@"INSERT INTO {_GetTableName()} (PersonID, UserName, Password, IsActive)
                                     VALUES (@PersonID, @UserName, @Password, @IsActive);
                                     SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);
                        command.Parameters.AddWithValue("@UserName", UserName);
                        command.Parameters.AddWithValue("@Password", Password);
                        command.Parameters.AddWithValue("@IsActive", IsActive);

                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            UserID = insertedID;
                            _LogInfo("Insert", UserID);
                            OnDataChanged?.Invoke("Insert", UserID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Add User Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Add User Error: " + ex.Message);
            }
            return UserID;
        }

        public static bool GetUserInfoByID(int UserID, ref int PersonID, ref string UserName, ref string Password, ref bool IsActive)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT * FROM {_GetTableName()} WHERE UserID = @UserID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", UserID);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                PersonID = (int)reader["PersonID"];
                                UserName = (string)reader["UserName"];
                                Password = (string)reader["Password"];
                                IsActive = (bool)reader["IsActive"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Find User Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Find User Error: " + ex.Message);
            }
            return isFound;
        }

        public static bool UpdateUser(int UserID, int PersonID, string UserName, string Password, bool IsActive)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $@"UPDATE {_GetTableName()} SET PersonID=@PersonID, UserName=@UserName, 
                                     Password=@Password, IsActive=@IsActive WHERE UserID=@UserID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", UserID);
                        command.Parameters.AddWithValue("@PersonID", PersonID);
                        command.Parameters.AddWithValue("@UserName", UserName);
                        command.Parameters.AddWithValue("@Password", Password);
                        command.Parameters.AddWithValue("@IsActive", IsActive);

                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            _LogInfo("Update", UserID);
                            OnDataChanged?.Invoke("Update", UserID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Update User Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Update User Error: " + ex.Message);
            }
            return (rowsAffected > 0);
        }

        public static bool DeleteUser(int UserID)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"DELETE FROM {_GetTableName()} WHERE UserID = @UserID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", UserID);
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            _LogInfo("Delete", UserID);
                            OnDataChanged?.Invoke("Delete", UserID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Delete User Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Delete User Error: " + ex.Message);
            }
            return (rowsAffected > 0);
        }

        public static DataTable GetAllUsers()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    // ملاحظة: تأكد من وجود جدول People في قاعدة البيانات لإتمام الـ Inner Join
                    string query = $@"SELECT Users.UserID, Users.PersonID, People.FullName, 
                                     Users.UserName, Users.IsActive FROM {_GetTableName()} 
                                     INNER JOIN People ON Users.PersonID = People.PersonID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader()) { if (reader.HasRows) dt.Load(reader); }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("List Users Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("List Users Error: " + ex.Message);
            }
            return dt;
        }

        public static bool GetUserInfoByUsernameAndPassword(string UserName, string Password, ref int UserID, ref int PersonID, ref bool IsActive)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT * FROM {_GetTableName()} WHERE UserName = @UserName AND Password = @Password";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", UserName);
                        command.Parameters.AddWithValue("@Password", Password);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                UserID = (int)reader["UserID"];
                                PersonID = (int)reader["PersonID"];
                                IsActive = (bool)reader["IsActive"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Login Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Login Error: " + ex.Message);
            }
            return isFound;
        }

        public static bool ChangePassword(int UserID, string NewPassword)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"UPDATE {_GetTableName()} SET Password = @NewPassword WHERE UserID = @UserID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", UserID);
                        command.Parameters.AddWithValue("@NewPassword", NewPassword);
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0) _LogInfo("ChangePassword", UserID);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Change Password Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Change Password Error: " + ex.Message);
            }
            return (rowsAffected > 0);
        }

        public static bool IsUserExist(string UserName)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT 1 FROM {_GetTableName()} WHERE UserName = @UserName";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", UserName);
                        connection.Open();
                        isFound = (command.ExecuteScalar() != null);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Check Username Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Check Username Error: " + ex.Message);
            }
            return isFound;
        }

        public static bool UpdateStatus(int UserID, bool IsActive)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"UPDATE {_GetTableName()} SET IsActive = @IsActive WHERE UserID = @UserID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", UserID);
                        command.Parameters.AddWithValue("@IsActive", IsActive);
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0) _LogInfo("UpdateStatus", UserID);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Status Update Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Status Update Error: " + ex.Message);
            }
            return (rowsAffected > 0);
        }
    }
}