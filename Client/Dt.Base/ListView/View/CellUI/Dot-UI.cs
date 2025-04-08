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
    public partial class Dot
    {
        #region 创建Content
        /// <summary>
        /// 获取单元格界面元素，提供给Dot.Content用
        /// </summary>
        /// <returns></returns>
        internal UIElement GetCellUI()
        {
            UIElement elem = null;
            List<MethodInfo> methods;

            if (!string.IsNullOrEmpty(Call)
                && (methods = GetAllCellUIMethods(Call)).Count > 0)
            {
                // 自定义单元格UI，支持多个方法顺序调用，方法原型：static void Fun(Env e)
                var args = new Env(this);
                methods.ForEach((mi) => mi.Invoke(null, new object[] { args }));

                // 未创建UI时使用默认UI，如：只设置背景色
                if (args.UI == null)
                {
                    // 内部将默认UI的set回调附加到事件上！
                    args.UI = args.CreateDefaultUI();
                }
                elem = args.UI;

                // DataContext切换时设置可视元素属性值的回调方法
                _set = args.InternalSet;
            }
            else
            {
                elem = CreateDefaultUI();
            }
            return elem;
        }

        internal Action<CallArgs> SetCallback => _set;
        #endregion

        #region 默认UI
        internal UIElement CreateDefaultUI()
        {
            // ID为null或空对应数据对象本身，直接输出对象
            if (string.IsNullOrEmpty(ID))
            {
                var tb = CreateTrimmedTextBlock();
                _set = c =>
                {
                    var str = c.Data?.ToString();
                    tb.Text = str;
                    c.Dot.ToggleVisible(string.IsNullOrEmpty(str));
                };
                return tb;
            }

            // 根据数据类型生成可视元素
            Type tp = null;
            if (_data is Row dr && dr.Contains(ID))
            {
                // 从Row取
                tp = dr.Cells[ID].Type;
                // 界面元素直接返回
                if (tp.IsSubclassOf(typeof(UIElement)))
                    return (UIElement)dr[ID];
            }
            else
            {
                // 输出对象属性
                var pi = _data.GetType().GetProperty(ID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (pi != null)
                {
                    tp = pi.PropertyType;
                    // 界面元素直接返回
                    if (tp.IsSubclassOf(typeof(UIElement)))
                        return (UIElement)pi.GetValue(_data);
                }
            }

            if (tp == null)
                return null;

            if (tp == typeof(string))
                return CreateDefaultBlock();

            // 支持可空类型
            if (tp.IsGenericType && tp.GetGenericTypeDefinition() == typeof(Nullable<>))
                tp = tp.GetGenericArguments()[0];

            if (tp == typeof(int) || tp == typeof(long) || tp == typeof(short))
                return CreateIntBlock();

            if (tp == typeof(double) || tp == typeof(float))
                return CreateDoubleBlock();

            if (tp == typeof(bool))
                return CreateBoolBlock();

            if (tp == typeof(DateTime))
                return CreateDateTimeBlock();

            if (tp == typeof(Icons))
                return CreateIcons();

            if (tp == typeof(Color))
                return CreateColorRect();

            if (tp == typeof(SolidColorBrush))
                return CreateBrushRect();

            return CreateDefaultBlock();
        }

        TextBlock CreateDefaultBlock()
        {
            var tb = CreateTrimmedTextBlock();
            _set = c =>
            {
                var str = c.Str;
                tb.Text = str;
                c.Dot.ToggleVisible(str == "");
            };
            return tb;
        }

        TextBlock CreateIntBlock()
        {
            var tb = new TextBlock
            {
                Style = Res.LvTextBlock,
                TextAlignment = TextAlignment.Right,
            };
            _set = c =>
            {
                var str = c.Str;
                tb.Text = str;
                c.Dot.ToggleVisible(str == "");
            };
            return tb;
        }

        TextBlock CreateDoubleBlock()
        {
            var tb = new TextBlock
            {
                Style = Res.LvTextBlock,
                TextAlignment = TextAlignment.Right,
            };
            _set = c => tb.Text = c.Double.ToString(string.IsNullOrEmpty(c.Format) ? "n2" : c.Format);
            return tb;
        }

        TextBlock CreateBoolBlock()
        {
            // 字符模拟CheckBox
            var tb = new TextBlock
            {
                Style = Res.LvTextBlock,
                FontFamily = Res.IconFont,
                TextAlignment = TextAlignment.Center,
            };
            _set = c => tb.Text = c.Bool ? "\uE059" : "\uE057";
            return tb;
        }

        TextBlock CreateDateTimeBlock()
        {
            var tb = new TextBlock { Style = Res.LvTextBlock };
            _set = c =>
            {
                string str;
                // null或DateTime.MinValue时无显示内容
                if (c.Date == default)
                    str = "";
                else
                    str = c.Date.ToString(string.IsNullOrEmpty(c.Format) ? "yyyy-MM-dd" : c.Format);

                tb.Text = str;
                c.Dot.ToggleVisible(str == "");
            };
            return tb;
        }

        TextBlock CreateIcons()
        {
            // 图标
            var tb = new TextBlock
            {
                Style = Res.LvTextBlock,
                FontFamily = Res.IconFont,
                TextAlignment = TextAlignment.Center,
            };
            _set = c =>
            {
                var txt = Res.GetIconChar(c.GetVal<Icons>(c.ID));
                tb.Text = string.IsNullOrEmpty(txt) ? "" : txt;
                c.Dot.ToggleVisible(tb.Text == "");
            };
            return tb;
        }

        Rectangle CreateColorRect()
        {
            var rc = new Rectangle { IsHitTestVisible = false };
            _set = c => rc.Fill = new SolidColorBrush(c.GetVal<Color>(c.ID));
            return rc;
        }

        Rectangle CreateBrushRect()
        {
            var rc = new Rectangle { IsHitTestVisible = false };
            _set = c => rc.Fill = c.GetVal<SolidColorBrush>(c.ID);
            return rc;
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
        internal static List<MethodInfo> GetAllCellUIMethods(string p_method)
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

            mi = Kit.GetMethodByAlias(typeof(LvCallAttribute), arr[0], arr[1], BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
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