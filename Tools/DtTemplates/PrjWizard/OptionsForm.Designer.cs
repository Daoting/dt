
namespace Dt.PrjWizard
{
    partial class OptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            this._btnOK = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.cbWebAssembly = new System.Windows.Forms.CheckBox();
            this.cbIOS = new System.Windows.Forms.CheckBox();
            this.cbAndroid = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rbAll = new System.Windows.Forms.RadioButton();
            this.rbCustom = new System.Windows.Forms.RadioButton();
            this.rbNone = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _btnOK
            // 
            this._btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._btnOK.Location = new System.Drawing.Point(236, 313);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(75, 23);
            this._btnOK.TabIndex = 22;
            this._btnOK.Text = "创建";
            this._btnOK.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(338, 313);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 23;
            this.button1.Text = "取消";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // cbWebAssembly
            // 
            this.cbWebAssembly.AutoSize = true;
            this.cbWebAssembly.Checked = true;
            this.cbWebAssembly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbWebAssembly.Location = new System.Drawing.Point(42, 40);
            this.cbWebAssembly.Name = "cbWebAssembly";
            this.cbWebAssembly.Size = new System.Drawing.Size(90, 16);
            this.cbWebAssembly.TabIndex = 24;
            this.cbWebAssembly.Text = "WebAssembly";
            this.cbWebAssembly.UseVisualStyleBackColor = true;
            // 
            // cbIOS
            // 
            this.cbIOS.AutoSize = true;
            this.cbIOS.Checked = true;
            this.cbIOS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIOS.Location = new System.Drawing.Point(42, 72);
            this.cbIOS.Name = "cbIOS";
            this.cbIOS.Size = new System.Drawing.Size(42, 16);
            this.cbIOS.TabIndex = 25;
            this.cbIOS.Text = "iOS";
            this.cbIOS.UseVisualStyleBackColor = true;
            // 
            // cbAndroid
            // 
            this.cbAndroid.AutoSize = true;
            this.cbAndroid.Checked = true;
            this.cbAndroid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAndroid.Location = new System.Drawing.Point(42, 104);
            this.cbAndroid.Name = "cbAndroid";
            this.cbAndroid.Size = new System.Drawing.Size(66, 16);
            this.cbAndroid.TabIndex = 26;
            this.cbAndroid.Text = "Android";
            this.cbAndroid.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 12);
            this.label1.TabIndex = 27;
            this.label1.Text = "选择新项目支持的平台";
            // 
            // rbAll
            // 
            this.rbAll.AutoSize = true;
            this.rbAll.Checked = true;
            this.rbAll.Location = new System.Drawing.Point(42, 173);
            this.rbAll.Name = "rbAll";
            this.rbAll.Size = new System.Drawing.Size(371, 16);
            this.rbAll.TabIndex = 28;
            this.rbAll.TabStop = true;
            this.rbAll.Text = "使用搬运工标准服务，包括通用后台管理、文件服务、消息服务等";
            this.rbAll.UseVisualStyleBackColor = true;
            // 
            // rbCustom
            // 
            this.rbCustom.AutoSize = true;
            this.rbCustom.Location = new System.Drawing.Point(42, 212);
            this.rbCustom.Name = "rbCustom";
            this.rbCustom.Size = new System.Drawing.Size(215, 16);
            this.rbCustom.TabIndex = 29;
            this.rbCustom.Text = "不使用搬运工标准服务，自定义服务";
            this.rbCustom.UseVisualStyleBackColor = true;
            // 
            // rbNone
            // 
            this.rbNone.AutoSize = true;
            this.rbNone.Location = new System.Drawing.Point(42, 251);
            this.rbNone.Name = "rbNone";
            this.rbNone.Size = new System.Drawing.Size(131, 16);
            this.rbNone.TabIndex = 30;
            this.rbNone.Text = "单机版应用，无服务";
            this.rbNone.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 148);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 31;
            this.label2.Text = "选择服务内容";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(42, 318);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(125, 12);
            this.label3.TabIndex = 32;
            this.label3.Text = "创建后也可以手动调整";
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 367);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rbNone);
            this.Controls.Add(this.rbCustom);
            this.Controls.Add(this.rbAll);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbAndroid);
            this.Controls.Add(this.cbIOS);
            this.Controls.Add(this.cbWebAssembly);
            this.Controls.Add(this.button1);
            this.Controls.Add(this._btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "新搬运工应用";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button _btnOK;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox cbWebAssembly;
        private System.Windows.Forms.CheckBox cbIOS;
        private System.Windows.Forms.CheckBox cbAndroid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbAll;
        private System.Windows.Forms.RadioButton rbCustom;
        private System.Windows.Forms.RadioButton rbNone;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}