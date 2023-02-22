
namespace Dt
{
    partial class SingleForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SingleForm));
            this.label2 = new System.Windows.Forms.Label();
            this._ns = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._clsRoot = new System.Windows.Forms.TextBox();
            this._btnOK = new System.Windows.Forms.Button();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this._svcUrl = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this._dg = new System.Windows.Forms.DataGridView();
            this.Tbl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cls = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.序号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this._ns.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(12, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 21);
            this.label1.TabIndex = 18;
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _clsRoot
            // 
            this._clsRoot.Location = new System.Drawing.Point(187, 47);
            this._clsRoot.Name = "_clsRoot";
            this._clsRoot.Size = new System.Drawing.Size(269, 21);
            this._clsRoot.TabIndex = 12;
            // 
            // _btnOK
            // 
            this._btnOK.Location = new System.Drawing.Point(381, 275);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(75, 23);
            this._btnOK.TabIndex = 100;
            this._btnOK.Text = "确认";
            this._btnOK.UseVisualStyleBackColor = true;
            this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(14, 52);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(77, 12);
            this.linkLabel2.TabIndex = 103;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "框架类的词根";
            // 
            // _svcUrl
            // 
            this._svcUrl.Location = new System.Drawing.Point(187, 27);
            this._svcUrl.Name = "_svcUrl";
            this._svcUrl.Size = new System.Drawing.Size(269, 21);
            this._svcUrl.TabIndex = 105;
            // 
            // label7
            // 
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Location = new System.Drawing.Point(12, 27);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(176, 21);
            this.label7.TabIndex = 106;
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Location = new System.Drawing.Point(14, 31);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(53, 12);
            this.linkLabel3.TabIndex = 110;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "服务地址";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 190);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(209, 108);
            this.label4.TabIndex = 143;
            this.label4.Text = "* 生成框架前确保实体类已存在\r\n\r\n* 通常只选择一个表，也可增加扩展表\r\n\r\n* 选择多个表时采用虚拟实体方式\r\n\r\n* 按F2或双击【实体类名】可编辑\r\n\r" +
    "\n* 可通过修改[序号]控制实体顺序\r\n";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(381, 185);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 141;
            this.btnAdd.Text = "选择";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
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
            this.序号});
            this._dg.Location = new System.Drawing.Point(12, 67);
            this._dg.Name = "_dg";
            this._dg.RowHeadersVisible = false;
            this._dg.RowTemplate.Height = 23;
            this._dg.Size = new System.Drawing.Size(444, 112);
            this._dg.TabIndex = 140;
            // 
            // Tbl
            // 
            this.Tbl.HeaderText = "表名";
            this.Tbl.Name = "Tbl";
            this.Tbl.ReadOnly = true;
            this.Tbl.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Tbl.Width = 140;
            // 
            // Cls
            // 
            this.Cls.HeaderText = "实体类名";
            this.Cls.Name = "Cls";
            this.Cls.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Cls.Width = 140;
            // 
            // 序号
            // 
            this.序号.HeaderText = "序号";
            this.序号.Name = "序号";
            this.序号.Width = 120;
            // 
            // SingleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 315);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this._dg);
            this.Controls.Add(this.linkLabel3);
            this.Controls.Add(this._svcUrl);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this._btnOK);
            this.Controls.Add(this._clsRoot);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._ns);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SingleForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加单实体框架文件";
            ((System.ComponentModel.ISupportInitialize)(this._dg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _ns;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _clsRoot;
        private System.Windows.Forms.Button _btnOK;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.TextBox _svcUrl;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.DataGridView _dg;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tbl;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cls;
        private System.Windows.Forms.DataGridViewTextBoxColumn 序号;
    }
}