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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Text;
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
            new PropertyMetadata(Res.默认前景, OnForegroundChanged));

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background",
            typeof(SolidColorBrush),
            typeof(ViewItem),
            new PropertyMetadata(Res.TransparentBrush, OnBackgroundChanged));

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
            new PropertyMetadata(16d, OnFontSizeChanged));

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

        #region 成员变量
        readonly object _data;
        bool _isInit;
        #endregion

        public ViewItem(object p_data)
        {
            _data = p_data ?? throw new Exception("视图项数据不可为空！");
        }

        /// <summary>
        /// 获取视图项数据
        /// </summary>
        public object Data
        {
            get { return _data; }
        }

        /// <summary>
        /// 获取Row数据源
        /// </summary>
        public Row Row
        {
            get { return _data as Row; }
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
                    var pi = _data.GetType().GetProperty(p_colName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (pi != null)
                        val = pi.GetValue(_data);
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
        /// 获取单元格界面元素，提供给Dot.Content用
        /// </summary>
        /// <param name="p_dot"></param>
        /// <returns></returns>
        internal object GetCellUI(Dot p_dot)
        {
            object elem;

            // 从缓存取
            if (GetCacheUI(p_dot.ID, out elem))
                return elem;

            List<MethodInfo> lsMethods;
            if (!string.IsNullOrEmpty(p_dot.Call)
                && (lsMethods = GetAllCellUIMethods(p_dot.Call)).Count > 0)
            {
                // 自定义单元格UI，支持多个方法顺序调用。方法原型：static void Fun(Env e)
                var args = new Env(this, p_dot);
                lsMethods.ForEach((mi) => mi.Invoke(null, new object[] { args }));

                if (args.UI == null && args.Root == null)
                {
                    // 未创建UI也未设置样式，则无子元素时返回null
                    elem = null;
                }
                else if (args.UI == null)
                {
                    // 已设置样式但未创建UI，则创建默认UI，如：只设置背景色
                    var def = CreateDefaultUI(p_dot);
                    if (def != null)
                        args.Root.Content = def;
                    elem = args.Root;
                }
                else if (args.Root == null)
                {
                    // 只创建UI，未设置样式
                    elem = args.UI;
                }
                else
                {
                    // 已设置样式且已创建UI
                    args.Root.Content = args.UI;
                    elem = args.Root;
                }
            }
            else
            {
                // 默认方式：根据数据类型生成可视元素
                elem = CreateDefaultUI(p_dot);
            }

            AddCacheUI(p_dot.ID, elem);
            return elem;
        }

        #region 默认UI
        internal UIElement CreateDefaultUI(Dot p_dot)
        {
            // 默认方式：根据数据类型生成可视元素
            if (_data is Row dr && dr.Contains(p_dot.ID))
            {
                // 从Row取
                return CreateCellUI(dr.Cells[p_dot.ID], p_dot);
            }

            if (p_dot.ID == "#")
            {
                // # 直接输出对象
                return CreateObjectUI(_data);
            }

            // 输出对象属性
            object val;
            var pi = _data.GetType().GetProperty(p_dot.ID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (pi != null && (val = pi.GetValue(_data)) != null)
                return CreatePropertyUI(pi, val, p_dot);

            return null;
        }

        /// <summary>
        /// 根据Cell创建UI
        /// </summary>
        /// <param name="p_dc"></param>
        /// <param name="p_dot"></param>
        /// <returns></returns>
        static UIElement CreateCellUI(Cell p_dc, Dot p_dot)
        {
            if (p_dc.Type == typeof(string))
            {
                string txt = p_dc.GetVal<string>();
                if (string.IsNullOrEmpty(txt))
                    return null;

                var tb = new TextBlock
                {
                    Style = Res.LvTextBlock,
                    Text = txt,
                };
#if WIN
                tb.IsTextTrimmedChanged += OnIsTextTrimmedChanged;
#endif
                return tb;
            }

            if (p_dc.Type == typeof(int) || p_dc.Type == typeof(long) || p_dc.Type == typeof(short))
            {
                string txt = p_dc.GetVal<string>();
                if (string.IsNullOrEmpty(txt))
                    return null;

                return new TextBlock
                {
                    Style = Res.LvTextBlock,
                    Text = txt,
                    TextAlignment = TextAlignment.Right,
                };
            }

            if (p_dc.Type == typeof(double) || p_dc.Type == typeof(float))
            {
                return new TextBlock
                {
                    Style = Res.LvTextBlock,
                    Text = p_dc.GetVal<double>().ToString(string.IsNullOrEmpty(p_dot.Format) ? "n2" : p_dot.Format),
                    TextAlignment = TextAlignment.Right,
                };
            }

            if (p_dc.Type == typeof(bool))
            {
                // 字符模拟CheckBox
                return new TextBlock
                {
                    Style = Res.LvTextBlock,
                    Text = p_dc.GetVal<bool>() ? "\uE059" : "\uE057",
                    FontFamily = Res.IconFont,
                    TextAlignment = TextAlignment.Center,
                };
            }

            if (p_dc.Type == typeof(DateTime))
            {
                return new TextBlock
                {
                    Style = Res.LvTextBlock,
                    Text = p_dc.GetVal<DateTime>().ToString(string.IsNullOrEmpty(p_dot.Format) ? "yyyy-MM-dd" : p_dot.Format),
                };
            }

            if (p_dc.Type == typeof(Icons))
            {
                // 图标
                var txt = Res.GetIconChar(p_dc.GetVal<Icons>());
                if (string.IsNullOrEmpty(txt))
                    return null;

                return new TextBlock
                {
                    Style = Res.LvTextBlock,
                    Text = txt,
                    FontFamily = Res.IconFont,
                    TextAlignment = TextAlignment.Center,
                };
            }

            if (p_dc.Type == typeof(Color))
            {
                return new Rectangle
                {
                    Fill = new SolidColorBrush(p_dc.GetVal<Color>()),
                    IsHitTestVisible = false,
                };
            }

            if (p_dc.Type == typeof(SolidColorBrush))
            {
                return new Rectangle
                {
                    Fill = p_dc.GetVal<SolidColorBrush>(),
                    IsHitTestVisible = false,
                };
            }

            string val = p_dc.GetVal<string>();
            if (string.IsNullOrEmpty(val))
                return null;

            return new TextBlock
            {
                Style = Res.LvTextBlock,
                Text = val,
            };
        }

        /// <summary>
        /// 根据PropertyInfo创建UI
        /// </summary>
        /// <param name="p_pi"></param>
        /// <param name="p_val"></param>
        /// <param name="p_dot"></param>
        /// <returns></returns>
        static UIElement CreatePropertyUI(PropertyInfo p_pi, object p_val, Dot p_dot)
        {
            if (p_pi.PropertyType == typeof(string))
            {
                string txt = (string)p_val;
                if (string.IsNullOrEmpty(txt))
                    return null;

                var tb = new TextBlock
                {
                    Style = Res.LvTextBlock,
                    Text = txt,
                };
#if WIN
                tb.IsTextTrimmedChanged += OnIsTextTrimmedChanged;
#endif
                return tb;
            }

            if (p_pi.PropertyType == typeof(int) || p_pi.PropertyType == typeof(long) || p_pi.PropertyType == typeof(short))
            {
                string txt = p_val.ToString();
                if (string.IsNullOrEmpty(txt))
                    return null;

                return new TextBlock
                {
                    Style = Res.LvTextBlock,
                    Text = txt,
                    TextAlignment = TextAlignment.Right,
                };
            }

            if (p_pi.PropertyType == typeof(double) || p_pi.PropertyType == typeof(float))
            {
                return new TextBlock
                {
                    Style = Res.LvTextBlock,
                    Text = ((double)p_val).ToString(string.IsNullOrEmpty(p_dot.Format) ? "n2" : p_dot.Format),
                    TextAlignment = TextAlignment.Right,
                };
            }

            if (p_pi.PropertyType == typeof(bool))
            {
                // 字符模拟CheckBox
                return new TextBlock
                {
                    Style = Res.LvTextBlock,
                    Text = (bool)p_val ? "\uE059" : "\uE057",
                    FontFamily = Res.IconFont,
                    TextAlignment = TextAlignment.Center,
                };
            }

            if (p_pi.PropertyType == typeof(DateTime))
            {
                return new TextBlock
                {
                    Style = Res.LvTextBlock,
                    Text = ((DateTime)p_val).ToString(string.IsNullOrEmpty(p_dot.Format) ? "yyyy-MM-dd" : p_dot.Format),
                };
            }

            if (p_pi.PropertyType == typeof(Icons))
            {
                // 图标
                var txt = Res.GetIconChar((Icons)p_val);
                if (string.IsNullOrEmpty(txt))
                    return null;

                return new TextBlock
                {
                    Style = Res.LvTextBlock,
                    Text = txt,
                    FontFamily = Res.IconFont,
                    TextAlignment = TextAlignment.Center,
                };
            }

            if (p_pi.PropertyType == typeof(Color))
            {
                return new Rectangle
                {
                    Fill = new SolidColorBrush((Color)p_val),
                    IsHitTestVisible = false,
                };
            }

            if (p_pi.PropertyType == typeof(SolidColorBrush))
            {
                return new Rectangle
                {
                    Fill = (SolidColorBrush)p_val,
                    IsHitTestVisible = false,
                };
            }

            string val = p_val.ToString();
            if (string.IsNullOrEmpty(val))
                return null;

            return new TextBlock
            {
                Style = Res.LvTextBlock,
                Text = val,
            };
        }

        /// <summary>
        /// # 时直接输出对象
        /// </summary>
        /// <param name="p_data"></param>
        /// <returns></returns>
        static UIElement CreateObjectUI(object p_data)
        {
            string val = p_data.ToString();
            if (string.IsNullOrEmpty(val))
                return null;

            var tb = new TextBlock
            {
                Style = Res.LvTextBlock,
                Text = val,
            };
#if WIN
            tb.IsTextTrimmedChanged += OnIsTextTrimmedChanged;
#endif
            return tb;
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 初始化视图行，包括调用外部 CellEx.SetStyle 设置行样式、附加值变化事件、初始化缓存字典等
        /// </summary>
        internal void Init()
        {
            if (_isInit)
                return;

            _isInit = true;
            Host.SetItemStyle(this);
            if (_data is Row row)
                row.Changed += (s, e) => OnValueChanged();
            else if (_data is INotifyPropertyChanged pro)
                pro.PropertyChanged += (s, e) => OnValueChanged();
            OnInit();
        }

        /// <summary>
        /// 值变化
        /// </summary>
        void OnValueChanged()
        {
            // 清除缓存，再次绑定时重新生成
            ClearCacheUI();
            // 重新设置行/项目样式
            Host.SetItemStyle(this);
            ValueChanged?.Invoke();
        }

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
        #endregion

        #region 虚方法
        protected virtual void OnInit()
        {
        }

        protected virtual void AddCacheUI(string p_key, object p_ui)
        {
        }

        protected virtual bool GetCacheUI(string p_key, out object p_ui)
        {
            p_ui = null;
            return false;
        }

        protected virtual void ClearCacheUI()
        {
        }
        #endregion

        #region 自定义单元格UI
        static readonly Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_method">方法名，形如：Def.Icon,Def.小灰</param>
        /// <returns></returns>
        static List<MethodInfo> GetAllCellUIMethods(string p_method)
        {
            List<MethodInfo> ls = new List<MethodInfo>();

            // 逗号隔开多个方法
            var arrMethods = p_method.Split(',');
            foreach (var method in arrMethods)
            {
                var mi = GetMethod(method);
                if (mi != null)
                    ls.Add(mi);
            }
            return ls;
        }

        static MethodInfo GetMethod(string p_method)
        {
            if (_methods.TryGetValue(p_method, out var mi))
                return mi;

            var arr = p_method.Split('.');
            if (arr.Length != 2)
            {
                Log.Warning($"自定义单元格UI的方法名 {p_method} 不符合规范");
                return null;
            }

            mi = Kit.GetMethodByAlias(typeof(CellUIAttribute), arr[0], arr[1], BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
            if (mi != null)
            {
                var pars = mi.GetParameters();
                if (pars.Length == 1
                    && pars[0].ParameterType == typeof(Env)
                    && mi.ReturnType == typeof(void))
                {
                    _methods[p_method] = mi;
                    return mi;
                }
            }
            Log.Warning("未找到自定义单元格UI方法：" + p_method);
            return null;
        }
        #endregion

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