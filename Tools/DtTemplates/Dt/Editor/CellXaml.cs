using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Dt.Editor
{
    public partial class CellXaml : Form
    {
        readonly Dictionary<string, Control> _ls = new Dictionary<string, Control>();
        ICellControl _cur;

        public CellXaml()
        {
            InitializeComponent();
        }

        void _lb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_lb.SelectedIndex == -1)
                return;

            _split.Panel2.Controls.Clear();
            _cur = null;

            Control con;
            var name = _lb.SelectedItem.ToString();
            if (_ls.TryGetValue(name, out con))
            {
                _cur = con as ICellControl;
            }
            else
            {
                // 特殊
                if (name == "批量生成格")
                    name = "BatchCells";

                var tp = Type.GetType($"Dt.Editor.{name}");
                if (tp != null)
                {
                    con = Activator.CreateInstance(tp) as Control;
                    con.Location = new Point((_split.Panel2.ClientSize.Width - con.ClientSize.Width) / 2, 0);
                    _cur = con as ICellControl;
                    _ls[name] = con;
                }
            }

            if (con != null && _cur != null)
            {
                _cur.Reset();
                _split.Panel2.Controls.Add(con);
            }
        }

        void _btn_Click(object sender, EventArgs e)
        {
            if (_cur == null)
                return;

            string txt = _cur.GetText();
            if (!string.IsNullOrEmpty(txt))
            {
                Kit.Paste(txt);
                _cur.Reset();
            }
        }
    }

    public interface ICellControl
    {
        /// <summary>
        /// 获取要插入的文本
        /// </summary>
        /// <returns></returns>
        string GetText();

        /// <summary>
        /// 重置默认值
        /// </summary>
        void Reset();
    }
}
