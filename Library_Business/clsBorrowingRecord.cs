using System;
using System.Data;
using Library_DataAccess;
using Library_Common; // استدعاء نظام التسجيل الموحد

namespace Library_Business
{
    public class clsBorrowingRecord
    {
        // 1. الإيفنتات (Events) للتواصل مع الواجهات (UI)
        public static event Action<string, int> OnDataChanged;
        public static event Action<string> OnError;

        // 2. ربط الإيفنتات تلقائياً مع طبقة الداتا (Static Constructor)
        static clsBorrowingRecord()
        {
            // ربط أحداث طبقة الداتا بأحداث طبقة البيزنس (Bridge) وتوثيق أخطاء الداتا لير
            clsBorrowingRecordData.OnDataChanged += (action, id) => OnDataChanged?.Invoke(action, id);

            clsBorrowingRecordData.OnError += (msg) =>
            {
                EventLogger.Log($"Data Layer Error in BorrowingRecord: {msg}", "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke(msg);
            };
        }

        // 3. الخصائص (Properties)
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int BorrowID { get; private set; }
        public int ApplicationID { get; set; }
        public int BookID { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }

        // 4. استخدام Delegates للربط مع طبقة الداتا (Expert Approach)
        private static Predicate<int> _IsAvailable = clsBorrowingRecordData.IsBookAvailableForBorrow;
        private static Func<int, decimal> _CalculateFine = clsBorrowingRecordData.GetTotalFinesForRecord;

        // 5. المنشئات (Constructors)
        public clsBorrowingRecord()
        {
            this.BorrowID = -1;
            this.ApplicationID = -1;
            this.BookID = -1;
            this.DueDate = DateTime.Now;
            this.ActualReturnDate = null;
            Mode = enMode.AddNew;
        }

        private clsBorrowingRecord(int BorrowID, int ApplicationID, int BookID,
                                   DateTime DueDate, DateTime? ActualReturnDate)
        {
            this.BorrowID = BorrowID;
            this.ApplicationID = ApplicationID;
            this.BookID = BookID;
            this.DueDate = DueDate;
            this.ActualReturnDate = ActualReturnDate;
            Mode = enMode.Update;
        }

        // 6. العمليات الأساسية (Operations)

        public static clsBorrowingRecord Find(int BorrowID)
        {
            try
            {
                int ApplicationID = -1, BookID = -1;
                DateTime DueDate = DateTime.Now;
                DateTime? ActualReturnDate = null;

                if (clsBorrowingRecordData.GetBorrowingInfoByID(BorrowID, ref ApplicationID, ref BookID, ref DueDate, ref ActualReturnDate))
                    return new clsBorrowingRecord(BorrowID, ApplicationID, BookID, DueDate, ActualReturnDate);
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Business Layer Find Error (BorrowID:{BorrowID}): {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
            }
            return null;
        }

        private bool _AddNew()
        {
            // قبل الإضافة، نتحقق من توفر الكتاب باستخدام الـ Predicate
            if (!_IsAvailable(this.BookID))
            {
                string msg = $"Borrowing Denied: Book ID {this.BookID} is already borrowed.";
                EventLogger.Log(msg, "WARN", EventLogger.LogTarget.TextFile);
                OnError?.Invoke("الكتاب مستعار حالياً ولا يمكن إتمام العملية.");
                return false;
            }

            this.BorrowID = clsBorrowingRecordData.AddNewBorrowingRecord(this.ApplicationID, this.BookID, this.DueDate);
            return (this.BorrowID != -1);
        }

        private bool _Update()
        {
            return clsBorrowingRecordData.UpdateBorrowingRecord(this.BorrowID, this.ApplicationID, this.BookID, this.DueDate, this.ActualReturnDate);
        }

        public bool Save()
        {
            try
            {
                switch (Mode)
                {
                    case enMode.AddNew:
                        if (_AddNew())
                        {
                            EventLogger.Log($"New Borrowing Record Created - ID: {this.BorrowID} (BookID: {this.BookID})", "INFO", EventLogger.LogTarget.TextFile);
                            Mode = enMode.Update;
                            return true;
                        }
                        break;

                    case enMode.Update:
                        if (_Update())
                        {
                            EventLogger.Log($"Borrowing Record Updated - ID: {this.BorrowID}", "INFO", EventLogger.LogTarget.TextFile);
                            return true;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Business Layer Save Error (Borrowing): {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
            }
            return false;
        }

        // 7. دوال العرض والبحث الثابتة (Static Methods)
        public static DataTable GetAllBorrowingRecords() => clsBorrowingRecordData.GetAllBorrowingRecords();

        public static DataTable Search(string SearchText) => clsBorrowingRecordData.SearchBorrowingRecords(SearchText);

        // 8. دوال المنطق الخاص (Special Business Logic)

        // تسجيل إرجاع الكتاب
        public bool ReturnBook()
        {
            try
            {
                this.ActualReturnDate = DateTime.Now;
                bool result = clsBorrowingRecordData.RegisterBookReturn(this.BorrowID, (DateTime)this.ActualReturnDate);

                if (result)
                {
                    decimal fine = GetCurrentFine();
                    EventLogger.Log($"Book Returned - BorrowID: {this.BorrowID}, Total Fine: {fine:C}", "INFO", EventLogger.LogTarget.TextFile);
                }
                return result;
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Error Registering Return (ID:{this.BorrowID}): {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
                return false;
            }
        }

        // حساب الغرامة الحالية باستخدام الـ Func المرتبط بطبقة الداتا
        public decimal GetCurrentFine()
        {
            try
            {
                return _CalculateFine(this.BorrowID);
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Error Calculating Fine (ID:{this.BorrowID}): {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
                return 0;
            }
        }
    }
}