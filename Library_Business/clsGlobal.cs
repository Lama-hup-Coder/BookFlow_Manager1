using System;

namespace Library_Business
{
    public static class clsGlobal
    {
        // تخزين كائن المستخدم الذي سجل دخوله بنجاح
        // هذا يغنينا عن إرسال بيانات المستخدم بين الفورمز
        public static clsUser CurrentUser;

        /// <summary>
        /// دالة تسجيل الخروج وتصفير البيانات
        /// </summary>
        public static void Logout()
        {
            CurrentUser = null;
        }
    }
}