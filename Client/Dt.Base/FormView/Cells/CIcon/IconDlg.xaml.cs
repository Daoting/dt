#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Linq;
using System.Reflection;
using Windows.System;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base.FormView
{
    public sealed partial class IconDlg : Dlg
    {
        public IconDlg()
        {
            InitializeComponent();
            _lv.Data = IconItem.GetAllIcons();
            _lv.ItemClick += OnItemClick;
            _lv.Filter = OnFilter;
        }

        public CIcon Owner { get; internal set; }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            Owner.SelectIcon(((IconItem)e.Data).Icon);
            Close();
        }

        void OnSearch(object sender, string e)
        {
            _lv.Refresh();
        }

        bool OnFilter(object p_obj)
        {
            return ((IconItem)p_obj).IsMatched(_sb.Text);
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
        static Nl<IconItem> _icons;
        readonly string _pinyin;

        public IconItem(Icons p_icon)
        {
            Icon = p_icon;
            Hex = Convert.ToString(0xE000 + (int)p_icon, 16).ToUpper();
            _pinyin = Kit.GetPinYin(p_icon.ToString());
        }

        /// <summary>
        /// 图标
        /// </summary>
        public Icons Icon { get; }

        /// <summary>
        /// 图标名称
        /// </summary>
        public string Name
        {
            get { return Icon.ToString(); }
        }

        /// <summary>
        /// 16进制值
        /// </summary>
        public string Hex { get; }

        /// <summary>
        /// 完整名称
        /// </summary>
        public string FullName
        {
            get { return $"{Icon} ({Hex})"; }
        }

        /// <summary>
        /// 获取所有图标项列表
        /// </summary>
        /// <returns></returns>
        public static Nl<IconItem> GetAllIcons()
        {
            if (_icons == null)
            {
                _icons = new Nl<IconItem>();
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

        /// <summary>
        /// 是否匹配名称
        /// </summary>
        /// <param name="p_txt"></param>
        /// <returns></returns>
        public bool IsMatched(string p_txt)
        {
            if (string.IsNullOrEmpty(p_txt))
                return true;

            return _pinyin.IndexOf(p_txt, StringComparison.OrdinalIgnoreCase) >= 0
                || Name.IndexOf(p_txt, StringComparison.OrdinalIgnoreCase) >= 0
                || Hex.IndexOf(p_txt, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}