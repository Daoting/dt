
namespace Dt.Editor
{
    partial class FvXaml
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
            this.label1 = new System.Windows.Forms.Label();
            this._name = new System.Windows.Forms.TextBox();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this._cbTbls = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this._svcUrl = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._cbReadonly = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this._cbSvcName = new System.Windows.Forms.ComboBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(250, 21);
            this.label1.TabIndex = 2;
            this.label1.Text = "Name";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _name
            // 
            this._name.Location = new System.Drawing.Point(249, 0);
            this._name.Name = "_name";
            this._name.Size = new System.Drawing.Size(211, 21);
            this._name.TabIndex = 3;
            this._name.Text = "_fv";
            // 
            // linkLabel4
            // 
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.Location = new System.Drawing.Point(4, 130);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(41, 12);
            this.linkLabel4.TabIndex = 118;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = "选择表";
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Location = new System.Drawing.Point(4, 91);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(53, 12);
            this.linkLabel3.TabIndex = 117;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "服务地址";
            // 
            // _cbTbls
            // 
            this._cbTbls.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbTbls.FormattingEnabled = true;
            this._cbTbls.Location = new System.Drawing.Point(183, 125);
            this._cbTbls.Name = "_cbTbls";
            this._cbTbls.Size = new System.Drawing.Size(277, 20);
            this._cbTbls.TabIndex = 115;
            this._cbTbls.DropDown += new System.EventHandler(this._cbTbls_DropDown);
            // 
            // label9
            // 
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Location = new System.Drawing.Point(0, 125);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(184, 20);
            this.label9.TabIndex = 116;
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Location = new System.Drawing.Point(0, 66);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(460, 21);
            this.label10.TabIndex = 114;
            this.label10.Text = "根据表结构自动生成格";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _svcUrl
            // 
            this._svcUrl.Location = new System.Drawing.Point(183, 86);
            this._svcUrl.Name = "_svcUrl";
            this._svcUrl.Size = new System.Drawing.Size(277, 21);
            this._svcUrl.TabIndex = 112;
            // 
            // label11
            // 
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label11.Location = new System.Drawing.Point(0, 86);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(184, 21);
            this.label11.TabIndex = 113;
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(0, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(250, 21);
            this.label2.TabIndex = 120;
            this.label2.Text = "只读";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _cbReadonly
            // 
            this._cbReadonly.AutoSize = true;
            this._cbReadonly.Location = new System.Drawing.Point(320, 23);
            this._cbReadonly.Name = "_cbReadonly";
            this._cbReadonly.Size = new System.Drawing.Size(15, 14);
            this._cbReadonly.TabIndex = 119;
            this._cbReadonly.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(249, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(211, 21);
            this.label3.TabIndex = 121;
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _cbSvcName
            // 
            this._cbSvcName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbSvcName.FormattingEnabled = true;
            this._cbSvcName.Location = new System.Drawing.Point(183, 106);
            this._cbSvcName.Name = "_cbSvcName";
            this._cbSvcName.Size = new System.Drawing.Size(277, 20);
            this._cbSvcName.TabIndex = 149;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(4, 110);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(53, 12);
            this.linkLabel1.TabIndex = 148;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "服务名称";
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Location = new System.Drawing.Point(0, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(184, 20);
            this.label4.TabIndex = 147;
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FvXaml
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._cbSvcName);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._cbReadonly);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.linkLabel4);
            this.Controls.Add(this.linkLabel3);
            this.Controls.Add(this._cbTbls);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this._svcUrl);
            this.Controls.Add(this.label11);
            this.Controls.Add(this._name);
            this.Controls.Add(this.label1);
            this.Name = "FvXaml";
            this.Size = new System.Drawing.Size(460, 300);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _name;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.ComboBox _cbTbls;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox _svcUrl;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox _cbReadonly;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox _cbSvcName;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label4;
    }
}
