
namespace Dt.Editor
{
    partial class CellHeader
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
            this._id = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._title = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _id
            // 
            this._id.Location = new System.Drawing.Point(186, 0);
            this._id.Name = "_id";
            this._id.Size = new System.Drawing.Size(194, 21);
            this._id.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(187, 21);
            this.label2.TabIndex = 11;
            this.label2.Text = "列名(ID)";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _title
            // 
            this._title.Location = new System.Drawing.Point(186, 20);
            this._title.Name = "_title";
            this._title.Size = new System.Drawing.Size(194, 21);
            this._title.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(0, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 21);
            this.label1.TabIndex = 9;
            this.label1.Text = "标题(Title)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CellHeader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._id);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._title);
            this.Controls.Add(this.label1);
            this.Name = "CellHeader";
            this.Size = new System.Drawing.Size(380, 42);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _id;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _title;
        private System.Windows.Forms.Label label1;
    }
}
