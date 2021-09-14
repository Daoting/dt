
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
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this._groupName = new System.Windows.Forms.TextBox();
            this._enteredBrush = new System.Windows.Forms.ComboBox();
            this._pressedBrush = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this._minItemWidth = new System.Windows.Forms.TextBox();
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
            this.label4.Location = new System.Drawing.Point(0, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(250, 21);
            this.label4.TabIndex = 8;
            this.label4.Text = "行/项高(ItemHeight)";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _itemHeight
            // 
            this._itemHeight.Location = new System.Drawing.Point(249, 58);
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
            this._showGroupHeader.Location = new System.Drawing.Point(0, 178);
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
            this._showItemBorder.Location = new System.Drawing.Point(249, 178);
            this._showItemBorder.Name = "_showItemBorder";
            this._showItemBorder.Size = new System.Drawing.Size(192, 16);
            this._showItemBorder.TabIndex = 11;
            this._showItemBorder.Text = "显示行分割线(ShowItemBorder)";
            this._showItemBorder.UseVisualStyleBackColor = true;
            // 
            // _autoScrollBottom
            // 
            this._autoScrollBottom.AutoSize = true;
            this._autoScrollBottom.Location = new System.Drawing.Point(0, 212);
            this._autoScrollBottom.Name = "_autoScrollBottom";
            this._autoScrollBottom.Size = new System.Drawing.Size(264, 16);
            this._autoScrollBottom.TabIndex = 12;
            this._autoScrollBottom.Text = "切换数据自动滚动到底端(AutoScrollBottom)";
            this._autoScrollBottom.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(0, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(250, 20);
            this.label5.TabIndex = 13;
            this.label5.Text = "鼠标进入时行背景色(EnteredBrush)";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point(0, 97);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(250, 20);
            this.label6.TabIndex = 14;
            this.label6.Text = "点击时行背景色(PressedBrush)";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Location = new System.Drawing.Point(0, 116);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(250, 21);
            this.label7.TabIndex = 15;
            this.label7.Text = "分组列名(GroupName)";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _groupName
            // 
            this._groupName.Location = new System.Drawing.Point(249, 116);
            this._groupName.Name = "_groupName";
            this._groupName.Size = new System.Drawing.Size(211, 21);
            this._groupName.TabIndex = 16;
            // 
            // _enteredBrush
            // 
            this._enteredBrush.FormattingEnabled = true;
            this._enteredBrush.Items.AddRange(new object[] {
            "默认",
            "无色",
            "深黄遮罩",
            "深暗遮罩"});
            this._enteredBrush.Location = new System.Drawing.Point(249, 78);
            this._enteredBrush.Name = "_enteredBrush";
            this._enteredBrush.Size = new System.Drawing.Size(211, 20);
            this._enteredBrush.TabIndex = 17;
            // 
            // _pressedBrush
            // 
            this._pressedBrush.FormattingEnabled = true;
            this._pressedBrush.Items.AddRange(new object[] {
            "默认",
            "无色",
            "深黄遮罩",
            "深暗遮罩"});
            this._pressedBrush.Location = new System.Drawing.Point(249, 97);
            this._pressedBrush.Name = "_pressedBrush";
            this._pressedBrush.Size = new System.Drawing.Size(211, 20);
            this._pressedBrush.TabIndex = 18;
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Location = new System.Drawing.Point(0, 136);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(250, 21);
            this.label8.TabIndex = 19;
            this.label8.Text = "磁贴项最小宽度(MinItemWidth)";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _minItemWidth
            // 
            this._minItemWidth.Location = new System.Drawing.Point(249, 136);
            this._minItemWidth.Name = "_minItemWidth";
            this._minItemWidth.Size = new System.Drawing.Size(211, 21);
            this._minItemWidth.TabIndex = 20;
            this._minItemWidth.Text = "160";
            // 
            // LvXaml
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._minItemWidth);
            this.Controls.Add(this.label8);
            this.Controls.Add(this._pressedBrush);
            this.Controls.Add(this._enteredBrush);
            this.Controls.Add(this._groupName);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
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
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox _groupName;
        private System.Windows.Forms.ComboBox _enteredBrush;
        private System.Windows.Forms.ComboBox _pressedBrush;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox _minItemWidth;
    }
}
