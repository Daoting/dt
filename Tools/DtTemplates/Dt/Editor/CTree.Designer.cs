
namespace Dt.Editor
{
    partial class CTree
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
            this._selectionMode = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this._tgtID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._srcID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._refreshData = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _footer
            // 
            this._footer.Location = new System.Drawing.Point(0, 151);
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
            this._selectionMode.TabIndex = 45;
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Location = new System.Drawing.Point(0, 40);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(187, 20);
            this.label8.TabIndex = 44;
            this.label8.Text = "选择模式(SelectionMode)";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _tgtID
            // 
            this._tgtID.Location = new System.Drawing.Point(186, 79);
            this._tgtID.Name = "_tgtID";
            this._tgtID.Size = new System.Drawing.Size(194, 21);
            this._tgtID.TabIndex = 53;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(0, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(187, 21);
            this.label3.TabIndex = 52;
            this.label3.Text = "目标属性列表，用\'#\'隔开(TgtID)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _srcID
            // 
            this._srcID.Location = new System.Drawing.Point(186, 59);
            this._srcID.Name = "_srcID";
            this._srcID.Size = new System.Drawing.Size(194, 21);
            this._srcID.TabIndex = 51;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(0, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(187, 21);
            this.label2.TabIndex = 50;
            this.label2.Text = "源属性列表，用\'#\'隔开(SrcID)";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _refreshData
            // 
            this._refreshData.Location = new System.Drawing.Point(3, 106);
            this._refreshData.Name = "_refreshData";
            this._refreshData.Size = new System.Drawing.Size(360, 38);
            this._refreshData.TabIndex = 54;
            this._refreshData.Text = "总加载数据源(RefreshData)，true表示每次显示对话框时都加载数据源，false表示只第一次加载\r\n";
            this._refreshData.UseVisualStyleBackColor = true;
            // 
            // CTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._refreshData);
            this.Controls.Add(this._tgtID);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._srcID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._selectionMode);
            this.Controls.Add(this.label8);
            this.Controls.Add(this._footer);
            this.Controls.Add(this._header);
            this.Name = "CTree";
            this.Size = new System.Drawing.Size(380, 325);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CellFooter _footer;
        private CellHeader _header;
        private System.Windows.Forms.ComboBox _selectionMode;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox _tgtID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _srcID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox _refreshData;
    }
}
