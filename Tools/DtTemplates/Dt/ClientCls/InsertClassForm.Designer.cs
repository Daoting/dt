
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
            this._btnEntity.Location = new System.Drawing.Point(12, 164);
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
            this._btnCList.Location = new System.Drawing.Point(12, 117);
            this._btnCList.Name = "_btnCList";
            this._btnCList.Size = new System.Drawing.Size(190, 23);
            this._btnCList.TabIndex = 30;
            this._btnCList.Text = "CList的Ex类";
            this._btnCList.UseVisualStyleBackColor = true;
            this._btnCList.Click += new System.EventHandler(this._btnCList_Click);
            // 
            // _btnAgent
            // 
            this._btnAgent.Location = new System.Drawing.Point(266, 117);
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
            this._lbl.Location = new System.Drawing.Point(21, 220);
            this._lbl.Name = "_lbl";
            this._lbl.Size = new System.Drawing.Size(29, 12);
            this._lbl.TabIndex = 32;
            this._lbl.Text = "Info";
            // 
            // InsertClassForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 280);
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
    }
}