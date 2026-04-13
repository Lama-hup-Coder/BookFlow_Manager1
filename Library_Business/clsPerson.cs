using System;
using System.Data;
using Library_DataAccess;
using Library_Common; // استدعاء نظام التوثيق المشترك

namespace Library_Business
{
    public class clsPerson
    {
        // 1. الأيفنتات لتبليغ الواجهات بتغير البيانات أو حدوث خطأ
        public static event Action<string, int> OnDataChanged;
        public static event Action<string> OnError;

        // 2. الربط التلقائي مع طبقة البيانات (Static Constructor)
        static clsPerson()
        {
            // ربط إيفنتات الداتا لير بإيفنتات البيزنس لير وتوثيق أخطاء الداتا لير
            clsPersonData.OnDataChanged += (action, id) => OnDataChanged?.Invoke(action, id);

            clsPersonData.OnError += (errorMessage) =>
            {
                EventLogger.Log($"Data Layer Error in clsPerson: {errorMessage}", "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke(errorMessage);
            };
        }

        // 3. الأوضاع (Modes)
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        // 4. الخصائص (Properties)
        public int PersonID { get; set; }
        public string NationalNo { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public short Gendor { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        // 5. المشيد الافتراضي (لإضافة شخص جديد)
        public clsPerson()
        {
            this.PersonID = -1;
            this.NationalNo = "";
            this.FullName = "";
            this.DateOfBirth = DateTime.Now;
            this.Gendor = 0;
            this.Phone = "";
            this.Email = "";
            this.Address = "";
            Mode = enMode.AddNew;
        }

        // 6. المشيد الخاص (لجلب بيانات شخص موجود)
        private clsPerson(int PersonID, string NationalNo, string FullName, DateTime DateOfBirth,
            short Gendor, string Phone, string Email, string Address)
        {
            this.PersonID = PersonID;
            this.NationalNo = NationalNo;
            this.FullName = FullName;
            this.DateOfBirth = DateOfBirth;
            this.Gendor = Gendor;
            this.Phone = Phone;
            this.Email = Email;
            this.Address = Address;
            Mode = enMode.Update;
        }

        // --- [ الدوال الاستاتيكية للبحث والجلب ] ---

        public static clsPerson Find(int ID)
        {
            try
            {
                string NationalNo = "", FullName = "", Phone = "", Email = "", Address = "";
                DateTime DateOfBirth = DateTime.Now;
                short Gendor = 0;

                if (clsPersonData.GetPersonInfoByID(ID, ref NationalNo, ref FullName, ref DateOfBirth,
                    ref Gendor, ref Phone, ref Email, ref Address))
                {
                    return new clsPerson(ID, NationalNo, FullName, DateOfBirth, Gendor, Phone, Email, Address);
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Business Layer Find Person Error (ID:{ID}): {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
            }
            return null;
        }

        public static clsPerson Find(string NationalNo)
        {
            try
            {
                int PersonID = -1;
                string FullName = "", Phone = "", Email = "", Address = "";
                DateTime DateOfBirth = DateTime.Now;
                short Gendor = 0;

                if (clsPersonData.GetPersonByNationalNo(NationalNo, ref PersonID, ref FullName, ref DateOfBirth,
                    ref Gendor, ref Phone, ref Email, ref Address))
                {
                    return new clsPerson(PersonID, NationalNo, FullName, DateOfBirth, Gendor, Phone, Email, Address);
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Business Layer Find Person Error (NationalNo:{NationalNo}): {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
            }
            return null;
        }

        public static object GetProperty(int ID, string PropertyName)
        {
            return clsPersonData.GetColumnValue(ID, PropertyName);
        }

        // --- [ دوال العمليات العامة ] ---

        public static DataTable GetAllPeople() => clsPersonData.GetAllPeople();

        public static DataTable Search(string FilterValue) => clsPersonData.SearchPeople(FilterValue);

        public static bool IsExist(int ID) => clsPersonData.IsPersonExist(ID);

        public static int Count() => clsPersonData.GetTotalCount();

        public static bool Delete(int ID)
        {
            try
            {
                bool result = clsPersonData.DeletePerson(ID);
                if (result)
                    EventLogger.Log($"Person Deleted - ID: {ID}", "INFO", EventLogger.LogTarget.TextFile);
                return result;
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Business Layer Delete Person Error (ID:{ID}): {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
                return false;
            }
        }

        // --- [ منطق الحفظ الخاص - Private Methods ] ---

        private bool _AddNew()
        {
            this.PersonID = clsPersonData.AddNewPerson(this.NationalNo, this.FullName, this.DateOfBirth,
                this.Gendor, this.Phone, this.Email, this.Address);
            return (this.PersonID != -1);
        }

        private bool _Update()
        {
            return clsPersonData.UpdatePerson(this.PersonID, this.NationalNo, this.FullName, this.DateOfBirth,
                this.Gendor, this.Phone, this.Email, this.Address);
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
                            EventLogger.Log($"New Person Added: {this.FullName} (ID: {this.PersonID})", "INFO", EventLogger.LogTarget.TextFile);
                            Mode = enMode.Update;
                            return true;
                        }
                        return false;
                    case enMode.Update:
                        if (_Update())
                        {
                            EventLogger.Log($"Person Updated: {this.FullName} (ID: {this.PersonID})", "INFO", EventLogger.LogTarget.TextFile);
                            return true;
                        }
                        return false;
                }
            }
            catch (Exception ex)
            {
                EventLogger.Log($"Business Layer Save Person Error: {ex.Message}", "ERROR", EventLogger.LogTarget.Both);
            }
            return false;
        }
    }
}