namespace BookFlow_Manager1.Books.Controls
{
    partial class ctrlManageBooks
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctrlManageBooks));
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblFilterBy = new System.Windows.Forms.Label();
            this.cbFilterBy = new System.Windows.Forms.ComboBox();
            this.txtFilterValue = new System.Windows.Forms.TextBox();
            this.dgvBooks = new System.Windows.Forms.DataGridView();
            this.cmsLibraryOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.updateBookToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteBookToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pbHeaderIcon = new System.Windows.Forms.PictureBox();
            this.btnAddBook = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBooks)).BeginInit();
            this.cmsLibraryOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbHeaderIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Algerian", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblTitle.Location = new System.Drawing.Point(333, 25);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(331, 89);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "إدارة الكتب";
            // 
            // lblFilterBy
            // 
            this.lblFilterBy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFilterBy.AutoSize = true;
            this.lblFilterBy.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilterBy.Location = new System.Drawing.Point(663, 135);
            this.lblFilterBy.Name = "lblFilterBy";
            this.lblFilterBy.Size = new System.Drawing.Size(144, 29);
            this.lblFilterBy.TabIndex = 1;
            this.lblFilterBy.Text = "البحث بواسطه :";
            // 
            // cbFilterBy
            // 
            this.cbFilterBy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFilterBy.FormattingEnabled = true;
            this.cbFilterBy.Items.AddRange(new object[] {
            "None",
            "رقم الكتب",
            "العنوان",
            "المؤلف"});
            this.cbFilterBy.Location = new System.Drawing.Point(490, 142);
            this.cbFilterBy.Name = "cbFilterBy";
            this.cbFilterBy.Size = new System.Drawing.Size(168, 24);
            this.cbFilterBy.TabIndex = 2;
            // 
            // txtFilterValue
            // 
            this.txtFilterValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilterValue.Location = new System.Drawing.Point(230, 144);
            this.txtFilterValue.Name = "txtFilterValue";
            this.txtFilterValue.Size = new System.Drawing.Size(255, 22);
            this.txtFilterValue.TabIndex = 3;
            this.txtFilterValue.TextChanged += new System.EventHandler(this.txtFilterValue_TextChanged);
            // 
            // dgvBooks
            // 
            this.dgvBooks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBooks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBooks.ContextMenuStrip = this.cmsLibraryOptions;
            this.dgvBooks.Location = new System.Drawing.Point(3, 176);
            this.dgvBooks.Name = "dgvBooks";
            this.dgvBooks.RowHeadersWidth = 51;
            this.dgvBooks.RowTemplate.Height = 24;
            this.dgvBooks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBooks.Size = new System.Drawing.Size(1104, 832);
            this.dgvBooks.TabIndex = 5;
            // 
            // cmsLibraryOptions
            // 
            this.cmsLibraryOptions.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsLibraryOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateBookToolStripMenuItem,
            this.deleteBookToolStripMenuItem});
            this.cmsLibraryOptions.Name = "cmsLibraryOptions";
            this.cmsLibraryOptions.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cmsLibraryOptions.Size = new System.Drawing.Size(215, 84);
            // 
            // updateBookToolStripMenuItem
            // 
            this.updateBookToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("updateBookToolStripMenuItem.Image")));
            this.updateBookToolStripMenuItem.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.updateBookToolStripMenuItem.Name = "updateBookToolStripMenuItem";
            this.updateBookToolStripMenuItem.Size = new System.Drawing.Size(214, 26);
            this.updateBookToolStripMenuItem.Text = "Update Book";
            this.updateBookToolStripMenuItem.Click += new System.EventHandler(this.updateBookToolStripMenuItem_Click);
            // 
            // deleteBookToolStripMenuItem
            // 
            this.deleteBookToolStripMenuItem.Image = global::BookFlow_Manager1.Properties.Resources.Delete_32_2;
            this.deleteBookToolStripMenuItem.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.deleteBookToolStripMenuItem.Name = "deleteBookToolStripMenuItem";
            this.deleteBookToolStripMenuItem.Size = new System.Drawing.Size(214, 26);
            this.deleteBookToolStripMenuItem.Text = "Delete Book";
            this.deleteBookToolStripMenuItem.Click += new System.EventHandler(this.deleteBookToolStripMenuItem_Click);
            // 
            // pbHeaderIcon
            // 
            this.pbHeaderIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbHeaderIcon.Image = global::BookFlow_Manager1.Properties.Resources.icons8_books_48;
            this.pbHeaderIcon.Location = new System.Drawing.Point(656, 48);
            this.pbHeaderIcon.Name = "pbHeaderIcon";
            this.pbHeaderIcon.Size = new System.Drawing.Size(50, 50);
            this.pbHeaderIcon.TabIndex = 6;
            this.pbHeaderIcon.TabStop = false;
            // 
            // btnAddBook
            // 
            this.btnAddBook.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddBook.Image = ((System.Drawing.Image)(resources.GetObject("btnAddBook.Image")));
            this.btnAddBook.Location = new System.Drawing.Point(149, 138);
            this.btnAddBook.Name = "btnAddBook";
            this.btnAddBook.Size = new System.Drawing.Size(75, 35);
            this.btnAddBook.TabIndex = 4;
            this.btnAddBook.UseVisualStyleBackColor = true;
            this.btnAddBook.Click += new System.EventHandler(this.btnAddBook_Click);
            // 
            // ctrlManageBooks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pbHeaderIcon);
            this.Controls.Add(this.dgvBooks);
            this.Controls.Add(this.btnAddBook);
            this.Controls.Add(this.txtFilterValue);
            this.Controls.Add(this.cbFilterBy);
            this.Controls.Add(this.lblFilterBy);
            this.Controls.Add(this.lblTitle);
            this.Name = "ctrlManageBooks";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Size = new System.Drawing.Size(1110, 1011);
            this.Load += new System.EventHandler(this.ctrlManageBooks_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBooks)).EndInit();
            this.cmsLibraryOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbHeaderIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblFilterBy;
        private System.Windows.Forms.ComboBox cbFilterBy;
        private System.Windows.Forms.TextBox txtFilterValue;
        private System.Windows.Forms.Button btnAddBook;
        private System.Windows.Forms.DataGridView dgvBooks;
        private System.Windows.Forms.PictureBox pbHeaderIcon;
        private System.Windows.Forms.ContextMenuStrip cmsLibraryOptions;
        private System.Windows.Forms.ToolStripMenuItem updateBookToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteBookToolStripMenuItem;
    }
}
