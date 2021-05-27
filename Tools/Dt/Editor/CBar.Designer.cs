
namespace Dt.Editor
{
    partial class CBar
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
            this.label1 = new System.Windows.Forms.Label();
            this._title = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._rowSpan = new System.Windows.Forms.TextBox();
            this._isHorStretch = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 21);
            this.label1.TabIndex = 3;
            this.label1.Text = "Title";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _title
            // 
            this._title.Location = new System.Drawing.Point(186, 0);
            this._title.Name = "_title";
            this._title.Size = new System.Drawing.Size(194, 21);
            this._title.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(0, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(187, 21);
            this.label2.TabIndex = 5;
            this.label2.Text = "占用行数(RowSpan)";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _rowSpan
            // 
            this._rowSpan.Location = new System.Drawing.Point(186, 20);
            this._rowSpan.Name = "_rowSpan";
            this._rowSpan.Size = new System.Drawing.Size(194, 21);
            this._rowSpan.TabIndex = 6;
            // 
            // _isHorStretch
            // 
            this._isHorStretch.AutoSize = true;
            this._isHorStretch.Location = new System.Drawing.Point(6, 57);
            this._isHorStretch.Name = "_isHorStretch";
            this._isHorStretch.Size = new System.Drawing.Size(156, 16);
            this._isHorStretch.TabIndex = 7;
            this._isHorStretch.Text = "水平填充(IsHorStretch)";
            this._isHorStretch.UseVisualStyleBackColor = true;
            // 
            // CBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._isHorStretch);
            this.Controls.Add(this._rowSpan);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._title);
            this.Controls.Add(this.label1);
            this.Name = "CBar";
            this.Size = new System.Drawing.Size(380, 327);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _title;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _rowSpan;
        private System.Windows.Forms.CheckBox _isHorStretch;
    }
}
