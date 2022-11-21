#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-11-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Shapes;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 常用的单元格UI类型
    /// </summary>
    [CellUI]
    public class Def
    {
        /// <summary>
        /// 显示为图标字符
        /// </summary>
        /// <param name="e"></param>
        public static void Icon(Env e)
        {
            var tb = new TextBlock
            {
                Style = Res.LvTextBlock,
                FontFamily = Res.IconFont,
                TextAlignment = TextAlignment.Center,
            };
            e.UI = tb;

            e.Set += c =>
            {
                var val = c.CellVal;
                string txt = null;
                if (val != null)
                {
                    if (val is int || val is byte)
                        txt = Res.GetIconChar((Icons)val);
                    else
                        txt = Res.ParseIconChar(val.ToString());
                }
                tb.Text = string.IsNullOrEmpty(txt) ? "" : txt;
                c.Dot.ToggleVisible(string.IsNullOrEmpty(txt));
            };
        }

        /// <summary>
        /// 显示为CheckBox字符
        /// </summary>
        /// <param name="e"></param>
        public static void CheckBox(Env e)
        {
            // 字符模拟CheckBox
            var tb = new TextBlock
            {
                Style = Res.LvTextBlock,
                FontFamily = Res.IconFont,
                TextAlignment = TextAlignment.Center,
            };
            e.UI = tb;
            e.Set += c =>
            {
                tb.Text = c.Bool ? "\uE059" : "\uE057";
                c.Dot.ToggleVisible(c.CellVal == null);
            };
        }

        /// <summary>
        /// 显示为图片
        /// </summary>
        /// <param name="e"></param>
        public static void Image(Env e)
        {
            Image img = new Image();
            e.UI = img;

            e.Set += c =>
            {
                string path = c.Str;
                if (path.StartsWith("ms-appx:", StringComparison.OrdinalIgnoreCase))
                {
                    // 因 uno 中的 Image.Source 目前只支持ms-appx，故 ms-appdata 和 http都暂不支持！！！
                    img.Source = new BitmapImage(new Uri(path));
                }
                else if (path != "")
                {
                    // 文件服务的路径，json格式同FileList
                    _ = Kit.LoadImage(path, img);
                }
                c.Dot.ToggleVisible(path == "");
            };
        }

        /// <summary>
        /// 显示为文件列表链接
        /// </summary>
        /// <param name="e"></param>
        public static void FileLink(Env e)
        {
            TextBlock tb = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Foreground = Res.主蓝,
            };
            tb.PointerPressed += OnFileLinkPressed;
            e.UI = tb;

            e.Set += c =>
            {
                string str = c.Str;
                int cnt = str.Split(new string[] { "[\"" }, StringSplitOptions.None).Length - 1;
                tb.Text = cnt <= 0 ? "" : $"共{cnt}个文件";
                tb.Tag = str;
                c.Dot.ToggleVisible(cnt <= 0);
            };
        }

        /// <summary>
        /// 显示为枚举类型的名称
        /// </summary>
        /// <param name="e"></param>
        public static void EnumText(Env e)
        {
            var tb = new TextBlock { Style = Res.LvTextBlock };
            e.UI = tb;

            e.Set += c =>
            {
                string txt = "";
                if (string.IsNullOrEmpty(c.Format) || c.Str == "")
                {
                    txt = "无枚举";
                }
                else
                {
                    // 将byte int等数值类型转成枚举类型，显示枚举项
                    Type type = Type.GetType(c.Format, false, true);
                    if (type != null)
                    {
                        try
                        {
                            txt = Enum.ToObject(type, c.CellVal).ToString();
                        }
                        catch { }
                    }
                }
                tb.Text = txt;
            };
        }

        /// <summary>
        /// 自适应时间转换器，如 昨天，09:13, 2015-04-09
        /// </summary>
        /// <param name="e"></param>
        public static void AutoDate(Env e)
        {
            var tb = new TextBlock { Style = Res.LvTextBlock };
            e.UI = tb;

            e.Set += c =>
            {
                var val = c.CellVal;
                DateTime dt;
                if (val.GetType() == typeof(DateTime))
                {
                    dt = (DateTime)val;
                }
                else
                {
                    try
                    {
                        dt = (DateTime)System.Convert.ChangeType(val, typeof(DateTime));
                    }
                    catch
                    {
                        tb.Text = "";
                        return;
                    }
                }

                TimeSpan ts = DateTime.Now.Date - dt.Date;
                switch (ts.Days)
                {
                    case 0:
                        tb.Text = dt.ToString("HH:mm:ss");
                        break;
                    case 1:
                        tb.Text = "昨天";
                        break;
                    case -1:
                        tb.Text = "明天";
                        break;
                    default:
                        tb.Text = dt.ToString("yyyy-MM-dd");
                        break;
                }
            };
        }

        /// <summary>
        /// 红底白字的警告圈样式
        /// </summary>
        /// <param name="e"></param>
        public static void Warning(Env e)
        {
            var grid = new Grid { Children = { new Ellipse { Fill = Res.RedBrush, Width = 23, Height = 23 } } };
            var tb = new TextBlock { Foreground = Res.WhiteBrush, FontSize = 14, TextAlignment = TextAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            grid.Children.Add(tb);
            e.UI = grid;

            e.Set += c =>
            {
                var txt = c.Str;
                if (txt.Length > 2)
                    txt = "┅";
                tb.Text = txt;
                c.Dot.ToggleVisible(txt == "");
            };
        }

        /// <summary>
        /// 深灰小字
        /// </summary>
        /// <param name="e"></param>
        public static void 小灰(Env e)
        {
            e.Dot.Foreground = Res.深灰2;
            e.Dot.FontSize = Res.小字;
        }

        /// <summary>
        /// 黑底白字
        /// </summary>
        /// <param name="e"></param>
        public static void 黑白(Env e)
        {
            e.Dot.Foreground = Res.WhiteBrush;
            e.Dot.Background = Res.BlackBrush;
            e.Dot.Padding = _defPadding;
        }

        /// <summary>
        /// 蓝底白字
        /// </summary>
        /// <param name="e"></param>
        public static void 蓝白(Env e)
        {
            e.Dot.Foreground = Res.WhiteBrush;
            e.Dot.Background = Res.主蓝;
            e.Dot.Padding = _defPadding;
        }

        /// <summary>
        /// 红底白字
        /// </summary>
        /// <param name="e"></param>
        public static void 红白(Env e)
        {
            e.Dot.Foreground = Res.WhiteBrush;
            e.Dot.Background = Res.RedBrush;
            e.Dot.Padding = _defPadding;
        }

        static void OnFileLinkPressed(object sender, PointerRoutedEventArgs e)
        {
            var tb = sender as TextBlock;
            if (tb == null || tb.Tag == null)
                return;

            Dlg dlg;
            e.Handled = true;
            if (Kit.IsPhoneUI)
            {
                dlg = new Dlg { ClipElement = tb, Title = "文件列表", };
            }
            else
            {
                dlg = new Dlg()
                {
                    WinPlacement = DlgPlacement.TargetBottomLeft,
                    PlacementTarget = tb,
                    ClipElement = tb,
                    MaxHeight = 500,
                    MaxWidth = 400,
                    Title = "文件列表",
                };
            }
            FileList fl = new FileList();
            fl.Data = (string)tb.Tag;

            ScrollViewer sv = new ScrollViewer
            {
                VerticalScrollMode = ScrollMode.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollMode = ScrollMode.Disabled,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            };
            sv.Content = fl;
            dlg.Content = sv;
            dlg.Show();
        }

        static Thickness _defPadding = new Thickness(10, 4, 10, 4);
    }
}
