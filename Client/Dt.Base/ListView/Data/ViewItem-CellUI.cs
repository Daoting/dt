#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.ListView;
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
        internal UIElement GetCellUI(Dot p_dot)
        {
            UIElement elem = null;
            List<MethodInfo> methods;

            if (!string.IsNullOrEmpty(p_dot.Call)
                && (methods = GetAllCellUIMethods(p_dot.Call)).Count > 0)
            {
                // 自定义单元格UI，支持多个方法顺序调用，方法原型：static void Fun(Env e)
                var args = new Env(this, p_dot);
                methods.ForEach((mi) => mi.Invoke(null, new object[] { args }));

                // 初次未触发Set事件，手动调用
                args.InitSet();

                // 可能未创建UI，null时后续会创建默认UI，如：只设置背景色
                elem = args.UI;
            }

            if (elem == null)
                elem = CreateDefaultUI(p_dot);
            return elem;
        }

        #region 默认UI
        internal UIElement CreateDefaultUI(Dot p_dot)
        {
            UIElement elem = null;

            // 默认方式：根据数据类型生成可视元素
            if (_data is Row dr && dr.Contains(p_dot.ID))
            {
                // 从Row取
                elem = CreateCellUI(dr.Cells[p_dot.ID].Type, p_dot);
            }
            else if (p_dot.ID == "#")
            {
                // # 直接输出对象
                var tb = CreateTrimmedTextBlock();
                tb.SetBinding(TextBlock.TextProperty, new LvBind(p_dot, (c) =>
                {
                    var str = c.Data.ToString();
                    c.Dot.ToggleVisible(str == "");
                    return str;
                }));
                elem = tb;
            }
            else
            {
                // 输出对象属性
                var pi = _data.GetType().GetProperty(p_dot.ID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (pi != null)
                {
                    elem = CreateCellUI(pi.PropertyType, p_dot);
                }
            }
            return elem;
        }

        /// <summary>
        /// 根据数据类型创建UI
        /// </summary>
        /// <param name="p_tp"></param>
        /// <param name="p_dot"></param>
        /// <returns></returns>
        static UIElement CreateCellUI(Type p_tp, Dot p_dot)
        {
            if (p_tp == typeof(string))
                return CreateBindTextBlock(p_dot);

            if (p_tp == typeof(int) || p_tp == typeof(long) || p_tp == typeof(short))
            {
                var tb = new TextBlock
                {
                    Style = Res.LvTextBlock,
                    TextAlignment = TextAlignment.Right,
                };
                tb.SetBinding(TextBlock.TextProperty, new LvBind(p_dot, (c) =>
                {
                    var str = c.Str;
                    c.Dot.ToggleVisible(str == "");
                    return str;
                }));
                return tb;
            }

            if (p_tp == typeof(double) || p_tp == typeof(float))
            {
                var tb = new TextBlock
                {
                    Style = Res.LvTextBlock,
                    TextAlignment = TextAlignment.Right,
                };
                tb.SetBinding(TextBlock.TextProperty, new LvBind(p_dot, (c) => c.Double.ToString(string.IsNullOrEmpty(c.Format) ? "n2" : c.Format)));
                return tb;
            }

            if (p_tp == typeof(bool))
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

            if (p_tp == typeof(DateTime))
            {
                var tb = new TextBlock { Style = Res.LvTextBlock };
                tb.SetBinding(TextBlock.TextProperty, new LvBind(p_dot, (c) => c.Date.ToString(string.IsNullOrEmpty(c.Format) ? "yyyy-MM-dd" : c.Format)));
                return tb;
            }

            if (p_tp == typeof(Icons))
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
                    c.Dot.ToggleVisible(string.IsNullOrEmpty(txt));
                    return string.IsNullOrEmpty(txt) ? "" : txt;
                }));
                return tb;
            }

            if (p_tp == typeof(Color))
            {
                var rc = new Rectangle { IsHitTestVisible = false };
                rc.SetBinding(Rectangle.FillProperty, new LvBind(p_dot, (c) => c.GetVal<Color>(c.ID)));
                return rc;
            }

            if (p_tp == typeof(SolidColorBrush))
            {
                var rc = new Rectangle { IsHitTestVisible = false };
                rc.SetBinding(Rectangle.FillProperty, new LvBind(p_dot, (c) => c.GetVal<SolidColorBrush>(c.ID)));
                return rc;
            }

            return CreateBindTextBlock(p_dot);
        }

        static TextBlock CreateBindTextBlock(Dot p_dot)
        {
            var tb = CreateTrimmedTextBlock();
            tb.SetBinding(TextBlock.TextProperty, new LvBind(p_dot, (c) =>
            {
                var str = c.Str;
                c.Dot.ToggleVisible(str == "");
                return str;
            }));
            return tb;
        }

        static TextBlock CreateTrimmedTextBlock()
        {
            var tb = new TextBlock { Style = Res.LvTextBlock };
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