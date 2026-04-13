using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Library_Common; // استدعاء مكتبة الـ Log

namespace Library_DataAccess
{
    [DataTableName("BookCategories")]
    public static class clsCategoryData
    {
        // 1. تعريف الأحداث (الأخطاء وتغير البيانات)
        public static event Action<string> OnError;
        public static event Action<string, int> OnDataChanged;

        private static int _CommandTimeout = 30;

        // 2. دالة الريفليكشن لجلب اسم الجدول ديناميكياً
        private static string _GetTableName()
        {
            var attribute = typeof(clsCategoryData).GetCustomAttribute<DataTableNameAttribute>();
            return attribute?.TableName ?? "BookCategories";
        }

        // [إضافة] دالة لتوثيق النجاح في الملف النصي
        private static void _LogAction(string action, int id)
        {
            EventLogger.Log($"Category {action} - ID: {id}", "INFO", EventLogger.LogTarget.TextFile);
        }

        // --- الدالة المساعدة (Helper) المحدثة لإطلاق الأحداث والـ Log ---
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

                        // إذا نجحت العملية، نطلق حدث تغيير البيانات ونكتب في الـ Log
                        if (rowsAffected > 0 && !string.IsNullOrEmpty(actionName))
                        {
                            _LogAction(actionName, recordID); // توثيق النجاح
                            OnDataChanged?.Invoke(actionName, recordID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // توثيق الخطأ في السجلين
                EventLogger.Log($"{actionName} Execution Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke($"{actionName} Execution Error: " + ex.Message);
            }
            return rowsAffected;
        }

        // 3. GetCategoryByID
        public static bool GetCategoryByID(int CategoryID, ref string CategoryName, ref short DefaultBorrowingDays, ref decimal LateFeePerDay)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT * FROM {_GetTableName()} WHERE CategoryID = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", CategoryID);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                CategoryName = (string)reader["CategoryName"];
                                DefaultBorrowingDays = Convert.ToInt16(reader["DefaultBorrowingDays"]);
                                LateFeePerDay = Convert.ToDecimal(reader["LateFeePerDay"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("GetByID Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("GetByID Error: " + ex.Message);
            }
            return isFound;
        }

        // 4. AddNewCategory
        public static int AddNewCategory(string CategoryName, short DefaultBorrowingDays, decimal LateFeePerDay)
        {
            int ID = -1;
            string query = $@"INSERT INTO {_GetTableName()} (CategoryName, DefaultBorrowingDays, LateFeePerDay) 
                             VALUES (@Name, @Days, @Fee); SELECT SCOPE_IDENTITY();";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", CategoryName);
                        command.Parameters.AddWithValue("@Days", DefaultBorrowingDays);
                        command.Parameters.AddWithValue("@Fee", LateFeePerDay);

                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            ID = insertedID;
                            _LogAction("Insert", ID); // توثيق نجاح الإضافة
                            OnDataChanged?.Invoke("Insert", ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Add Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Add Error: " + ex.Message);
            }
            return ID;
        }

        // 5. UpdateCategory
        public static bool UpdateCategory(int ID, string CategoryName, short DefaultBorrowingDays, decimal LateFeePerDay)
        {
            string query = $@"UPDATE {_GetTableName()} 
                             SET CategoryName = @Name, DefaultBorrowingDays = @Days, LateFeePerDay = @Fee 
                             WHERE CategoryID = @ID";

            SqlParameter[] parameters = {
                new SqlParameter("@ID", ID),
                new SqlParameter("@Name", CategoryName),
                new SqlParameter("@Days", DefaultBorrowingDays),
                new SqlParameter("@Fee", LateFeePerDay)
            };

            return _ExecuteNonQuery(query, parameters, "Update", ID) > 0;
        }

        // 6. DeleteCategory
        public static bool DeleteCategory(int ID)
        {
            string query = $"DELETE FROM {_GetTableName()} WHERE CategoryID = @ID";
            SqlParameter[] parameters = { new SqlParameter("@ID", ID) };

            return _ExecuteNonQuery(query, parameters, "Delete", ID) > 0;
        }

        // 7. GetAllCategories
        public static DataTable GetAllCategories()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT * FROM {_GetTableName()} ORDER BY CategoryName";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader()) { if (reader.HasRows) dt.Load(reader); }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("GetAll Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("GetAll Error: " + ex.Message);
            }
            return dt;
        }

        // 8. IsCategoryExist (By Name)
        public static bool IsCategoryExist(string CategoryName)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT TOP 1 1 FROM {_GetTableName()} WHERE CategoryName = @Name";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", CategoryName);
                        connection.Open();
                        isFound = (command.ExecuteScalar() != null);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("IsExist Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("IsExist Error: " + ex.Message);
            }
            return isFound;
        }

        // 9. GetCategoriesSummary (الـ Join والـ Group By)
        public static DataTable GetCategoriesSummary()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $@"SELECT C.CategoryID, C.CategoryName, C.DefaultBorrowingDays, C.LateFeePerDay, 
                                     COUNT(B.BookID) AS BooksCount
                                     FROM {_GetTableName()} C LEFT JOIN Books B ON C.CategoryID = B.CategoryID
                                     GROUP BY C.CategoryID, C.CategoryName, C.DefaultBorrowingDays, C.LateFeePerDay";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader()) { if (reader.HasRows) dt.Load(reader); }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Summary Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Summary Error: " + ex.Message);
            }
            return dt;
        }
    }
}