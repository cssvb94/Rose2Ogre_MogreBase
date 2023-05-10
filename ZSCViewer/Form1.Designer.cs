
namespace ZSCViewer
{
    partial class Form1
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
            this.open_zsc_dialog = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.zsc_page = new System.Windows.Forms.TabPage();
            this.objects_view = new System.Windows.Forms.DataGridView();
            this.effects_view = new System.Windows.Forms.DataGridView();
            this.parts_view = new System.Windows.Forms.DataGridView();
            this.stb_page = new System.Windows.Forms.TabPage();
            this.stb_view = new System.Windows.Forms.DataGridView();
            this.open_stb_dialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openZSCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSTBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zon_page = new System.Windows.Forms.TabPage();
            this.open_zon_dialog = new System.Windows.Forms.OpenFileDialog();
            this.openZONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zon_view = new System.Windows.Forms.DataGridView();
            this.tabControl1.SuspendLayout();
            this.zsc_page.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objects_view)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.effects_view)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.parts_view)).BeginInit();
            this.stb_page.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.stb_view)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.zon_page.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zon_view)).BeginInit();
            this.SuspendLayout();
            // 
            // open_zsc_dialog
            // 
            this.open_zsc_dialog.Filter = "ZSC files (*.ZSC)|*.ZSC|All files (*.*)|*.*\"";
            this.open_zsc_dialog.InitialDirectory = "./";
            this.open_zsc_dialog.RestoreDirectory = true;
            this.open_zsc_dialog.Title = "Open ZSC file";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.zsc_page);
            this.tabControl1.Controls.Add(this.stb_page);
            this.tabControl1.Controls.Add(this.zon_page);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1254, 629);
            this.tabControl1.TabIndex = 4;
            // 
            // zsc_page
            // 
            this.zsc_page.Controls.Add(this.objects_view);
            this.zsc_page.Controls.Add(this.effects_view);
            this.zsc_page.Controls.Add(this.parts_view);
            this.zsc_page.Location = new System.Drawing.Point(4, 22);
            this.zsc_page.Name = "zsc_page";
            this.zsc_page.Padding = new System.Windows.Forms.Padding(3);
            this.zsc_page.Size = new System.Drawing.Size(1246, 603);
            this.zsc_page.TabIndex = 0;
            this.zsc_page.Text = "ZSC";
            this.zsc_page.UseVisualStyleBackColor = true;
            // 
            // objects_view
            // 
            this.objects_view.AllowUserToAddRows = false;
            this.objects_view.AllowUserToDeleteRows = false;
            this.objects_view.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.objects_view.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.objects_view.Location = new System.Drawing.Point(6, 6);
            this.objects_view.Name = "objects_view";
            this.objects_view.ReadOnly = true;
            this.objects_view.Size = new System.Drawing.Size(391, 562);
            this.objects_view.TabIndex = 7;
            this.objects_view.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.objects_view_RowEnter);
            // 
            // effects_view
            // 
            this.effects_view.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.effects_view.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.effects_view.Location = new System.Drawing.Point(403, 245);
            this.effects_view.Name = "effects_view";
            this.effects_view.Size = new System.Drawing.Size(335, 173);
            this.effects_view.TabIndex = 6;
            // 
            // parts_view
            // 
            this.parts_view.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.parts_view.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.parts_view.Location = new System.Drawing.Point(403, 6);
            this.parts_view.Name = "parts_view";
            this.parts_view.Size = new System.Drawing.Size(813, 233);
            this.parts_view.TabIndex = 5;
            // 
            // stb_page
            // 
            this.stb_page.Controls.Add(this.stb_view);
            this.stb_page.Location = new System.Drawing.Point(4, 22);
            this.stb_page.Name = "stb_page";
            this.stb_page.Padding = new System.Windows.Forms.Padding(3);
            this.stb_page.Size = new System.Drawing.Size(1246, 603);
            this.stb_page.TabIndex = 1;
            this.stb_page.Text = "STB";
            this.stb_page.UseVisualStyleBackColor = true;
            // 
            // stb_view
            // 
            this.stb_view.AllowUserToAddRows = false;
            this.stb_view.AllowUserToDeleteRows = false;
            this.stb_view.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.stb_view.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.stb_view.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stb_view.Location = new System.Drawing.Point(3, 3);
            this.stb_view.Name = "stb_view";
            this.stb_view.ReadOnly = true;
            this.stb_view.Size = new System.Drawing.Size(1240, 597);
            this.stb_view.TabIndex = 0;
            // 
            // open_stb_dialog
            // 
            this.open_stb_dialog.Filter = "STB files (*.STB)|*.STB|All files (*.*)|*.*\"";
            this.open_stb_dialog.InitialDirectory = "./";
            this.open_stb_dialog.RestoreDirectory = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1254, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openZSCToolStripMenuItem,
            this.openSTBToolStripMenuItem,
            this.openZONToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openZSCToolStripMenuItem
            // 
            this.openZSCToolStripMenuItem.Name = "openZSCToolStripMenuItem";
            this.openZSCToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openZSCToolStripMenuItem.Text = "Open ZSC";
            this.openZSCToolStripMenuItem.Click += new System.EventHandler(this.openZSCToolStripMenuItem_Click);
            // 
            // openSTBToolStripMenuItem
            // 
            this.openSTBToolStripMenuItem.Name = "openSTBToolStripMenuItem";
            this.openSTBToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openSTBToolStripMenuItem.Text = "Open STB";
            this.openSTBToolStripMenuItem.Click += new System.EventHandler(this.openSTBToolStripMenuItem_Click);
            // 
            // zon_page
            // 
            this.zon_page.Controls.Add(this.zon_view);
            this.zon_page.Location = new System.Drawing.Point(4, 22);
            this.zon_page.Name = "zon_page";
            this.zon_page.Padding = new System.Windows.Forms.Padding(3);
            this.zon_page.Size = new System.Drawing.Size(1246, 603);
            this.zon_page.TabIndex = 2;
            this.zon_page.Text = "ZON";
            this.zon_page.UseVisualStyleBackColor = true;
            // 
            // open_zon_dialog
            // 
            this.open_zon_dialog.Filter = "ZON files (*.ZON)|*.ZON|All files (*.*)|*.*\"";
            this.open_zon_dialog.InitialDirectory = "./";
            // 
            // openZONToolStripMenuItem
            // 
            this.openZONToolStripMenuItem.Name = "openZONToolStripMenuItem";
            this.openZONToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openZONToolStripMenuItem.Text = "Open ZON";
            this.openZONToolStripMenuItem.Click += new System.EventHandler(this.openZONToolStripMenuItem_Click);
            // 
            // zon_view
            // 
            this.zon_view.AllowUserToAddRows = false;
            this.zon_view.AllowUserToDeleteRows = false;
            this.zon_view.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.zon_view.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zon_view.Location = new System.Drawing.Point(3, 3);
            this.zon_view.Name = "zon_view";
            this.zon_view.ReadOnly = true;
            this.zon_view.Size = new System.Drawing.Size(1240, 597);
            this.zon_view.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1254, 653);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Form1";
            this.Text = "ZSC Viewer";
            this.tabControl1.ResumeLayout(false);
            this.zsc_page.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.objects_view)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.effects_view)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.parts_view)).EndInit();
            this.stb_page.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.stb_view)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.zon_page.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.zon_view)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog open_zsc_dialog;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage zsc_page;
        private System.Windows.Forms.DataGridView objects_view;
        private System.Windows.Forms.DataGridView effects_view;
        private System.Windows.Forms.DataGridView parts_view;
        private System.Windows.Forms.TabPage stb_page;
        private System.Windows.Forms.OpenFileDialog open_stb_dialog;
        private System.Windows.Forms.DataGridView stb_view;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openZSCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSTBToolStripMenuItem;
        private System.Windows.Forms.TabPage zon_page;
        private System.Windows.Forms.ToolStripMenuItem openZONToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog open_zon_dialog;
        private System.Windows.Forms.DataGridView zon_view;
    }
}

