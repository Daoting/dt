
namespace Dt.Editor
{
    partial class LvXaml
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
            this._name = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._viewMode = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this._selectionMode = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this._itemHeight = new System.Windows.Forms.TextBox();
            this._showGroupHeader = new System.Windows.Forms.CheckBox();
            this._showItemBorder = new System.Windows.Forms.CheckBox();
            this._autoScrollBottom = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this._groupName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this._minItemWidth = new System.Windows.Forms.TextBox();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this._cbTbls = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this._svcUrl = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this._showToolbar = new System.Windows.Forms.CheckBox();
            this._showFilter = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(250, 21);
            this.label1.TabIndex = 2;
            this.label1.Text = "Name";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _name
            // 
            this._name.Location = new System.Drawing.Point(249, 0);
            this._name.Name = "_name";
            this._name.Size = new System.Drawing.Size(211, 21);
            this._name.TabIndex = 3;
            this._name.Text = "_lv";
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(0, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(250, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "视图类型(ViewMode)";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _viewMode
            // 
            this._viewMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._viewMode.FormattingEnabled = true;
            this._viewMode.Items.AddRange(new object[] {
            "List",
            "Table",
            "Tile"});
            this._viewMode.Location = new System.Drawing.Point(249, 20);
            this._viewMode.Name = "_viewMode";
            this._viewMode.Size = new System.Drawing.Size(211, 20);
            this._viewMode.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(0, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(250, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "选择模式(SelectionMode)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _selectionMode
            // 
            this._selectionMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._selectionMode.FormattingEnabled = true;
            this._selectionMode.Items.AddRange(new object[] {
            "None",
            "Single",
            "Multiple"});
            this._selectionMode.Location = new System.Drawing.Point(249, 39);
            this._selectionMode.Name = "_selectionMode";
            this._selectionMode.Size = new System.Drawing.Size(211, 20);
            this._selectionMode.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Location = new System.Drawing.Point(0, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(250, 21);
            this.label4.TabIndex = 8;
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _itemHeight
            // 
            this._itemHeight.Location = new System.Drawing.Point(249, 78);
            this._itemHeight.Name = "_itemHeight";
            this._itemHeight.Size = new System.Drawing.Size(211, 21);
            this._itemHeight.TabIndex = 9;
            this._itemHeight.Text = "0";
            // 
            // _showGroupHeader
            // 
            this._showGroupHeader.AutoSize = true;
            this._showGroupHeader.Checked = true;
            this._showGroupHeader.CheckState = System.Windows.Forms.CheckState.Checked;
            this._showGroupHeader.Location = new System.Drawing.Point(0, 129);
            this._showGroupHeader.Name = "_showGroupHeader";
            this._showGroupHeader.Size = new System.Drawing.Size(198, 16);
            this._showGroupHeader.TabIndex = 10;
            this._showGroupHeader.Text = "显示分组导航(ShowGroupHeader)";
            this._showGroupHeader.UseVisualStyleBackColor = true;
            // 
            // _showItemBorder
            // 
            this._showItemBorder.AutoSize = true;
            this._showItemBorder.Checked = true;
            this._showItemBorder.CheckState = System.Windows.Forms.CheckState.Checked;
            this._showItemBorder.Location = new System.Drawing.Point(249, 129);
            this._showItemBorder.Name = "_showItemBorder";
            this._showItemBorder.Size = new System.Drawing.Size(192, 16);
            this._showItemBorder.TabIndex = 11;
            this._showItemBorder.Text = "显示行分割线(ShowItemBorder)";
            this._showItemBorder.UseVisualStyleBackColor = true;
            // 
            // _autoScrollBottom
            // 
            this._autoScrollBottom.AutoSize = true;
            this._autoScrollBottom.Location = new System.Drawing.Point(0, 179);
            this._autoScrollBottom.Name = "_autoScrollBottom";
            this._autoScrollBottom.Size = new System.Drawing.Size(264, 16);
            this._autoScrollBottom.TabIndex = 12;
            this._autoScrollBottom.Text = "切换数据自动滚动到底端(AutoScrollBottom)";
            this._autoScrollBottom.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Location = new System.Drawing.Point(0, 58);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(250, 21);
            this.label7.TabIndex = 15;
            this.label7.Text = "分组列名(GroupName)";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _groupName
            // 
            this._groupName.Location = new System.Drawing.Point(249, 58);
            this._groupName.Name = "_groupName";
            this._groupName.Size = new System.Drawing.Size(211, 21);
            this._groupName.TabIndex = 16;
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Location = new System.Drawing.Point(0, 98);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(250, 21);
            this.label8.TabIndex = 19;
            this.label8.Text = "磁贴项最小宽度(MinItemWidth)";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _minItemWidth
            // 
            this._minItemWidth.Location = new System.Drawing.Point(249, 98);
            this._minItemWidth.Name = "_minItemWidth";
            this._minItemWidth.Size = new System.Drawing.Size(211, 21);
            this._minItemWidth.TabIndex = 20;
            this._minItemWidth.Text = "160";
            // 
            // linkLabel4
            // 
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.Location = new System.Drawing.Point(4, 271);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(41, 12);
            this.linkLabel4.TabIndex = 118;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = "选择表";
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Location = new System.Drawing.Point(4, 251);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(53, 12);
            this.linkLabel3.TabIndex = 117;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "服务地址";
            // 
            // _cbTbls
            // 
            this._cbTbls.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbTbls.FormattingEnabled = true;
            this._cbTbls.Location = new System.Drawing.Point(183, 266);
            this._cbTbls.Name = "_cbTbls";
            this._cbTbls.Size = new System.Drawing.Size(277, 20);
            this._cbTbls.TabIndex = 115;
            this._cbTbls.DropDown += new System.EventHandler(this._cbTbls_DropDown);
            // 
            // label9
            // 
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Location = new System.Drawing.Point(0, 266);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(184, 21);
            this.label9.TabIndex = 116;
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Location = new System.Drawing.Point(0, 226);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(460, 21);
            this.label10.TabIndex = 114;
            this.label10.Text = "根据表结构自动生成项模板或列";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _svcUrl
            // 
            this._svcUrl.Location = new System.Drawing.Point(183, 246);
            this._svcUrl.Name = "_svcUrl";
            this._svcUrl.Size = new System.Drawing.Size(277, 21);
            this._svcUrl.TabIndex = 112;
            // 
            // label11
            // 
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label11.Location = new System.Drawing.Point(0, 246);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(184, 21);
            this.label11.TabIndex = 113;
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(3, 82);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(119, 12);
            this.linkLabel1.TabIndex = 119;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "行/项高(ItemHeight)";
            // 
            // _showToolbar
            // 
            this._showToolbar.AutoSize = true;
            this._showToolbar.Location = new System.Drawing.Point(0, 154);
            this._showToolbar.Name = "_showToolbar";
            this._showToolbar.Size = new System.Drawing.Size(84, 16);
            this._showToolbar.TabIndex = 120;
            this._showToolbar.Text = "显示工具栏";
            this._showToolbar.UseVisualStyleBackColor = true;
            // 
            // _showFilter
            // 
            this._showFilter.AutoSize = true;
            this._showFilter.Location = new System.Drawing.Point(249, 154);
            this._showFilter.Name = "_showFilter";
            this._showFilter.Size = new System.Drawing.Size(84, 16);
            this._showFilter.TabIndex = 121;
            this._showFilter.Text = "显示筛选框";
            this._showFilter.UseVisualStyleBackColor = true;
            // 
            // LvXaml
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._showFilter);
            this.Controls.Add(this._showToolbar);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.linkLabel4);
            this.Controls.Add(this.linkLabel3);
            this.Controls.Add(this._cbTbls);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this._svcUrl);
            this.Controls.Add(this.label11);
            this.Controls.Add(this._minItemWidth);
            this.Controls.Add(this.label8);
            this.Controls.Add(this._groupName);
            this.Controls.Add(this.label7);
            this.Controls.Add(this._autoScrollBottom);
            this.Controls.Add(this._showItemBorder);
            this.Controls.Add(this._showGroupHeader);
            this.Controls.Add(this._itemHeight);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._selectionMode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._viewMode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._name);
            this.Controls.Add(this.label1);
            this.Name = "LvXaml";
            this.Size = new System.Drawing.Size(460, 300);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _name;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox _viewMode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox _selectionMode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _itemHeight;
        private System.Windows.Forms.CheckBox _showGroupHeader;
        private System.Windows.Forms.CheckBox _showItemBorder;
        private System.Windows.Forms.CheckBox _autoScrollBottom;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox _groupName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox _minItemWidth;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.ComboBox _cbTbls;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox _svcUrl;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.CheckBox _showToolbar;
        private System.Windows.Forms.CheckBox _showFilter;
    }
}
