using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Library_Common; // [إضافة] لاستخدام نظام التسجيل

namespace Library_DataAccess
{
    [DataTableName("ApplicationTypes")]
    public static class clsApplicationTypeData
    {
        // 2. الأحداث (Events) الضرورية لعملية الـ Monitoring
        public static event Action<string> OnError;
        public static event Action<string, int> OnDataChanged;

        // دالة مساعدة لجلب اسم الجدول ديناميكياً
        private static string _GetTableName()
        {
            var attr = typeof(clsApplicationTypeData).GetCustomAttribute<DataTableNameAttribute>();
            return attr?.TableName ?? "ApplicationTypes";
        }

        // [إضافة] دالة داخلية لتوثيق العمليات الناجحة في الملف النصي
        private static void _LogInfo(string action, int id)
        {
            EventLogger.Log($"ApplicationType {action} - ID: {id}", "INFO", EventLogger.LogTarget.TextFile);
        }

        // 1. جلب تفاصيل النوع بواسطة ID
        public static bool GetApplicationTypeInfoByID(int ApplicationTypeID,
            ref string ApplicationTypeTitle, ref float ApplicationFees)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT * FROM {_GetTableName()} WHERE ApplicationTypeID = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ApplicationTypeID);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                ApplicationTypeTitle = reader["ApplicationTypeTitle"] != DBNull.Value ? reader["ApplicationTypeTitle"].ToString() : "";
                                ApplicationFees = reader["ApplicationFees"] != DBNull.Value ? Convert.ToSingle(reader["ApplicationFees"]) : 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // [تعديل] تسجيل الخطأ في السجلين مع استدعاء الحدث
                EventLogger.Log("FetchByID Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("FetchByID Error: " + ex.Message);
            }
            return isFound;
        }

        // 2. جلب كل الأنواع
        public static DataTable GetAllApplicationTypes()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT * FROM {_GetTableName()} ORDER BY ApplicationTypeTitle";
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

        // 3. إضافة نوع جديد
        public static int AddNewApplicationType(string Title, float Fees)
        {
            int ID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $@"INSERT INTO {_GetTableName()} (ApplicationTypeTitle, ApplicationFees)
                                     VALUES (@Title, @Fees);
                                     SELECT SCOPE_IDENTITY();";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Title", Title);
                        command.Parameters.AddWithValue("@Fees", Fees);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            ID = insertedID;
                            _LogInfo("Insert", ID); // [إضافة] توثيق النجاح
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

        // 4. تحديث البيانات
        public static bool UpdateApplicationType(int ID, string Title, float Fees)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $@"UPDATE {_GetTableName()} 
                                     SET ApplicationTypeTitle = @Title, ApplicationFees = @Fees 
                                     WHERE ApplicationTypeID = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ID);
                        command.Parameters.AddWithValue("@Title", Title);
                        command.Parameters.AddWithValue("@Fees", Fees);
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Update Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Update Error: " + ex.Message);
                return false;
            }

            bool success = (rowsAffected > 0);
            if (success)
            {
                _LogInfo("Update", ID); // [إضافة] توثيق النجاح
                OnDataChanged?.Invoke("Update", ID);
            }
            return success;
        }

        // 5. حذف نوع
        public static bool DeleteApplicationType(int ApplicationTypeID)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"DELETE FROM {_GetTableName()} WHERE ApplicationTypeID = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ApplicationTypeID);
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Delete Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Delete Error: " + ex.Message);
                return false;
            }

            bool success = (rowsAffected > 0);
            if (success)
            {
                _LogInfo("Delete", ApplicationTypeID); // [إضافة] توثيق النجاح
                OnDataChanged?.Invoke("Delete", ApplicationTypeID);
            }
            return success;
        }

        // 6. فحص الوجود
        public static bool IsApplicationTypeExist(int ApplicationTypeID)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT TOP 1 1 FROM {_GetTableName()} WHERE ApplicationTypeID = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ApplicationTypeID);
                        connection.Open();
                        isFound = (command.ExecuteScalar() != null);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("IsExist Error: " + ex.Message, "ERROR", EventLogger.LogTarget.TextFile);
                isFound = false;
            }
            return isFound;
        }

        // 6 مكرر. فحص التبعيات
        public static bool DoesApplicationTypeHaveAnyDependency(int ApplicationTypeID)
        {
            bool hasDependency = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT TOP 1 1 FROM Applications WHERE ApplicationTypeID = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ApplicationTypeID);
                        connection.Open();
                        hasDependency = (command.ExecuteScalar() != null);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Dependency Check Error: " + ex.Message, "ERROR", EventLogger.LogTarget.TextFile);
                hasDependency = true;
            }
            return hasDependency;
        }

        // 7. جلب السعر (Scalar)
        public static float GetFees(int ApplicationTypeID)
        {
            float Fees = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT ApplicationFees FROM {_GetTableName()} WHERE ApplicationTypeID = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ApplicationTypeID);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                            float.TryParse(result.ToString(), out Fees);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("GetFees Error: " + ex.Message, "ERROR", EventLogger.LogTarget.TextFile);
                Fees = 0;
            }
            return Fees;
        }

        // 7 مكرر. جلب العنوان (Scalar)
        public static string GetTitle(int ApplicationTypeID)
        {
            string Title = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT ApplicationTypeTitle FROM {_GetTableName()} WHERE ApplicationTypeID = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ApplicationTypeID);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                            Title = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("GetTitle Error: " + ex.Message, "ERROR", EventLogger.LogTarget.TextFile);
                Title = "";
            }
            return Title;
        }

        // 8. فحص تكرار العنوان
        public static bool IsTitleExist(string Title)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT TOP 1 1 FROM {_GetTableName()} WHERE ApplicationTypeTitle = @Title";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Title", Title);
                        connection.Open();
                        isFound = (command.ExecuteScalar() != null);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("IsTitleExist Error: " + ex.Message, "ERROR", EventLogger.LogTarget.TextFile);
                isFound = false;
            }
            return isFound;
        }

        // 9. البحث الجزئي
        public static DataTable GetAllApplicationTypesByPartialTitle(string PartialTitle)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = $"SELECT * FROM {_GetTableName()} WHERE ApplicationTypeTitle LIKE @SearchText + '%' ORDER BY ApplicationTypeTitle";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SearchText", PartialTitle);
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
                EventLogger.Log("Search Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Search Error: " + ex.Message);
            }
            return dt;
        }
    }
}