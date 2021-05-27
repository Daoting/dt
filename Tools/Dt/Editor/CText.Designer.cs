
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
            this._fv = new Dt.Editor.FvCell();
            this._acceptsReturn = new System.Windows.Forms.CheckBox();
            this._updateTimely = new System.Windows.Forms.CheckBox();
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
            // _acceptsReturn
            // 
            this._acceptsReturn.AutoSize = true;
            this._acceptsReturn.Location = new System.Drawing.Point(4, 274);
            this._acceptsReturn.Name = "_acceptsReturn";
            this._acceptsReturn.Size = new System.Drawing.Size(162, 16);
            this._acceptsReturn.TabIndex = 1;
            this._acceptsReturn.Text = "允许多行(AcceptsReturn)";
            this._acceptsReturn.UseVisualStyleBackColor = true;
            // 
            // _updateTimely
            // 
            this._updateTimely.AutoSize = true;
            this._updateTimely.Location = new System.Drawing.Point(200, 274);
            this._updateTimely.Name = "_updateTimely";
            this._updateTimely.Size = new System.Drawing.Size(168, 16);
            this._updateTimely.TabIndex = 2;
            this._updateTimely.Text = "实时更新值(UpdateTimely)";
            this._updateTimely.UseVisualStyleBackColor = true;
            // 
            // CText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._updateTimely);
            this.Controls.Add(this._acceptsReturn);
            this.Controls.Add(this._fv);
            this.Name = "CText";
            this.Size = new System.Drawing.Size(380, 340);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FvCell _fv;
        private System.Windows.Forms.CheckBox _acceptsReturn;
        private System.Windows.Forms.CheckBox _updateTimely;
    }
}
