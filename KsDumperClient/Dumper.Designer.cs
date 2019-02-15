namespace KsDumperClient
{
    partial class Dumper
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.refreshMenuBtn = new System.Windows.Forms.ToolStripButton();
            this.hideSystemProcessMenuBtn = new System.Windows.Forms.ToolStripButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.logsTextBox = new System.Windows.Forms.RichTextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.dumpMainModuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.openInExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.processList = new KsDumperClient.Utility.ProcessListView();
            this.PIDHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PathHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.BaseAddressHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.EntryPointHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ImageSizeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ImageTypeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.AllowMerge = false;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshMenuBtn,
            this.hideSystemProcessMenuBtn});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Margin = new System.Windows.Forms.Padding(2);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(2, 3, 2, 2);
            this.toolStrip1.ShowItemToolTips = false;
            this.toolStrip1.Size = new System.Drawing.Size(1004, 27);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // refreshMenuBtn
            // 
            this.refreshMenuBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.refreshMenuBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshMenuBtn.Name = "refreshMenuBtn";
            this.refreshMenuBtn.Size = new System.Drawing.Size(50, 19);
            this.refreshMenuBtn.Text = "Refresh";
            this.refreshMenuBtn.Click += new System.EventHandler(this.refreshMenuBtn_Click);
            // 
            // hideSystemProcessMenuBtn
            // 
            this.hideSystemProcessMenuBtn.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.hideSystemProcessMenuBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.hideSystemProcessMenuBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.hideSystemProcessMenuBtn.Name = "hideSystemProcessMenuBtn";
            this.hideSystemProcessMenuBtn.Size = new System.Drawing.Size(135, 19);
            this.hideSystemProcessMenuBtn.Text = "Show System Processes";
            this.hideSystemProcessMenuBtn.Click += new System.EventHandler(this.hideSystemProcessMenuBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.logsTextBox);
            this.groupBox1.Location = new System.Drawing.Point(5, 525);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(992, 222);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Logs";
            // 
            // logsTextBox
            // 
            this.logsTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.logsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.logsTextBox.Location = new System.Drawing.Point(12, 19);
            this.logsTextBox.Name = "logsTextBox";
            this.logsTextBox.ReadOnly = true;
            this.logsTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.logsTextBox.Size = new System.Drawing.Size(968, 197);
            this.logsTextBox.TabIndex = 0;
            this.logsTextBox.Text = "";
            this.logsTextBox.TextChanged += new System.EventHandler(this.logsTextBox_TextChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dumpMainModuleToolStripMenuItem,
            this.toolStripSeparator1,
            this.openInExplorerToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(182, 76);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // dumpMainModuleToolStripMenuItem
            // 
            this.dumpMainModuleToolStripMenuItem.Name = "dumpMainModuleToolStripMenuItem";
            this.dumpMainModuleToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.dumpMainModuleToolStripMenuItem.Text = "Dump Main Module";
            this.dumpMainModuleToolStripMenuItem.Click += new System.EventHandler(this.dumpMainModuleToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(178, 6);
            // 
            // openInExplorerToolStripMenuItem
            // 
            this.openInExplorerToolStripMenuItem.Name = "openInExplorerToolStripMenuItem";
            this.openInExplorerToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.openInExplorerToolStripMenuItem.Text = "Open In Explorer";
            this.openInExplorerToolStripMenuItem.Click += new System.EventHandler(this.openInExplorerToolStripMenuItem_Click);
            // 
            // processList
            // 
            this.processList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.PIDHeader,
            this.NameHeader,
            this.PathHeader,
            this.BaseAddressHeader,
            this.EntryPointHeader,
            this.ImageSizeHeader,
            this.ImageTypeHeader});
            this.processList.ContextMenuStrip = this.contextMenuStrip1;
            this.processList.FullRowSelect = true;
            this.processList.Location = new System.Drawing.Point(5, 28);
            this.processList.MultiSelect = false;
            this.processList.Name = "processList";
            this.processList.Size = new System.Drawing.Size(992, 491);
            this.processList.TabIndex = 2;
            this.processList.UseCompatibleStateImageBehavior = false;
            this.processList.View = System.Windows.Forms.View.Details;
            // 
            // PIDHeader
            // 
            this.PIDHeader.Text = "PID";
            this.PIDHeader.Width = 76;
            // 
            // NameHeader
            // 
            this.NameHeader.Text = "Name";
            this.NameHeader.Width = 143;
            // 
            // PathHeader
            // 
            this.PathHeader.Text = "Path";
            this.PathHeader.Width = 375;
            // 
            // BaseAddressHeader
            // 
            this.BaseAddressHeader.Text = "Base Address";
            this.BaseAddressHeader.Width = 106;
            // 
            // EntryPointHeader
            // 
            this.EntryPointHeader.Text = "Entry Point";
            this.EntryPointHeader.Width = 106;
            // 
            // ImageSizeHeader
            // 
            this.ImageSizeHeader.Text = "Image Size";
            this.ImageSizeHeader.Width = 88;
            // 
            // ImageTypeHeader
            // 
            this.ImageTypeHeader.Text = "Image Type";
            this.ImageTypeHeader.Width = 72;
            // 
            // Dumper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 756);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.processList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Dumper";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "KsDumper";
            this.Load += new System.EventHandler(this.Dumper_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private KsDumperClient.Utility.ProcessListView processList;
        private System.Windows.Forms.ColumnHeader PIDHeader;
        private System.Windows.Forms.ColumnHeader NameHeader;
        private System.Windows.Forms.ColumnHeader PathHeader;
        private System.Windows.Forms.ColumnHeader BaseAddressHeader;
        private System.Windows.Forms.ColumnHeader EntryPointHeader;
        private System.Windows.Forms.ColumnHeader ImageSizeHeader;
        private System.Windows.Forms.ColumnHeader ImageTypeHeader;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton refreshMenuBtn;
        private System.Windows.Forms.ToolStripButton hideSystemProcessMenuBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox logsTextBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem dumpMainModuleToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem openInExplorerToolStripMenuItem;
    }
}

