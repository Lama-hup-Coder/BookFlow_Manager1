using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Library_Common; // [إضافة] لاستخدام نظام التسجيل الموحد

namespace Library_DataAccess
{
    [DataTableName("BorrowingRecords")]
    public static class clsBorrowingRecordData
    {
        // 1. تعريف الأحداث (الأخطاء وتغيير البيانات)
        public static event Action<string> OnError;
        public static event Action<string, int> OnDataChanged;

        private static int _CommandTimeout = 30;

        // 2. استخدام الـ Reflection لجلب اسم الجدول ديناميكياً
        private static string _GetTableName()
        {
            var attribute = typeof(clsBorrowingRecordData).GetCustomAttribute<DataTableNameAttribute>();
            return attribute?.TableName ?? "BorrowingRecords";
        }

        // [إضافة] دالة مساعدة لتوثيق النجاح في الملف النصي
        private static void _LogAction(string action, int id)
        {
            EventLogger.Log($"BorrowingRecord {action} - ID: {id}", "INFO", EventLogger.LogTarget.TextFile);
        }

        // --- دوال الـ CRUD (تتعامل مع الجدول الأصلي) ---

        public static bool GetBorrowingInfoByID(int BorrowID, ref int ApplicationID, ref int BookID,
                                               ref DateTime DueDate, ref DateTime? ActualReturnDate)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT * FROM {_GetTableName()} WHERE BorrowID = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", BorrowID);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                ApplicationID = (int)reader["ApplicationID"];
                                BookID = (int)reader["BookID"];
                                DueDate = (DateTime)reader["DueDate"];
                                ActualReturnDate = (reader["ActualReturnDate"] == DBNull.Value) ? (DateTime?)null : (DateTime)reader["ActualReturnDate"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // [إضافة] تسجيل الخطأ في السجلين عند فشل الجلب
                EventLogger.Log("GetByID Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("GetByID Error: " + ex.Message);
            }
            return isFound;
        }

        public static int AddNewBorrowingRecord(int ApplicationID, int BookID, DateTime DueDate)
        {
            int ID = -1;
            string query = $@"INSERT INTO {_GetTableName()} (ApplicationID, BookID, DueDate) 
                             VALUES (@AppID, @BookID, @Due); SELECT SCOPE_IDENTITY();";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AppID", ApplicationID);
                        command.Parameters.AddWithValue("@BookID", BookID);
                        command.Parameters.AddWithValue("@Due", DueDate);
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            ID = insertedID;
                            _LogAction("Insert", ID); // [إضافة] توثيق نجاح الإضافة
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

        public static bool UpdateBorrowingRecord(int BorrowID, int ApplicationID, int BookID,
                                                DateTime DueDate, DateTime? ActualReturnDate)
        {
            int rowsAffected = 0;
            string query = $@"UPDATE {_GetTableName()} 
                             SET ApplicationID = @AppID, BookID = @BookID, DueDate = @Due, ActualReturnDate = @Actual 
                             WHERE BorrowID = @ID";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", BorrowID);
                        command.Parameters.AddWithValue("@AppID", ApplicationID);
                        command.Parameters.AddWithValue("@BookID", BookID);
                        command.Parameters.AddWithValue("@Due", DueDate);
                        command.Parameters.AddWithValue("@Actual", (object)ActualReturnDate ?? DBNull.Value);

                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            _LogAction("Update", BorrowID); // [إضافة] توثيق نجاح التعديل
                            OnDataChanged?.Invoke("Update", BorrowID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Update Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Update Error: " + ex.Message);
            }
            return (rowsAffected > 0);
        }

        // --- دوال العرض والبحث (تستخدم الفيو v_BorrowingRecordsDetails) ---

        public static DataTable GetAllBorrowingRecords()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM v_BorrowingRecordsDetails ORDER BY DueDate DESC";
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

        public static DataTable SearchBorrowingRecords(string SearchText)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT * FROM v_BorrowingRecordsDetails 
                                     WHERE MemberName LIKE @Search + '%' 
                                     OR BookTitle LIKE @Search + '%' 
                                     OR LibraryCardNumber LIKE @Search + '%'";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Search", SearchText);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader()) { if (reader.HasRows) dt.Load(reader); }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Search Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Search Error: " + ex.Message);
            }
            return dt;
        }

        // --- دوال المنطق والحساب ---

        public static bool RegisterBookReturn(int BorrowID, DateTime ActualReturnDate)
        {
            int rowsAffected = 0;
            string query = $"UPDATE {_GetTableName()} SET ActualReturnDate = @Actual WHERE BorrowID = @ID";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", BorrowID);
                        command.Parameters.AddWithValue("@Actual", ActualReturnDate);
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            _LogAction("Return", BorrowID); // [إضافة] توثيق عملية الإرجاع
                            OnDataChanged?.Invoke("Return", BorrowID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Return Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Return Error: " + ex.Message);
            }
            return (rowsAffected > 0);
        }

        public static bool IsBookAvailableForBorrow(int BookID)
        {
            bool isAvailable = true;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT TOP 1 1 FROM {_GetTableName()} WHERE BookID = @BookID AND ActualReturnDate IS NULL";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BookID", BookID);
                        connection.Open();
                        isAvailable = (command.ExecuteScalar() == null);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Availability Check Error: " + ex.Message, "ERROR", EventLogger.LogTarget.TextFile);
                OnError?.Invoke("Availability Check Error: " + ex.Message);
            }
            return isAvailable;
        }

        public static decimal GetTotalFinesForRecord(int BorrowID)
        {
            decimal totalFine = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $@"SELECT DATEDIFF(day, B.DueDate, GETDATE()) * C.LateFeePerDay
                                     FROM BorrowingRecords B
                                     INNER JOIN Books BK ON B.BookID = BK.BookID
                                     INNER JOIN BookCategories C ON BK.CategoryID = C.CategoryID
                                     WHERE B.BorrowID = @BorrowID AND B.ActualReturnDate IS NULL AND GETDATE() > B.DueDate";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BorrowID", BorrowID);
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && decimal.TryParse(result.ToString(), out decimal fine))
                        {
                            totalFine = fine > 0 ? fine : 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Fine Calculation Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Fine Calculation Error: " + ex.Message);
            }
            return totalFine;
        }
    }
}