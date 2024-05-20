namespace TestToolkit
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _tb = new TextBox();
            button1 = new Button();
            panel1 = new Panel();
            _lbl = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // _tb
            // 
            _tb.AcceptsReturn = true;
            _tb.Location = new Point(12, 37);
            _tb.Multiline = true;
            _tb.Name = "_tb";
            _tb.Size = new Size(446, 555);
            _tb.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(418, 7);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 1;
            button1.Text = "美化";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.Controls.Add(_lbl);
            panel1.Location = new Point(468, 37);
            panel1.Name = "panel1";
            panel1.Size = new Size(446, 555);
            panel1.TabIndex = 2;
            // 
            // _lbl
            // 
            _lbl.AutoSize = true;
            _lbl.Location = new Point(13, 11);
            _lbl.Name = "_lbl";
            _lbl.Size = new Size(43, 17);
            _lbl.TabIndex = 0;
            _lbl.Text = "label1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(924, 594);
            Controls.Add(panel1);
            Controls.Add(button1);
            Controls.Add(_tb);
            Name = "Form1";
            Text = "Form1";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox _tb;
        private Button button1;
        private Panel panel1;
        private Label _lbl;
    }
}
