using System;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Library_DataAccess;
using Library_Common; // استدعاء نظام التسجيل الموحد

namespace Library_Business
{
    public class clsApplicationType
    {
        // --- [1. الأتريبيوتس الذكية] ---
        [AttributeUsage(AttributeTargets.Property)]
        public class ColumnInfoAttribute : Attribute
        {
            public string ColumnName { get; }
            public bool IsPrimaryKey { get; }
            public ColumnInfoAttribute(string columnName, bool isPrimaryKey = false)
            {
                ColumnName = columnName;
                IsPrimaryKey = isPrimaryKey;
            }
        }

        [AttributeUsage(AttributeTargets.Property)]
        public class RequiredFieldAttribute : Attribute
        {
            public string ErrorMessage { get; }
            public RequiredFieldAttribute(string errorMessage) => ErrorMessage = errorMessage;
        }

        // --- [2. التخزين المؤقت للريفليكشن - Caching] ---
        private static readonly PropertyInfo[] _propertiesCache = typeof(clsApplicationType).GetProperties();

        // --- [3. الأحداث - Events] ---
        public static event Action<string> OnError;
        public event Action<clsApplicationType> OnRecordChanged;
        public static event Action<int> OnRecordDeleted;

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        // --- [4. الخصائص مع الأتريبيوتس] ---
        [ColumnInfo("ApplicationTypeID", isPrimaryKey: true)]
        public int ID { get; set; }

        [RequiredField("Title is required and cannot be empty")]
        [ColumnInfo("ApplicationTypeTitle")]
        public string Title { get; set; }

        [RequiredField("Fees must be 0 or greater")]
        [ColumnInfo("ApplicationFees")]
        public float Fees { get; set; }

        // --- [5. الربط التلقائي للأخطاء عبر الـ Static Constructor] ---
        static clsApplicationType()
        {
            clsApplicationTypeData.OnError += (dbErrorMessage) =>
            {
                // تسجيل خطأ الداتا لير في اللوك قبل رفعه للبزنس
                EventLogger.Log("Data Layer Error in clsApplicationType: " + dbErrorMessage, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke("Data Layer Error: " + dbErrorMessage);
            };
        }

        // --- [6. محرك التحقق الذكي - Validation Engine] ---
        public bool Validate(out string errorMessage)
        {
            errorMessage = "";
            try
            {
                // فحص منع تكرار العنوان
                if (Mode == enMode.AddNew || (Mode == enMode.Update && _IsTitleChanged()))
                {
                    if (clsApplicationTypeData.IsTitleExist(this.Title))
                    {
                        errorMessage = "This Application Title already exists!";
                        // تسجيل محاولة تكرار العنوان كتحذير
                        EventLogger.Log($"Duplicate Title Attempt: {this.Title}", "WARN", EventLogger.LogTarget.TextFile);
                        return false;
                    }
                }

                // فحص الحقول المطلوبة باستخدام Reflection
                foreach (PropertyInfo property in _propertiesCache)
                {
                    var attribute = property.GetCustomAttribute<RequiredFieldAttribute>();
                    if (attribute != null)
                    {
                        var value = property.GetValue(this);

                        if (value is string stringValue && string.IsNullOrWhiteSpace(stringValue))
                        {
                            errorMessage = attribute.ErrorMessage;
                            return false;
                        }

                        if (value is float floatValue && floatValue < 0)
                        {
                            errorMessage = attribute.ErrorMessage;
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Validation Engine Exception: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
            }
            return true;
        }

        private bool _IsTitleChanged()
        {
            string oldTitle = clsApplicationTypeData.GetTitle(this.ID);
            return (this.Title != oldTitle);
        }

        // --- [7. العمليات الأساسية - CRUD Operations] ---

        public clsApplicationType()
        {
            this.ID = -1;
            this.Title = "";
            this.Fees = 0;
            Mode = enMode.AddNew;
        }

        private clsApplicationType(int ID, string Title, float Fees)
        {
            this.ID = ID;
            this.Title = Title;
            this.Fees = Fees;
            Mode = enMode.Update;
        }

        public bool Save()
        {
            if (!Validate(out string error)) return false;

            bool success = false;
            try
            {
                switch (Mode)
                {
                    case enMode.AddNew:
                        success = _AddNew();
                        if (success) EventLogger.Log($"AppType Added: {this.Title} (ID:{this.ID})", "INFO", EventLogger.LogTarget.TextFile);
                        break;
                    case enMode.Update:
                        success = _Update();
                        if (success) EventLogger.Log($"AppType Updated: {this.Title} (ID:{this.ID})", "INFO", EventLogger.LogTarget.TextFile);
                        break;
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Save Operation Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
            }

            if (success) OnRecordChanged?.Invoke(this);
            return success;
        }

        private bool _AddNew()
        {
            this.ID = clsApplicationTypeData.AddNewApplicationType(this.Title, this.Fees);
            if (this.ID != -1)
            {
                Mode = enMode.Update;
                return true;
            }
            return false;
        }

        private bool _Update()
        {
            return clsApplicationTypeData.UpdateApplicationType(this.ID, this.Title, this.Fees);
        }

        public static clsApplicationType Find(int ID)
        {
            try
            {
                string Title = "";
                float Fees = 0;
                if (clsApplicationTypeData.GetApplicationTypeInfoByID(ID, ref Title, ref Fees))
                    return new clsApplicationType(ID, Title, Fees);
            }
            catch (Exception ex)
            {
                EventLogger.Log("Find ApplicationType Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
            }
            return null;
        }

        public static bool Delete(int ID)
        {
            try
            {
                // منع الحذف إذا كان هناك سجلات مرتبطة
                if (clsApplicationTypeData.DoesApplicationTypeHaveAnyDependency(ID))
                {
                    EventLogger.Log($"Delete Denied: AppType ID {ID} has dependencies.", "WARN", EventLogger.LogTarget.TextFile);
                    return false;
                }

                if (clsApplicationTypeData.DeleteApplicationType(ID))
                {
                    EventLogger.Log($"AppType Deleted - ID: {ID}", "INFO", EventLogger.LogTarget.TextFile);
                    OnRecordDeleted?.Invoke(ID);
                    return true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log("Delete ApplicationType Error: " + ex.Message, "ERROR", EventLogger.LogTarget.Both);
            }
            return false;
        }

        // --- [8. دوال مساعدة إضافية] ---

        public static DataTable GetAll()
            => clsApplicationTypeData.GetAllApplicationTypes();

        public static bool IsExist(int ID)
            => clsApplicationTypeData.IsApplicationTypeExist(ID);

        public static DataTable Search(string PartialTitle)
            => clsApplicationTypeData.GetAllApplicationTypesByPartialTitle(PartialTitle);
    }
}