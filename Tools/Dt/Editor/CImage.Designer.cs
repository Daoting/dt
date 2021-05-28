
namespace Dt.Editor
{
    partial class CImage
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
            this._footer = new Dt.Editor.CellFooter();
            this._header = new Dt.Editor.CellHeader();
            this._imageHeight = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this._imageStretch = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this._fixedVolume = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._showDefaultMenu = new System.Windows.Forms.CheckBox();
            this._enableClick = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _footer
            // 
            this._footer.Location = new System.Drawing.Point(0, 170);
            this._footer.Name = "_footer";
            this._footer.Size = new System.Drawing.Size(380, 170);
            this._footer.TabIndex = 43;
            // 
            // _header
            // 
            this._header.Dock = System.Windows.Forms.DockStyle.Top;
            this._header.Location = new System.Drawing.Point(0, 0);
            this._header.Name = "_header";
            this._header.Size = new System.Drawing.Size(380, 42);
            this._header.TabIndex = 42;
            // 
            // _imageHeight
            // 
            this._imageHeight.Location = new System.Drawing.Point(186, 40);
            this._imageHeight.Name = "_imageHeight";
            this._imageHeight.Size = new System.Drawing.Size(194, 21);
            this._imageHeight.TabIndex = 45;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(0, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(187, 21);
            this.label5.TabIndex = 44;
            this.label5.Text = "图像高度(ImageHeight)";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _imageStretch
            // 
            this._imageStretch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._imageStretch.FormattingEnabled = true;
            this._imageStretch.Items.AddRange(new object[] {
            "None",
            "Fill",
            "Uniform",
            "UniformToFill"});
            this._imageStretch.Location = new System.Drawing.Point(186, 60);
            this._imageStretch.Name = "_imageStretch";
            this._imageStretch.Size = new System.Drawing.Size(194, 20);
            this._imageStretch.TabIndex = 47;
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Location = new System.Drawing.Point(0, 60);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(187, 20);
            this.label8.TabIndex = 46;
            this.label8.Text = "图像填充模式(ImageStretch)";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _fixedVolume
            // 
            this._fixedVolume.Location = new System.Drawing.Point(186, 79);
            this._fixedVolume.Name = "_fixedVolume";
            this._fixedVolume.Size = new System.Drawing.Size(194, 21);
            this._fixedVolume.TabIndex = 49;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(0, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 21);
            this.label1.TabIndex = 48;
            this.label1.Text = "固定卷名(FixedVolume)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _showDefaultMenu
            // 
            this._showDefaultMenu.AutoSize = true;
            this._showDefaultMenu.Location = new System.Drawing.Point(3, 113);
            this._showDefaultMenu.Name = "_showDefaultMenu";
            this._showDefaultMenu.Size = new System.Drawing.Size(234, 16);
            this._showDefaultMenu.TabIndex = 50;
            this._showDefaultMenu.Text = "显示默认上下文菜单(ShowDefaultMenu)";
            this._showDefaultMenu.UseVisualStyleBackColor = true;
            // 
            // _enableClick
            // 
            this._enableClick.AutoSize = true;
            this._enableClick.Location = new System.Drawing.Point(3, 139);
            this._enableClick.Name = "_enableClick";
            this._enableClick.Size = new System.Drawing.Size(174, 16);
            this._enableClick.TabIndex = 51;
            this._enableClick.Text = "文件项可点击(EnableClick)";
            this._enableClick.UseVisualStyleBackColor = true;
            // 
            // CImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._enableClick);
            this.Controls.Add(this._showDefaultMenu);
            this.Controls.Add(this._fixedVolume);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._imageStretch);
            this.Controls.Add(this.label8);
            this.Controls.Add(this._imageHeight);
            this.Controls.Add(this.label5);
            this.Controls.Add(this._footer);
            this.Controls.Add(this._header);
            this.Name = "CImage";
            this.Size = new System.Drawing.Size(380, 342);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CellFooter _footer;
        private CellHeader _header;
        private System.Windows.Forms.TextBox _imageHeight;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox _imageStretch;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox _fixedVolume;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox _showDefaultMenu;
        private System.Windows.Forms.CheckBox _enableClick;
    }
}
