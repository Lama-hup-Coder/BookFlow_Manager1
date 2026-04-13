using System;
using System.Data;
using Library_Business;

namespace Library_TestConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "BookFlowDB Application Test System";
            Console.WriteLine("=== نظام اختبار الطلبات (Applications) ===\n");

            // 1. اختبار إضافة طلب جديد
            TestAddNewApplication();

            Console.WriteLine("\n-------------------------------------------");

            // 2. اختبار جلب كافة البيانات وعرضها
            TestGetAllApplications();

            Console.WriteLine("\nانتهى الاختبار. اضغط أي مفتاح للخروج...");
            Console.ReadKey();
        }

        static void TestAddNewApplication()
        {
            Console.WriteLine("[الاختبار 1]: محاولة إضافة طلب جديد...");

            clsApplication app = new clsApplication();

            // تأكد أن هذه المعرفات (1) موجودة فعلياً في جداول People و Users و ApplicationTypes
            app.ApplicantPersonID = 1;
            app.ApplicationDate = DateTime.Now;
            app.ApplicationTypeID = 1;
            app.ApplicationStatus = clsApplication.enApplicationStatus.New;
            app.PaidFees = 15.5f;
            app.CreatedByUserID = 1;

            if (app.Save())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[نجاح]: تم حفظ الطلب بنجاح! المعرف الجديد هو: {app.ApplicationID}");
                Console.ResetColor();

                // اختبار تحديث الحالة فوراً
                if (app.UpdateStatus(clsApplication.enApplicationStatus.Completed))
                {
                    Console.WriteLine("[تحديث]: تم تغيير حالة الطلب إلى 'مكتمل' بنجاح.");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[فشل]: لم يتم حفظ الطلب. تحقق من سجل الأخطاء (Log).");
                Console.ResetColor();
            }
        }

        static void TestGetAllApplications()
        {
            Console.WriteLine("[الاختبار 2]: جلب عرض كافة الطلبات من قاعدة البيانات...");

            DataTable dt = clsApplication.GetAllApplications();

            if (dt.Rows.Count > 0)
            {
                Console.WriteLine($"{"ID",-5} | {"Full Name",-20} | {"Type",-15} | {"Status",-10} | {"Fees",-7}");
                Console.WriteLine(new string('-', 65));

                foreach (DataRow row in dt.Rows)
                {
                    Console.WriteLine($"{row["ApplicationID"],-5} | " +
                                      $"{row["FullName"],-20} | " +
                                      $"{row["Type"],-15} | " +
                                      $"{row["Status"],-10} | " +
                                      $"{row["PaidFees"],-7}");
                }
            }
            else
            {
                Console.WriteLine("لا توجد بيانات لعرضها.");
            }
        }
    }
}