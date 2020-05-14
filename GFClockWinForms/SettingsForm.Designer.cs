namespace GFClockWinForms
{
    partial class SettingsForm
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
            this.buttonClose = new System.Windows.Forms.Button();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.groupBoxGFClockDisplay = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.labelDetails = new System.Windows.Forms.Label();
            this.labelClockType = new System.Windows.Forms.Label();
            this.dataGridViewDetails = new System.Windows.Forms.DataGridView();
            this.ColumnImage = new DataGridViewImageCellWithCustomValueColumn();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDetails = new System.Windows.Forms.DataGridViewLinkColumn();
            this.comboBoxClockType = new System.Windows.Forms.ComboBox();
            this.linkLabelIntro = new System.Windows.Forms.LinkLabel();
            this.groupBoxHistory = new System.Windows.Forms.GroupBox();
            this.groupBoxGFClockDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDetails)).BeginInit();
            this.groupBoxHistory.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(484, 523);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(2);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(87, 54);
            this.buttonClose.TabIndex = 3;
            this.buttonClose.Text = "&Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton2.AutoCheck = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(4, 41);
            this.radioButton1.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(158, 24);
            this.radioButton1.TabIndex = 1;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Show &both hands";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // groupBoxGFClockDisplay
            // 
            this.groupBoxGFClockDisplay.Controls.Add(this.radioButton2);
            this.groupBoxGFClockDisplay.Controls.Add(this.radioButton1);
            this.groupBoxGFClockDisplay.Location = new System.Drawing.Point(15, 436);
            this.groupBoxGFClockDisplay.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxGFClockDisplay.Name = "groupBoxGFClockDisplay";
            this.groupBoxGFClockDisplay.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxGFClockDisplay.Size = new System.Drawing.Size(219, 113);
            this.groupBoxGFClockDisplay.TabIndex = 2;
            this.groupBoxGFClockDisplay.TabStop = false;
            this.groupBoxGFClockDisplay.Text = "&Grandfather Clock Display";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoCheck = true;
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(4, 69);
            this.radioButton2.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(145, 24);
            this.radioButton2.TabIndex = 0;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Show &one hand";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // labelDetails
            // 
            this.labelDetails.AutoSize = true;
            this.labelDetails.Location = new System.Drawing.Point(12, 89);
            this.labelDetails.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelDetails.Name = "labelDetails";
            this.labelDetails.Size = new System.Drawing.Size(58, 20);
            this.labelDetails.TabIndex = 2;
            this.labelDetails.Text = "&Details";
            // 
            // labelClockType
            // 
            this.labelClockType.AutoSize = true;
            this.labelClockType.Location = new System.Drawing.Point(12, 40);
            this.labelClockType.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelClockType.Name = "labelClockType";
            this.labelClockType.Size = new System.Drawing.Size(86, 20);
            this.labelClockType.TabIndex = 0;
            this.labelClockType.Text = "Clock &Type";
            // 
            // dataGridViewDetails
            // 
            this.dataGridViewDetails.AllowUserToAddRows = false;
            this.dataGridViewDetails.AllowUserToDeleteRows = false;
            this.dataGridViewDetails.AllowUserToOrderColumns = true;
            this.dataGridViewDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnImage,
            this.ColumnName,
            this.ColumnDate,
            this.ColumnDetails});
            this.dataGridViewDetails.Location = new System.Drawing.Point(16, 122);
            this.dataGridViewDetails.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridViewDetails.Name = "dataGridViewDetails";
            this.dataGridViewDetails.ReadOnly = true;
            this.dataGridViewDetails.RowHeadersVisible = false;
            this.dataGridViewDetails.RowHeadersWidth = 102;
            this.dataGridViewDetails.RowTemplate.Height = 40;
            this.dataGridViewDetails.Size = new System.Drawing.Size(512, 182);
            this.dataGridViewDetails.TabIndex = 3;
            // 
            // ColumnImage
            // 
            this.ColumnImage.HeaderText = "";
            this.ColumnImage.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.ColumnImage.MinimumWidth = 12;
            this.ColumnImage.Name = "ColumnImage";
            this.ColumnImage.ReadOnly = true;
            this.ColumnImage.Width = 80;
            // 
            // ColumnName
            // 
            this.ColumnName.HeaderText = "Name";
            this.ColumnName.MinimumWidth = 12;
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            this.ColumnName.Width = 250;
            // 
            // ColumnDate
            // 
            this.ColumnDate.HeaderText = "Date";
            this.ColumnDate.MinimumWidth = 12;
            this.ColumnDate.Name = "ColumnDate";
            this.ColumnDate.ReadOnly = true;
            this.ColumnDate.Width = 250;
            // 
            // ColumnDetails
            // 
            this.ColumnDetails.HeaderText = "Details";
            this.ColumnDetails.MinimumWidth = 12;
            this.ColumnDetails.Name = "ColumnDetails";
            this.ColumnDetails.ReadOnly = true;
            this.ColumnDetails.Width = 250;
            // 
            // comboBoxClockType
            // 
            this.comboBoxClockType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxClockType.FormattingEnabled = true;
            this.comboBoxClockType.Items.AddRange(new object[] {
            "All"});
            this.comboBoxClockType.Location = new System.Drawing.Point(104, 38);
            this.comboBoxClockType.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxClockType.Name = "comboBoxClockType";
            this.comboBoxClockType.Size = new System.Drawing.Size(142, 28);
            this.comboBoxClockType.TabIndex = 1;
            // 
            // linkLabelIntro
            // 
            this.linkLabelIntro.AutoSize = true;
            this.linkLabelIntro.LinkArea = new System.Windows.Forms.LinkArea(100, 81);
            this.linkLabelIntro.Location = new System.Drawing.Point(10, 14);
            this.linkLabelIntro.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkLabelIntro.Name = "linkLabelIntro";
            this.linkLabelIntro.Size = new System.Drawing.Size(568, 60);
            this.linkLabelIntro.TabIndex = 0;
            this.linkLabelIntro.TabStop = true;
            this.linkLabelIntro.Text = "This dialog exists only to support discussion on the keyboard accessiblity of \r\nW" +
    "inForms controls at Real-world learnings on keyboard accessibility in \r\nWinForms" +
    " and WPF apps: Part 1.";
            this.linkLabelIntro.UseCompatibleTextRendering = true;
            // 
            // groupBoxHistory
            // 
            this.groupBoxHistory.Controls.Add(this.labelClockType);
            this.groupBoxHistory.Controls.Add(this.comboBoxClockType);
            this.groupBoxHistory.Controls.Add(this.labelDetails);
            this.groupBoxHistory.Controls.Add(this.dataGridViewDetails);
            this.groupBoxHistory.Location = new System.Drawing.Point(15, 95);
            this.groupBoxHistory.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxHistory.Name = "groupBoxHistory";
            this.groupBoxHistory.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxHistory.Size = new System.Drawing.Size(548, 326);
            this.groupBoxHistory.TabIndex = 1;
            this.groupBoxHistory.TabStop = false;
            this.groupBoxHistory.Text = "A Bit Of History";
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(582, 585);
            this.Controls.Add(this.linkLabelIntro);
            this.Controls.Add(this.groupBoxHistory);
            this.Controls.Add(this.groupBoxGFClockDisplay);
            this.Controls.Add(this.buttonClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.groupBoxGFClockDisplay.ResumeLayout(false);
            this.groupBoxGFClockDisplay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDetails)).EndInit();
            this.groupBoxHistory.ResumeLayout(false);
            this.groupBoxHistory.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.GroupBox groupBoxGFClockDisplay;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.Label labelDetails;
        private System.Windows.Forms.Label labelClockType;
        private System.Windows.Forms.DataGridView dataGridViewDetails;
        private System.Windows.Forms.ComboBox comboBoxClockType;
        private System.Windows.Forms.LinkLabel linkLabelIntro;
        private System.Windows.Forms.GroupBox groupBoxHistory;
        //private DataGridViewImageCellWithCustomValueColumn ColumnImage;
        private DataGridViewImageCellWithCustomValueColumn ColumnImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDate;
        private System.Windows.Forms.DataGridViewLinkColumn ColumnDetails;
    }
}