
namespace Dt.Editor
{
    partial class CMask
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this._footer = new Dt.Editor.CellFooter();
            this._header = new Dt.Editor.CellHeader();
            this._maskType = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this._mask = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._autoComplete = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this._showPlaceHolder = new System.Windows.Forms.CheckBox();
            this._saveLiteral = new System.Windows.Forms.CheckBox();
            this._useAsDisplayFormat = new System.Windows.Forms.CheckBox();
            this._allowNullInput = new System.Windows.Forms.CheckBox();
            this._ignoreBlank = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _footer
            // 
            this._footer.Location = new System.Drawing.Point(0, 237);
            this._footer.Name = "_footer";
            this._footer.Size = new System.Drawing.Size(380, 170);
            this._footer.TabIndex = 43;
            // 
            // _header
            // 
            this._header.Dock = System.Windows.Forms.DockStyle.Top;
            this._header.Location = new System.Drawing.Point(0, 0);
            this._header.Name = "_header";
            this._header.Size = new System.Drawing.Size(380, 42);
            this._header.TabIndex = 42;
            // 
            // _maskType
            // 
            this._maskType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._maskType.FormattingEnabled = true;
            this._maskType.Items.AddRange(new object[] {
            "Simple",
            "DateTime",
            "DateTimeAdvancingCaret",
            "Numeric",
            "RegEx",
            "Regular"});
            this._maskType.Location = new System.Drawing.Point(186, 40);
            this._maskType.Name = "_maskType";
            this._maskType.Size = new System.Drawing.Size(194, 20);
            this._maskType.TabIndex = 45;
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Location = new System.Drawing.Point(0, 40);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(187, 20);
            this.label8.TabIndex = 44;
            this.label8.Text = "掩码类型(MaskType)";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _mask
            // 
            this._mask.Location = new System.Drawing.Point(186, 59);
            this._mask.Name = "_mask";
            this._mask.Size = new System.Drawing.Size(194, 21);
            this._mask.TabIndex = 47;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(0, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(187, 21);
            this.label2.TabIndex = 46;
            this.label2.Text = "掩码内容(Mask)";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _autoComplete
            // 
            this._autoComplete.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._autoComplete.FormattingEnabled = true;
            this._autoComplete.Items.AddRange(new object[] {
            "Default",
            "None",
            "Strong",
            "Optimistic"});
            this._autoComplete.Location = new System.Drawing.Point(186, 79);
            this._autoComplete.Name = "_autoComplete";
            this._autoComplete.Size = new System.Drawing.Size(194, 20);
            this._autoComplete.TabIndex = 49;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(0, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 20);
            this.label1.TabIndex = 48;
            this.label1.Text = "自动完成方式(AutoComplete)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _showPlaceHolder
            // 
            this._showPlaceHolder.AutoSize = true;
            this._showPlaceHolder.Location = new System.Drawing.Point(4, 112);
            this._showPlaceHolder.Name = "_showPlaceHolder";
            this._showPlaceHolder.Size = new System.Drawing.Size(210, 16);
            this._showPlaceHolder.TabIndex = 50;
            this._showPlaceHolder.Text = "是否显示占位符(ShowPlaceHolder)";
            this._showPlaceHolder.UseVisualStyleBackColor = true;
            // 
            // _saveLiteral
            // 
            this._saveLiteral.AutoSize = true;
            this._saveLiteral.Location = new System.Drawing.Point(4, 135);
            this._saveLiteral.Name = "_saveLiteral";
            this._saveLiteral.Size = new System.Drawing.Size(174, 16);
            this._saveLiteral.TabIndex = 51;
            this._saveLiteral.Text = "是否保存文本(SaveLiteral)";
            this._saveLiteral.UseVisualStyleBackColor = true;
            // 
            // _useAsDisplayFormat
            // 
            this._useAsDisplayFormat.AutoSize = true;
            this._useAsDisplayFormat.Location = new System.Drawing.Point(4, 158);
            this._useAsDisplayFormat.Name = "_useAsDisplayFormat";
            this._useAsDisplayFormat.Size = new System.Drawing.Size(204, 16);
            this._useAsDisplayFormat.TabIndex = 52;
            this._useAsDisplayFormat.Text = "格式化显示(UseAsDisplayFormat)";
            this._useAsDisplayFormat.UseVisualStyleBackColor = true;
            // 
            // _allowNullInput
            // 
            this._allowNullInput.AutoSize = true;
            this._allowNullInput.Location = new System.Drawing.Point(4, 181);
            this._allowNullInput.Name = "_allowNullInput";
            this._allowNullInput.Size = new System.Drawing.Size(180, 16);
            this._allowNullInput.TabIndex = 53;
            this._allowNullInput.Text = "是否可为空(AllowNullInput)";
            this._allowNullInput.UseVisualStyleBackColor = true;
            // 
            // _ignoreBlank
            // 
            this._ignoreBlank.AutoSize = true;
            this._ignoreBlank.Location = new System.Drawing.Point(4, 204);
            this._ignoreBlank.Name = "_ignoreBlank";
            this._ignoreBlank.Size = new System.Drawing.Size(174, 16);
            this._ignoreBlank.TabIndex = 54;
            this._ignoreBlank.Text = "是否忽略空格(IgnoreBlank)";
            this._ignoreBlank.UseVisualStyleBackColor = true;
            // 
            // CMask
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._ignoreBlank);
            this.Controls.Add(this._allowNullInput);
            this.Controls.Add(this._useAsDisplayFormat);
            this.Controls.Add(this._saveLiteral);
            this.Controls.Add(this._showPlaceHolder);
            this.Controls.Add(this._autoComplete);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._mask);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._maskType);
            this.Controls.Add(this.label8);
            this.Controls.Add(this._footer);
            this.Controls.Add(this._header);
            this.Name = "CMask";
            this.Size = new System.Drawing.Size(380, 416);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CellFooter _footer;
        private CellHeader _header;
        private System.Windows.Forms.ComboBox _maskType;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox _mask;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox _autoComplete;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox _showPlaceHolder;
        private System.Windows.Forms.CheckBox _saveLiteral;
        private System.Windows.Forms.CheckBox _useAsDisplayFormat;
        private System.Windows.Forms.CheckBox _allowNullInput;
        private System.Windows.Forms.CheckBox _ignoreBlank;
    }
}
