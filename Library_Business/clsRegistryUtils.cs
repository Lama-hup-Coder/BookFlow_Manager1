using System;
using Microsoft.Win32;
using Library_Common; // استدعاء نظام التوثيق المشترك

namespace Library_Business
{
    /// <summary>
    /// كلاس مستقل لإدارة عمليات الريجستري (حفظ، قراءة، حذف) لمشروع المكتبة
    /// </summary>
    public static class clsRegistryUtils
    {
        // إضافة حدث لتبليغ الأخطاء للواجهات
        public static event Action<string> OnError;

        // المسار الخاص بمشروع المكتبة في الريجستري
        private static string _basePath = @"SOFTWARE\Library_Project";

        /// <summary>
        /// حفظ قيمة نصية في الريجستري
        /// </summary>
        public static bool SaveValue(string valueName, string valueData)
        {
            try
            {
                // الحفظ تحت مستخدم الجهاز الحالي لضمان الصلاحيات
                Registry.SetValue($@"HKEY_CURRENT_USER\{_basePath}", valueName, valueData, RegistryValueKind.String);

                // توثيق النجاح
                EventLogger.Log($"Registry Value Saved: [{valueName}]", "INFO", EventLogger.LogTarget.TextFile);
                return true;
            }
            catch (Exception ex)
            {
                string errorMsg = $"Registry Save Error ({valueName}): " + ex.Message;
                EventLogger.Log(errorMsg, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke(errorMsg);
                return false;
            }
        }

        /// <summary>
        /// قراءة قيمة من الريجستري
        /// </summary>
        public static string ReadValue(string valueName)
        {
            try
            {
                object data = Registry.GetValue($@"HKEY_CURRENT_USER\{_basePath}", valueName, null);

                if (data != null)
                {
                    return data.ToString();
                }
                else
                {
                    EventLogger.Log($"Registry Read: Value [{valueName}] not found.", "WARN", EventLogger.LogTarget.TextFile);
                    return null;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = $"Registry Read Error ({valueName}): " + ex.Message;
                EventLogger.Log(errorMsg, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke(errorMsg);
                return null;
            }
        }

        /// <summary>
        /// حذف قيمة محددة نهائياً من الريجستري
        /// </summary>
        public static bool DeleteValue(string valueName)
        {
            try
            {
                // فتح المفتاح الأساسي بمعمارية 64-بت لضمان الوصول للمسار الصحيح
                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                {
                    using (RegistryKey key = baseKey.OpenSubKey(_basePath, true))
                    {
                        if (key != null)
                        {
                            key.DeleteValue(valueName);
                            EventLogger.Log($"Registry Value Deleted: [{valueName}]", "INFO", EventLogger.LogTarget.TextFile);
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                string errorMsg = $"Registry Delete Error ({valueName}): " + ex.Message;
                EventLogger.Log(errorMsg, "ERROR", EventLogger.LogTarget.Both);
                OnError?.Invoke(errorMsg);
                return false;
            }
        }
    }
}