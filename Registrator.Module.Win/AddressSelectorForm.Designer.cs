namespace Registrator.Module.Win
{
    partial class AddressSelectorForm
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
            this.saveButton = new DevExpress.XtraEditors.SimpleButton();
            this.cancelButton = new DevExpress.XtraEditors.SimpleButton();
            this.areaText = new DevExpress.XtraEditors.ButtonEdit();
            this.cityText = new DevExpress.XtraEditors.ButtonEdit();
            this.townText = new DevExpress.XtraEditors.ButtonEdit();
            this.regionText = new DevExpress.XtraEditors.ButtonEdit();
            this.streetText = new DevExpress.XtraEditors.ButtonEdit();
            this.houseText = new DevExpress.XtraEditors.TextEdit();
            this.buildText = new DevExpress.XtraEditors.TextEdit();
            this.flatText = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.areaText.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cityText.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.townText.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.regionText.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.streetText.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.houseText.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.buildText.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.flatText.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(82, 228);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "Сохранить";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(186, 228);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Отмена";
            // 
            // areaText
            // 
            this.areaText.Location = new System.Drawing.Point(35, 47);
            this.areaText.Name = "areaText";
            this.areaText.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.areaText.Size = new System.Drawing.Size(211, 20);
            this.areaText.TabIndex = 2;
            // 
            // cityText
            // 
            this.cityText.Location = new System.Drawing.Point(35, 73);
            this.cityText.Name = "cityText";
            this.cityText.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.cityText.Size = new System.Drawing.Size(211, 20);
            this.cityText.TabIndex = 3;
            // 
            // townText
            // 
            this.townText.Location = new System.Drawing.Point(35, 99);
            this.townText.Name = "townText";
            this.townText.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.townText.Size = new System.Drawing.Size(211, 20);
            this.townText.TabIndex = 4;
            // 
            // regionText
            // 
            this.regionText.Location = new System.Drawing.Point(35, 12);
            this.regionText.Name = "regionText";
            this.regionText.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.regionText.Size = new System.Drawing.Size(211, 20);
            this.regionText.TabIndex = 5;
            // 
            // streetText
            // 
            this.streetText.Location = new System.Drawing.Point(35, 125);
            this.streetText.Name = "streetText";
            this.streetText.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.streetText.Size = new System.Drawing.Size(211, 20);
            this.streetText.TabIndex = 6;
            // 
            // houseText
            // 
            this.houseText.Location = new System.Drawing.Point(35, 162);
            this.houseText.Name = "houseText";
            this.houseText.Size = new System.Drawing.Size(66, 20);
            this.houseText.TabIndex = 7;
            // 
            // buildText
            // 
            this.buildText.Location = new System.Drawing.Point(107, 162);
            this.buildText.Name = "buildText";
            this.buildText.Size = new System.Drawing.Size(63, 20);
            this.buildText.TabIndex = 8;
            // 
            // flatText
            // 
            this.flatText.Location = new System.Drawing.Point(183, 162);
            this.flatText.Name = "flatText";
            this.flatText.Size = new System.Drawing.Size(63, 20);
            this.flatText.TabIndex = 9;
            // 
            // AddressSelectorForm
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.flatText);
            this.Controls.Add(this.buildText);
            this.Controls.Add(this.houseText);
            this.Controls.Add(this.streetText);
            this.Controls.Add(this.regionText);
            this.Controls.Add(this.townText);
            this.Controls.Add(this.cityText);
            this.Controls.Add(this.areaText);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.saveButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddressSelectorForm";
            this.Text = "AddressSelectorForm";
            ((System.ComponentModel.ISupportInitialize)(this.areaText.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cityText.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.townText.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.regionText.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.streetText.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.houseText.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.buildText.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.flatText.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton saveButton;
        private DevExpress.XtraEditors.SimpleButton cancelButton;
        private DevExpress.XtraEditors.ButtonEdit areaText;
        private DevExpress.XtraEditors.ButtonEdit cityText;
        private DevExpress.XtraEditors.ButtonEdit townText;
        private DevExpress.XtraEditors.ButtonEdit regionText;
        private DevExpress.XtraEditors.ButtonEdit streetText;
        private DevExpress.XtraEditors.TextEdit houseText;
        private DevExpress.XtraEditors.TextEdit buildText;
        private DevExpress.XtraEditors.TextEdit flatText;
    }
}