
namespace Dt.Editor
{
    partial class CBool
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
            this._fv = new Dt.Editor.FvCell();
            this._trueVal = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this._falseVal = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._isSwitch = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _fv
            // 
            this._fv.Dock = System.Windows.Forms.DockStyle.Top;
            this._fv.Location = new System.Drawing.Point(0, 0);
            this._fv.Name = "_fv";
            this._fv.Size = new System.Drawing.Size(380, 267);
            this._fv.TabIndex = 0;
            // 
            // _trueVal
            // 
            this._trueVal.Location = new System.Drawing.Point(186, 264);
            this._trueVal.Name = "_trueVal";
            this._trueVal.Size = new System.Drawing.Size(194, 21);
            this._trueVal.TabIndex = 24;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(0, 264);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(187, 21);
            this.label5.TabIndex = 23;
            this.label5.Text = "True时的值(TrueVal)";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _falseVal
            // 
            this._falseVal.Location = new System.Drawing.Point(186, 284);
            this._falseVal.Name = "_falseVal";
            this._falseVal.Size = new System.Drawing.Size(194, 21);
            this._falseVal.TabIndex = 26;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(0, 284);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 21);
            this.label1.TabIndex = 25;
            this.label1.Text = "False时的值(FalseVal)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _isSwitch
            // 
            this._isSwitch.AutoSize = true;
            this._isSwitch.Location = new System.Drawing.Point(4, 321);
            this._isSwitch.Name = "_isSwitch";
            this._isSwitch.Size = new System.Drawing.Size(144, 16);
            this._isSwitch.TabIndex = 27;
            this._isSwitch.Text = "显示为开关(IsSwitch)";
            this._isSwitch.UseVisualStyleBackColor = true;
            // 
            // CBool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._isSwitch);
            this.Controls.Add(this._falseVal);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._trueVal);
            this.Controls.Add(this.label5);
            this.Controls.Add(this._fv);
            this.Name = "CBool";
            this.Size = new System.Drawing.Size(380, 340);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FvCell _fv;
        private System.Windows.Forms.TextBox _trueVal;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox _falseVal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox _isSwitch;
    }
}
