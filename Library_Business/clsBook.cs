using System;
using System.Data;
using Library_DataAccess;
using Library_Common; // استدعاء نظام التسجيل الموحد

namespace Library_Business
{
    public class clsBook
    {
        // 1. الأوضاع (Modes)
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        // 2. الخصائص (Properties)
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int CategoryID { get; set; }
        public bool IsAvailable { get; set; }

        // 3. الأحداث (Events)
        public static event Action<string, int> OnDataChanged;
        public static event Action<string> OnError;

        // 4. الربط الجوهري (Static Constructor)
        static clsBook()
        {
            // ربط أحداث طبقة الداتا بأحداث طبقة البزنس وتوثيق أخطاء الداتا لير فوراً
            clsBookData.OnDataChanged += (action, id) => OnDataChanged?.Invoke(action, id);

            clsBookData.OnError += (msg) =>
            {
                EventLogger.Log($"Data Layer Error in clsBook: {msg}", "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke(msg);
            };
        }

        // --- المنشئات (Constructors) ---

        public clsBook()
        {
            this.BookID = -1;
            this.Title = "";
            this.Author = "";
            this.ISBN = "";
            this.CategoryID = -1;
            this.IsAvailable = true;
            this.Mode = enMode.AddNew;
        }

        private clsBook(int BookID, string Title, string Author, string ISBN, int CategoryID, bool IsAvailable)
        {
            this.BookID = BookID;
            this.Title = Title;
            this.Author = Author;
            this.ISBN = ISBN;
            this.CategoryID = CategoryID;
            this.IsAvailable = IsAvailable;
            this.Mode = enMode.Update;
        }

        // --- العمليات الثابتة (Static Methods) ---

        public static clsBook Find(int BookID)
        {
            try
            {
                string Title = "", Author = "", ISBN = "";
                int CategoryID = -1;
                bool IsAvailable = false;

                if (clsBookData.GetBookInfoByID(BookID, ref Title, ref Author, ref ISBN, ref CategoryID, ref IsAvailable))
                    return new clsBook(BookID, Title, Author, ISBN, CategoryID, IsAvailable);
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Business Layer Find Error (ID:{BookID}): {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
            }
            return null;
        }

        public static DataTable GetAllBooks()
        {
            return clsBookData.GetAllBooks();
        }

        public static DataTable GetAllBooksByCategory(int CategoryID)
        {
            return clsBookData.GetAllBooksByCategory(CategoryID);
        }

        public static bool IsExist(string ISBN)
        {
            return clsBookData.IsBookExist(ISBN);
        }


       
        public static int CountBooks()
        {
            // استدعاء دالة العد من طبقة الداتا لير
            return clsBookData.GetTotalBooksCount();
        }

        // تحقق عن طريق الـ ID (يستخدم غالباً قبل الحذف أو التعديل)
        public static bool IsExist(int BookID)
        {
            return clsBookData.IsBookExist(BookID);
        }
        public static bool Delete(int BookID)
        {
            try
            {
                bool result = clsBookData.DeleteBook(BookID);
                if (result)
                {
                    EventLogger.Log($"Book Deleted Successfully - ID: {BookID}", "INFO", EventLogger.LogTarget.TextFile);
                }
                return result;
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Business Layer Delete Error (ID:{BookID}): {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
                return false;
            }
        }

        public static bool CanDelete(int BookID)
        {
            return clsBookData.CanDeleteBook(BookID);
        }

        public static bool UpdateAvailability(int BookID, bool IsAvailable)
        {
            try
            {
                bool result = clsBookData.UpdateAvailability(BookID, IsAvailable);
                if (result)
                {
                    EventLogger.Log($"Book Availability Updated to {IsAvailable} - ID: {BookID}", "INFO", EventLogger.LogTarget.TextFile);
                }
                return result;
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Business Layer Availability Update Error: {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
                return false;
            }
        }

        // --- العمليات الداخلية (Private Methods) ---

        private bool _AddNewBook()
        {
            this.BookID = clsBookData.AddNewBook(this.Title, this.Author, this.ISBN, this.CategoryID, this.IsAvailable);
            return (this.BookID != -1);
        }

        private bool _UpdateBook()
        {
            return clsBookData.UpdateBook(this.BookID, this.Title, this.Author, this.ISBN, this.CategoryID, this.IsAvailable);
        }

        // --- دالة الحفظ الذكية (Save) ---

        public bool Save()
        {
            try
            {
                switch (Mode)
                {
                    case enMode.AddNew:
                        if (_AddNewBook())
                        {
                            EventLogger.Log($"New Book Added: {this.Title} (ID: {this.BookID})", "INFO", EventLogger.LogTarget.TextFile);
                            Mode = enMode.Update;
                            return true;
                        }
                        break;

                    case enMode.Update:
                        if (_UpdateBook())
                        {
                            EventLogger.Log($"Book Info Updated: {this.Title} (ID: {this.BookID})", "INFO", EventLogger.LogTarget.TextFile);
                            return true;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Business Layer Save Error: {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
            }
            return false;
        }
    }
}