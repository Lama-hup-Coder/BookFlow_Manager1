using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Library_Common; // تأكد من استدعاء نظام التسجيل الموحد

namespace Library_DataAccess
{
    [DataTableName("People")]
    public class clsPersonData
    {
        public static event Action<string> OnError;
        public static event Action<string, int> OnDataChanged;

        private static string _GetTableName()
        {
            var attribute = typeof(clsPersonData).GetCustomAttribute<DataTableNameAttribute>();
            return attribute?.TableName ?? "People";
        }

        // دالة مساعدة لتسجيل النجاح (اختياري لتقليل تكرار الكود)
        private static void _LogInfo(string action, int id)
        {
            EventLogger.Log($"Person {action} - ID: {id}", "INFO", EventLogger.LogTarget.TextFile);
        }

        public static int AddNewPerson(string NationalNo, string FullName, DateTime DateOfBirth,
            short Gendor, string Phone, string Email, string Address)
        {
            int PersonID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $@"INSERT INTO {_GetTableName()} (NationalNo, FullName, DateOfBirth, Gendor, Phone, Email, Address)
                                     VALUES (@NationalNo, @FullName, @DateOfBirth, @Gendor, @Phone, @Email, @Address);
                                     SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NationalNo", NationalNo);
                        command.Parameters.AddWithValue("@FullName", FullName);
                        command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                        command.Parameters.AddWithValue("@Gendor", Gendor);
                        command.Parameters.AddWithValue("@Phone", (object)Phone ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Email", (object)Email ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Address", (object)Address ?? DBNull.Value);

                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            PersonID = insertedID;
                            _LogInfo("Insert", PersonID); // توثيق النجاح
                            OnDataChanged?.Invoke("Insert", PersonID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Add Person Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Add Person Error: " + ex.Message);
            }
            return PersonID;
        }

        public static bool UpdatePerson(int PersonID, string NationalNo, string FullName, DateTime DateOfBirth,
            short Gendor, string Phone, string Email, string Address)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $@"UPDATE {_GetTableName()} 
                                    SET NationalNo = @NationalNo, FullName = @FullName, DateOfBirth = @DateOfBirth, 
                                        Gendor = @Gendor, Phone = @Phone, Email = @Email, Address = @Address
                                    WHERE PersonID = @PersonID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);
                        command.Parameters.AddWithValue("@NationalNo", NationalNo);
                        command.Parameters.AddWithValue("@FullName", FullName);
                        command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                        command.Parameters.AddWithValue("@Gendor", Gendor);
                        command.Parameters.AddWithValue("@Phone", (object)Phone ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Email", (object)Email ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Address", (object)Address ?? DBNull.Value);

                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            _LogInfo("Update", PersonID); // توثيق النجاح
                            OnDataChanged?.Invoke("Update", PersonID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Update Person Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Update Person Error: " + ex.Message);
            }
            return (rowsAffected > 0);
        }

        public static bool DeletePerson(int PersonID)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"DELETE FROM {_GetTableName()} WHERE PersonID = @PersonID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            _LogInfo("Delete", PersonID); // توثيق النجاح
                            OnDataChanged?.Invoke("Delete", PersonID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Delete Person Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Delete Person Error: " + ex.Message);
            }
            return (rowsAffected > 0);
        }

        public static bool GetPersonInfoByID(int PersonID, ref string NationalNo, ref string FullName,
            ref DateTime DateOfBirth, ref short Gendor, ref string Phone, ref string Email, ref string Address)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT * FROM {_GetTableName()} WHERE PersonID = @PersonID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                NationalNo = reader["NationalNo"] as string ?? "";
                                FullName = reader["FullName"] as string ?? "";
                                DateOfBirth = (DateTime)reader["DateOfBirth"];
                                Gendor = reader["Gendor"] != DBNull.Value ? Convert.ToInt16(reader["Gendor"]) : (short)0;
                                Phone = reader["Phone"] as string ?? "";
                                Email = reader["Email"] as string ?? "";
                                Address = reader["Address"] as string ?? "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Get Person By ID Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Get Person By ID Error: " + ex.Message);
                isFound = false;
            }
            return isFound;
        }

        public static DataTable GetAllPeople()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT * FROM {_GetTableName()}";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader()) { if (reader.HasRows) dt.Load(reader); }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("List All People Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("List All People Error: " + ex.Message);
            }
            return dt;
        }

        public static bool IsPersonExist(int PersonID)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT Found=1 FROM {_GetTableName()} WHERE PersonID = @PersonID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);
                        connection.Open();
                        isFound = (command.ExecuteScalar() != null);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Check Person Existence Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Check Person Existence Error: " + ex.Message);
            }
            return isFound;
        }

        public static object GetColumnValue(int PersonID, string ColumnName)
        {
            object value = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT {ColumnName} FROM {_GetTableName()} WHERE PersonID = @PersonID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);
                        connection.Open();
                        value = command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Get Column Value Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Get Column Value Error: " + ex.Message);
            }
            return value;
        }

        public static int GetTotalCount()
        {
            int count = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT COUNT(*) FROM {_GetTableName()}";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        count = (int)command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Get Total Count Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Get Total Count Error: " + ex.Message);
            }
            return count;
        }

        public static DataTable SearchPeople(string FilterValue)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $@"SELECT * FROM {_GetTableName()} 
                                     WHERE FullName LIKE @Filter 
                                     OR NationalNo LIKE @Filter";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Filter", "%" + FilterValue + "%");
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader()) { if (reader.HasRows) dt.Load(reader); }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Search People Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Search People Error: " + ex.Message);
            }
            return dt;
        }

        public static bool GetPersonByNationalNo(string NationalNo, ref int PersonID, ref string FullName,
            ref DateTime DateOfBirth, ref short Gendor, ref string Phone, ref string Email, ref string Address)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT * FROM {_GetTableName()} WHERE NationalNo = @NationalNo";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NationalNo", NationalNo);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                PersonID = (int)reader["PersonID"];
                                FullName = reader["FullName"] as string ?? "";
                                DateOfBirth = (DateTime)reader["DateOfBirth"];
                                Gendor = (reader["Gendor"] != DBNull.Value) ? Convert.ToInt16(reader["Gendor"]) : (short)0; 
                                Phone = reader["Phone"] as string ?? "";
                                Email = reader["Email"] as string ?? "";
                                Address = reader["Address"] as string ?? "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Get Person By NationalNo Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Get Person By NationalNo Error: " + ex.Message);
            }
            return isFound;
        }
    }
}