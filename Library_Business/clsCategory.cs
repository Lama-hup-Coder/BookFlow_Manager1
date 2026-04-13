using System;
using System.Data;
using Library_DataAccess;
using Library_Common; // استدعاء نظام التسجيل الموحد

namespace Library_Business
{
    public class clsCategory
    {
        // 1. الأيفنتات (Events) للتواصل مع الطبقات الأعلى
        public static event Action<string, int> OnDataChanged;
        public static event Action<string> OnError;

        // 2. ربط الإيفنتات تلقائياً مع طبقة الداتا (Static Constructor)
        static clsCategory()
        {
            // ربط أحداث طبقة الداتا بأحداث طبقة البيزنس (Bridge) وتوثيق أخطاء الداتا لير
            clsCategoryData.OnDataChanged += (action, id) => OnDataChanged?.Invoke(action, id);

            clsCategoryData.OnError += (msg) =>
            {
                EventLogger.Log($"Data Layer Error in clsCategory: {msg}", "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke(msg);
            };
        }

        // 3. الأوضاع (Modes)
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        // 4. الخصائص (Properties)
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public short DefaultBorrowingDays { get; set; }
        public decimal LateFeePerDay { get; set; }

        // خاصية جلب الكتب التابعة لهذا التصنيف
        public DataTable Books
        {
            get
            {
                try
                {
                    return clsBook.GetAllBooksByCategory(this.CategoryID);
                }
                catch (Exception ex)
                {
                    EventLogger.Log($"Error fetching books for category {CategoryID}: {ex.Message}", "ERROR", EventLogger.LogTarget.TextFile);
                    return new DataTable();
                }
            }
        }

        // 5. المنشئ الافتراضي (Default Constructor)
        public clsCategory()
        {
            this.CategoryID = -1;
            this.CategoryName = "";
            this.DefaultBorrowingDays = 10;
            this.LateFeePerDay = 0;
            Mode = enMode.AddNew;
        }

        // 6. منشئ خاص للبيانات القادمة من قاعدة البيانات (Private Constructor)
        private clsCategory(int CategoryID, string CategoryName, short DefaultBorrowingDays, decimal LateFeePerDay)
        {
            this.CategoryID = CategoryID;
            this.CategoryName = CategoryName;
            this.DefaultBorrowingDays = DefaultBorrowingDays;
            this.LateFeePerDay = LateFeePerDay;
            Mode = enMode.Update;
        }

        // 7. دالة البحث (Find)
        public static clsCategory Find(int CategoryID)
        {
            try
            {
                string CategoryName = "";
                short DefaultBorrowingDays = 0;
                decimal LateFeePerDay = 0;

                if (clsCategoryData.GetCategoryByID(CategoryID, ref CategoryName, ref DefaultBorrowingDays, ref LateFeePerDay))
                {
                    return new clsCategory(CategoryID, CategoryName, DefaultBorrowingDays, LateFeePerDay);
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Business Layer Find Category Error (ID:{CategoryID}): {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
            }
            return null;
        }

        // 8. الدوال المساعدة للحفظ (Internal Helpers)
        private bool _AddNewCategory()
        {
            this.CategoryID = clsCategoryData.AddNewCategory(this.CategoryName, this.DefaultBorrowingDays, this.LateFeePerDay);
            return (this.CategoryID != -1);
        }

        private bool _UpdateCategory()
        {
            return clsCategoryData.UpdateCategory(this.CategoryID, this.CategoryName, this.DefaultBorrowingDays, this.LateFeePerDay);
        }

        // 9. دالة الحفظ الذكية (Smart Save)
        public bool Save()
        {
            try
            {
                switch (Mode)
                {
                    case enMode.AddNew:
                        if (_AddNewCategory())
                        {
                            EventLogger.Log($"New Category Added: {this.CategoryName} (ID: {this.CategoryID})", "INFO", EventLogger.LogTarget.TextFile);
                            Mode = enMode.Update;
                            return true;
                        }
                        break;

                    case enMode.Update:
                        if (_UpdateCategory())
                        {
                            EventLogger.Log($"Category Settings Updated: {this.CategoryName} (ID: {this.CategoryID})", "INFO", EventLogger.LogTarget.TextFile);
                            return true;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Business Layer Save Category Error: {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
            }
            return false;
        }

        // 10. الدوال الثابتة (Static Methods)

        public static bool Delete(int CategoryID)
        {
            try
            {
                bool result = clsCategoryData.DeleteCategory(CategoryID);
                if (result)
                {
                    EventLogger.Log($"Category Deleted Successfully - ID: {CategoryID}", "INFO", EventLogger.LogTarget.TextFile);
                }
                return result;
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Business Layer Delete Category Error (ID:{CategoryID}): {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
                return false;
            }
        }

        public static DataTable GetAllCategories() => clsCategoryData.GetAllCategories();

        public static bool IsExist(string CategoryName) => clsCategoryData.IsCategoryExist(CategoryName);

        // دالة جلب ملخص التصنيفات مع عدد الكتب (الإحصائية)
        public static DataTable GetSummary()
        {
            try
            {
                return clsCategoryData.GetCategoriesSummary();
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Error fetching Categories Summary: {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
                return new DataTable();
            }
        }
    }
}