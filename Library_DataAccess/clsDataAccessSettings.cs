using System;
using System.Configuration; // تأكدي من بقاء هذا السطر

namespace Library_DataAccess
{
    public static class clsDataAccessSettings
    {
        // السطر الجديد الذي يقرأ من ملف الإعدادات الخارجي
        public static string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["BookFlowDbString"].ConnectionString;
    }
}