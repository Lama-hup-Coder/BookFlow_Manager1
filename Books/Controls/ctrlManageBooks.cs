using BookFlow_Manager1.Books.Forms;
using Library_Business;// تأكدي من أن هذا هو اسم مشروع البزنس
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace BookFlow_Manager1.Books.Controls
{
    public partial class ctrlManageBooks : UserControl
    {
        // تعريف جدول البيانات على مستوى الكلاس
        private DataTable _dtAllBooks;

        public ctrlManageBooks()
        {
            InitializeComponent();
        }

        // 
        private void _RefreshBooksList()
        {
            // 1. جلب البيانات من طبقة البزنس
            _dtAllBooks = clsBook.GetAllBooks();
            dgvBooks.DataSource = _dtAllBooks;

            if (dgvBooks.Columns.Count > 0)
            {
                // نتحقق من وجود العمود أولاً لتجنب خطأ الـ Null
                if (dgvBooks.Columns.Contains("BookID"))
                    dgvBooks.Columns["BookID"].HeaderText = "رقم الكتاب";

                if (dgvBooks.Columns.Contains("Title"))
                    dgvBooks.Columns["Title"].HeaderText = "عنوان الكتاب";

                if (dgvBooks.Columns.Contains("Author"))
                    dgvBooks.Columns["Author"].HeaderText = "المؤلف";

                if (dgvBooks.Columns.Contains("ISBN"))
                    dgvBooks.Columns["ISBN"].HeaderText = "الرقم الدولي ISBN";

                if (dgvBooks.Columns.Contains("CategoryID"))
                    dgvBooks.Columns["CategoryID"].HeaderText = "رقم التصنيف";

                if (dgvBooks.Columns.Contains("IsAvailable"))
                    dgvBooks.Columns["IsAvailable"].HeaderText = "حالة التوفر";
            }
        }

        private void ctrlManageBooks_Load(object sender, EventArgs e)
        {
            // استدعاء الدالة عند تحميل الصفحة
            _RefreshBooksList();

            cbFilterBy.SelectedIndex = 0;

            // تنسيق إضافي لملء الجدول في المساحة الفارغة
            dgvBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBooks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBooks.AllowUserToAddRows = false;
            dgvBooks.ReadOnly = true;
        }

        private void btnAddBook_Click(object sender, EventArgs e)
        {
            // 1. إنشاء نسخة من الفورم (بوضع الإضافة الافتراضي)
            frmAddUpdateBook frm = new frmAddUpdateBook();

            // 2. إظهار الفورم كـ Dialog 
            // (هذا يمنع المستخدم من الضغط على الواجهة الرئيسية حتى يغلق الفورم)
            frm.ShowDialog();

            // 3. بعد إغلاق الفورم، يجب تحديث الجدول (DataGridView) 
            // لكي يظهر الكتاب الجديد الذي تمت إضافته فوراً
            _RefreshBooksList();
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            string FilterColumn = "";

            // مطابقة النص العربي المختار مع اسم العمود الحقيقي في قاعدة البيانات
            switch (cbFilterBy.Text)
            {
                case "رقم الكتاب":
                    FilterColumn = "BookID";
                    break;
                case "العنوان":
                    FilterColumn = "Title";
                    break;
                case "المؤلف":
                    FilterColumn = "Author";
                    break;
                case "الرقم الدولي": // أضيفيها للقائمة إذا أردتِ البحث بـ ISBN
                    FilterColumn = "ISBN";
                    break;
                default:
                    FilterColumn = "None";
                    break;
            }

            if (txtFilterValue.Text.Trim() == "" || FilterColumn == "None")
            {
                _dtAllBooks.DefaultView.RowFilter = "";
            }
            else
            {
                if (FilterColumn == "BookID")
                    _dtAllBooks.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, txtFilterValue.Text.Trim());
                else
                    _dtAllBooks.DefaultView.RowFilter = string.Format("[{0}] LIKE '{1}%'", FilterColumn, txtFilterValue.Text.Trim());
            }

            // السطر السحري: أخبري الجدول أن البيانات "تغيرت" ليقوم بإعادة الرسم
            dgvBooks.DataSource = _dtAllBooks.DefaultView;
        }

        private void _cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            // إخفاء حقل البحث إذا كان الخيار "لا شيء"
            txtFilterValue.Visible = (cbFilterBy.Text != "None");

            if (txtFilterValue.Visible)
            {
                txtFilterValue.Text = "";
                txtFilterValue.Focus();
            }
        }

        

        private void updateBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // الحصول على الـ ID الخاص بالكتاب من السطر المحدد حالياً
            // تأكدي أن اسم العمود "BookID" مطابق لما هو موجود في قاعدة البيانات والـ DataGridView
            if (dgvBooks.CurrentRow != null)
            {
                int BookID = (int)dgvBooks.CurrentRow.Cells["BookID"].Value;

                // استدعاء فورم الإضافة والتعديل وتمرير الـ ID له
                // نحن نستخدم نفس الفورم للحالتين لتقليل تكرار الكود (DRY Principle)
                frmAddUpdateBook frm = new frmAddUpdateBook(BookID);

                frm.ShowDialog();

                // تحديث القائمة بعد الانتهاء من التعديل
                _RefreshBooksList();
            }
        }

        private void deleteBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _DeleteBook();
        }

        private void _DeleteBook()
        {
            // 1. التحقق من أن المستخدم اختار سطراً في الجدول
            if (dgvBooks.CurrentRow == null) return;

            // 2. الحصول على ID الكتاب المختار
            int BookID = (int)dgvBooks.CurrentRow.Cells["BookID"].Value;
            string BookTitle = dgvBooks.CurrentRow.Cells["Title"].Value.ToString();

            // 3. التحقق من وجود الكتاب فعلياً (البديل الذكي الذي اقترحتِه)
            // جربي هذا الآن
            if (!clsBook.IsExist(BookID))
            {
                MessageBox.Show("عذراً، الكتاب غير موجود.");
                _RefreshBooksList();
                return;
            }

            // 4. رسالة تأكيد للمستخدم قبل الحذف النهائي
            if (MessageBox.Show($"هل أنتِ متأكدة من حذف الكتاب: [{BookTitle}]؟\nملاحظة: لا يمكن التراجع عن هذه العملية.",
                "تأكيد الحذف", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                // 5. محاولة الحذف من خلال طبقة البزنس
                // ملاحظة: دالة Delete تستدعي CanDelete داخلياً أو تفشل إذا وجد ارتباط
                if (clsBook.Delete(BookID))
                {
                    MessageBox.Show("تم حذف الكتاب بنجاح من النظام.", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 6. تحديث الواجهة فوراً
                    _RefreshBooksList();
                }
                else
                {
                    // هذه الرسالة تظهر غالباً إذا كان الكتاب مرتبطاً بعمليات استعارة
                    MessageBox.Show("فشل الحذف! الكتاب مرتبط بسجلات أخرى (مثل عمليات استعارة نشطة). يرجى التحقق أولاً.",
                        "قيد قاعدة بيانات", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }
    }
}
    
