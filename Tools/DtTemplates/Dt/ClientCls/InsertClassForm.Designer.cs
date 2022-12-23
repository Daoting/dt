
namespace Dt
{
    partial class InsertClassForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InsertClassForm));
            this.label2 = new System.Windows.Forms.Label();
            this._ns = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._cls = new System.Windows.Forms.TextBox();
            this._btnLv = new System.Windows.Forms.Button();
            this._btnEntity = new System.Windows.Forms.Button();
            this._btnFv = new System.Windows.Forms.Button();
            this._btnCList = new System.Windows.Forms.Button();
            this._btnAgent = new System.Windows.Forms.Button();
            this._lbl = new System.Windows.Forms.Label();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this._cbTbls = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this._svcUrl = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(12, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(176, 21);
            this.label2.TabIndex = 3;
            this.label2.Text = "命名空间";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _ns
            // 
            this._ns.Location = new System.Drawing.Point(187, 7);
            this._ns.Name = "_ns";
            this._ns.Size = new System.Drawing.Size(269, 21);
            this._ns.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(12, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 21);
            this.label1.TabIndex = 18;
            this.label1.Text = "类名";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _cls
            // 
            this._cls.Location = new System.Drawing.Point(187, 27);
            this._cls.Name = "_cls";
            this._cls.Size = new System.Drawing.Size(269, 21);
            this._cls.TabIndex = 19;
            // 
            // _btnLv
            // 
            this._btnLv.Location = new System.Drawing.Point(12, 73);
            this._btnLv.Name = "_btnLv";
            this._btnLv.Size = new System.Drawing.Size(190, 23);
            this._btnLv.TabIndex = 22;
            this._btnLv.Text = "Lv的Call类";
            this._btnLv.UseVisualStyleBackColor = true;
            this._btnLv.Click += new System.EventHandler(this._btnLv_Click);
            // 
            // _btnEntity
            // 
            this._btnEntity.Location = new System.Drawing.Point(266, 236);
            this._btnEntity.Name = "_btnEntity";
            this._btnEntity.Size = new System.Drawing.Size(190, 23);
            this._btnEntity.TabIndex = 28;
            this._btnEntity.Text = "Entity子类(加后缀 Obj)";
            this._btnEntity.UseVisualStyleBackColor = true;
            this._btnEntity.Click += new System.EventHandler(this._btnEntity_Click);
            // 
            // _btnFv
            // 
            this._btnFv.Location = new System.Drawing.Point(266, 73);
            this._btnFv.Name = "_btnFv";
            this._btnFv.Size = new System.Drawing.Size(190, 23);
            this._btnFv.TabIndex = 29;
            this._btnFv.Text = "Fv的Call类";
            this._btnFv.UseVisualStyleBackColor = true;
            this._btnFv.Click += new System.EventHandler(this._btnFv_Click);
            // 
            // _btnCList
            // 
            this._btnCList.Location = new System.Drawing.Point(12, 107);
            this._btnCList.Name = "_btnCList";
            this._btnCList.Size = new System.Drawing.Size(190, 23);
            this._btnCList.TabIndex = 30;
            this._btnCList.Text = "CList的Ex类";
            this._btnCList.UseVisualStyleBackColor = true;
            this._btnCList.Click += new System.EventHandler(this._btnCList_Click);
            // 
            // _btnAgent
            // 
            this._btnAgent.Location = new System.Drawing.Point(266, 107);
            this._btnAgent.Name = "_btnAgent";
            this._btnAgent.Size = new System.Drawing.Size(190, 23);
            this._btnAgent.TabIndex = 31;
            this._btnAgent.Text = "Agent类(加前缀 At)";
            this._btnAgent.UseVisualStyleBackColor = true;
            this._btnAgent.Click += new System.EventHandler(this._btnAgent_Click);
            // 
            // _lbl
            // 
            this._lbl.AutoSize = true;
            this._lbl.Location = new System.Drawing.Point(13, 54);
            this._lbl.Name = "_lbl";
            this._lbl.Size = new System.Drawing.Size(29, 12);
            this._lbl.TabIndex = 32;
            this._lbl.Text = "Info";
            // 
            // linkLabel4
            // 
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.Location = new System.Drawing.Point(16, 205);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(41, 12);
            this.linkLabel4.TabIndex = 133;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = "选择表";
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Location = new System.Drawing.Point(16, 185);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(53, 12);
            this.linkLabel3.TabIndex = 132;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "服务地址";
            // 
            // _cbTbls
            // 
            this._cbTbls.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbTbls.FormattingEnabled = true;
            this._cbTbls.Location = new System.Drawing.Point(189, 200);
            this._cbTbls.Name = "_cbTbls";
            this._cbTbls.Size = new System.Drawing.Size(267, 20);
            this._cbTbls.TabIndex = 130;
            this._cbTbls.DropDown += new System.EventHandler(this._cbTbls_DropDown);
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(12, 200);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(178, 21);
            this.label3.TabIndex = 131;
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Location = new System.Drawing.Point(12, 160);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(444, 21);
            this.label10.TabIndex = 129;
            this.label10.Text = "根据表结构生成Entity子类";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _svcUrl
            // 
            this._svcUrl.Location = new System.Drawing.Point(189, 180);
            this._svcUrl.Name = "_svcUrl";
            this._svcUrl.Size = new System.Drawing.Size(267, 21);
            this._svcUrl.TabIndex = 127;
            // 
            // label11
            // 
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label11.Location = new System.Drawing.Point(12, 180);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(178, 21);
            this.label11.TabIndex = 128;
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(68, 241);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(179, 12);
            this.label4.TabIndex = 134;
            this.label4.Text = "*未选择表时只生成空Entity子类";
            // 
            // InsertClassForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 285);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.linkLabel4);
            this.Controls.Add(this.linkLabel3);
            this.Controls.Add(this._cbTbls);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label10);
            this.Controls.Add(this._svcUrl);
            this.Controls.Add(this.label11);
            this.Controls.Add(this._lbl);
            this.Controls.Add(this._btnAgent);
            this.Controls.Add(this._btnCList);
            this.Controls.Add(this._btnFv);
            this.Controls.Add(this._btnEntity);
            this.Controls.Add(this._btnLv);
            this.Controls.Add(this._cls);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._ns);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InsertClassForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加类";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _ns;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _cls;
        private System.Windows.Forms.Button _btnLv;
        private System.Windows.Forms.Button _btnEntity;
        private System.Windows.Forms.Button _btnFv;
        private System.Windows.Forms.Button _btnCList;
        private System.Windows.Forms.Button _btnAgent;
        private System.Windows.Forms.Label _lbl;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.ComboBox _cbTbls;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox _svcUrl;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label4;
    }
}