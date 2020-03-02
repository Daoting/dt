#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 视图项基类，是数据和UI的中间对象
    /// 继承DependencyObject为节省资源，实现INotifyPropertyChanged作为DataContext能更新
    /// </summary>
    public abstract partial class ViewItem : DependencyObject, INotifyPropertyChanged
    {
        #region 静态内容
        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            "Foreground",
            typeof(SolidColorBrush),
            typeof(ViewItem),
            new PropertyMetadata(AtRes.默认前景, OnForegroundChanged));

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background",
            typeof(SolidColorBrush),
            typeof(ViewItem),
            new PropertyMetadata(AtRes.TransparentBrush, OnBackgroundChanged));

        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register(
            "FontWeight",
            typeof(FontWeight),
            typeof(ViewItem),
            new PropertyMetadata(FontWeights.Normal, OnFontWeightChanged));

        public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register(
            "FontStyle",
            typeof(FontStyle),
            typeof(ViewItem),
            new PropertyMetadata(FontStyle.Normal, OnFontStyleChanged));

        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
            "FontSize",
            typeof(double),
            typeof(ViewItem),
            new PropertyMetadata(15d, OnFontSizeChanged));

        static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ViewItem)d).OnPropertyChanged("Foreground");
        }

        static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ViewItem)d).OnPropertyChanged("Background");
        }

        static void OnFontWeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ViewItem)d).OnPropertyChanged("FontWeight");
        }

        static void OnFontStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ViewItem)d).OnPropertyChanged("FontStyle");
        }

        static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ViewItem)d).OnPropertyChanged("FontSize");
        }
        #endregion

        protected Dictionary<string, object> _cacheUI;
        object _data;

        /// <summary>
        /// 获取视图项数据
        /// </summary>
        public object Data
        {
            get { return _data; }
            protected set
            {
                _data = value ?? throw new Exception("视图项数据不可为空！");
                Host.SetItemStyle(this);
                if (_data is Row row)
                    row.Changed += (s, e) => OnValueChanged();
                else if (_data is INotifyPropertyChanged pro)
                    pro.PropertyChanged += (s, e) => OnValueChanged();
            }
        }

        /// <summary>
        /// 获取Row数据源
        /// </summary>
        public Row Row
        {
            get { return Data as Row; }
        }

        /// <summary>
        /// 获取设置行前景画刷
        /// </summary>
        public SolidColorBrush Foreground
        {
            get { return (SolidColorBrush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        /// <summary>
        /// 获取设置行背景画刷
        /// </summary>
        public SolidColorBrush Background
        {
            get { return (SolidColorBrush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        /// <summary>
        /// 获取设置行字体粗细
        /// </summary>
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        /// <summary>
        /// 获取设置行文本样式
        /// </summary>
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        /// <summary>
        /// 获取设置行文本大小
        /// </summary>
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// 获取单元格值，外部绑定用
        /// </summary>
        /// <param name="p_colName">列名</param>
        /// <returns></returns>
        public object this[string p_colName]
        {
            get
            {
                object val = null;
                if (_data is Row dr && dr.Contains(p_colName))
                {
                    // 从Row取
                    val = dr[p_colName];
                }
                else
                {
                    // 对象属性
                    var pi = Data.GetType().GetProperty(p_colName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (pi != null)
                        val = pi.GetValue(Data);
                }
                return val;
            }
        }

        /// <summary>
        /// 值变化时的回调
        /// </summary>
        internal Action ValueChanged { get; set; }

        /// <summary>
        /// 宿主
        /// </summary>
        protected abstract IViewItemHost Host { get; }

        /// <summary>
        /// 获取单元格界面元素，提供给单元格容器ContentPresenter或Dot绑定用
        /// </summary>
        /// <param name="p_cell"></param>
        /// <returns></returns>
        internal object GetCellUI(ICellUI p_cell)
        {
            object elem = null;
            // 从缓存取
            if (_cacheUI != null && _cacheUI.TryGetValue(p_cell.ID, out elem))
                return elem;

            object val;
            MethodInfo mi = Host.GetViewExMethod(p_cell.ID);
            if (mi != null)
            {
                // 从外部扩展方法中获取
                // 扩展列方法原型： object ColName(ViewItem p_vr)
                object obj = mi.Invoke(null, new object[] { this });
                if (obj != null)
                {
                    // uno中的Image不是继承UIElement！
                    elem = obj as DependencyObject;
                    if (elem == null)
                        elem = new TextBlock { Style = AtRes.LvTextBlock, Text = obj.ToString(), };
                }
            }
            else if (p_cell.UIType == CellUIType.Default)
            {
                // 默认方式：根据数据类型生成可视元素
                if (Data is Row dr && dr.Contains(p_cell.ID))
                {
                    // 从Row取
                    elem = CreateCellUI(dr.Cells[p_cell.ID]);
                }
                else if (p_cell.ID == "#")
                {
                    // # 直接输出对象
                    elem = CreateObjectUI(Data);
                }
                else
                {
                    // 输出对象属性
                    var pi = Data.GetType().GetProperty(p_cell.ID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (pi != null && (val = pi.GetValue(Data)) != null)
                        elem = CreatePropertyUI(pi, val);
                }
            }
            else if ((val = this[p_cell.ID]) != null)
            {
                // 自定义方式：按设置的内容类型生成可视元素
                switch (p_cell.UIType)
                {
                    case CellUIType.Icon:
                        elem = CreateIcon(val);
                        break;
                    case CellUIType.CheckBox:
                        elem = CreateCheckBox(val);
                        break;
                    case CellUIType.Image:
                        elem = CreateImage(val);
                        break;
                }
            }

            if (_cacheUI != null)
                _cacheUI[p_cell.ID] = elem;
            return elem;
        }

        /// <summary>
        /// 值变化
        /// </summary>
        void OnValueChanged()
        {
            // 清除缓存，再次绑定时重新生成
            if (_cacheUI != null)
                _cacheUI.Clear();
            // 重新设置行/项目样式
            Host.SetItemStyle(this);
            ValueChanged?.Invoke();
        }

        #region 默认UI
        /// <summary>
        /// 根据Cell创建UI
        /// </summary>
        /// <param name="p_dc"></param>
        /// <returns></returns>
        static UIElement CreateCellUI(Cell p_dc)
        {
            TextBlock tb;
            if (p_dc.Type == typeof(string))
            {
                tb = new TextBlock
                {
                    Style = AtRes.LvTextBlock,
                    Text = p_dc.GetVal<string>(),
                };
                tb.IsTextTrimmedChanged += OnIsTextTrimmedChanged;
                return tb;
            }

            if (p_dc.Type == typeof(int) || p_dc.Type == typeof(long) || p_dc.Type == typeof(short))
            {
                tb = new TextBlock
                {
                    Style = AtRes.LvTextBlock,
                    Text = p_dc.GetVal<string>(),
                    TextAlignment = TextAlignment.Right,
                };
                return tb;
            }

            if (p_dc.Type == typeof(double) || p_dc.Type == typeof(float))
            {
                tb = new TextBlock
                {
                    Style = AtRes.LvTextBlock,
                    Text = p_dc.GetVal<double>().ToString("n2"),
                    TextAlignment = TextAlignment.Right,
                };
                return tb;
            }

            if (p_dc.Type == typeof(bool))
            {
                // 字符模拟CheckBox
                tb = new TextBlock
                {
                    Style = AtRes.LvTextBlock,
                    Text = p_dc.GetVal<bool>() ? "\uE06A" : "\uE068",
                    FontFamily = AtRes.IconFont,
                    TextAlignment = TextAlignment.Center,
                };
                return tb;
            }

            if (p_dc.Type == typeof(DateTime))
            {
                tb = new TextBlock
                {
                    Style = AtRes.LvTextBlock,
                    Text = p_dc.GetVal<DateTime>().ToString("yyyy-MM-dd"),
                };
                return tb;
            }

            if (p_dc.Type == typeof(Icons))
            {
                // 图标
                tb = new TextBlock
                {
                    Style = AtRes.LvTextBlock,
                    Text = AtRes.GetIconChar(p_dc.GetVal<Icons>()),
                    FontFamily = AtRes.IconFont,
                    TextAlignment = TextAlignment.Center,
                };
                return tb;
            }

            if (p_dc.Type == typeof(Color))
            {
                var rc = new Rectangle
                {
                    Fill = new SolidColorBrush(p_dc.GetVal<Color>()),
                    IsHitTestVisible = false,
                };
                return rc;
            }

            if (p_dc.Type == typeof(SolidColorBrush))
            {
                var rc = new Rectangle
                {
                    Fill = p_dc.GetVal<SolidColorBrush>(),
                    IsHitTestVisible = false,
                };
                return rc;
            }

            tb = new TextBlock
            {
                Style = AtRes.LvTextBlock,
                Text = p_dc.GetVal<string>(),
            };
            return tb;
        }

        /// <summary>
        /// 根据PropertyInfo创建UI
        /// </summary>
        /// <param name="p_pi"></param>
        /// <param name="p_val"></param>
        /// <returns></returns>
        static UIElement CreatePropertyUI(PropertyInfo p_pi, object p_val)
        {
            TextBlock tb;
            if (p_pi.PropertyType == typeof(string))
            {
                tb = new TextBlock
                {
                    Style = AtRes.LvTextBlock,
                    Text = (string)p_val,
                };
                tb.IsTextTrimmedChanged += OnIsTextTrimmedChanged;
                return tb;
            }

            if (p_pi.PropertyType == typeof(int) || p_pi.PropertyType == typeof(long) || p_pi.PropertyType == typeof(short))
            {
                tb = new TextBlock
                {
                    Style = AtRes.LvTextBlock,
                    Text = p_val.ToString(),
                    TextAlignment = TextAlignment.Right,
                };
                return tb;
            }

            if (p_pi.PropertyType == typeof(double) || p_pi.PropertyType == typeof(float))
            {
                tb = new TextBlock
                {
                    Style = AtRes.LvTextBlock,
                    Text = ((double)p_val).ToString("n2"),
                    TextAlignment = TextAlignment.Right,
                };
                return tb;
            }

            if (p_pi.PropertyType == typeof(bool))
            {
                // 字符模拟CheckBox
                tb = new TextBlock
                {
                    Style = AtRes.LvTextBlock,
                    Text = (bool)p_val ? "\uE06A" : "\uE068",
                    FontFamily = AtRes.IconFont,
                    TextAlignment = TextAlignment.Center,
                };
                return tb;
            }

            if (p_pi.PropertyType == typeof(DateTime))
            {
                tb = new TextBlock
                {
                    Style = AtRes.LvTextBlock,
                    Text = ((DateTime)p_val).ToString("yyyy-MM-dd"),
                };
                return tb;
            }

            if (p_pi.PropertyType == typeof(Icons))
            {
                // 图标
                tb = new TextBlock
                {
                    Style = AtRes.LvTextBlock,
                    Text = AtRes.GetIconChar((Icons)p_val),
                    FontFamily = AtRes.IconFont,
                    TextAlignment = TextAlignment.Center,
                };
                return tb;
            }

            if (p_pi.PropertyType == typeof(Color))
            {
                var rc = new Rectangle
                {
                    Fill = new SolidColorBrush((Color)p_val),
                    IsHitTestVisible = false,
                };
                return rc;
            }

            if (p_pi.PropertyType == typeof(SolidColorBrush))
            {
                var rc = new Rectangle
                {
                    Fill = (SolidColorBrush)p_val,
                    IsHitTestVisible = false,
                };
                return rc;
            }

            tb = new TextBlock
            {
                Style = AtRes.LvTextBlock,
                Text = p_val.ToString(),
            };
            return tb;
        }

        /// <summary>
        /// # 时直接输出对象
        /// </summary>
        /// <param name="p_data"></param>
        /// <returns></returns>
        static UIElement CreateObjectUI(object p_data)
        {
            var tb = new TextBlock
            {
                Style = AtRes.LvTextBlock,
                Text = p_data.ToString(),
            };
            tb.IsTextTrimmedChanged += OnIsTextTrimmedChanged;
            return tb;
        }
        #endregion

        #region 自定义UI
        TextBlock CreateIcon(object p_val)
        {
            var tb = new TextBlock
            {
                Style = AtRes.LvTextBlock,
                FontFamily = AtRes.IconFont,
                TextAlignment = TextAlignment.Center,
            };

            if (p_val is int)
                tb.Text = AtRes.GetIconChar((Icons)p_val);
            else
                tb.Text = AtRes.ParseIconChar(p_val.ToString());
            return tb;
        }

        TextBlock CreateCheckBox(object p_val)
        {
            // 字符模拟CheckBox
            var tb = new TextBlock
            {
                Style = AtRes.LvTextBlock,
                FontFamily = AtRes.IconFont,
                TextAlignment = TextAlignment.Center,
            };

            bool b;
            if (p_val is bool)
            {
                b = (bool)p_val;
            }
            else
            {
                string temp = p_val.ToString().ToLower();
                b = (temp == "1" || temp == "true");
            }
            tb.Text = b ? "\uE06A" : "\uE068";
            return tb;
        }

        Image CreateImage(object p_val)
        {
            Image img = new Image();
            if (p_val is string str)
            {
                img.Source = new BitmapImage(new Uri(str));
            }
            else if (p_val is Uri uri)
            {
                img.Source = new BitmapImage(uri);
            }
            else
            {
                throw new Exception($"无法显示{p_val.GetType().FullName}类型的图片！");
            }
            return img;
        }
        #endregion

        /// <summary>
        /// 提示被截断的长文本
        /// </summary>
        /// <param name="p_tb"></param>
        /// <param name="e"></param>
        static void OnIsTextTrimmedChanged(TextBlock p_tb, IsTextTrimmedChangedEventArgs e)
        {
            p_tb.IsTextTrimmedChanged -= OnIsTextTrimmedChanged;
            ToolTipService.SetToolTip(p_tb, p_tb.Text);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 触发属性变化事件
        /// </summary>
        /// <param name="propertyName">通知更改时的属性名称</param>
        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}