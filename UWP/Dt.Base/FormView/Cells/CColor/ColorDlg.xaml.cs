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
using System.Collections.Generic;
using System.Reflection;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base.FormView
{
    public sealed partial class ColorDlg : Dlg
    {
        static Nl<ColorItem> _colors;

        public ColorDlg()
        {
            InitializeComponent();
            _lv.Data = GetColorList();
            _lv.ItemClick += OnItemClick;
        }

        public CColor Owner { get; internal set; }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            Owner.SelectColor(((ColorItem)e.Data).Brush);
            Close();
        }

        static Nl<ColorItem> GetColorList()
        {
            if (_colors == null)
            {
                Type type = typeof(Windows.UI.Colors);
                _colors = new Nl<ColorItem>();
                _colors.Add(new ColorItem(new Color(), "无"));
#if UWP
                foreach (PropertyInfo info in type.GetProperties())
                {
                    var color = (Color)info.GetValue(type);
                    _colors.Add(new ColorItem(color, $"{info.Name} ({color})"));
                }
#else
                foreach (FieldInfo info in type.GetFields())
                {
                    _colors.Add(new ColorItem(Colors.Parse(info.Name), info.Name));
                }
#endif
            }
            return _colors;
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

    public class ColorItem
    {
        public ColorItem(Color p_color, string p_name)
        {
            Brush = new SolidColorBrush(p_color);
            Name = p_name;
        }

        public SolidColorBrush Brush { get; set; }

        public string Name { get; set; }
    }
}
