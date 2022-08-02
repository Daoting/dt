
namespace Dt
{
    partial class SingleTblForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SingleTblForm));
            this.label2 = new System.Windows.Forms.Label();
            this._nameSpace = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._entityName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._entityTitle = new System.Windows.Forms.TextBox();
            this._btnOK = new System.Windows.Forms.Button();
            this._info = new System.Windows.Forms.Label();
            this._cbSearch = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this._cbWin = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this._agentName = new System.Windows.Forms.TextBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
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
            // _nameSpace
            // 
            this._nameSpace.Location = new System.Drawing.Point(187, 7);
            this._nameSpace.Name = "_nameSpace";
            this._nameSpace.Size = new System.Drawing.Size(269, 21);
            this._nameSpace.TabIndex = 10;
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
            // _entityName
            // 
            this._entityName.Location = new System.Drawing.Point(187, 47);
            this._entityName.Name = "_entityName";
            this._entityName.Size = new System.Drawing.Size(269, 21);
            this._entityName.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(176, 21);
            this.label3.TabIndex = 20;
            this.label3.Text = "实体中文标题";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _entityTitle
            // 
            this._entityTitle.Location = new System.Drawing.Point(187, 67);
            this._entityTitle.Name = "_entityTitle";
            this._entityTitle.Size = new System.Drawing.Size(269, 21);
            this._entityTitle.TabIndex = 13;
            // 
            // _btnOK
            // 
            this._btnOK.Location = new System.Drawing.Point(381, 190);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(75, 23);
            this._btnOK.TabIndex = 100;
            this._btnOK.Text = "确认";
            this._btnOK.UseVisualStyleBackColor = true;
            this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
            // 
            // _info
            // 
            this._info.AutoSize = true;
            this._info.ForeColor = System.Drawing.Color.Black;
            this._info.Location = new System.Drawing.Point(10, 146);
            this._info.Name = "_info";
            this._info.Size = new System.Drawing.Size(161, 12);
            this._info.TabIndex = 23;
            this._info.Text = "实现单表增删改查(CRUD)功能";
            // 
            // _cbSearch
            // 
            this._cbSearch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbSearch.FormattingEnabled = true;
            this._cbSearch.Items.AddRange(new object[] {
            "通用搜索面板",
            "自定义搜索面板"});
            this._cbSearch.Location = new System.Drawing.Point(187, 87);
            this._cbSearch.Name = "_cbSearch";
            this._cbSearch.Size = new System.Drawing.Size(269, 20);
            this._cbSearch.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Location = new System.Drawing.Point(12, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(176, 21);
            this.label4.TabIndex = 26;
            this.label4.Text = "搜索面板";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _cbWin
            // 
            this._cbWin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbWin.FormattingEnabled = true;
            this._cbWin.Items.AddRange(new object[] {
            "窗口三栏，左侧搜索栏",
            "窗口两栏，左侧列表栏，搜索合并到列表栏"});
            this._cbWin.Location = new System.Drawing.Point(187, 107);
            this._cbWin.Name = "_cbWin";
            this._cbWin.Size = new System.Drawing.Size(269, 20);
            this._cbWin.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(12, 107);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(176, 21);
            this.label5.TabIndex = 28;
            this.label5.Text = "窗口布局";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _agentName
            // 
            this._agentName.Location = new System.Drawing.Point(187, 27);
            this._agentName.Name = "_agentName";
            this._agentName.Size = new System.Drawing.Size(269, 21);
            this._agentName.TabIndex = 11;
            this._agentName.Text = "AtLob";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(14, 31);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(65, 12);
            this.linkLabel1.TabIndex = 102;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "数据提供类";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point(12, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(176, 21);
            this.label6.TabIndex = 30;
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(14, 51);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(53, 12);
            this.linkLabel2.TabIndex = 103;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "实体名称";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // SingleTblForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 229);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this._agentName);
            this.Controls.Add(this.label6);
            this.Controls.Add(this._cbWin);
            this.Controls.Add(this.label5);
            this.Controls.Add(this._cbSearch);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._info);
            this.Controls.Add(this._btnOK);
            this.Controls.Add(this._entityTitle);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._entityName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._nameSpace);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SingleTblForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加单表框架文件";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _nameSpace;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _entityName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _entityTitle;
        private System.Windows.Forms.Button _btnOK;
        private System.Windows.Forms.Label _info;
        private System.Windows.Forms.ComboBox _cbSearch;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox _cbWin;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox _agentName;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.LinkLabel linkLabel2;
    }
}