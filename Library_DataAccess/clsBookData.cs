using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Library_Common; // [إضافة] لاستخدام نظام التسجيل الموحد

namespace Library_DataAccess
{
    [DataTableName("Books")]
    public static class clsBookData
    {
        // 1. تعريف الأحداث (Events)
        public static event Action<string> OnError;
        public static event Action<string, int> OnDataChanged;

        private static int _CommandTimeout = 30;

        // 2. استخدام الـ Reflection لجلب اسم الجدول ديناميكياً
        private static string _GetTableName()
        {
            var attribute = typeof(clsBookData).GetCustomAttribute<DataTableNameAttribute>();
            return attribute?.TableName ?? "Books";
        }

        // [إضافة] دالة مساعدة داخلية لتوثيق النجاح في الملف النصي فقط
        private static void _LogAction(string action, int id)
        {
            EventLogger.Log($"Book {action} - ID: {id}", "INFO", EventLogger.LogTarget.TextFile);
        }

        // 3. دالة تنفيذ الأوامر الموحدة (Private) - تم حقن الـ Log فيها
        private static int _ExecuteNonQuery(string query, SqlParameter[] parameters)
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
                    }
                }
            }
            catch (Exception ex)
            {
                // [إضافة] تسجيل الخطأ في السجلين (ويندوز وملف) عند فشل أي أمر (Update, Delete)
                EventLogger.Log("Execution Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Execution Error: " + ex.Message);
            }
            return rowsAffected;
        }

        // --- الدوال الأساسية (CRUD) ---

        public static bool GetBookInfoByID(int BookID, ref string Title, ref string Author,
                                          ref string ISBN, ref int CategoryID, ref bool IsAvailable)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT * FROM {_GetTableName()} WHERE BookID = @BookID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BookID", BookID);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                Title = reader["Title"] != DBNull.Value ? (string)reader["Title"] : "";
                                Author = reader["Author"] != DBNull.Value ? (string)reader["Author"] : "";
                                ISBN = reader["ISBN"] != DBNull.Value ? (string)reader["ISBN"] : "";
                                CategoryID = (int)reader["CategoryID"];
                                IsAvailable = (bool)reader["IsAvailable"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("GetBookInfo Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("GetBookInfo Error: " + ex.Message);
            }
            return isFound;
        }

        public static int AddNewBook(string Title, string Author, string ISBN, int CategoryID, bool IsAvailable)
        {
            int BookID = -1;
            string query = $@"INSERT INTO {_GetTableName()} (Title, Author, ISBN, CategoryID, IsAvailable)
                             VALUES (@Title, @Author, @ISBN, @CategoryID, @IsAvailable);
                             SELECT SCOPE_IDENTITY();";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Title", Title);
                        command.Parameters.AddWithValue("@Author", Author);
                        command.Parameters.AddWithValue("@ISBN", ISBN);
                        command.Parameters.AddWithValue("@CategoryID", CategoryID);
                        command.Parameters.AddWithValue("@IsAvailable", IsAvailable);

                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            BookID = insertedID;
                            _LogAction("Insert", BookID); // [إضافة] توثيق النجاح
                            OnDataChanged?.Invoke("Insert", BookID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Add Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Add Error: " + ex.Message);
            }
            return BookID;
        }

        public static bool UpdateBook(int BookID, string Title, string Author, string ISBN, int CategoryID, bool IsAvailable)
        {
            string query = $@"UPDATE {_GetTableName()} SET Title=@Title, Author=@Author, ISBN=@ISBN, 
                             CategoryID=@CategoryID, IsAvailable=@IsAvailable WHERE BookID=@BookID";

            SqlParameter[] parameters = {
                new SqlParameter("@BookID", BookID),
                new SqlParameter("@Title", Title),
                new SqlParameter("@Author", Author),
                new SqlParameter("@ISBN", ISBN),
                new SqlParameter("@CategoryID", CategoryID),
                new SqlParameter("@IsAvailable", IsAvailable)
            };

            bool success = _ExecuteNonQuery(query, parameters) > 0;
            if (success)
            {
                _LogAction("Update", BookID); // [إضافة] توثيق النجاح
                OnDataChanged?.Invoke("Update", BookID);
            }
            return success;
        }

        public static bool UpdateAvailability(int BookID, bool IsAvailable)
        {
            string query = $"UPDATE {_GetTableName()} SET IsAvailable = @IsAvailable WHERE BookID = @BookID";
            SqlParameter[] parameters = {
                new SqlParameter("@BookID", BookID),
                new SqlParameter("@IsAvailable", IsAvailable)
            };

            bool success = _ExecuteNonQuery(query, parameters) > 0;
            if (success)
            {
                _LogAction("UpdateAvailability", BookID); // [إضافة] توثيق النجاح
                OnDataChanged?.Invoke("UpdateAvailability", BookID);
            }
            return success;
        }

        public static bool DeleteBook(int BookID)
        {
            string query = $"DELETE FROM {_GetTableName()} WHERE BookID = @BookID";
            SqlParameter[] parameters = { new SqlParameter("@BookID", BookID) };

            bool success = _ExecuteNonQuery(query, parameters) > 0;
            if (success)
            {
                _LogAction("Delete", BookID); // [إضافة] توثيق النجاح
                OnDataChanged?.Invoke("Delete", BookID);
            }
            return success;
        }

        // --- دوال التحقق والعرض ---

        public static bool IsBookExist(string ISBN)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT TOP 1 Found=1 FROM {_GetTableName()} WHERE ISBN = @ISBN";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ISBN", ISBN);
                        connection.Open();
                        isFound = (command.ExecuteScalar() != null);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("IsExist Error: " + ex.Message, "ERROR", EventLogger.LogTarget.TextFile);
                OnError?.Invoke("IsExist Error: " + ex.Message);
            }
            return isFound;
        }

        public static DataTable GetAllBooksByCategory(int CategoryID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT * FROM {_GetTableName()} WHERE CategoryID = @CategoryID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CategoryID", CategoryID);
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
                EventLogger.Log("GetByCategory Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("GetByCategory Error: " + ex.Message);
            }
            return dt;
        }

        public static DataTable GetAllBooks()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $@"SELECT B.BookID, B.Title, B.Author, B.ISBN, 
                                     BC.CategoryName, B.IsAvailable FROM {_GetTableName()} B 
                                     INNER JOIN BookCategories BC ON B.CategoryID = BC.CategoryID";
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
                OnError?.Invoke("GetAll Error: " + ex.Message);
            }
            return dt;
        }

        public static bool CanDeleteBook(int BookID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT TOP 1 Found=1 FROM BorrowingRecords WHERE BookID = @BookID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BookID", BookID);
                        connection.Open();
                        return command.ExecuteScalar() == null;
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("CanDelete Check Error: " + ex.Message, "ERROR", EventLogger.LogTarget.TextFile);
                return false;
            }
        }
    }
}