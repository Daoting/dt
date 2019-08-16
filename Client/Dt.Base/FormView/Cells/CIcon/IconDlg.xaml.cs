#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.System;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base.FormView
{
    public sealed partial class IconDlg : Dlg
    {
        static List<IconItem> _icons;

        public IconDlg()
        {
            InitializeComponent();
            _lv.Data = GetIconList();
            _lv.ItemClick += OnItemClick;
            _lv.Filter = OnFilter;
        }

        public CIcon Owner { get; internal set; }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            Owner.SelectIcon(((IconItem)e.Data).Icon);
            Close();
        }

        static List<IconItem> GetIconList()
        {
            if (_icons == null)
            {
                _icons = new List<IconItem>();
                Type type = typeof(Icons);
                foreach (var fi in from f in type.GetRuntimeFields()
                                   where f.IsLiteral
                                   select f)
                {
                    _icons.Add(new IconItem((Icons)fi.GetValue(type)));
                }
            }
            return _icons;
        }

        void OnSearch(object sender, string e)
        {
            _lv.Refresh();
        }

        bool OnFilter(object p_obj)
        {
            string txt = _sb.Text;
            if (string.IsNullOrEmpty(txt))
                return true;
#if UWP
            return ((IconItem)p_obj).Name.Contains(txt, StringComparison.OrdinalIgnoreCase);
#else
            return ((IconItem)p_obj).Name.Contains(txt);
#endif
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            // 回车跳下一格
            if (e.Key == VirtualKey.Enter)
            {
                Owner.Owner.GotoNextCell(Owner);
                e.Handled = true;
                if (IsOpened)
                    Close();
            }
        }
    }

    public class IconItem
    {
        public IconItem(Icons p_icon)
        {
            Icon = p_icon;
            string hex = Convert.ToString(0xE000 + (int)p_icon, 16).ToUpper();
            Name = $"{p_icon} ({hex})";
        }

        public Icons Icon { get; set; }

        public string Name { get; set; }
    }
}