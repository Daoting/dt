#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System.Reflection;
using Windows.UI;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// CellUI 相关
    /// </summary>
    public abstract partial class ViewItem
    {
        /// <summary>
        /// 获取单元格界面元素，提供给Dot.Content用
        /// </summary>
        /// <param name="p_dot"></param>
        /// <returns></returns>
        internal CellUIItem GetCellUI(Dot p_dot)
        {
            // 从缓存取
            if (GetCacheUI(p_dot.ID, out var ci))
                return ci;

            List<MethodInfo> methods;
            if (!string.IsNullOrEmpty(p_dot.Call)
                && (methods = GetAllCellUIMethods(p_dot.Call)).Count > 0)
            {
                // 自定义单元格UI，支持多个方法顺序调用
                ci = GetCustomUI(p_dot, methods);
            }

            if (ci == null)
            {
                // 默认方式：根据数据类型生成可视元素
                ci = CreateDefaultUI(p_dot);
            }

            // 绑定方式无需缓存
            if (!ci.IsBinding)
                AddCacheUI(p_dot.ID, ci);
            return ci;
        }

        #region 自定义UI
        /// <summary>
        /// 自定义UI
        /// </summary>
        /// <param name="p_dot"></param>
        /// <param name="p_methods"></param>
        /// <returns></returns>
        CellUIItem GetCustomUI(Dot p_dot, List<MethodInfo> p_methods)
        {
            // 方法原型：static void Fun(Env e)
            var args = new Env(this, p_dot);
            p_methods.ForEach((mi) => mi.Invoke(null, new object[] { args }));

            if (args.UI == null)
            {
                // 已设置样式但未创建UI，则创建默认UI，如：只设置背景色
                args.UI = CreateDefaultUI(p_dot).UI;
            }

            return new CellUIItem(args.UI, args.IsBinding);
        }
        #endregion

        #region 默认UI
        internal CellUIItem CreateDefaultUI(Dot p_dot)
        {
            // 默认方式：根据数据类型生成可视元素
            if (_data is Row dr && dr.Contains(p_dot.ID))
            {
                // 从Row取，都采用绑定方式
                var ui = CreateCellUI(dr.Cells[p_dot.ID], p_dot);
                return new CellUIItem(ui, true);
            }

            if (p_dot.ID == "#")
            {
                // # 直接输出对象，不绑定
                var ui = CreateUnbindTextBlock(_data.ToString());
                return new CellUIItem(ui, false);
            }

            // 输出对象属性
            object val;
            var pi = _data.GetType().GetProperty(p_dot.ID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (pi != null && (val = pi.GetValue(_data)) != null)
            {
                var ui = CreatePropertyUI(pi, val, p_dot);
                return new CellUIItem(ui, false);
            }

            return new CellUIItem(null, false);
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
                return CreateBindTextBlock(p_dot);

            if (p_dc.Type == typeof(int) || p_dc.Type == typeof(long) || p_dc.Type == typeof(short))
            {
                var tb = new TextBlock
                {
                    Style = Res.LvTextBlock,
                    TextAlignment = TextAlignment.Right,
                };
                tb.SetBinding(TextBlock.TextProperty, new LvBind(p_dot, (c) => c.Str));
                return tb;
            }

            if (p_dc.Type == typeof(double) || p_dc.Type == typeof(float))
            {
                var tb = new TextBlock
                {
                    Style = Res.LvTextBlock,
                    TextAlignment = TextAlignment.Right,
                };
                tb.SetBinding(TextBlock.TextProperty, new LvBind(p_dot, (c) => c.Double.ToString(string.IsNullOrEmpty(c.Format) ? "n2" : c.Format)));
                return tb;
            }

            if (p_dc.Type == typeof(bool))
            {
                // 字符模拟CheckBox
                var tb = new TextBlock
                {
                    Style = Res.LvTextBlock,
                    FontFamily = Res.IconFont,
                    TextAlignment = TextAlignment.Center,
                };
                tb.SetBinding(TextBlock.TextProperty, new LvBind(p_dot, (c) => c.Bool ? "\uE059" : "\uE057"));
                return tb;
            }

            if (p_dc.Type == typeof(DateTime))
            {
                var tb = new TextBlock { Style = Res.LvTextBlock };
                tb.SetBinding(TextBlock.TextProperty, new LvBind(p_dot, (c) => c.Date.ToString(string.IsNullOrEmpty(c.Format) ? "yyyy-MM-dd" : c.Format)));
                return tb;
            }

            if (p_dc.Type == typeof(Icons))
            {
                // 图标
                var tb = new TextBlock
                {
                    Style = Res.LvTextBlock,
                    FontFamily = Res.IconFont,
                    TextAlignment = TextAlignment.Center,
                };
                tb.SetBinding(TextBlock.TextProperty, new LvBind(p_dot, (c) =>
                {
                    var txt = Res.GetIconChar(c.GetVal<Icons>(c.ID));
                    return string.IsNullOrEmpty(txt) ? "" : txt;
                }));
                return tb;
            }

            if (p_dc.Type == typeof(Color))
            {
                var rc = new Rectangle { IsHitTestVisible = false };
                rc.SetBinding(Rectangle.FillProperty, new LvBind(p_dot, (c) => c.GetVal<Color>(c.ID)));
                return rc;
            }

            if (p_dc.Type == typeof(SolidColorBrush))
            {
                var rc = new Rectangle { IsHitTestVisible = false };
                rc.SetBinding(Rectangle.FillProperty, new LvBind(p_dot, (c) => c.GetVal<SolidColorBrush>(c.ID)));
                return rc;
            }

            return CreateBindTextBlock(p_dot);
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

            return CreateUnbindTextBlock(p_val.ToString());
        }

        static TextBlock CreateBindTextBlock(Dot p_dot)
        {
            var tb = new TextBlock { Style = Res.LvTextBlock };
            tb.SetBinding(TextBlock.TextProperty, new LvBind(p_dot, (c) => c.Str));
#if WIN
            tb.IsTextTrimmedChanged += OnIsTextTrimmedChanged;
#endif
            return tb;
        }

        static TextBlock CreateUnbindTextBlock(string p_val)
        {
            if (string.IsNullOrEmpty(p_val))
                return null;

            var tb = new TextBlock
            {
                Style = Res.LvTextBlock,
                Text = p_val,
            };
#if WIN
            tb.IsTextTrimmedChanged += OnIsTextTrimmedChanged;
#endif
            return tb;
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

        #region 缓存
        Dictionary<string, CellUIItem> _cacheUI;

        void InitCache()
        {
            if (EnableCache)
                _cacheUI = new Dictionary<string, CellUIItem>(StringComparer.OrdinalIgnoreCase);
        }

        bool GetCacheUI(string p_key, out CellUIItem p_item)
        {
            if (_cacheUI != null && _cacheUI.TryGetValue(p_key, out p_item))
                return true;

            p_item = null;
            return false;
        }

        void AddCacheUI(string p_key, CellUIItem p_ui)
        {
            if (_cacheUI != null)
                _cacheUI[p_key] = p_ui;
        }

        /// <summary>
        /// 清除缓存，DataContext切换时重新生成
        /// </summary>
        void ClearCache()
        {
            _cacheUI?.Clear();
        }
        #endregion

        #region 自定义回调
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
    }
}