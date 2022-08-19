
namespace Dt.Editor
{
    partial class MenuXaml
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuXaml));
            this._cbMenu = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this._save = new System.Windows.Forms.CheckBox();
            this._search = new System.Windows.Forms.CheckBox();
            this._edit = new System.Windows.Forms.CheckBox();
            this._add = new System.Windows.Forms.CheckBox();
            this._del = new System.Windows.Forms.CheckBox();
            this._setting = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _cbMenu
            // 
            this._cbMenu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbMenu.FormattingEnabled = true;
            this._cbMenu.Items.AddRange(new object[] {
            "工具栏菜单",
            "上下文菜单"});
            this._cbMenu.Location = new System.Drawing.Point(187, 7);
            this._cbMenu.Name = "_cbMenu";
            this._cbMenu.Size = new System.Drawing.Size(269, 20);
            this._cbMenu.TabIndex = 108;
            // 
            // label9
            // 
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Location = new System.Drawing.Point(12, 7);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(176, 21);
            this.label9.TabIndex = 109;
            this.label9.Text = "菜单类型";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Location = new System.Drawing.Point(12, 48);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(444, 21);
            this.label10.TabIndex = 121;
            this.label10.Text = "选择常用菜单项";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(381, 262);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 126;
            this.button1.Text = "添加";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // _save
            // 
            this._save.AutoSize = true;
            this._save.Location = new System.Drawing.Point(187, 84);
            this._save.Name = "_save";
            this._save.Size = new System.Drawing.Size(48, 16);
            this._save.TabIndex = 127;
            this._save.Text = "保存";
            this._save.UseVisualStyleBackColor = true;
            // 
            // _search
            // 
            this._search.AutoSize = true;
            this._search.Location = new System.Drawing.Point(31, 122);
            this._search.Name = "_search";
            this._search.Size = new System.Drawing.Size(48, 16);
            this._search.TabIndex = 128;
            this._search.Text = "搜索";
            this._search.UseVisualStyleBackColor = true;
            // 
            // _edit
            // 
            this._edit.AutoSize = true;
            this._edit.Location = new System.Drawing.Point(187, 122);
            this._edit.Name = "_edit";
            this._edit.Size = new System.Drawing.Size(48, 16);
            this._edit.TabIndex = 129;
            this._edit.Text = "编辑";
            this._edit.UseVisualStyleBackColor = true;
            // 
            // _add
            // 
            this._add.AutoSize = true;
            this._add.Location = new System.Drawing.Point(31, 84);
            this._add.Name = "_add";
            this._add.Size = new System.Drawing.Size(48, 16);
            this._add.TabIndex = 130;
            this._add.Text = "增加";
            this._add.UseVisualStyleBackColor = true;
            // 
            // _del
            // 
            this._del.AutoSize = true;
            this._del.Location = new System.Drawing.Point(381, 84);
            this._del.Name = "_del";
            this._del.Size = new System.Drawing.Size(48, 16);
            this._del.TabIndex = 131;
            this._del.Text = "删除";
            this._del.UseVisualStyleBackColor = true;
            // 
            // _setting
            // 
            this._setting.AutoSize = true;
            this._setting.Location = new System.Drawing.Point(381, 122);
            this._setting.Name = "_setting";
            this._setting.Size = new System.Drawing.Size(48, 16);
            this._setting.TabIndex = 132;
            this._setting.Text = "设置";
            this._setting.UseVisualStyleBackColor = true;
            // 
            // MenuXaml
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 300);
            this.Controls.Add(this._setting);
            this.Controls.Add(this._del);
            this.Controls.Add(this._add);
            this.Controls.Add(this._edit);
            this.Controls.Add(this._search);
            this.Controls.Add(this._save);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label10);
            this.Controls.Add(this._cbMenu);
            this.Controls.Add(this.label9);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MenuXaml";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加 Menu";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox _cbMenu;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox _save;
        private System.Windows.Forms.CheckBox _search;
        private System.Windows.Forms.CheckBox _edit;
        private System.Windows.Forms.CheckBox _add;
        private System.Windows.Forms.CheckBox _del;
        private System.Windows.Forms.CheckBox _setting;
    }
}