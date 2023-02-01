
namespace Dt
{
    partial class InsertEntityClsForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InsertEntityClsForm));
            this.label2 = new System.Windows.Forms.Label();
            this._ns = new System.Windows.Forms.TextBox();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this._svcUrl = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this._dg = new System.Windows.Forms.DataGridView();
            this.Tbl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cls = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Del = new System.Windows.Forms.DataGridViewButtonColumn();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this._rbDef = new System.Windows.Forms.RadioButton();
            this._rbReplace = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._dg)).BeginInit();
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
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Location = new System.Drawing.Point(14, 32);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(53, 12);
            this.linkLabel3.TabIndex = 132;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "服务地址";
            // 
            // _svcUrl
            // 
            this._svcUrl.Location = new System.Drawing.Point(187, 27);
            this._svcUrl.Name = "_svcUrl";
            this._svcUrl.Size = new System.Drawing.Size(269, 21);
            this._svcUrl.TabIndex = 127;
            // 
            // label11
            // 
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label11.Location = new System.Drawing.Point(12, 27);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(176, 21);
            this.label11.TabIndex = 128;
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _dg
            // 
            this._dg.AllowUserToAddRows = false;
            this._dg.AllowUserToDeleteRows = false;
            this._dg.AllowUserToResizeRows = false;
            this._dg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dg.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Tbl,
            this.Cls,
            this.Del});
            this._dg.Location = new System.Drawing.Point(12, 47);
            this._dg.Name = "_dg";
            this._dg.RowHeadersVisible = false;
            this._dg.RowTemplate.Height = 23;
            this._dg.Size = new System.Drawing.Size(444, 229);
            this._dg.TabIndex = 133;
            this._dg.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._dg_CellContentClick);
            // 
            // Tbl
            // 
            this.Tbl.HeaderText = "表名";
            this.Tbl.Name = "Tbl";
            this.Tbl.ReadOnly = true;
            this.Tbl.Width = 174;
            // 
            // Cls
            // 
            this.Cls.HeaderText = "实体类名";
            this.Cls.Name = "Cls";
            this.Cls.Width = 180;
            // 
            // Del
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = "删除";
            this.Del.DefaultCellStyle = dataGridViewCellStyle1;
            this.Del.HeaderText = "操作";
            this.Del.Name = "Del";
            this.Del.Text = "";
            this.Del.Width = 70;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(381, 282);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 134;
            this.btnAdd.Text = "增加";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(263, 282);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 135;
            this.btnClear.Text = "清空";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(381, 399);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 136;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // _rbDef
            // 
            this._rbDef.AutoSize = true;
            this._rbDef.Checked = true;
            this._rbDef.Location = new System.Drawing.Point(16, 373);
            this._rbDef.Name = "_rbDef";
            this._rbDef.Size = new System.Drawing.Size(83, 16);
            this._rbDef.TabIndex = 137;
            this._rbDef.TabStop = true;
            this._rbDef.Text = "不重新生成";
            this._rbDef.UseVisualStyleBackColor = true;
            // 
            // _rbReplace
            // 
            this._rbReplace.AutoSize = true;
            this._rbReplace.Location = new System.Drawing.Point(16, 398);
            this._rbReplace.Name = "_rbReplace";
            this._rbReplace.Size = new System.Drawing.Size(155, 16);
            this._rbReplace.TabIndex = 138;
            this._rbReplace.Text = "提示选择是否覆盖旧文件";
            this._rbReplace.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 287);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(185, 36);
            this.label1.TabIndex = 139;
            this.label1.Text = "* 按F2或双击【实体类名】可编辑\r\n\r\n* 建议实体类名后缀：X";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 352);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(185, 12);
            this.label3.TabIndex = 140;
            this.label3.Text = "实体类自定义部分的cs文件存在时";
            // 
            // InsertEntityClsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 434);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._rbReplace);
            this.Controls.Add(this._rbDef);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this._dg);
            this.Controls.Add(this.linkLabel3);
            this.Controls.Add(this._svcUrl);
            this.Controls.Add(this.label11);
            this.Controls.Add(this._ns);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InsertEntityClsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加实体类";
            ((System.ComponentModel.ISupportInitialize)(this._dg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _ns;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.TextBox _svcUrl;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DataGridView _dg;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.RadioButton _rbDef;
        private System.Windows.Forms.RadioButton _rbReplace;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tbl;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cls;
        private System.Windows.Forms.DataGridViewButtonColumn Del;
        private System.Windows.Forms.Label label3;
    }
}