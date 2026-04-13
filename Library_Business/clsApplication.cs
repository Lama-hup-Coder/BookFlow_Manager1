using Library_DataAccess;
using System;
using System.Data;
using System.Reflection;
using Library_Common;

namespace Library_Business
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredFieldAttribute : Attribute
    {
        public string ErrorMessage { get; }
        public RequiredFieldAttribute(string errorMessage) => ErrorMessage = errorMessage;
    }

    public class clsApplication
    {
        // --- Events & Enums ---
        public event Action<int> OnApplicationSaved;
        public static event Action<string> OnError;

        public enum enMode { NewMode = 0, UpdateMode = 1 };
        public enum enApplicationStatus { New = 1, Cancelled = 2, Completed = 3 };

        // 2. Properties
        public int ApplicationID { get; set; }

        [RequiredField("يجب تحديد صاحب الطلب")]
        public int ApplicantPersonID { get; set; }

        public DateTime ApplicationDate { get; set; }

        [RequiredField("يجب تحديد نوع الطلب")]
        public int ApplicationTypeID { get; set; }

        public enApplicationStatus ApplicationStatus { get; set; }

        // [تعديل]: تم حذف LastStatusDate لعدم وجود عمود مطابق في قاعدة البيانات BookFlowDB
        // وتم استبداله بمنطق داخلي إذا لزم الأمر في الواجهات.

        public float PaidFees { get; set; }

        [RequiredField("يجب تحديد المستخدم المسؤول")]
        public int CreatedByUserID { get; set; }

        public enMode Mode { get; set; } = enMode.NewMode;

        // --- الربط الكائني (Composition) ---
        public clsApplicationType ApplicationTypeInfo { get; private set; }
        public clsPerson PersonInfo { get; private set; }
        public clsMember MemberInfo { get; private set; }
        public clsUser UserInfo { get; private set; }

        // 3. Constructors
        public clsApplication()
        {
            this.ApplicationID = -1;
            this.ApplicantPersonID = -1;
            this.ApplicationDate = DateTime.Now;
            this.ApplicationTypeID = -1;
            this.ApplicationStatus = enApplicationStatus.New;
            this.PaidFees = 0;
            this.CreatedByUserID = -1;
            this.Mode = enMode.NewMode;
        }

        private clsApplication(int ApplicationID, int ApplicantPersonID, DateTime ApplicationDate,
            int ApplicationTypeID, byte ApplicationStatus, float PaidFees, int CreatedByUserID)
        {
            this.ApplicationID = ApplicationID;
            this.ApplicantPersonID = ApplicantPersonID;
            this.ApplicationDate = ApplicationDate;
            this.ApplicationTypeID = ApplicationTypeID;
            this.ApplicationStatus = (enApplicationStatus)ApplicationStatus;
            this.PaidFees = PaidFees;
            this.CreatedByUserID = CreatedByUserID;

            // الربط الكائني - يفضل استدعاء الـ Find عند الحاجة لتقليل الضغط على الـ DB
            this.PersonInfo = clsPerson.Find(this.ApplicantPersonID);
            this.ApplicationTypeInfo = clsApplicationType.Find(this.ApplicationTypeID);
            this.UserInfo = clsUser.FindByUserID(this.CreatedByUserID);
            this.MemberInfo = clsMember.Find(this.ApplicantPersonID);

            this.Mode = enMode.UpdateMode;
        }

        // 4. Validation Engine (Using Reflection)
        public bool Validate(out string errorMessage)
        {
            errorMessage = "";
            try
            {
                PropertyInfo[] properties = this.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    var attribute = property.GetCustomAttribute<RequiredFieldAttribute>();
                    if (attribute != null)
                    {
                        var value = property.GetValue(this);
                        if (value is int intValue && intValue <= 0)
                        {
                            errorMessage = attribute.ErrorMessage;
                            EventLogger.Log($"Validation Failed: {errorMessage} for AppID: {this.ApplicationID}", "WARN", EventLogger.LogTarget.TextFile);
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Validation Engine Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
            }
            return true;
        }

        // 5. Save & Data Operations
        public bool Save()
        {
            if (!Validate(out string error))
            {
                OnError?.Invoke(error);
                return false;
            }

            try
            {
                switch (Mode)
                {
                    case enMode.NewMode:
                        if (_AddNewApplication())
                        {
                            Mode = enMode.UpdateMode;
                            EventLogger.Log($"Application Created Successfully - ID: {this.ApplicationID}", "INFO", EventLogger.LogTarget.TextFile);
                            OnApplicationSaved?.Invoke(this.ApplicationID);
                            return true;
                        }
                        break;

                    case enMode.UpdateMode:
                        if (_UpdateApplication())
                        {
                            EventLogger.Log($"Application Updated Successfully - ID: {this.ApplicationID}", "INFO", EventLogger.LogTarget.TextFile);
                            return true;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Business Layer Save Error: {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("حدث خطأ داخلي أثناء حفظ الطلب.");
            }
            return false;
        }

        private bool _AddNewApplication()
        {
            // [تعديل]: تمرير 6 بارامترات فقط لتطابق الـ Data Layer الجديدة
            this.ApplicationID = clsApplicationData.AddNewApplication(
                this.ApplicantPersonID,
                this.ApplicationDate,
                this.ApplicationTypeID,
                (byte)this.ApplicationStatus,
                this.PaidFees,
                this.CreatedByUserID);

            return (this.ApplicationID != -1);
        }

        private bool _UpdateApplication()
        {
            // [تنبيه]: تأكد أن UpdateApplication في الداتا لير لا تطلب LastStatusDate
            return clsApplicationData.UpdateApplication(
                this.ApplicationID,
                this.ApplicantPersonID,
                this.ApplicationDate,
                this.ApplicationTypeID,
                (byte)this.ApplicationStatus,
                this.PaidFees,
                this.CreatedByUserID);
        }

        // 6. Static Methods
        public static clsApplication Find(int ApplicationID)
        {
            try
            {
                int ApplicantPersonID = -1, ApplicationTypeID = -1, CreatedByUserID = -1;
                DateTime ApplicationDate = DateTime.Now;
                byte ApplicationStatus = 1;
                float PaidFees = 0;

                // [تعديل]: استدعاء الدالة بدون ريفرنس لـ LastStatusDate
                if (clsApplicationData.GetApplicationInfoByID(ApplicationID, ref ApplicantPersonID,
                    ref ApplicationDate, ref ApplicationTypeID, ref ApplicationStatus,
                    ref PaidFees, ref CreatedByUserID))
                {
                    return new clsApplication(ApplicationID, ApplicantPersonID, ApplicationDate,
                        ApplicationTypeID, ApplicationStatus, PaidFees, CreatedByUserID);
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Business Layer Find Error: {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
            }
            return null;
        }

        public static DataTable GetAllApplications() => clsApplicationData.GetAllApplications();

        public static bool Delete(int ID)
        {
            bool result = clsApplicationData.DeleteApplication(ID);
            if (result) EventLogger.Log($"Application Deleted - ID: {ID}", "INFO", EventLogger.LogTarget.TextFile);
            return result;
        }

        public static bool IsExist(int ID) => clsApplicationData.IsApplicationExist(ID);

        // 7. Instance Operations
        public bool UpdateStatus(enApplicationStatus NewStatus)
        {
            try
            {
                if (clsApplicationData.UpdateStatus(this.ApplicationID, (byte)NewStatus))
                {
                    EventLogger.Log($"Application Status Changed to {NewStatus} - ID: {this.ApplicationID}", "INFO", EventLogger.LogTarget.TextFile);
                    this.ApplicationStatus = NewStatus;
                    return true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Status Update Business Error: {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
            }
            return false;
        }
    }
}