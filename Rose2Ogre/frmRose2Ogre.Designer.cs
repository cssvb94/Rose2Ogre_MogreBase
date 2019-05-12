namespace Rose2Ogre
{
    partial class frmRose2Ogre
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRose2Ogre));
            this.txtZMD = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnZMD = new System.Windows.Forms.Button();
            this.lstZMS = new System.Windows.Forms.ListBox();
            this.menuZMS = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeSelectedZMS = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllZMS = new System.Windows.Forms.ToolStripMenuItem();
            this.btnZMS = new System.Windows.Forms.Button();
            this.btnZMO = new System.Windows.Forms.Button();
            this.lstZMO = new System.Windows.Forms.ListBox();
            this.menuZMO = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeSelectedZMO = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllZMO = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnConvert = new System.Windows.Forms.Button();
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.dlgSaveFile = new System.Windows.Forms.SaveFileDialog();
            this.menuZMS.SuspendLayout();
            this.menuZMO.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtZMD
            // 
            this.txtZMD.Location = new System.Drawing.Point(52, 6);
            this.txtZMD.Name = "txtZMD";
            this.txtZMD.ReadOnly = true;
            this.txtZMD.Size = new System.Drawing.Size(279, 20);
            this.txtZMD.TabIndex = 99;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "ZMD:";
            // 
            // btnZMD
            // 
            this.btnZMD.Location = new System.Drawing.Point(337, 4);
            this.btnZMD.Name = "btnZMD";
            this.btnZMD.Size = new System.Drawing.Size(99, 23);
            this.btnZMD.TabIndex = 1;
            this.btnZMD.Text = "Choose ZMD";
            this.btnZMD.UseVisualStyleBackColor = true;
            this.btnZMD.Click += new System.EventHandler(this.btnZMD_Click);
            // 
            // lstZMS
            // 
            this.lstZMS.ContextMenuStrip = this.menuZMS;
            this.lstZMS.FormattingEnabled = true;
            this.lstZMS.Location = new System.Drawing.Point(52, 32);
            this.lstZMS.Name = "lstZMS";
            this.lstZMS.Size = new System.Drawing.Size(279, 82);
            this.lstZMS.TabIndex = 99;
            this.lstZMS.TabStop = false;
            // 
            // menuZMS
            // 
            this.menuZMS.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeSelectedZMS,
            this.removeAllZMS});
            this.menuZMS.Name = "menuZMS";
            this.menuZMS.ShowImageMargin = false;
            this.menuZMS.Size = new System.Drawing.Size(139, 48);
            // 
            // removeSelectedZMS
            // 
            this.removeSelectedZMS.Name = "removeSelectedZMS";
            this.removeSelectedZMS.Size = new System.Drawing.Size(138, 22);
            this.removeSelectedZMS.Text = "Remove selected";
            this.removeSelectedZMS.Click += new System.EventHandler(this.removeSelectedZMS_Click);
            // 
            // removeAllZMS
            // 
            this.removeAllZMS.Name = "removeAllZMS";
            this.removeAllZMS.Size = new System.Drawing.Size(138, 22);
            this.removeAllZMS.Text = "Remove ALL";
            this.removeAllZMS.Click += new System.EventHandler(this.removeAllZMS_Click);
            // 
            // btnZMS
            // 
            this.btnZMS.Location = new System.Drawing.Point(337, 33);
            this.btnZMS.Name = "btnZMS";
            this.btnZMS.Size = new System.Drawing.Size(99, 23);
            this.btnZMS.TabIndex = 2;
            this.btnZMS.Text = "Add ZMS";
            this.btnZMS.UseVisualStyleBackColor = true;
            this.btnZMS.Click += new System.EventHandler(this.btnZMS_Click);
            // 
            // btnZMO
            // 
            this.btnZMO.Location = new System.Drawing.Point(337, 121);
            this.btnZMO.Name = "btnZMO";
            this.btnZMO.Size = new System.Drawing.Size(99, 23);
            this.btnZMO.TabIndex = 3;
            this.btnZMO.Text = "Add ZMO";
            this.btnZMO.UseVisualStyleBackColor = true;
            this.btnZMO.Click += new System.EventHandler(this.btnZMO_Click);
            // 
            // lstZMO
            // 
            this.lstZMO.ContextMenuStrip = this.menuZMO;
            this.lstZMO.FormattingEnabled = true;
            this.lstZMO.Location = new System.Drawing.Point(52, 120);
            this.lstZMO.Name = "lstZMO";
            this.lstZMO.Size = new System.Drawing.Size(279, 121);
            this.lstZMO.TabIndex = 99;
            this.lstZMO.TabStop = false;
            // 
            // menuZMO
            // 
            this.menuZMO.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeSelectedZMO,
            this.removeAllZMO});
            this.menuZMO.Name = "menuZMO";
            this.menuZMO.ShowImageMargin = false;
            this.menuZMO.Size = new System.Drawing.Size(139, 48);
            // 
            // removeSelectedZMO
            // 
            this.removeSelectedZMO.Name = "removeSelectedZMO";
            this.removeSelectedZMO.Size = new System.Drawing.Size(138, 22);
            this.removeSelectedZMO.Text = "Remove selected";
            this.removeSelectedZMO.Click += new System.EventHandler(this.removeSelectedZMO_Click);
            // 
            // removeAllZMO
            // 
            this.removeAllZMO.Name = "removeAllZMO";
            this.removeAllZMO.Size = new System.Drawing.Size(138, 22);
            this.removeAllZMO.Text = "Remove ALL";
            this.removeAllZMO.Click += new System.EventHandler(this.removeAllZMO_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "ZMS:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 121);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "ZMO:";
            // 
            // btnConvert
            // 
            this.btnConvert.Location = new System.Drawing.Point(103, 254);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(173, 23);
            this.btnConvert.TabIndex = 4;
            this.btnConvert.Text = "Convert to OGRE3D XML";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.InitialDirectory = ".\\";
            this.dlgOpenFile.Title = "Choose file";
            // 
            // dlgSaveFile
            // 
            this.dlgSaveFile.InitialDirectory = ".\\";
            this.dlgSaveFile.Title = "Export converted to";
            // 
            // frmRose2Ogre
            // 
            this.AcceptButton = this.btnConvert;
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 290);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnZMO);
            this.Controls.Add(this.lstZMO);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.btnZMS);
            this.Controls.Add(this.lstZMS);
            this.Controls.Add(this.btnZMD);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtZMD);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(460, 328);
            this.MinimumSize = new System.Drawing.Size(460, 328);
            this.Name = "frmRose2Ogre";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rose2Ogre";
            this.Load += new System.EventHandler(this.frmRose2Ogre_Load);
            this.Shown += new System.EventHandler(this.frmRose2Ogre_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.frmRose2Ogre_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.frmRose2Ogre_DragEnter);
            this.menuZMS.ResumeLayout(false);
            this.menuZMO.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtZMD;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnZMD;
        private System.Windows.Forms.ListBox lstZMS;
        private System.Windows.Forms.Button btnZMS;
        private System.Windows.Forms.Button btnZMO;
        private System.Windows.Forms.ListBox lstZMO;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.OpenFileDialog dlgOpenFile;
        private System.Windows.Forms.SaveFileDialog dlgSaveFile;
        private System.Windows.Forms.ContextMenuStrip menuZMS;
        private System.Windows.Forms.ToolStripMenuItem removeSelectedZMS;
        private System.Windows.Forms.ToolStripMenuItem removeAllZMS;
        private System.Windows.Forms.ContextMenuStrip menuZMO;
        private System.Windows.Forms.ToolStripMenuItem removeSelectedZMO;
        private System.Windows.Forms.ToolStripMenuItem removeAllZMO;
    }
}

