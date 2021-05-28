
namespace Dt.Editor
{
    partial class CellFooter
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
            this._titleWidth = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._placeholder = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this._rowSpan = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this._autoCookie = new System.Windows.Forms.CheckBox();
            this._isReadOnly = new System.Windows.Forms.CheckBox();
            this._isHorStretch = new System.Windows.Forms.CheckBox();
            this._isVerticalTitle = new System.Windows.Forms.CheckBox();
            this._showStar = new System.Windows.Forms.CheckBox();
            this._showTitle = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _titleWidth
            // 
            this._titleWidth.Location = new System.Drawing.Point(186, 0);
            this._titleWidth.Name = "_titleWidth";
            this._titleWidth.Size = new System.Drawing.Size(194, 21);
            this._titleWidth.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(187, 21);
            this.label3.TabIndex = 11;
            this.label3.Text = "列名宽度(TitleWidth)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _placeholder
            // 
            this._placeholder.Location = new System.Drawing.Point(186, 40);
            this._placeholder.Name = "_placeholder";
            this._placeholder.Size = new System.Drawing.Size(194, 21);
            this._placeholder.TabIndex = 24;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(0, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(187, 21);
            this.label5.TabIndex = 23;
            this.label5.Text = "占位符文本(Placeholder)";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _rowSpan
            // 
            this._rowSpan.Location = new System.Drawing.Point(186, 20);
            this._rowSpan.Name = "_rowSpan";
            this._rowSpan.Size = new System.Drawing.Size(194, 21);
            this._rowSpan.TabIndex = 22;
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Location = new System.Drawing.Point(0, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(187, 21);
            this.label4.TabIndex = 21;
            this.label4.Text = "占用行数(RowSpan)";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _autoCookie
            // 
            this._autoCookie.AutoSize = true;
            this._autoCookie.Location = new System.Drawing.Point(3, 128);
            this._autoCookie.Name = "_autoCookie";
            this._autoCookie.Size = new System.Drawing.Size(180, 16);
            this._autoCookie.TabIndex = 30;
            this._autoCookie.Text = "自动加载历史值(AutoCookie)";
            this._autoCookie.UseVisualStyleBackColor = true;
            // 
            // _isReadOnly
            // 
            this._isReadOnly.AutoSize = true;
            this._isReadOnly.Location = new System.Drawing.Point(201, 128);
            this._isReadOnly.Name = "_isReadOnly";
            this._isReadOnly.Size = new System.Drawing.Size(120, 16);
            this._isReadOnly.TabIndex = 29;
            this._isReadOnly.Text = "只读(IsReadOnly)";
            this._isReadOnly.UseVisualStyleBackColor = true;
            // 
            // _isHorStretch
            // 
            this._isHorStretch.AutoSize = true;
            this._isHorStretch.Location = new System.Drawing.Point(201, 74);
            this._isHorStretch.Name = "_isHorStretch";
            this._isHorStretch.Size = new System.Drawing.Size(180, 16);
            this._isHorStretch.TabIndex = 28;
            this._isHorStretch.Text = "是否水平填充(IsHorStretch)";
            this._isHorStretch.UseVisualStyleBackColor = true;
            // 
            // _isVerticalTitle
            // 
            this._isVerticalTitle.AutoSize = true;
            this._isVerticalTitle.Location = new System.Drawing.Point(3, 101);
            this._isVerticalTitle.Name = "_isVerticalTitle";
            this._isVerticalTitle.Size = new System.Drawing.Size(198, 16);
            this._isVerticalTitle.TabIndex = 27;
            this._isVerticalTitle.Text = "垂直显示标题(IsVerticalTitle)";
            this._isVerticalTitle.UseVisualStyleBackColor = true;
            // 
            // _showStar
            // 
            this._showStar.AutoSize = true;
            this._showStar.Location = new System.Drawing.Point(201, 101);
            this._showStar.Name = "_showStar";
            this._showStar.Size = new System.Drawing.Size(132, 16);
            this._showStar.TabIndex = 26;
            this._showStar.Text = "显示红星(ShowStar)";
            this._showStar.UseVisualStyleBackColor = true;
            // 
            // _showTitle
            // 
            this._showTitle.AutoSize = true;
            this._showTitle.Location = new System.Drawing.Point(3, 74);
            this._showTitle.Name = "_showTitle";
            this._showTitle.Size = new System.Drawing.Size(150, 16);
            this._showTitle.TabIndex = 25;
            this._showTitle.Text = "显示标题列(ShowTitle)";
            this._showTitle.UseVisualStyleBackColor = true;
            // 
            // CellFooter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._autoCookie);
            this.Controls.Add(this._isReadOnly);
            this.Controls.Add(this._isHorStretch);
            this.Controls.Add(this._isVerticalTitle);
            this.Controls.Add(this._showStar);
            this.Controls.Add(this._showTitle);
            this.Controls.Add(this._placeholder);
            this.Controls.Add(this.label5);
            this.Controls.Add(this._rowSpan);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._titleWidth);
            this.Controls.Add(this.label3);
            this.Name = "CellFooter";
            this.Size = new System.Drawing.Size(380, 151);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _titleWidth;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _placeholder;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox _rowSpan;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox _autoCookie;
        private System.Windows.Forms.CheckBox _isReadOnly;
        private System.Windows.Forms.CheckBox _isHorStretch;
        private System.Windows.Forms.CheckBox _isVerticalTitle;
        private System.Windows.Forms.CheckBox _showStar;
        private System.Windows.Forms.CheckBox _showTitle;
    }
}
