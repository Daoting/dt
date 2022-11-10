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
            var val = e.CellVal;
            if (val == null)
                return;

            string txt;
            if (val is int || val is byte)
                txt = Res.GetIconChar((Icons)val);
            else
                txt = Res.ParseIconChar(val.ToString());

            // 无字符，返回null
            if (string.IsNullOrEmpty(txt))
                return;

            e.UI = new TextBlock
            {
                Style = Res.LvTextBlock,
                Text = txt,
                FontFamily = Res.IconFont,
                TextAlignment = TextAlignment.Center,
            };
        }

        /// <summary>
        /// 显示为CheckBox字符
        /// </summary>
        /// <param name="e"></param>
        public static void CheckBox(Env e)
        {
            var val = e.CellVal;
            if (val == null)
                return;

            // 字符模拟CheckBox
            var tb = new TextBlock
            {
                Style = Res.LvTextBlock,
                FontFamily = Res.IconFont,
                TextAlignment = TextAlignment.Center,
            };

            bool b;
            if (val is bool)
            {
                b = (bool)val;
            }
            else
            {
                string temp = val.ToString().ToLower();
                b = (temp == "1" || temp == "true");
            }
            tb.Text = b ? "\uE059" : "\uE057";
            e.UI = tb;
        }

        /// <summary>
        /// 显示为图片
        /// </summary>
        /// <param name="e"></param>
        public static void Image(Env e)
        {
            var val = e.CellVal;
            if (val == null)
                return;

            Image img = new Image();
            string path = val.ToString();

            if (path.StartsWith("ms-appx:", StringComparison.OrdinalIgnoreCase))
            {
                // 因 uno 中的 Image.Source 目前只支持ms-appx，故 ms-appdata 和 http都暂不支持！！！
                img.Source = new BitmapImage(new Uri(path));
            }
            else
            {
                // 文件服务的路径，json格式同FileList
                _ = Kit.LoadImage(path, img);
            }
            e.UI = img;
        }

        /// <summary>
        /// 显示为文件列表链接
        /// </summary>
        /// <param name="e"></param>
        public static void FileLink(Env e)
        {
            var val = e.CellVal;
            if (val == null)
                return;

            int cnt = val.ToString().Split(new string[] { "[\"" }, StringSplitOptions.None).Length - 1;
            if (cnt <= 0)
                return;

            TextBlock tb = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Foreground = Res.主蓝,
                Text = $"共{cnt}个文件",
                Tag = val,
            };

            tb.PointerPressed += OnFileLinkPressed;
            e.UI = tb;
        }

        /// <summary>
        /// 显示为枚举类型的名称
        /// </summary>
        /// <param name="e"></param>
        public static void EnumText(Env e)
        {
            var val = e.CellVal;
            if (val == null)
                return;

            string tpName = e.Format;
            if (string.IsNullOrEmpty(tpName))
            {
                e.UI = new TextBlock { Style = Res.LvTextBlock, Text = "无枚举" };
                return;
            }

            // 将byte int等数值类型转成枚举类型，显示枚举项
            Type type = Type.GetType(tpName, false, true);
            if (type != null)
            {
                try
                {
                    var txt = Enum.ToObject(type, val).ToString();
                    e.UI = new TextBlock { Style = Res.LvTextBlock, Text = txt };
                    return;
                }
                catch { }
            }

            e.UI = new TextBlock { Style = Res.LvTextBlock, Text = "无枚举" };
        }

        /// <summary>
        /// 自适应时间转换器，如 昨天，09:13, 2015-04-09
        /// </summary>
        /// <param name="e"></param>
        public static void AutoDate(Env e)
        {
            var val = e.CellVal;
            if (val == null)
                return;

            var tb = new TextBlock { Style = Res.LvTextBlock };
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
                    e.UI = tb;
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
            e.UI = tb;
        }

        /// <summary>
        /// 红底白字的警告圈样式
        /// </summary>
        /// <param name="e"></param>
        public static void Warning(Env e)
        {
            var val = e.CellVal;
            if (val == null)
                return;

            var txt = val.ToString();
            if (txt == "")
                return;

            if (txt.Length > 2)
                txt = "┅";

            e.UI = new Grid
            {
                Children =
                {
                    new Ellipse { Fill = Res.RedBrush, Width = 23, Height = 23 },
                    new TextBlock {Text = txt, Foreground = Res.WhiteBrush, FontSize = 14, TextAlignment = TextAlignment.Center, VerticalAlignment = VerticalAlignment.Center },
                }
            };
        }

        /// <summary>
        /// 深灰小字
        /// </summary>
        /// <param name="e"></param>
        public static void 小灰(Env e)
        {
            e.Foreground = Res.深灰2;
            e.FontSize = Res.小字;
        }

        /// <summary>
        /// 黑底白字
        /// </summary>
        /// <param name="e"></param>
        public static void 黑白(Env e)
        {
            e.Foreground = Res.WhiteBrush;
            e.Background = Res.BlackBrush;
            e.Padding = _defPadding;
        }

        /// <summary>
        /// 蓝底白字
        /// </summary>
        /// <param name="e"></param>
        public static void 蓝白(Env e)
        {
            e.Foreground = Res.WhiteBrush;
            e.Background = Res.主蓝;
            e.Padding = _defPadding;
        }

        /// <summary>
        /// 红底白字
        /// </summary>
        /// <param name="e"></param>
        public static void 红白(Env e)
        {
            e.Foreground = Res.WhiteBrush;
            e.Background = Res.RedBrush;
            e.Padding = _defPadding;
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
