using BookFlow_Manager1.Books;
using BookFlow_Manager1.Books.Controls;
using Library_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookFlow_Manager1
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

       

        private void _ShowControl(UserControl control)
        {
            // تنظيف المساحة البيضاء من أي صفحات قديمة
            pnlMainContent.Controls.Clear();

            // جعل الصفحة الجديدة تمتد لتملأ المساحة بالكامل
            control.Dock = DockStyle.Fill;

            // إضافة الصفحة الجديدة للمساحة البيضاء
            pnlMainContent.Controls.Add(control);
        }

        private void btnManageBooks_Click(object sender, EventArgs e)
        {
           
            // 1. تغيير عنوان الصفحة في الهيدر (Label العنوان)
            lblTitle.Text = "إدارة الكتب";

            // 2. إنشاء نسخة من الكنترول الذي صممتيه
            ctrlManageBooks ctrl = new ctrlManageBooks();

            // 3. استدعاء الدالة التي تنظف المساحة البيضاء وتعرض الكنترول الجديد
            _ShowControl(ctrl);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // استبدلي lblTotalBooks باسم الليبل الموجود عندك في التصميم
            lblBooksCount.Text = clsBook.CountBooks().ToString("D2");
        }
    }
}
