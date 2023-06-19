
namespace Dt
{
    partial class ManyToManyForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManyToManyForm));
            this.label2 = new System.Windows.Forms.Label();
            this._ns = new System.Windows.Forms.TextBox();
            this._btnOK = new System.Windows.Forms.Button();
            this._svcUrl = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this._dgChild = new System.Windows.Forms.DataGridView();
            this.Tbl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cls = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Alias = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.序号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            this._dgParent = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.prefix = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._btnParent = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._btnMid = new System.Windows.Forms.Button();
            this._dgMid = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._cbSvcName = new System.Windows.Forms.ComboBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._dgChild)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgParent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgMid)).BeginInit();
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
            this._ns.Size = new System.Drawing.Size(308, 21);
            this._ns.TabIndex = 10;
            // 
            // _btnOK
            // 
            this._btnOK.Location = new System.Drawing.Point(420, 524);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(75, 23);
            this._btnOK.TabIndex = 100;
            this._btnOK.Text = "确认";
            this._btnOK.UseVisualStyleBackColor = true;
            this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
            // 
            // _svcUrl
            // 
            this._svcUrl.Location = new System.Drawing.Point(187, 27);
            this._svcUrl.Name = "_svcUrl";
            this._svcUrl.Size = new System.Drawing.Size(308, 21);
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
            this.label4.Location = new System.Drawing.Point(10, 487);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(197, 60);
            this.label4.TabIndex = 143;
            this.label4.Text = "* 生成框架前确保实体类已存在\r\n\r\n* 单元格按F2或双击可编辑\r\n\r\n* 可通过修改[序号]控制子实体顺序";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(420, 183);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 141;
            this.btnAdd.Text = "选择";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnChild_Click);
            // 
            // _dgChild
            // 
            this._dgChild.AllowUserToAddRows = false;
            this._dgChild.AllowUserToDeleteRows = false;
            this._dgChild.AllowUserToResizeRows = false;
            this._dgChild.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgChild.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Tbl,
            this.Cls,
            this.Alias,
            this.序号});
            this._dgChild.Location = new System.Drawing.Point(12, 209);
            this._dgChild.Name = "_dgChild";
            this._dgChild.RowHeadersVisible = false;
            this._dgChild.RowTemplate.Height = 23;
            this._dgChild.Size = new System.Drawing.Size(483, 112);
            this._dgChild.TabIndex = 140;
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
            this.Cls.Width = 130;
            // 
            // Alias
            // 
            this.Alias.HeaderText = "类的词根";
            this.Alias.Name = "Alias";
            this.Alias.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Alias.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Alias.Width = 130;
            // 
            // 序号
            // 
            this.序号.HeaderText = "序号";
            this.序号.Name = "序号";
            this.序号.Width = 60;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 144;
            this.label3.Text = "主实体";
            // 
            // _dgParent
            // 
            this._dgParent.AllowUserToAddRows = false;
            this._dgParent.AllowUserToDeleteRows = false;
            this._dgParent.AllowUserToResizeRows = false;
            this._dgParent.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgParent.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.prefix});
            this._dgParent.Location = new System.Drawing.Point(12, 117);
            this._dgParent.Name = "_dgParent";
            this._dgParent.RowHeadersVisible = false;
            this._dgParent.RowTemplate.Height = 23;
            this._dgParent.Size = new System.Drawing.Size(483, 50);
            this._dgParent.TabIndex = 145;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "表名";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 160;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "实体类名";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 150;
            // 
            // prefix
            // 
            this.prefix.HeaderText = "类的词根";
            this.prefix.Name = "prefix";
            this.prefix.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.prefix.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.prefix.Width = 150;
            // 
            // _btnParent
            // 
            this._btnParent.Location = new System.Drawing.Point(420, 91);
            this._btnParent.Name = "_btnParent";
            this._btnParent.Size = new System.Drawing.Size(75, 23);
            this._btnParent.TabIndex = 146;
            this._btnParent.Text = "选择";
            this._btnParent.UseVisualStyleBackColor = true;
            this._btnParent.Click += new System.EventHandler(this._btnParent_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 194);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 147;
            this.label5.Text = "关联实体";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 347);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 150;
            this.label1.Text = "中间实体";
            // 
            // _btnMid
            // 
            this._btnMid.Location = new System.Drawing.Point(420, 336);
            this._btnMid.Name = "_btnMid";
            this._btnMid.Size = new System.Drawing.Size(75, 23);
            this._btnMid.TabIndex = 149;
            this._btnMid.Text = "选择";
            this._btnMid.UseVisualStyleBackColor = true;
            this._btnMid.Click += new System.EventHandler(this._btnMid_Click);
            // 
            // _dgMid
            // 
            this._dgMid.AllowUserToAddRows = false;
            this._dgMid.AllowUserToDeleteRows = false;
            this._dgMid.AllowUserToResizeRows = false;
            this._dgMid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgMid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn8,
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn9});
            this._dgMid.Location = new System.Drawing.Point(12, 362);
            this._dgMid.Name = "_dgMid";
            this._dgMid.RowHeadersVisible = false;
            this._dgMid.RowTemplate.Height = 23;
            this._dgMid.Size = new System.Drawing.Size(483, 112);
            this._dgMid.TabIndex = 148;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "表名";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "实体类名";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.HeaderText = "主实体外键";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.HeaderText = "关联实体外键";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.HeaderText = "序号";
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.Width = 60;
            // 
            // _cbSvcName
            // 
            this._cbSvcName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbSvcName.FormattingEnabled = true;
            this._cbSvcName.Location = new System.Drawing.Point(187, 47);
            this._cbSvcName.Name = "_cbSvcName";
            this._cbSvcName.Size = new System.Drawing.Size(308, 20);
            this._cbSvcName.TabIndex = 153;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(14, 51);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(53, 12);
            this.linkLabel1.TabIndex = 152;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "服务名称";
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point(12, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(176, 20);
            this.label6.TabIndex = 151;
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ManyToManyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 559);
            this.Controls.Add(this._cbSvcName);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._btnMid);
            this.Controls.Add(this._dgMid);
            this.Controls.Add(this.label5);
            this.Controls.Add(this._btnParent);
            this.Controls.Add(this._dgParent);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this._dgChild);
            this.Controls.Add(this.linkLabel3);
            this.Controls.Add(this._svcUrl);
            this.Controls.Add(this.label7);
            this.Controls.Add(this._btnOK);
            this.Controls.Add(this._ns);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManyToManyForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加多对多框架文件";
            ((System.ComponentModel.ISupportInitialize)(this._dgChild)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgParent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgMid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _ns;
        private System.Windows.Forms.Button _btnOK;
        private System.Windows.Forms.TextBox _svcUrl;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.DataGridView _dgChild;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView _dgParent;
        private System.Windows.Forms.Button _btnParent;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _btnMid;
        private System.Windows.Forms.DataGridView _dgMid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn prefix;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tbl;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cls;
        private System.Windows.Forms.DataGridViewTextBoxColumn Alias;
        private System.Windows.Forms.DataGridViewTextBoxColumn 序号;
        private System.Windows.Forms.ComboBox _cbSvcName;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label6;
    }
}