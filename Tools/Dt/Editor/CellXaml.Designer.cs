
namespace Dt.Editor
{
    partial class CellXaml
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CellXaml));
            this._lb = new System.Windows.Forms.ListBox();
            this._split = new System.Windows.Forms.SplitContainer();
            this._btn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._split)).BeginInit();
            this._split.Panel1.SuspendLayout();
            this._split.SuspendLayout();
            this.SuspendLayout();
            // 
            // _lb
            // 
            this._lb.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lb.FormattingEnabled = true;
            this._lb.ItemHeight = 12;
            this._lb.Items.AddRange(new object[] {
            "CBar",
            "CText",
            "CNum",
            "CBool",
            "CList"});
            this._lb.Location = new System.Drawing.Point(0, 0);
            this._lb.Name = "_lb";
            this._lb.Size = new System.Drawing.Size(123, 490);
            this._lb.TabIndex = 0;
            this._lb.SelectedIndexChanged += new System.EventHandler(this._lb_SelectedIndexChanged);
            // 
            // _split
            // 
            this._split.Dock = System.Windows.Forms.DockStyle.Top;
            this._split.Location = new System.Drawing.Point(0, 0);
            this._split.Name = "_split";
            // 
            // _split.Panel1
            // 
            this._split.Panel1.Controls.Add(this._lb);
            // 
            // _split.Panel2
            // 
            this._split.Panel2.AutoScroll = true;
            this._split.Size = new System.Drawing.Size(554, 490);
            this._split.SplitterDistance = 123;
            this._split.TabIndex = 1;
            // 
            // _btn
            // 
            this._btn.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._btn.Location = new System.Drawing.Point(0, 488);
            this._btn.Name = "_btn";
            this._btn.Size = new System.Drawing.Size(554, 23);
            this._btn.TabIndex = 1;
            this._btn.Text = "添加";
            this._btn.UseVisualStyleBackColor = true;
            this._btn.Click += new System.EventHandler(this._btn_Click);
            // 
            // CellXaml
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 511);
            this.Controls.Add(this._btn);
            this.Controls.Add(this._split);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CellXaml";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "添加格";
            this._split.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._split)).EndInit();
            this._split.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox _lb;
        private System.Windows.Forms.SplitContainer _split;
        private System.Windows.Forms.Button _btn;
    }
}