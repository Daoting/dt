
namespace Dt.Editor
{
    partial class CList
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
            this._selectionMode = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this._enum = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this._option = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._srcID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._tgtID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._refreshData = new System.Windows.Forms.CheckBox();
            this._isEditable = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this._viewMode = new System.Windows.Forms.ComboBox();
            this._customView = new System.Windows.Forms.CheckBox();
            this._staticData = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
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
            // _selectionMode
            // 
            this._selectionMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._selectionMode.FormattingEnabled = true;
            this._selectionMode.Items.AddRange(new object[] {
            "None",
            "Single",
            "Multiple"});
            this._selectionMode.Location = new System.Drawing.Point(186, 40);
            this._selectionMode.Name = "_selectionMode";
            this._selectionMode.Size = new System.Drawing.Size(194, 20);
            this._selectionMode.TabIndex = 41;
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Location = new System.Drawing.Point(0, 40);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(187, 20);
            this.label8.TabIndex = 40;
            this.label8.Text = "选择模式(SelectionMode)";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _enum
            // 
            this._enum.Location = new System.Drawing.Point(186, 59);
            this._enum.Name = "_enum";
            this._enum.Size = new System.Drawing.Size(194, 21);
            this._enum.TabIndex = 43;
            // 
            // label7
            // 
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Location = new System.Drawing.Point(0, 59);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(187, 21);
            this.label7.TabIndex = 42;
            this.label7.Text = "枚举类型数据源(Enum)";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _option
            // 
            this._option.Location = new System.Drawing.Point(186, 79);
            this._option.Name = "_option";
            this._option.Size = new System.Drawing.Size(194, 21);
            this._option.TabIndex = 45;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(0, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 21);
            this.label1.TabIndex = 44;
            this.label1.Text = "基础选项数据源(Option)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _srcID
            // 
            this._srcID.Location = new System.Drawing.Point(186, 118);
            this._srcID.Name = "_srcID";
            this._srcID.Size = new System.Drawing.Size(194, 21);
            this._srcID.TabIndex = 47;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(0, 118);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(187, 21);
            this.label2.TabIndex = 46;
            this.label2.Text = "源属性列表，用\'#\'隔开(SrcID)";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _tgtID
            // 
            this._tgtID.Location = new System.Drawing.Point(186, 138);
            this._tgtID.Name = "_tgtID";
            this._tgtID.Size = new System.Drawing.Size(194, 21);
            this._tgtID.TabIndex = 49;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(0, 138);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(187, 21);
            this.label3.TabIndex = 48;
            this.label3.Text = "目标属性列表，用\'#\'隔开(TgtID)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _refreshData
            // 
            this._refreshData.Location = new System.Drawing.Point(4, 186);
            this._refreshData.Name = "_refreshData";
            this._refreshData.Size = new System.Drawing.Size(360, 38);
            this._refreshData.TabIndex = 50;
            this._refreshData.Text = "总加载数据源(RefreshData)，true表示每次显示对话框时都加载数据源，false表示只第一次加载\r\n";
            this._refreshData.UseVisualStyleBackColor = true;
            // 
            // _isEditable
            // 
            this._isEditable.AutoSize = true;
            this._isEditable.Location = new System.Drawing.Point(4, 230);
            this._isEditable.Name = "_isEditable";
            this._isEditable.Size = new System.Drawing.Size(132, 16);
            this._isEditable.TabIndex = 51;
            this._isEditable.Text = "可编辑(IsEditable)";
            this._isEditable.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Location = new System.Drawing.Point(0, 158);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(187, 20);
            this.label4.TabIndex = 53;
            this.label4.Text = "内部Lv视图类型(ViewMode)";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _viewMode
            // 
            this._viewMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._viewMode.FormattingEnabled = true;
            this._viewMode.Items.AddRange(new object[] {
            "List",
            "Table",
            "Tile"});
            this._viewMode.Location = new System.Drawing.Point(186, 158);
            this._viewMode.Name = "_viewMode";
            this._viewMode.Size = new System.Drawing.Size(194, 20);
            this._viewMode.TabIndex = 54;
            // 
            // _customView
            // 
            this._customView.AutoSize = true;
            this._customView.Location = new System.Drawing.Point(4, 264);
            this._customView.Name = "_customView";
            this._customView.Size = new System.Drawing.Size(144, 16);
            this._customView.TabIndex = 55;
            this._customView.Text = "自定义内部Lv视图模板";
            this._customView.UseVisualStyleBackColor = true;
            // 
            // _staticData
            // 
            this._staticData.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._staticData.FormattingEnabled = true;
            this._staticData.Items.AddRange(new object[] {
            "无",
            "字符串列表",
            "IDStr列表",
            "整数列表"});
            this._staticData.Location = new System.Drawing.Point(186, 99);
            this._staticData.Name = "_staticData";
            this._staticData.Size = new System.Drawing.Size(194, 20);
            this._staticData.TabIndex = 57;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(0, 99);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(187, 20);
            this.label5.TabIndex = 56;
            this.label5.Text = "静态数据源";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _footer
            // 
            this._footer.Location = new System.Drawing.Point(0, 305);
            this._footer.Name = "_footer";
            this._footer.Size = new System.Drawing.Size(380, 170);
            this._footer.TabIndex = 58;
            // 
            // CList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._footer);
            this.Controls.Add(this._staticData);
            this.Controls.Add(this.label5);
            this.Controls.Add(this._customView);
            this.Controls.Add(this._viewMode);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._isEditable);
            this.Controls.Add(this._refreshData);
            this.Controls.Add(this._tgtID);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._srcID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._option);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._enum);
            this.Controls.Add(this.label7);
            this.Controls.Add(this._selectionMode);
            this.Controls.Add(this.label8);
            this.Controls.Add(this._header);
            this.Name = "CList";
            this.Size = new System.Drawing.Size(380, 478);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CellHeader _header;
        private System.Windows.Forms.ComboBox _selectionMode;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox _enum;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox _option;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _srcID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _tgtID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox _refreshData;
        private System.Windows.Forms.CheckBox _isEditable;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox _viewMode;
        private System.Windows.Forms.CheckBox _customView;
        private System.Windows.Forms.ComboBox _staticData;
        private System.Windows.Forms.Label label5;
        private Dt.Editor.CellFooter _footer;
    }
}
