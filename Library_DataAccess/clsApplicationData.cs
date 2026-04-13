using System;
using System.Data;
using System.Data.SqlClient;
using Library_Common;

namespace Library_DataAccess
{
    public static class clsApplicationData
    {
        public enum enChangeType { Add = 1, Update = 2, Delete = 3, StatusUpdate = 4 }

        // الأحداث (Events) لإخطار الطبقات العليا بالتغييرات أو الأخطاء
        public static event Action<Exception> OnErrorOccurred;
        public static event Action<int, enChangeType> OnDataChanged;

        private static void _HandleError(Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n[DATABASE ERROR]: " + ex.Message);
            Console.ResetColor();

            // تسجيل الخطأ في الملف النصي الموحد
            try { EventLogger.Log("DB Error: " + ex.Message, "ERROR", EventLogger.LogTarget.TextFile); }
            catch { }

            OnErrorOccurred?.Invoke(ex);
        }

        // 1. جلب معلومات طلب محدد
        public static bool GetApplicationInfoByID(int ApplicationID,
            ref int ApplicantPersonID, ref DateTime ApplicationDate, ref int ApplicationTypeID,
            ref byte ApplicationStatus, ref float PaidFees, ref int CreatedByUserID)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT ApplicantPersonID, ApplicationDate, ApplicationTypeID, 
                                     ApplicationStatus, PaidFees, CreatedByUserID 
                                     FROM Applications WHERE ApplicationID = @ID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ApplicationID);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                ApplicantPersonID = (int)reader["ApplicantPersonID"];
                                ApplicationDate = (DateTime)reader["ApplicationDate"];
                                ApplicationTypeID = (int)reader["ApplicationTypeID"];
                                ApplicationStatus = (byte)reader["ApplicationStatus"];
                                PaidFees = Convert.ToSingle(reader["PaidFees"]);
                                CreatedByUserID = (int)reader["CreatedByUserID"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { _HandleError(ex); }
            return isFound;
        }

        // 2. إضافة طلب جديد (بدون LastStatusDate)
        public static int AddNewApplication(int ApplicantPersonID, DateTime ApplicationDate,
            int ApplicationTypeID, byte ApplicationStatus, float PaidFees, int CreatedByUserID)
        {
            int ID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"INSERT INTO Applications (ApplicantPersonID, ApplicationDate, ApplicationTypeID, 
                                     ApplicationStatus, PaidFees, CreatedByUserID)
                                     VALUES (@ApplicantPersonID, @ApplicationDate, @ApplicationTypeID, 
                                     @ApplicationStatus, @PaidFees, @CreatedByUserID);
                                     SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
                        command.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
                        command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                        command.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
                        command.Parameters.AddWithValue("@PaidFees", PaidFees);
                        command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            ID = insertedID;
                            OnDataChanged?.Invoke(ID, enChangeType.Add);
                        }
                    }
                }
            }
            catch (Exception ex) { _HandleError(ex); }
            return ID;
        }

        // 3. تحديث طلب موجود
        public static bool UpdateApplication(int ApplicationID, int ApplicantPersonID, DateTime ApplicationDate,
            int ApplicationTypeID, byte ApplicationStatus, float PaidFees, int CreatedByUserID)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"UPDATE Applications SET ApplicantPersonID = @ApplicantPersonID, 
                                     ApplicationDate = @ApplicationDate, ApplicationTypeID = @ApplicationTypeID, 
                                     ApplicationStatus = @ApplicationStatus, PaidFees = @PaidFees, 
                                     CreatedByUserID = @CreatedByUserID WHERE ApplicationID = @ID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ApplicationID);
                        command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
                        command.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
                        command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                        command.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
                        command.Parameters.AddWithValue("@PaidFees", PaidFees);
                        command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0) OnDataChanged?.Invoke(ApplicationID, enChangeType.Update);
                    }
                }
            }
            catch (Exception ex) { _HandleError(ex); }
            return (rowsAffected > 0);
        }

        // 4. حذف طلب
        public static bool DeleteApplication(int ApplicationID)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "DELETE FROM Applications WHERE ApplicationID = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ApplicationID);
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0) OnDataChanged?.Invoke(ApplicationID, enChangeType.Delete);
                    }
                }
            }
            catch (Exception ex) { _HandleError(ex); }
            return (rowsAffected > 0);
        }

        // 5. التحقق من وجود الطلب
        public static bool IsApplicationExist(int ApplicationID)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT Found=1 FROM Applications WHERE ApplicationID = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ApplicationID);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                            isFound = reader.HasRows;
                    }
                }
            }
            catch (Exception ex) { _HandleError(ex); }
            return isFound;
        }

        // 6. جلب كافة الطلبات (مع FullName)
        public static DataTable GetAllApplications()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT Applications.ApplicationID, People.FullName, 
                                    ApplicationTypes.ApplicationTypeTitle AS [Type], Applications.ApplicationDate, 
                                    CASE WHEN Applications.ApplicationStatus = 1 THEN 'New'
                                         WHEN Applications.ApplicationStatus = 2 THEN 'Cancelled'
                                         WHEN Applications.ApplicationStatus = 3 THEN 'Completed'
                                    END AS Status, Applications.PaidFees, Users.UserName AS [Created By]
                                    FROM Applications 
                                    INNER JOIN People ON Applications.ApplicantPersonID = People.PersonID
                                    INNER JOIN ApplicationTypes ON Applications.ApplicationTypeID = ApplicationTypes.ApplicationTypeID
                                    INNER JOIN Users ON Applications.CreatedByUserID = Users.UserID
                                    ORDER BY Applications.ApplicationID DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                            dt.Load(reader);
                    }
                }
            }
            catch (Exception ex) { _HandleError(ex); }
            return dt;
        }

        // 7. تحديث الحالة فقط
        public static bool UpdateStatus(int ApplicationID, byte NewStatus)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "UPDATE Applications SET ApplicationStatus = @NewStatus WHERE ApplicationID = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ApplicationID);
                        command.Parameters.AddWithValue("@NewStatus", NewStatus);
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0) OnDataChanged?.Invoke(ApplicationID, enChangeType.StatusUpdate);
                    }
                }
            }
            catch (Exception ex) { _HandleError(ex); }
            return (rowsAffected > 0);
        }

        // 8. جلب معرف طلب نشط لنفس الشخص ونفس النوع
        public static int GetActiveApplicationID(int PersonID, int ApplicationTypeID)
        {
            int ActiveApplicationID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT ApplicationID FROM Applications 
                                     WHERE ApplicantPersonID = @PersonID AND ApplicationTypeID = @TypeID 
                                     AND ApplicationStatus = 1"; // 1 = New
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);
                        command.Parameters.AddWithValue("@TypeID", ApplicationTypeID);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int id))
                            ActiveApplicationID = id;
                    }
                }
            }
            catch (Exception ex) { _HandleError(ex); }
            return ActiveApplicationID;
        }

        // 9. التحقق من وجود تبعيات (مثلاً في جدول الاستعارات)
        public static bool DoesApplicationHaveAnyDependency(int ApplicationID)
        {
            bool hasDependency = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    // ملاحظة: تأكد من اسم جدول BorrowingRecords في قاعدتك
                    string query = @"SELECT TOP 1 1 FROM BorrowingRecords WHERE ApplicationID = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ApplicationID);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        hasDependency = (result != null);
                    }
                }
            }
            catch (Exception ex) { _HandleError(ex); }
            return hasDependency;
        }
    }
}