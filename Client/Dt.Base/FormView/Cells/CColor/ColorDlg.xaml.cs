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
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using System.Globalization;
#endregion

namespace Dt.Base.FormView
{
    public sealed partial class ColorDlg : Dlg
    {
        static Table _colors;

        public ColorDlg()
        {
            InitializeComponent();
            _lv.Data = GetColorList();
            _lv.ItemClick += OnItemClick;
        }

        public CColor Owner { get; internal set; }

        void OnItemClick(ItemClickArgs e)
        {
            Owner.SelectColor((Color)e.Row[0]);
            Close();
        }

        static Table GetColorList()
        {
            if (_colors == null)
            {
                _colors = new Table { { "color", typeof(Color) } };
                _colors.AddRow(new { color = new Color() });
                Type type = typeof(Microsoft.UI.Colors);
                foreach (PropertyInfo info in type.GetProperties(BindingFlags.Static | BindingFlags.Public))
                {
                    if (info.GetValue(type) is Color color)
                    {
                        _colors.AddRow(new { color = color });
                    }
                }
            }
            return _colors;
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            // 回车跳下一格
            if (e.Key == VirtualKey.Enter)
            {
                if (e.OriginalSource == _tb && !ApplyColor())
                    return;

                Owner.Owner.GotoNextCell(Owner);
                e.Handled = true;
                if (IsOpened)
                    Close();
            }
        }

        void OnCustom(object sender, RoutedEventArgs e)
        {
            if (ApplyColor())
                Close();
        }
        
        bool ApplyColor()
        {
            string strColor = _tb.Text.Trim();
            if (strColor.StartsWith("#") && strColor.Length == 9)
            {
                try
                {
                    Color color = Color.FromArgb(
                        byte.Parse(strColor.Substring(1, 2), NumberStyles.HexNumber),
                        byte.Parse(strColor.Substring(3, 2), NumberStyles.HexNumber),
                        byte.Parse(strColor.Substring(5, 2), NumberStyles.HexNumber),
                        byte.Parse(strColor.Substring(7, 2), NumberStyles.HexNumber));
                    Owner.SelectColor(color);
                    
                    return true;
                }
                catch (Exception ex)
                {
                    Kit.Warn("颜色不正确！" + ex.Message);
                }
            }
            else
            {
                Kit.Warn(strColor + " 颜色格式不正确！");
            }
            return false;
        }
    }
}
