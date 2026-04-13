using System;
using System.IO;
using System.Diagnostics;

namespace Library_Common
{
    public static class EventLogger
    {
        // 1. الخيارات المتاحة للمبرمج لتحديد مكان التسجيل
        public enum LogTarget
        {
            TextFile,      // ملف نصي فقط (الأكثر أماناً)
            EventLog,      // سجل ويندوز (يحتاج صلاحيات)
            Both           // الاثنين معاً
        }

        // تحديد مسار ملف اللوج في نفس مجلد تشغيل البرنامج
        private static string _txtFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log.txt");
        private static string _sourceName = "LibraryProject";

        // 2. الدالة الرئيسية (نقطة الدخول لكل عمليات التسجيل)
        public static void Log(string message, string eventType = "INFO", LogTarget target = LogTarget.TextFile)
        {
            // ملاحظة: جعلنا القيمة الافتراضية TextFile لضمان النجاح دائماً في البداية
            switch (target)
            {
                case LogTarget.TextFile:
                    LogToTextFile(message, eventType);
                    break;

                case LogTarget.EventLog:
                    LogToWindowsEvent(message, eventType);
                    break;

                case LogTarget.Both:
                    LogToTextFile(message, eventType);
                    LogToWindowsEvent(message, eventType);
                    break;
            }
        }

        // 3. دالة الكتابة في الملف النصي (التي ستنشئ ملف Log.txt)
        private static void LogToTextFile(string message, string eventType)
        {
            try
            {
                string logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{eventType.ToUpper()}] {message}";

                // استخدام AppendAllText يضمن فتح الملف، الكتابة، ثم إغلاقه فوراً
                File.AppendAllText(_txtFilePath, logLine + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // إذا فشل الملف النصي، نطبع السبب في الكونسول مباشرة
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"[INTERNAL LOGGER ERROR]: Could not write to TextFile. {ex.Message}");
                Console.ResetColor();
            }
        }

        // 4. دالة الكتابة في سجل ويندوز (محمية ومنفصلة)
        private static void LogToWindowsEvent(string message, string eventType)
        {
            try
            {
                // محاولة الكتابة مباشرة؛ الويندوز سيتعامل مع السورس إذا كان موجوداً
                EventLogEntryType type = EventLogEntryType.Information;

                if (eventType.ToUpper() == "ERROR") type = EventLogEntryType.Error;
                if (eventType.ToUpper() == "WARNING") type = EventLogEntryType.Warning;

                EventLog.WriteEntry(_sourceName, message, type);
            }
            catch (Exception ex)
            {
                // إذا فشل سجل ويندوز بسبب الصلاحيات، نطبع تحذير ولا نعطل البرنامج
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"[LOG WARNING]: Windows Event Log skipped: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}