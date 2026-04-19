using Library_Business;
using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;

namespace BookFlow_Manager1.Books.Forms
{
    public partial class frmAddUpdateBook : Form
    {
        public enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode;

        private int _BookID;
        private clsBook _Book;

        // تعريف الـ ErrorProvider برمجياً
        private ErrorProvider _errorProvider = new ErrorProvider();

        public frmAddUpdateBook()
        {
            InitializeComponent();
            _Mode = enMode.AddNew;
            _SubscribeToEvents(); // الربط اليدوي المضمون
        }

        public frmAddUpdateBook(int BookID)
        {
            InitializeComponent();
            _BookID = BookID;
            _Mode = enMode.Update;
            _SubscribeToEvents();
        }

        // دالة لربط الأحداث برمجياً لضمان عملها
        private void _SubscribeToEvents()
        {
            txtTitle.TextChanged += txtFields_TextChanged;
            txtAuthor.TextChanged += txtFields_TextChanged;
            txtISBN.TextChanged += txtFields_TextChanged;

            // ضبط إعدادات الـ ErrorProvider
            _errorProvider.BlinkStyle = ErrorBlinkStyle.AlwaysBlink;
        }

        private void _FillCategoriesInComboBox()
        {
            DataTable dtCategories = clsCategory.GetAllCategories();
            if (dtCategories != null)
            {
                cbCategories.DataSource = dtCategories;
                cbCategories.DisplayMember = "CategoryName";
                cbCategories.ValueMember = "CategoryID";
            }
        }

        private void _LoadData()
        {
            _FillCategoriesInComboBox();

            if (_Mode == enMode.AddNew)
            {
                lblTitle.Text = "إضافة كتاب جديد";
                this.Text = "إضافة كتاب";
                _Book = new clsBook();
                btnSave.Enabled = false; // يبدأ معطلاً في الإضافة
                return;
            }

            _Book = clsBook.Find(_BookID);

            if (_Book == null)
            {
                MessageBox.Show("عذراً، لم يتم العثور على الكتاب", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            lblTitle.Text = "تعديل بيانات الكتاب";
            this.Text = "تعديل كتاب";

            txtTitle.Text = _Book.Title;
            txtAuthor.Text = _Book.Author;
            txtISBN.Text = _Book.ISBN;
            cbCategories.SelectedValue = _Book.CategoryID;
            btnSave.Enabled = true; // يكون مفعلاً في التعديل لأن البيانات موجودة أصلاً
        }

        private void frmAddUpdateBook_Load(object sender, EventArgs e)
        {
            _LoadData();
        }

        private void txtFields_TextChanged(object sender, EventArgs e)
        {
            // 1. التحقق من الحقول وإظهار العلامة الحمراء
            _ValidateField(txtTitle, "عنوان الكتاب مطلوب");
            _ValidateField(txtAuthor, "اسم المؤلف مطلوب");
            _ValidateField(txtISBN, "الرقم الدولي مطلوب");

            // 2. تفعيل أو تعطيل زر الحفظ
            btnSave.Enabled = (txtTitle.Text.Trim().Length > 0 &&
                               txtAuthor.Text.Trim().Length > 0 &&
                               txtISBN.Text.Trim().Length > 0);
        }

        // دالة مساعدة للـ Validation والعلامة الحمراء
        private void _ValidateField(TextBox textBox, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
                _errorProvider.SetError(textBox, errorMessage);
            else
                _errorProvider.SetError(textBox, ""); // إزالة العلامة عند الكتابة
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtISBN.Text.Trim().Length < 10)
            {
                _errorProvider.SetError(txtISBN, "يجب أن يكون 10 أرقام على الأقل");
                txtISBN.Focus();
                return;
            }

            _Book.Title = txtTitle.Text.Trim();
            _Book.Author = txtAuthor.Text.Trim();
            _Book.ISBN = txtISBN.Text.Trim();
            _Book.CategoryID = (int)cbCategories.SelectedValue;

            if (_Mode == enMode.AddNew)
                _Book.IsAvailable = true;

            if (_Book.Save())
            {
                MessageBox.Show("تم حفظ البيانات بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _Mode = enMode.Update;
                _BookID = _Book.BookID;
                lblTitle.Text = "تعديل بيانات الكتاب";
                this.Text = "تعديل كتاب";
            }
            else
            {
                MessageBox.Show("حدث خطأ أثناء الحفظ", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtISBN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void txtAuthor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        private void btnClose_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}