namespace Registrator.Module.Win.Editors
{
    partial class CustomCheckedListBoxEdit
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
            this.checkAll = new DevExpress.XtraEditors.CheckEdit();
            this.countersLabel = new System.Windows.Forms.Label();
            this.bottomPanel = new System.Windows.Forms.TableLayoutPanel();
            this.checkedListBox = new Registrator.Module.Win.Editors.AurumCheckedListBoxControl();
            ((System.ComponentModel.ISupportInitialize)(this.checkAll.Properties)).BeginInit();
            this.bottomPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkedListBox)).BeginInit();
            this.SuspendLayout();
            // 
            // checkAll
            // 
            this.checkAll.AutoSizeInLayoutControl = true;
            this.checkAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkAll.Location = new System.Drawing.Point(0, 0);
            this.checkAll.Margin = new System.Windows.Forms.Padding(0);
            this.checkAll.Name = "checkAll";
            this.checkAll.Properties.Caption = "Выбрать все";
            this.checkAll.Size = new System.Drawing.Size(176, 19);
            this.checkAll.TabIndex = 1;
            this.checkAll.CheckedChanged += new System.EventHandler(this.checkAll_CheckedChanged);
            // 
            // countersLabel
            // 
            this.countersLabel.AutoSize = true;
            this.countersLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.countersLabel.Location = new System.Drawing.Point(179, 0);
            this.countersLabel.Name = "countersLabel";
            this.countersLabel.Size = new System.Drawing.Size(53, 19);
            this.countersLabel.TabIndex = 2;
            this.countersLabel.Text = "0/0";
            this.countersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // bottomPanel
            // 
            this.bottomPanel.ColumnCount = 2;
            this.bottomPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.bottomPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.bottomPanel.Controls.Add(this.countersLabel, 0, 0);
            this.bottomPanel.Controls.Add(this.checkAll, 0, 0);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 27);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.RowCount = 1;
            this.bottomPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.bottomPanel.Size = new System.Drawing.Size(235, 19);
            this.bottomPanel.TabIndex = 4;
            // 
            // checkedListBox
            // 
            this.checkedListBox.Appearance.ForeColor = System.Drawing.SystemColors.GrayText;
            this.checkedListBox.Appearance.Options.UseForeColor = true;
            this.checkedListBox.CheckOnClick = true;
            this.checkedListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBox.HorizontalScrollbar = true;
            this.checkedListBox.ItemTextCriteria = null;
            this.checkedListBox.Location = new System.Drawing.Point(0, 0);
            this.checkedListBox.Name = "checkedListBox";
            this.checkedListBox.Size = new System.Drawing.Size(235, 27);
            this.checkedListBox.TabIndex = 0;
            this.checkedListBox.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.checkedListBox_ItemCheck);
            this.checkedListBox.CheckMemberChanged += new System.EventHandler(this.checkedListBox_CheckMemberChanged);
            this.checkedListBox.SelectedIndexChanged += new System.EventHandler(this.decimalEdit1_SelectedIndexChanged);
            // 
            // CustomCheckedListBoxEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkedListBox);
            this.Controls.Add(this.bottomPanel);
            this.Name = "CustomCheckedListBoxEdit";
            this.Size = new System.Drawing.Size(235, 46);
            ((System.ComponentModel.ISupportInitialize)(this.checkAll.Properties)).EndInit();
            this.bottomPanel.ResumeLayout(false);
            this.bottomPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkedListBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AurumCheckedListBoxControl checkedListBox;
        private DevExpress.XtraEditors.CheckEdit checkAll;
        private System.Windows.Forms.Label countersLabel;
        private System.Windows.Forms.TableLayoutPanel bottomPanel;

    }
}
