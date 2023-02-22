
namespace Dt
{
    partial class OnToManyForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OnToManyForm));
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
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.序号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            this._dgParent = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.prefix = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._btnParent = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._dgChild)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgParent)).BeginInit();
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
            // _btnOK
            // 
            this._btnOK.Location = new System.Drawing.Point(381, 427);
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
            this.label4.Location = new System.Drawing.Point(12, 306);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(221, 144);
            this.label4.TabIndex = 143;
            this.label4.Text = "* 生成框架前确保实体类已存在\r\n\r\n* 单元格按F2或双击可编辑\r\n\r\n* 类的词根指生成的窗口、列表、表单、\r\n  查询等类的词根，命名规范如：\r\n  查询：" +
    "词根 + Query\r\n  窗口：词根 + Win\r\n  列表：词根 + List\r\n  表单：词根 + Form\r\n\r\n* 可通过修改[序号]控制子实体顺序";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(381, 153);
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
            this.Column1,
            this.序号});
            this._dgChild.Location = new System.Drawing.Point(12, 179);
            this._dgChild.Name = "_dgChild";
            this._dgChild.RowHeadersVisible = false;
            this._dgChild.RowTemplate.Height = 23;
            this._dgChild.Size = new System.Drawing.Size(444, 112);
            this._dgChild.TabIndex = 140;
            // 
            // Tbl
            // 
            this.Tbl.HeaderText = "表名";
            this.Tbl.Name = "Tbl";
            this.Tbl.ReadOnly = true;
            this.Tbl.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Tbl.Width = 90;
            // 
            // Cls
            // 
            this.Cls.HeaderText = "实体类名";
            this.Cls.Name = "Cls";
            this.Cls.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Cls.Width = 90;
            // 
            // Alias
            // 
            this.Alias.HeaderText = "类的词根";
            this.Alias.Name = "Alias";
            this.Alias.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Alias.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Alias.Width = 90;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "父主键名称";
            this.Column1.Name = "Column1";
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column1.Width = 90;
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
            this.label3.Location = new System.Drawing.Point(12, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 144;
            this.label3.Text = "父实体";
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
            this._dgParent.Location = new System.Drawing.Point(12, 87);
            this._dgParent.Name = "_dgParent";
            this._dgParent.RowHeadersVisible = false;
            this._dgParent.RowTemplate.Height = 23;
            this._dgParent.Size = new System.Drawing.Size(444, 50);
            this._dgParent.TabIndex = 145;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "表名";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 150;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "实体类名";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 140;
            // 
            // prefix
            // 
            this.prefix.HeaderText = "类的词根";
            this.prefix.Name = "prefix";
            this.prefix.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.prefix.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.prefix.Width = 140;
            // 
            // _btnParent
            // 
            this._btnParent.Location = new System.Drawing.Point(381, 61);
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
            this.label5.Location = new System.Drawing.Point(10, 164);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 147;
            this.label5.Text = "子实体";
            // 
            // OnToManyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 467);
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
            this.Name = "OnToManyForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加一对多框架文件";
            ((System.ComponentModel.ISupportInitialize)(this._dgChild)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgParent)).EndInit();
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
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn prefix;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tbl;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cls;
        private System.Windows.Forms.DataGridViewTextBoxColumn Alias;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn 序号;
    }
}