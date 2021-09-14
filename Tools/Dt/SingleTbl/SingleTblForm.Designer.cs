
namespace Dt.SingleTbl
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
            this._nameSpace.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(12, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 21);
            this.label1.TabIndex = 18;
            this.label1.Text = "实体类型名称";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _entityName
            // 
            this._entityName.Location = new System.Drawing.Point(187, 27);
            this._entityName.Name = "_entityName";
            this._entityName.Size = new System.Drawing.Size(269, 21);
            this._entityName.TabIndex = 19;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(12, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(176, 21);
            this.label3.TabIndex = 20;
            this.label3.Text = "实体中文标题";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _entityTitle
            // 
            this._entityTitle.Location = new System.Drawing.Point(187, 47);
            this._entityTitle.Name = "_entityTitle";
            this._entityTitle.Size = new System.Drawing.Size(269, 21);
            this._entityTitle.TabIndex = 21;
            // 
            // _btnOK
            // 
            this._btnOK.Location = new System.Drawing.Point(381, 201);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(75, 23);
            this._btnOK.TabIndex = 22;
            this._btnOK.Text = "确认";
            this._btnOK.UseVisualStyleBackColor = true;
            this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
            // 
            // _info
            // 
            this._info.AutoSize = true;
            this._info.ForeColor = System.Drawing.Color.Red;
            this._info.Location = new System.Drawing.Point(12, 201);
            this._info.Name = "_info";
            this._info.Size = new System.Drawing.Size(11, 12);
            this._info.TabIndex = 23;
            this._info.Text = " ";
            // 
            // SingleTblForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 240);
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
            this.Text = "插入单表框架文件";
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
    }
}