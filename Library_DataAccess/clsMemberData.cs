using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Library_Common;

namespace Library_DataAccess
{
    [DataTableName("Members")]
    public static class clsMemberData
    {
        public static event Action<string> OnError;
        public static event Action<string, int> OnDataChanged;

        private static int _CommandTimeout = 30;

        private static string _GetTableName()
        {
            var attribute = typeof(clsMemberData).GetCustomAttribute<DataTableNameAttribute>();
            return attribute?.TableName ?? "Members";
        }

        private static void _LogAction(string action, int id)
        {
            EventLogger.Log($"Member {action} - ID: {id}", "INFO", EventLogger.LogTarget.TextFile);
        }

        private static int _ExecuteNonQuery(string query, SqlParameter[] parameters, string actionName, int recordID)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandTimeout = _CommandTimeout;
                        if (parameters != null) command.Parameters.AddRange(parameters);
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0 && !string.IsNullOrEmpty(actionName))
                        {
                            _LogAction(actionName, recordID);
                            OnDataChanged?.Invoke(actionName, recordID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log($"{actionName} Execution Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke($"{actionName} Execution Error: " + ex.Message);
            }
            return rowsAffected;
        }

        // --- [ الدوال التي تم تعديلها أو إضافتها للمطابقة ] ---

        // 1. الدالة الأساسية لجلب المعلومات (تم توحيد المسمى)
        public static bool GetMemberFullInfoByID(int MemberID, ref int PersonID, ref string FullName,
                                                 ref string LibraryCardNumber, ref DateTime MembershipDate, ref bool IsActive)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $@"SELECT M.*, P.FullName FROM {_GetTableName()} M
                                     INNER JOIN People P ON M.PersonID = P.PersonID 
                                     WHERE M.MemberID = @MemberID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MemberID", MemberID);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                PersonID = (int)reader["PersonID"];
                                FullName = (string)reader["FullName"];
                                LibraryCardNumber = (string)reader["LibraryCardNumber"];
                                MembershipDate = (DateTime)reader["MembershipDate"];
                                IsActive = (bool)reader["IsActive"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("GetFullByID Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("GetFullByID Error: " + ex.Message);
            }
            return isFound;
        }

        // 2. الدالة المفقودة: جلب معلومات العضو برقم البطاقة
        public static bool GetMemberInfoByLibraryCardNumber(string LibraryCardNumber, ref int MemberID, ref int PersonID,
                                                            ref string FullName, ref DateTime MembershipDate, ref bool IsActive)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $@"SELECT M.*, P.FullName FROM {_GetTableName()} M
                                     INNER JOIN People P ON M.PersonID = P.PersonID 
                                     WHERE M.LibraryCardNumber = @Card";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Card", LibraryCardNumber);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                MemberID = (int)reader["MemberID"];
                                PersonID = (int)reader["PersonID"];
                                FullName = (string)reader["FullName"];
                                MembershipDate = (DateTime)reader["MembershipDate"];
                                IsActive = (bool)reader["IsActive"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("GetByCard Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("GetByCard Error: " + ex.Message);
            }
            return isFound;
        }

        // 3. الدالة المفقودة: التحقق هل الشخص عضو بالفعل
        public static bool IsPersonAlreadyMember(int PersonID)
        {
            bool isExist = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT 1 FROM {_GetTableName()} WHERE PersonID = @PersonID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);
                        connection.Open();
                        isExist = (command.ExecuteScalar() != null);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("IsPersonMember Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
            }
            return isExist;
        }

        public static int AddNewMember(int PersonID, string LibraryCardNumber, DateTime MembershipDate, bool IsActive)
        {
            int MemberID = -1;
            string query = $@"INSERT INTO {_GetTableName()} (PersonID, LibraryCardNumber, MembershipDate, IsActive)
                             VALUES (@PersonID, @LibraryCardNumber, @MembershipDate, @IsActive);
                             SELECT SCOPE_IDENTITY();";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);
                        command.Parameters.AddWithValue("@LibraryCardNumber", LibraryCardNumber);
                        command.Parameters.AddWithValue("@MembershipDate", MembershipDate);
                        command.Parameters.AddWithValue("@IsActive", IsActive);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            MemberID = insertedID;
                            _LogAction("Insert", MemberID);
                            OnDataChanged?.Invoke("Insert", MemberID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Add Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Add Error: " + ex.Message);
            }
            return MemberID;
        }

        public static bool UpdateMember(int MemberID, int PersonID, string LibraryCardNumber, bool IsActive)
        {
            string query = $"UPDATE {_GetTableName()} SET PersonID = @PersonID, LibraryCardNumber = @LibraryCardNumber, IsActive = @IsActive WHERE MemberID = @MemberID";
            SqlParameter[] parameters = {
                new SqlParameter("@MemberID", MemberID),
                new SqlParameter("@PersonID", PersonID),
                new SqlParameter("@LibraryCardNumber", LibraryCardNumber),
                new SqlParameter("@IsActive", IsActive)
            };
            return _ExecuteNonQuery(query, parameters, "Update", MemberID) > 0;
        }

        public static bool DeleteMember(int MemberID)
        {
            string query = $"DELETE FROM {_GetTableName()} WHERE MemberID = @MemberID";
            SqlParameter[] parameters = { new SqlParameter("@MemberID", MemberID) };
            return _ExecuteNonQuery(query, parameters, "Delete", MemberID) > 0;
        }

        public static bool SetMemberActiveStatus(int MemberID, bool IsActive)
        {
            string query = $"UPDATE {_GetTableName()} SET IsActive = @IsActive WHERE MemberID = @MemberID";
            SqlParameter[] parameters = {
                new SqlParameter("@MemberID", MemberID),
                new SqlParameter("@IsActive", IsActive)
            };
            return _ExecuteNonQuery(query, parameters, "StatusUpdate", MemberID) > 0;
        }

        public static DataTable GetAllMembers()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $@"SELECT M.MemberID, P.FullName, M.LibraryCardNumber, 
                                     M.MembershipDate, M.IsActive
                                     FROM {_GetTableName()} M INNER JOIN People P ON M.PersonID = P.PersonID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows) dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("GetAll Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
            }
            return dt;
        }

        public static bool IsLibraryCardExist(string LibraryCardNumber)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT 1 FROM {_GetTableName()} WHERE LibraryCardNumber = @Card";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Card", LibraryCardNumber);
                        connection.Open();
                        isFound = (command.ExecuteScalar() != null);
                    }
                }
            }
            catch (Exception ex) { EventLogger.Log("Exist Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both); }
            return isFound;
        }

        public static int GetActiveMembersCount()
        {
            int Count = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT COUNT(*) FROM {_GetTableName()} WHERE IsActive = 1";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        Count = (int)command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex) { EventLogger.Log("Count Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both); }
            return Count;
        }
    }
}