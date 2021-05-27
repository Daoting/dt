
namespace Dt.Editor
{
    partial class CNum
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
            this._fv = new Dt.Editor.FvCell();
            this._decimals = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this._maximum = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._minimum = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._customUnit = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._nullValue = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this._largeChange = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this._smallChange = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this._isInteger = new System.Windows.Forms.CheckBox();
            this._updateTimely = new System.Windows.Forms.CheckBox();
            this._autoReverse = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this._valueFormat = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // _fv
            // 
            this._fv.Dock = System.Windows.Forms.DockStyle.Top;
            this._fv.Location = new System.Drawing.Point(0, 0);
            this._fv.Name = "_fv";
            this._fv.Size = new System.Drawing.Size(380, 267);
            this._fv.TabIndex = 0;
            // 
            // _decimals
            // 
            this._decimals.Location = new System.Drawing.Point(186, 263);
            this._decimals.Name = "_decimals";
            this._decimals.Size = new System.Drawing.Size(194, 21);
            this._decimals.TabIndex = 22;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(0, 263);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(187, 21);
            this.label5.TabIndex = 21;
            this.label5.Text = "小数位数(Decimals)";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _maximum
            // 
            this._maximum.Location = new System.Drawing.Point(186, 283);
            this._maximum.Name = "_maximum";
            this._maximum.Size = new System.Drawing.Size(194, 21);
            this._maximum.TabIndex = 24;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(0, 283);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 21);
            this.label1.TabIndex = 23;
            this.label1.Text = "最大值(Maximum)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _minimum
            // 
            this._minimum.Location = new System.Drawing.Point(186, 303);
            this._minimum.Name = "_minimum";
            this._minimum.Size = new System.Drawing.Size(194, 21);
            this._minimum.TabIndex = 26;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(0, 303);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(187, 21);
            this.label2.TabIndex = 25;
            this.label2.Text = "最小值(Minimum)";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _customUnit
            // 
            this._customUnit.Location = new System.Drawing.Point(186, 323);
            this._customUnit.Name = "_customUnit";
            this._customUnit.Size = new System.Drawing.Size(194, 21);
            this._customUnit.TabIndex = 28;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(0, 323);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(187, 21);
            this.label3.TabIndex = 27;
            this.label3.Text = "自定义单位(CustomUnit)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _nullValue
            // 
            this._nullValue.Location = new System.Drawing.Point(186, 343);
            this._nullValue.Name = "_nullValue";
            this._nullValue.Size = new System.Drawing.Size(194, 21);
            this._nullValue.TabIndex = 30;
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Location = new System.Drawing.Point(0, 343);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(187, 21);
            this.label4.TabIndex = 29;
            this.label4.Text = "为空时的串(NullValue)";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _largeChange
            // 
            this._largeChange.Location = new System.Drawing.Point(186, 363);
            this._largeChange.Name = "_largeChange";
            this._largeChange.Size = new System.Drawing.Size(194, 21);
            this._largeChange.TabIndex = 32;
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point(0, 363);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(187, 21);
            this.label6.TabIndex = 31;
            this.label6.Text = "最大变化量(LargeChange)";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _smallChange
            // 
            this._smallChange.Location = new System.Drawing.Point(186, 383);
            this._smallChange.Name = "_smallChange";
            this._smallChange.Size = new System.Drawing.Size(194, 21);
            this._smallChange.TabIndex = 34;
            // 
            // label7
            // 
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Location = new System.Drawing.Point(0, 383);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(187, 21);
            this.label7.TabIndex = 33;
            this.label7.Text = "最小变化量(SmallChange)";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _isInteger
            // 
            this._isInteger.AutoSize = true;
            this._isInteger.Location = new System.Drawing.Point(0, 439);
            this._isInteger.Name = "_isInteger";
            this._isInteger.Size = new System.Drawing.Size(138, 16);
            this._isInteger.TabIndex = 35;
            this._isInteger.Text = "为整数值(IsInteger)";
            this._isInteger.UseVisualStyleBackColor = true;
            // 
            // _updateTimely
            // 
            this._updateTimely.AutoSize = true;
            this._updateTimely.Location = new System.Drawing.Point(198, 439);
            this._updateTimely.Name = "_updateTimely";
            this._updateTimely.Size = new System.Drawing.Size(168, 16);
            this._updateTimely.TabIndex = 36;
            this._updateTimely.Text = "实时更新值(UpdateTimely)";
            this._updateTimely.UseVisualStyleBackColor = true;
            // 
            // _autoReverse
            // 
            this._autoReverse.AutoSize = true;
            this._autoReverse.Location = new System.Drawing.Point(0, 466);
            this._autoReverse.Name = "_autoReverse";
            this._autoReverse.Size = new System.Drawing.Size(174, 16);
            this._autoReverse.TabIndex = 37;
            this._autoReverse.Text = "自动循环调值(AutoReverse)";
            this._autoReverse.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Location = new System.Drawing.Point(0, 403);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(187, 20);
            this.label8.TabIndex = 38;
            this.label8.Text = "格式化显示(ValueFormat)";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _valueFormat
            // 
            this._valueFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._valueFormat.FormattingEnabled = true;
            this._valueFormat.Items.AddRange(new object[] {
            "Numeric",
            "Currency",
            "Percentage"});
            this._valueFormat.Location = new System.Drawing.Point(186, 403);
            this._valueFormat.Name = "_valueFormat";
            this._valueFormat.Size = new System.Drawing.Size(194, 20);
            this._valueFormat.TabIndex = 39;
            // 
            // CNum
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._valueFormat);
            this.Controls.Add(this.label8);
            this.Controls.Add(this._autoReverse);
            this.Controls.Add(this._updateTimely);
            this.Controls.Add(this._isInteger);
            this.Controls.Add(this._smallChange);
            this.Controls.Add(this.label7);
            this.Controls.Add(this._largeChange);
            this.Controls.Add(this.label6);
            this.Controls.Add(this._nullValue);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._customUnit);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._minimum);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._maximum);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._decimals);
            this.Controls.Add(this.label5);
            this.Controls.Add(this._fv);
            this.Name = "CNum";
            this.Size = new System.Drawing.Size(380, 511);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FvCell _fv;
        private System.Windows.Forms.TextBox _decimals;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox _maximum;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _minimum;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _customUnit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _nullValue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _largeChange;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox _smallChange;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox _isInteger;
        private System.Windows.Forms.CheckBox _updateTimely;
        private System.Windows.Forms.CheckBox _autoReverse;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox _valueFormat;
    }
}
