
namespace Dt.Editor
{
    partial class BatchCells
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
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this._cbTbls = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this._svcUrl = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this._cbSvcName = new System.Windows.Forms.ComboBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // linkLabel4
            // 
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.Location = new System.Drawing.Point(7, 73);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(41, 12);
            this.linkLabel4.TabIndex = 125;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = "选择表";
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Location = new System.Drawing.Point(7, 34);
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
            this._cbTbls.Location = new System.Drawing.Point(119, 68);
            this._cbTbls.Name = "_cbTbls";
            this._cbTbls.Size = new System.Drawing.Size(258, 20);
            this._cbTbls.TabIndex = 122;
            this._cbTbls.DropDown += new System.EventHandler(this._cbTbls_DropDown);
            this._cbTbls.SelectedIndexChanged += new System.EventHandler(this._cbTbls_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Location = new System.Drawing.Point(3, 68);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(117, 20);
            this.label9.TabIndex = 123;
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Location = new System.Drawing.Point(3, 9);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(374, 21);
            this.label10.TabIndex = 121;
            this.label10.Text = "根据表结构批量生成格";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _svcUrl
            // 
            this._svcUrl.Location = new System.Drawing.Point(119, 29);
            this._svcUrl.Name = "_svcUrl";
            this._svcUrl.Size = new System.Drawing.Size(258, 21);
            this._svcUrl.TabIndex = 119;
            // 
            // label11
            // 
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label11.Location = new System.Drawing.Point(3, 29);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(117, 21);
            this.label11.TabIndex = 120;
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _cbSvcName
            // 
            this._cbSvcName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbSvcName.FormattingEnabled = true;
            this._cbSvcName.Location = new System.Drawing.Point(119, 49);
            this._cbSvcName.Name = "_cbSvcName";
            this._cbSvcName.Size = new System.Drawing.Size(258, 20);
            this._cbSvcName.TabIndex = 149;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(7, 53);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(53, 12);
            this.linkLabel1.TabIndex = 148;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "服务名称";
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(3, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(117, 20);
            this.label3.TabIndex = 147;
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BatchCells
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._cbSvcName);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.linkLabel4);
            this.Controls.Add(this.linkLabel3);
            this.Controls.Add(this._cbTbls);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this._svcUrl);
            this.Controls.Add(this.label11);
            this.Name = "BatchCells";
            this.Size = new System.Drawing.Size(380, 327);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.ComboBox _cbTbls;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox _svcUrl;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox _cbSvcName;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label3;
    }
}
