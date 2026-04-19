namespace BookFlow_Manager1
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.pnlSideMenu = new System.Windows.Forms.Panel();
            this.btnLogout = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnManageUsers = new System.Windows.Forms.Button();
            this.btnBorrowingOperations = new System.Windows.Forms.Button();
            this.btnManageMembers = new System.Windows.Forms.Button();
            this.btnManageBooks = new System.Windows.Forms.Button();
            this.btnDashboard = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlMainContent = new System.Windows.Forms.Panel();
            this.pnlBooks = new System.Windows.Forms.Panel();
            this.lblBooksCount = new System.Windows.Forms.Label();
            this.lblTotalBooks = new System.Windows.Forms.Label();
            this.pnlBorrowed = new System.Windows.Forms.Panel();
            this.lblBorrowedCount = new System.Windows.Forms.Label();
            this.lblBorrowedBooks = new System.Windows.Forms.Label();
            this.pnlUsers = new System.Windows.Forms.Panel();
            this.lblUsersCount = new System.Windows.Forms.Label();
            this.lblTotalUsers = new System.Windows.Forms.Label();
            this.pnlMembers = new System.Windows.Forms.Panel();
            this.lblMembersCount = new System.Windows.Forms.Label();
            this.lblTotalMembers = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlSideMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlMainContent.SuspendLayout();
            this.pnlBooks.SuspendLayout();
            this.pnlBorrowed.SuspendLayout();
            this.pnlUsers.SuspendLayout();
            this.pnlMembers.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSideMenu
            // 
            this.pnlSideMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.pnlSideMenu.Controls.Add(this.btnLogout);
            this.pnlSideMenu.Controls.Add(this.btnSettings);
            this.pnlSideMenu.Controls.Add(this.btnManageUsers);
            this.pnlSideMenu.Controls.Add(this.btnBorrowingOperations);
            this.pnlSideMenu.Controls.Add(this.btnManageMembers);
            this.pnlSideMenu.Controls.Add(this.btnManageBooks);
            this.pnlSideMenu.Controls.Add(this.btnDashboard);
            this.pnlSideMenu.Controls.Add(this.panel1);
            this.pnlSideMenu.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlSideMenu.Location = new System.Drawing.Point(1110, 0);
            this.pnlSideMenu.Name = "pnlSideMenu";
            this.pnlSideMenu.Size = new System.Drawing.Size(220, 1011);
            this.pnlSideMenu.TabIndex = 0;
            // 
            // btnLogout
            // 
            this.btnLogout.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnLogout.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Image = global::BookFlow_Manager1.Properties.Resources.Close_32;
            this.btnLogout.Location = new System.Drawing.Point(0, 400);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(220, 50);
            this.btnLogout.TabIndex = 8;
            this.btnLogout.Text = "تسجيل الخروج";
            this.btnLogout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLogout.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnLogout.UseVisualStyleBackColor = true;
            // 
            // btnSettings
            // 
            this.btnSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSettings.FlatAppearance.BorderSize = 0;
            this.btnSettings.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnSettings.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSettings.ForeColor = System.Drawing.Color.White;
            this.btnSettings.Image = ((System.Drawing.Image)(resources.GetObject("btnSettings.Image")));
            this.btnSettings.Location = new System.Drawing.Point(0, 350);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(220, 50);
            this.btnSettings.TabIndex = 7;
            this.btnSettings.Text = "الإعدادات";
            this.btnSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSettings.UseVisualStyleBackColor = true;
            // 
            // btnManageUsers
            // 
            this.btnManageUsers.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnManageUsers.FlatAppearance.BorderSize = 0;
            this.btnManageUsers.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnManageUsers.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.btnManageUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManageUsers.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnManageUsers.ForeColor = System.Drawing.Color.White;
            this.btnManageUsers.Image = global::BookFlow_Manager1.Properties.Resources.User_32__2;
            this.btnManageUsers.Location = new System.Drawing.Point(0, 300);
            this.btnManageUsers.Name = "btnManageUsers";
            this.btnManageUsers.Size = new System.Drawing.Size(220, 50);
            this.btnManageUsers.TabIndex = 6;
            this.btnManageUsers.Text = "المستخدمين";
            this.btnManageUsers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnManageUsers.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnManageUsers.UseVisualStyleBackColor = true;
            // 
            // btnBorrowingOperations
            // 
            this.btnBorrowingOperations.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnBorrowingOperations.FlatAppearance.BorderSize = 0;
            this.btnBorrowingOperations.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnBorrowingOperations.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.btnBorrowingOperations.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBorrowingOperations.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBorrowingOperations.ForeColor = System.Drawing.Color.White;
            this.btnBorrowingOperations.Image = ((System.Drawing.Image)(resources.GetObject("btnBorrowingOperations.Image")));
            this.btnBorrowingOperations.Location = new System.Drawing.Point(0, 250);
            this.btnBorrowingOperations.Name = "btnBorrowingOperations";
            this.btnBorrowingOperations.Size = new System.Drawing.Size(220, 50);
            this.btnBorrowingOperations.TabIndex = 5;
            this.btnBorrowingOperations.Text = "عمليات الإعارة";
            this.btnBorrowingOperations.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBorrowingOperations.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnBorrowingOperations.UseVisualStyleBackColor = true;
            // 
            // btnManageMembers
            // 
            this.btnManageMembers.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnManageMembers.FlatAppearance.BorderSize = 0;
            this.btnManageMembers.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnManageMembers.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.btnManageMembers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManageMembers.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnManageMembers.ForeColor = System.Drawing.Color.White;
            this.btnManageMembers.Image = ((System.Drawing.Image)(resources.GetObject("btnManageMembers.Image")));
            this.btnManageMembers.Location = new System.Drawing.Point(0, 200);
            this.btnManageMembers.Name = "btnManageMembers";
            this.btnManageMembers.Size = new System.Drawing.Size(220, 50);
            this.btnManageMembers.TabIndex = 4;
            this.btnManageMembers.Text = "إدارة المستعيرين";
            this.btnManageMembers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnManageMembers.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnManageMembers.UseVisualStyleBackColor = true;
            // 
            // btnManageBooks
            // 
            this.btnManageBooks.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnManageBooks.FlatAppearance.BorderSize = 0;
            this.btnManageBooks.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnManageBooks.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.btnManageBooks.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManageBooks.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnManageBooks.ForeColor = System.Drawing.Color.White;
            this.btnManageBooks.Image = global::BookFlow_Manager1.Properties.Resources.icons8_books_32;
            this.btnManageBooks.Location = new System.Drawing.Point(0, 150);
            this.btnManageBooks.Name = "btnManageBooks";
            this.btnManageBooks.Size = new System.Drawing.Size(220, 50);
            this.btnManageBooks.TabIndex = 3;
            this.btnManageBooks.Text = "إدارة الكتب";
            this.btnManageBooks.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnManageBooks.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnManageBooks.UseVisualStyleBackColor = true;
            this.btnManageBooks.Click += new System.EventHandler(this.btnManageBooks_Click);
            // 
            // btnDashboard
            // 
            this.btnDashboard.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDashboard.FlatAppearance.BorderSize = 0;
            this.btnDashboard.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnDashboard.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.btnDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDashboard.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDashboard.ForeColor = System.Drawing.Color.White;
            this.btnDashboard.Image = global::BookFlow_Manager1.Properties.Resources.icons8_dashboard_32;
            this.btnDashboard.Location = new System.Drawing.Point(0, 100);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(220, 50);
            this.btnDashboard.TabIndex = 2;
            this.btnDashboard.Text = "لوحة التحكم";
            this.btnDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDashboard.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDashboard.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(220, 100);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Algerian", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(30, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 31);
            this.label1.TabIndex = 1;
            this.label1.Text = "BookFlow";
            // 
            // pnlMainContent
            // 
            this.pnlMainContent.Controls.Add(this.pnlBooks);
            this.pnlMainContent.Controls.Add(this.pnlBorrowed);
            this.pnlMainContent.Controls.Add(this.pnlUsers);
            this.pnlMainContent.Controls.Add(this.pnlMembers);
            this.pnlMainContent.Controls.Add(this.panel2);
            this.pnlMainContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMainContent.Location = new System.Drawing.Point(0, 0);
            this.pnlMainContent.Name = "pnlMainContent";
            this.pnlMainContent.Padding = new System.Windows.Forms.Padding(20);
            this.pnlMainContent.Size = new System.Drawing.Size(1110, 1011);
            this.pnlMainContent.TabIndex = 1;
            // 
            // pnlBooks
            // 
            this.pnlBooks.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.pnlBooks.Controls.Add(this.lblBooksCount);
            this.pnlBooks.Controls.Add(this.lblTotalBooks);
            this.pnlBooks.Location = new System.Drawing.Point(302, 135);
            this.pnlBooks.Name = "pnlBooks";
            this.pnlBooks.Size = new System.Drawing.Size(200, 100);
            this.pnlBooks.TabIndex = 2;
            // 
            // lblBooksCount
            // 
            this.lblBooksCount.AutoSize = true;
            this.lblBooksCount.ForeColor = System.Drawing.Color.White;
            this.lblBooksCount.Location = new System.Drawing.Point(87, 55);
            this.lblBooksCount.Name = "lblBooksCount";
            this.lblBooksCount.Size = new System.Drawing.Size(19, 23);
            this.lblBooksCount.TabIndex = 2;
            this.lblBooksCount.Text = "0";
            // 
            // lblTotalBooks
            // 
            this.lblTotalBooks.AutoSize = true;
            this.lblTotalBooks.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalBooks.ForeColor = System.Drawing.Color.White;
            this.lblTotalBooks.Location = new System.Drawing.Point(31, 15);
            this.lblTotalBooks.Name = "lblTotalBooks";
            this.lblTotalBooks.Size = new System.Drawing.Size(134, 31);
            this.lblTotalBooks.TabIndex = 3;
            this.lblTotalBooks.Text = "جمالي الكتب";
            // 
            // pnlBorrowed
            // 
            this.pnlBorrowed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.pnlBorrowed.Controls.Add(this.lblBorrowedCount);
            this.pnlBorrowed.Controls.Add(this.lblBorrowedBooks);
            this.pnlBorrowed.Location = new System.Drawing.Point(578, 135);
            this.pnlBorrowed.Name = "pnlBorrowed";
            this.pnlBorrowed.Size = new System.Drawing.Size(200, 100);
            this.pnlBorrowed.TabIndex = 2;
            // 
            // lblBorrowedCount
            // 
            this.lblBorrowedCount.AutoSize = true;
            this.lblBorrowedCount.ForeColor = System.Drawing.Color.White;
            this.lblBorrowedCount.Location = new System.Drawing.Point(89, 55);
            this.lblBorrowedCount.Name = "lblBorrowedCount";
            this.lblBorrowedCount.Size = new System.Drawing.Size(19, 23);
            this.lblBorrowedCount.TabIndex = 4;
            this.lblBorrowedCount.Text = "0";
            // 
            // lblBorrowedBooks
            // 
            this.lblBorrowedBooks.AutoSize = true;
            this.lblBorrowedBooks.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBorrowedBooks.ForeColor = System.Drawing.Color.White;
            this.lblBorrowedBooks.Location = new System.Drawing.Point(15, 15);
            this.lblBorrowedBooks.Name = "lblBorrowedBooks";
            this.lblBorrowedBooks.Size = new System.Drawing.Size(160, 31);
            this.lblBorrowedBooks.TabIndex = 4;
            this.lblBorrowedBooks.Text = "كتب معارة حالياً\r\n";
            // 
            // pnlUsers
            // 
            this.pnlUsers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.pnlUsers.Controls.Add(this.lblUsersCount);
            this.pnlUsers.Controls.Add(this.lblTotalUsers);
            this.pnlUsers.Location = new System.Drawing.Point(839, 135);
            this.pnlUsers.Name = "pnlUsers";
            this.pnlUsers.Size = new System.Drawing.Size(200, 100);
            this.pnlUsers.TabIndex = 2;
            // 
            // lblUsersCount
            // 
            this.lblUsersCount.AutoSize = true;
            this.lblUsersCount.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblUsersCount.Location = new System.Drawing.Point(90, 55);
            this.lblUsersCount.Name = "lblUsersCount";
            this.lblUsersCount.Size = new System.Drawing.Size(19, 23);
            this.lblUsersCount.TabIndex = 5;
            this.lblUsersCount.Text = "0";
            // 
            // lblTotalUsers
            // 
            this.lblTotalUsers.AutoSize = true;
            this.lblTotalUsers.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalUsers.ForeColor = System.Drawing.Color.White;
            this.lblTotalUsers.Location = new System.Drawing.Point(20, 15);
            this.lblTotalUsers.Name = "lblTotalUsers";
            this.lblTotalUsers.Size = new System.Drawing.Size(177, 31);
            this.lblTotalUsers.TabIndex = 5;
            this.lblTotalUsers.Text = "مستخدمي النظام";
            // 
            // pnlMembers
            // 
            this.pnlMembers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.pnlMembers.Controls.Add(this.lblMembersCount);
            this.pnlMembers.Controls.Add(this.lblTotalMembers);
            this.pnlMembers.Location = new System.Drawing.Point(23, 135);
            this.pnlMembers.Name = "pnlMembers";
            this.pnlMembers.Size = new System.Drawing.Size(200, 100);
            this.pnlMembers.TabIndex = 1;
            // 
            // lblMembersCount
            // 
            this.lblMembersCount.AutoSize = true;
            this.lblMembersCount.ForeColor = System.Drawing.Color.White;
            this.lblMembersCount.Location = new System.Drawing.Point(77, 55);
            this.lblMembersCount.Name = "lblMembersCount";
            this.lblMembersCount.Size = new System.Drawing.Size(19, 23);
            this.lblMembersCount.TabIndex = 1;
            this.lblMembersCount.Text = "0";
            // 
            // lblTotalMembers
            // 
            this.lblTotalMembers.AutoSize = true;
            this.lblTotalMembers.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalMembers.ForeColor = System.Drawing.Color.White;
            this.lblTotalMembers.Location = new System.Drawing.Point(29, 15);
            this.lblTotalMembers.Name = "lblTotalMembers";
            this.lblTotalMembers.Size = new System.Drawing.Size(124, 31);
            this.lblTotalMembers.TabIndex = 0;
            this.lblTotalMembers.Text = "المستعيرون";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Gainsboro;
            this.panel2.Controls.Add(this.lblTitle);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(20, 20);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1070, 100);
            this.panel2.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 28.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(369, 18);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(282, 62);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "لوحة التحكم";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1330, 1011);
            this.Controls.Add(this.pnlMainContent);
            this.Controls.Add(this.pnlSideMenu);
            this.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmMain";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "نظام إدارة المكتبة - لوحة التحكم";
            this.pnlSideMenu.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlMainContent.ResumeLayout(false);
            this.pnlBooks.ResumeLayout(false);
            this.pnlBooks.PerformLayout();
            this.pnlBorrowed.ResumeLayout(false);
            this.pnlBorrowed.PerformLayout();
            this.pnlUsers.ResumeLayout(false);
            this.pnlUsers.PerformLayout();
            this.pnlMembers.ResumeLayout(false);
            this.pnlMembers.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlSideMenu;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDashboard;
        private System.Windows.Forms.Panel pnlMainContent;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnManageUsers;
        private System.Windows.Forms.Button btnBorrowingOperations;
        private System.Windows.Forms.Button btnManageMembers;
        private System.Windows.Forms.Button btnManageBooks;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlBooks;
        private System.Windows.Forms.Panel pnlBorrowed;
        private System.Windows.Forms.Panel pnlUsers;
        private System.Windows.Forms.Panel pnlMembers;
        private System.Windows.Forms.Label lblTotalMembers;
        private System.Windows.Forms.Label lblMembersCount;
        private System.Windows.Forms.Label lblBooksCount;
        private System.Windows.Forms.Label lblTotalBooks;
        private System.Windows.Forms.Label lblBorrowedCount;
        private System.Windows.Forms.Label lblBorrowedBooks;
        private System.Windows.Forms.Label lblUsersCount;
        private System.Windows.Forms.Label lblTotalUsers;
        private System.Windows.Forms.Button btnLogout;
    }
}

