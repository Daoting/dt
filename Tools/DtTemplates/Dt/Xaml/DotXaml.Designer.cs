
namespace Dt.Editor
{
    partial class DotXaml
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DotXaml));
            this.label2 = new System.Windows.Forms.Label();
            this._id = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._format = new System.Windows.Forms.TextBox();
            this._btnOK = new System.Windows.Forms.Button();
            this._font = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this._cbUI = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this._cbTbls = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this._svcUrl = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(12, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(176, 21);
            this.label2.TabIndex = 3;
            this.label2.Text = "ID";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _id
            // 
            this._id.Location = new System.Drawing.Point(187, 7);
            this._id.Name = "_id";
            this._id.Size = new System.Drawing.Size(269, 21);
            this._id.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(12, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 21);
            this.label1.TabIndex = 18;
            this.label1.Text = "格式串(如时间小数位枚举类型)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _format
            // 
            this._format.Location = new System.Drawing.Point(187, 47);
            this._format.Name = "_format";
            this._format.Size = new System.Drawing.Size(269, 21);
            this._format.TabIndex = 12;
            // 
            // _btnOK
            // 
            this._btnOK.Location = new System.Drawing.Point(381, 102);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(75, 23);
            this._btnOK.TabIndex = 100;
            this._btnOK.Text = "添加";
            this._btnOK.UseVisualStyleBackColor = true;
            this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
            // 
            // _font
            // 
            this._font.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._font.FormattingEnabled = true;
            this._font.Items.AddRange(new object[] {
            "默认",
            "小灰",
            "黑白",
            "蓝白",
            "红白"});
            this._font.Location = new System.Drawing.Point(187, 67);
            this._font.Name = "_font";
            this._font.Size = new System.Drawing.Size(269, 20);
            this._font.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Location = new System.Drawing.Point(12, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(176, 21);
            this.label4.TabIndex = 26;
            this.label4.Text = "文字样式";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _cbUI
            // 
            this._cbUI.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbUI.FormattingEnabled = true;
            this._cbUI.Items.AddRange(new object[] {
            "Default",
            "Icon",
            "CheckBox",
            "Image",
            "File",
            "Enum",
            "AutoDate",
            "Warning"});
            this._cbUI.Location = new System.Drawing.Point(187, 27);
            this._cbUI.Name = "_cbUI";
            this._cbUI.Size = new System.Drawing.Size(269, 20);
            this._cbUI.TabIndex = 108;
            // 
            // label9
            // 
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Location = new System.Drawing.Point(12, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(176, 21);
            this.label9.TabIndex = 109;
            this.label9.Text = "UI类型";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkLabel4
            // 
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.Location = new System.Drawing.Point(16, 233);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(41, 12);
            this.linkLabel4.TabIndex = 125;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = "选择表";
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Location = new System.Drawing.Point(16, 213);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(53, 12);
            this.linkLabel3.TabIndex = 124;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "服务地址";
            // 
            // _cbTbls
            // 
            this._cbTbls.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbTbls.FormattingEnabled = true;
            this._cbTbls.Location = new System.Drawing.Point(189, 228);
            this._cbTbls.Name = "_cbTbls";
            this._cbTbls.Size = new System.Drawing.Size(267, 20);
            this._cbTbls.TabIndex = 122;
            this._cbTbls.DropDown += new System.EventHandler(this._cbTbls_DropDown);
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(12, 228);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(178, 21);
            this.label3.TabIndex = 123;
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Location = new System.Drawing.Point(12, 188);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(444, 21);
            this.label10.TabIndex = 121;
            this.label10.Text = "根据表结构自动生成项模板";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _svcUrl
            // 
            this._svcUrl.Location = new System.Drawing.Point(189, 208);
            this._svcUrl.Name = "_svcUrl";
            this._svcUrl.Size = new System.Drawing.Size(267, 21);
            this._svcUrl.TabIndex = 119;
            // 
            // label11
            // 
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label11.Location = new System.Drawing.Point(12, 208);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(178, 21);
            this.label11.TabIndex = 120;
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(381, 262);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 126;
            this.button1.Text = "批量添加";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // DotXaml
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 300);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.linkLabel4);
            this.Controls.Add(this.linkLabel3);
            this.Controls.Add(this._cbTbls);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label10);
            this.Controls.Add(this._svcUrl);
            this.Controls.Add(this.label11);
            this.Controls.Add(this._cbUI);
            this.Controls.Add(this.label9);
            this.Controls.Add(this._font);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._btnOK);
            this.Controls.Add(this._format);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._id);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DotXaml";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加 Dot";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _id;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _format;
        private System.Windows.Forms.Button _btnOK;
        private System.Windows.Forms.ComboBox _font;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox _cbUI;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.ComboBox _cbTbls;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox _svcUrl;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button button1;
    }
}