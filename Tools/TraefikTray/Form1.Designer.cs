namespace Tray
{
    partial class Form1
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.startMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCm = new System.Windows.Forms.Button();
            this.tbCm = new System.Windows.Forms.RichTextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tbMsg = new System.Windows.Forms.RichTextBox();
            this.btnMsg = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tbFsm = new System.Windows.Forms.RichTextBox();
            this.btnFsm = new System.Windows.Forms.Button();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tbPub = new System.Windows.Forms.RichTextBox();
            this.btnPub = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tbTraefik = new System.Windows.Forms.RichTextBox();
            this.btnTraefix = new System.Windows.Forms.Button();
            this.contextMenuStrip.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Text = "搬运工助手";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startMenuItem,
            this.stopMenuItem,
            this.closeMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(149, 70);
            // 
            // startMenuItem
            // 
            this.startMenuItem.Name = "startMenuItem";
            this.startMenuItem.Size = new System.Drawing.Size(148, 22);
            this.startMenuItem.Text = "启动所有服务";
            this.startMenuItem.Click += new System.EventHandler(this.startMenuItem_Click);
            // 
            // stopMenuItem
            // 
            this.stopMenuItem.Name = "stopMenuItem";
            this.stopMenuItem.Size = new System.Drawing.Size(148, 22);
            this.stopMenuItem.Text = "停止所有服务";
            this.stopMenuItem.Click += new System.EventHandler(this.stopMenuItem_Click);
            // 
            // closeMenuItem
            // 
            this.closeMenuItem.Name = "closeMenuItem";
            this.closeMenuItem.Size = new System.Drawing.Size(148, 22);
            this.closeMenuItem.Text = "退出";
            this.closeMenuItem.Click += new System.EventHandler(this.closeMenuItem_Click);
            // 
            // btnCm
            // 
            this.btnCm.Location = new System.Drawing.Point(20, 20);
            this.btnCm.Name = "btnCm";
            this.btnCm.Size = new System.Drawing.Size(75, 23);
            this.btnCm.TabIndex = 1;
            this.btnCm.Text = "启动";
            this.btnCm.UseVisualStyleBackColor = true;
            this.btnCm.Click += new System.EventHandler(this.btnCm_Click);
            // 
            // tbCm
            // 
            this.tbCm.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tbCm.Location = new System.Drawing.Point(3, 49);
            this.tbCm.Name = "tbCm";
            this.tbCm.Size = new System.Drawing.Size(610, 443);
            this.tbCm.TabIndex = 2;
            this.tbCm.Text = "";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(624, 521);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tbCm);
            this.tabPage1.Controls.Add(this.btnCm);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(616, 495);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Cm";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tbMsg);
            this.tabPage2.Controls.Add(this.btnMsg);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(616, 495);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Msg";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tbMsg
            // 
            this.tbMsg.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tbMsg.Location = new System.Drawing.Point(3, 49);
            this.tbMsg.Name = "tbMsg";
            this.tbMsg.Size = new System.Drawing.Size(610, 443);
            this.tbMsg.TabIndex = 1;
            this.tbMsg.Text = "";
            // 
            // btnMsg
            // 
            this.btnMsg.Location = new System.Drawing.Point(20, 20);
            this.btnMsg.Name = "btnMsg";
            this.btnMsg.Size = new System.Drawing.Size(75, 23);
            this.btnMsg.TabIndex = 0;
            this.btnMsg.Text = "启动";
            this.btnMsg.UseVisualStyleBackColor = true;
            this.btnMsg.Click += new System.EventHandler(this.btnMsg_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tbFsm);
            this.tabPage3.Controls.Add(this.btnFsm);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(616, 495);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Fsm";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tbFsm
            // 
            this.tbFsm.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tbFsm.Location = new System.Drawing.Point(3, 49);
            this.tbFsm.Name = "tbFsm";
            this.tbFsm.Size = new System.Drawing.Size(610, 443);
            this.tbFsm.TabIndex = 1;
            this.tbFsm.Text = "";
            // 
            // btnFsm
            // 
            this.btnFsm.Location = new System.Drawing.Point(20, 20);
            this.btnFsm.Name = "btnFsm";
            this.btnFsm.Size = new System.Drawing.Size(75, 23);
            this.btnFsm.TabIndex = 0;
            this.btnFsm.Text = "启动";
            this.btnFsm.UseVisualStyleBackColor = true;
            this.btnFsm.Click += new System.EventHandler(this.btnFsm_Click);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.tbPub);
            this.tabPage5.Controls.Add(this.btnPub);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(616, 495);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Pub";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tbPub
            // 
            this.tbPub.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tbPub.Location = new System.Drawing.Point(3, 49);
            this.tbPub.Name = "tbPub";
            this.tbPub.Size = new System.Drawing.Size(610, 443);
            this.tbPub.TabIndex = 3;
            this.tbPub.Text = "";
            // 
            // btnPub
            // 
            this.btnPub.Location = new System.Drawing.Point(20, 20);
            this.btnPub.Name = "btnPub";
            this.btnPub.Size = new System.Drawing.Size(75, 23);
            this.btnPub.TabIndex = 2;
            this.btnPub.Text = "启动";
            this.btnPub.UseVisualStyleBackColor = true;
            this.btnPub.Click += new System.EventHandler(this.btnPub_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.tbTraefik);
            this.tabPage4.Controls.Add(this.btnTraefix);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(616, 495);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Traefik";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tbTraefik
            // 
            this.tbTraefik.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tbTraefik.Location = new System.Drawing.Point(3, 49);
            this.tbTraefik.Name = "tbTraefik";
            this.tbTraefik.Size = new System.Drawing.Size(610, 443);
            this.tbTraefik.TabIndex = 1;
            this.tbTraefik.Text = "";
            // 
            // btnTraefix
            // 
            this.btnTraefix.Location = new System.Drawing.Point(20, 20);
            this.btnTraefix.Name = "btnTraefix";
            this.btnTraefix.Size = new System.Drawing.Size(75, 23);
            this.btnTraefix.TabIndex = 0;
            this.btnTraefix.Text = "启动";
            this.btnTraefix.UseVisualStyleBackColor = true;
            this.btnTraefix.Click += new System.EventHandler(this.btnTraefix_Click);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(624, 521);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "搬运工助手";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.contextMenuStrip.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem closeMenuItem;
        private System.Windows.Forms.Button btnCm;
        private System.Windows.Forms.RichTextBox tbCm;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnMsg;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button btnFsm;
        private System.Windows.Forms.Button btnTraefix;
        private System.Windows.Forms.RichTextBox tbMsg;
        private System.Windows.Forms.RichTextBox tbFsm;
        private System.Windows.Forms.RichTextBox tbTraefik;
        private System.Windows.Forms.ToolStripMenuItem startMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopMenuItem;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.RichTextBox tbPub;
        private System.Windows.Forms.Button btnPub;
    }
}

