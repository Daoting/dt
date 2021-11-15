
namespace Dt.Editor
{
    partial class CText
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
            this._acceptsReturn = new System.Windows.Forms.CheckBox();
            this._updateTimely = new System.Windows.Forms.CheckBox();
            this._header = new Dt.Editor.CellHeader();
            this._footer = new Dt.Editor.CellFooter();
            this.SuspendLayout();
            // 
            // _acceptsReturn
            // 
            this._acceptsReturn.AutoSize = true;
            this._acceptsReturn.Location = new System.Drawing.Point(4, 52);
            this._acceptsReturn.Name = "_acceptsReturn";
            this._acceptsReturn.Size = new System.Drawing.Size(162, 16);
            this._acceptsReturn.TabIndex = 1;
            this._acceptsReturn.Text = "允许多行(AcceptsReturn)";
            this._acceptsReturn.UseVisualStyleBackColor = true;
            // 
            // _updateTimely
            // 
            this._updateTimely.AutoSize = true;
            this._updateTimely.Location = new System.Drawing.Point(200, 52);
            this._updateTimely.Name = "_updateTimely";
            this._updateTimely.Size = new System.Drawing.Size(168, 16);
            this._updateTimely.TabIndex = 2;
            this._updateTimely.Text = "实时更新值(UpdateTimely)";
            this._updateTimely.UseVisualStyleBackColor = true;
            // 
            // _header
            // 
            this._header.Dock = System.Windows.Forms.DockStyle.Top;
            this._header.Location = new System.Drawing.Point(0, 0);
            this._header.Name = "_header";
            this._header.Size = new System.Drawing.Size(380, 42);
            this._header.TabIndex = 3;
            // 
            // _footer
            // 
            this._footer.Location = new System.Drawing.Point(0, 81);
            this._footer.Name = "_footer";
            this._footer.Size = new System.Drawing.Size(380, 170);
            this._footer.TabIndex = 4;
            // 
            // CText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._footer);
            this.Controls.Add(this._header);
            this.Controls.Add(this._updateTimely);
            this.Controls.Add(this._acceptsReturn);
            this.Name = "CText";
            this.Size = new System.Drawing.Size(380, 258);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox _acceptsReturn;
        private System.Windows.Forms.CheckBox _updateTimely;
        private CellHeader _header;
        private CellFooter _footer;
    }
}
