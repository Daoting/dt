#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System.ComponentModel;
using System.Reflection;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 数据源相关
    /// </summary>
    public partial class FvCell : DtControl, IFvCell
    {
        #region 静态内容
        public static readonly DependencyProperty ValBindingProperty = DependencyProperty.Register(
            "ValBinding",
            typeof(Binding),
            typeof(FvCell),
            new PropertyMetadata(new Binding(), OnValBindingChanged));

        static void OnValBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FvCell cell = (FvCell)d;
            if (cell._isLoaded)
            {
                Binding bind = (Binding)e.NewValue;
                if (bind != null && bind.Source != null)
                {
                    if (cell._panel != null && cell._panel.Child != null)
                        cell._panel.Child.ClearValue(VisibilityProperty);
                    cell.SetValBinding();
                }
                else if (cell._panel != null && cell._panel.Child != null)
                {
                    // 未设置数据源时隐藏编辑器
                    cell._panel.Child.Visibility = Visibility.Collapsed;
                }
            }
        }
        #endregion

        /// <summary>
        /// 获取设置格的值
        /// </summary>
        public object Val
        {
            get
            {
                if (ValBinding.Source is ICell cell)
                    return cell.Val;
                return null;
            }
            set
            {
                if (ValBinding.Source is ICell cell)
                    cell.Val = value;
            }
        }

        /// <summary>
        /// 获取设置格的值绑定，内部绑定用，始终非null，ConverterParameter保存了绑定的数据源属性类型
        /// </summary>
        internal Binding ValBinding
        {
            get { return (Binding)GetValue(ValBindingProperty); }
            set { SetValue(ValBindingProperty, value); }
        }

        /// <summary>
        /// 回滚数据
        /// </summary>
        public void RejectChanges()
        {
            Cell dc = ValBinding.Source as Cell;
            if (dc != null)
                dc.RejectChanges();
        }

        /// <summary>
        /// 切换数据源，有ID
        /// </summary>
        /// <param name="p_data"></param>
        internal void OnDataChanged(object p_data)
        {
            Binding oldBind = ValBinding;
            if (oldBind.Source is ICell cell)
            {
                // 不需回滚，有时需要编辑结果！
                //cell.RejectChanges();
                cell.PropertyChanged -= OnDataPropertyChanged;
                cell.Message -= OnCellMessage;
            }

            // 空数据源
            if (p_data == null)
            {
                ClearValue(ValBindingProperty);
                // 修改状态背景色
                _panel?.ToggleIsChanged(false);
                return;
            }

            // Row数据源
            if (p_data is Row row)
            {
                // 包含ID列
                if (row.Contains(ID))
                {
                    var c = row.Cells[ID];
                    c.PropertyChanged += OnDataPropertyChanged;
                    c.Message += OnCellMessage;

                    // 设置新绑定，只设置Source引起immutable异常！
                    ValBinding = new FvCellBind(this, p_data)
                    {
                        Path = new PropertyPath("Val"),
                        Mode = BindingMode.TwoWay,
                        Source = c,
                        ConverterParameter = c.Type
                    };

                    // 修改状态背景色
                    _panel?.ToggleIsChanged(c.IsChanged);
                    return;
                }

                var pi = p_data.GetType().GetProperty(ID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (pi != null)
                {
                    // ID为Row的属性，OneTime且只读，只为同步显示用，无法保存
                    ValBinding = new FvCellBind(this, p_data)
                    {
                        Path = new PropertyPath(pi.Name),
                        Mode = BindingMode.OneTime,
                        Source = p_data,
                        ConverterParameter = pi.PropertyType
                    };
                    IsReadOnly = true;
                }
                else
                {
                    // ID既不是列，也不是属性，不绑定
                    ClearValue(ValBindingProperty);
                    Kit.Debug($"【{ID}】在数据源中既不是列，也不是属性，无法绑定！");
                }
                return;
            }

            // ID为普通对象的属性 或 以.分隔的多级子属性(如 Prop1.ChildProp.XXX)，解析绑定路径
            Binding bind = ParseBinding(p_data);
            if (bind == null)
            {
                // 绑定属性不存在
                ClearValue(ValBindingProperty);
                return;
            }

            // 控制只读状态
            if (bind.Mode == BindingMode.TwoWay)
                ClearValue(IsReadOnlyProperty);
            else
                IsReadOnly = true;

            ((PropertyView)bind.Source).PropertyChanged += OnDataPropertyChanged;
            ValBinding = bind;
        }

        /// <summary>
        /// 附加值改变时的处理方法，提供外部自定义显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ICell cell = (ICell)sender;
            if (e.PropertyName == "Val")
            {
                OnValChanged();
                if (!AutoCookie || string.IsNullOrEmpty(Owner.Name))
                    return;

                // 记录单元格最近一次编辑值
                Kit.RunAsync(async () =>
                {
                    string id = string.Format("{0}+{1}+{2}", BaseUri.AbsolutePath, Owner.Name, ID);
                    object val = cell.Val;
                    if (val != null && !string.IsNullOrEmpty(val.ToString()))
                    {
                        var clv = await CellLastValX.GetByID(id);
                        if (clv != null)
                        {
                            clv.Val = val.ToString();
                            await clv.Save(false);
                        }
                        else
                        {
                            await new CellLastValX(id, val.ToString()).Save(false);
                        }
                    }
                    else
                    {
                        // 删除旧记录
                        await CellLastValX.DelByID(id);
                    }
                });
            }
            else if (e.PropertyName == "IsChanged")
            {
                if (_panel != null)
                    _panel.ToggleIsChanged(cell.IsChanged);
            }
        }

        void OnCellMessage(object sender, CellMessageArgs e)
        {
            if (e.IsWarning)
            {
                Warn(e.Message);
            }
            else
            {
                Msg(e.Message);
            }
        }

        /// <summary>
        /// 解析目标路径
        /// </summary>
        /// <param name="p_data"></param>
        /// <returns></returns>
        Binding ParseBinding(object p_data)
        {
            object tgt = null;
            PropertyInfo pi = null;
            string[] arr = ID.Split('.');
            if (arr.Length > 1)
            {
                var type = p_data.GetType();
                for (int i = 0; i < arr.Length; i++)
                {
                    pi = type.GetProperty(arr[i], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (pi != null)
                    {
                        if (i == arr.Length - 1)
                            break;

                        type = pi.PropertyType;
                        if (i == 0)
                            tgt = pi.GetValue(p_data);
                        else if (tgt != null)
                            tgt = pi.GetValue(tgt);
                        else
                            return null;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                pi = p_data.GetType().GetProperty(ID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (pi != null)
                    tgt = p_data;
            }

            if (tgt != null)
            {
                return new FvCellBind(this, p_data)
                {
                    Path = new PropertyPath("Val"),
                    Mode = pi.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay,
                    Source = new PropertyView(Owner.DataView, pi, tgt),
                    ConverterParameter = pi.PropertyType
                };
            }
            return null;
        }

        /// <summary>
        /// 触发列值修改后事件
        /// </summary>
        void OnValChanged()
        {
            if (Changed != null)
                Changed.Invoke(this, Val);
        }
    }
}