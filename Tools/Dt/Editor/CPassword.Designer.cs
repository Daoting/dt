
namespace Dt.Editor
{
    partial class CPassword
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
            this._holder = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this._maxLength = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._header = new Dt.Editor.CellHeader();
            this._footer = new Dt.Editor.CellFooter();
            this.SuspendLayout();
            // 
            // _holder
            // 
            this._holder.Location = new System.Drawing.Point(186, 41);
            this._holder.Name = "_holder";
            this._holder.Size = new System.Drawing.Size(194, 21);
            this._holder.TabIndex = 24;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(0, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(187, 21);
            this.label5.TabIndex = 23;
            this.label5.Text = "占位符(Holder)";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _maxLength
            // 
            this._maxLength.Location = new System.Drawing.Point(186, 61);
            this._maxLength.Name = "_maxLength";
            this._maxLength.Size = new System.Drawing.Size(194, 21);
            this._maxLength.TabIndex = 26;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(0, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 21);
            this.label1.TabIndex = 25;
            this.label1.Text = "最大字符数(MaxLength)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _header
            // 
            this._header.Dock = System.Windows.Forms.DockStyle.Top;
            this._header.Location = new System.Drawing.Point(0, 0);
            this._header.Name = "_header";
            this._header.Size = new System.Drawing.Size(380, 42);
            this._header.TabIndex = 28;
            // 
            // _footer
            // 
            this._footer.Location = new System.Drawing.Point(0, 81);
            this._footer.Name = "_footer";
            this._footer.Size = new System.Drawing.Size(380, 170);
            this._footer.TabIndex = 29;
            // 
            // CPassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._footer);
            this.Controls.Add(this._header);
            this.Controls.Add(this._maxLength);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._holder);
            this.Controls.Add(this.label5);
            this.Name = "CPassword";
            this.Size = new System.Drawing.Size(380, 262);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox _holder;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox _maxLength;
        private System.Windows.Forms.Label label1;
        private CellHeader _header;
        private CellFooter _footer;
    }
}
