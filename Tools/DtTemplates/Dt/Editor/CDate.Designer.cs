
namespace Dt.Editor
{
    partial class CDate
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
            this._header = new Dt.Editor.CellHeader();
            this._format = new System.Windows.Forms.ComboBox();
            this.lable1 = new System.Windows.Forms.Label();
            this._alwaysTouchPicker = new System.Windows.Forms.CheckBox();
            this._footer = new Dt.Editor.CellFooter();
            this.SuspendLayout();
            // 
            // _header
            // 
            this._header.Dock = System.Windows.Forms.DockStyle.Top;
            this._header.Location = new System.Drawing.Point(0, 0);
            this._header.Name = "_header";
            this._header.Size = new System.Drawing.Size(380, 42);
            this._header.TabIndex = 0;
            // 
            // _format
            // 
            this._format.FormattingEnabled = true;
            this._format.Items.AddRange(new object[] {
            "默认(yyyy-MM-dd)",
            "yyyy-MM-dd HH:mm:ss"});
            this._format.Location = new System.Drawing.Point(186, 40);
            this._format.Name = "_format";
            this._format.Size = new System.Drawing.Size(194, 20);
            this._format.TabIndex = 41;
            // 
            // lable1
            // 
            this.lable1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lable1.Location = new System.Drawing.Point(0, 40);
            this.lable1.Name = "lable1";
            this.lable1.Size = new System.Drawing.Size(187, 20);
            this.lable1.TabIndex = 40;
            this.lable1.Text = "格式化串(Format)";
            this.lable1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _alwaysTouchPicker
            // 
            this._alwaysTouchPicker.AutoSize = true;
            this._alwaysTouchPicker.Location = new System.Drawing.Point(4, 73);
            this._alwaysTouchPicker.Name = "_alwaysTouchPicker";
            this._alwaysTouchPicker.Size = new System.Drawing.Size(258, 16);
            this._alwaysTouchPicker.TabIndex = 42;
            this._alwaysTouchPicker.Text = "始终为触摸模式选择器(AlwaysTouchPicker)";
            this._alwaysTouchPicker.UseVisualStyleBackColor = true;
            // 
            // _footer
            // 
            this._footer.Location = new System.Drawing.Point(0, 107);
            this._footer.Name = "_footer";
            this._footer.Size = new System.Drawing.Size(380, 170);
            this._footer.TabIndex = 43;
            // 
            // CDate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._footer);
            this.Controls.Add(this._alwaysTouchPicker);
            this.Controls.Add(this._format);
            this.Controls.Add(this.lable1);
            this.Controls.Add(this._header);
            this.Name = "CDate";
            this.Size = new System.Drawing.Size(380, 287);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CellHeader _header;
        private System.Windows.Forms.ComboBox _format;
        private System.Windows.Forms.Label lable1;
        private System.Windows.Forms.CheckBox _alwaysTouchPicker;
        private CellFooter _footer;
    }
}
