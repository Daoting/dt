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
using System.Reflection;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 数据源相关
    /// </summary>
    public partial class Fv
    {
        #region 静态内容
        public readonly static DependencyProperty DataProperty = DependencyProperty.Register(
            "Data",
            typeof(object),
            typeof(Fv),
            new PropertyMetadata(null, OnDataChanged));

        public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register(
            "IsDirty",
            typeof(bool),
            typeof(Fv),
            new PropertyMetadata(false, OnIsDirtyChanged));

        static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Fv fv = (Fv)d;
            // 未显示前不处理
            if (fv._isLoaded)
            {
                if (e.OldValue != null)
                {
                    // 移除旧数据事件
                    if (e.OldValue is Row row)
                        row.Changed -= fv.OnCellValueChanged;
                    else if (fv.DataView != null)
                        fv.DataView.Changed -= fv.OnPropertyValueChanged;
                }
                fv.OnDataChanged();
            }
        }

        static void OnIsDirtyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Fv)d).OnDirty();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置数据源，Row或普通对象
        /// </summary>
        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        /// <summary>
        /// 获取Row数据源
        /// </summary>
        public Row Row
        {
            get { return GetValue(DataProperty) as Row; }
        }

        /// <summary>
        /// 获取表单数据是否已修改
        /// </summary>
        public bool IsDirty
        {
            get { return (bool)GetValue(IsDirtyProperty); }
            set { SetValue(IsDirtyProperty, value); }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 名称列表中的格不允许为空，空时给出警告并返回true
        /// </summary>
        /// <param name="p_names"></param>
        /// <returns></returns>
        public bool ExistNull(params string[] p_names)
        {
            foreach (string name in p_names)
            {
                if (string.IsNullOrEmpty(name))
                    continue;

                var item = Items[name];
                if (item != null)
                {
                    object val = item.Val;
                    if (val == null || val.ToString() == "")
                    {
                        item.Warn("不可为空！");
                        GotoCell(name);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 提交自上次调用以来对该行进行的所有更改
        /// </summary>
        public void AcceptChanges()
        {
            if (Data is Row row)
                row.AcceptChanges();
            else if (DataView != null)
                DataView.AcceptChanges();
        }

        /// <summary>
        /// 回滚自该表加载以来或上次调用 AcceptChanges 以来对该行进行的所有更改
        /// </summary>
        public void RejectChanges()
        {
            if (Data is Row row)
                row.RejectChanges();
            else if (DataView != null)
                DataView.RejectChanges();
        }

        /// <summary>
        /// 是否丢弃所有的修改
        /// </summary>
        /// <returns>true 未修改或丢弃修改</returns>
        public async Task<bool> DiscardChanges()
        {
            if (Data is Row row)
            {
                if (row.IsChanged)
                {
                    if (!await Kit.Confirm("数据未保存，确认要丢弃所有修改吗？"))
                        return false;
                    row.RejectChanges();
                }
            }
            else if (DataView != null)
            {
                var dv = DataView;
                if (dv.IsChanged)
                {
                    if (!await Kit.Confirm("数据未保存，确认要丢弃所有修改吗？"))
                        return false;
                    dv.RejectChanges();
                }
            }
            return true;
        }

        /// <summary>
        /// 获取单元格cookie值，FvCell.AutoCookie为true时有效
        /// </summary>
        /// <param name="p_cellID"></param>
        /// <returns></returns>
        public async Task<string> GetCookie(string p_cellID)
        {
            FvCell cell;
            if (string.IsNullOrEmpty(Name)
                || (cell = this[p_cellID]) == null
                || !cell.AutoCookie)
                return null;

            string path = $"{BaseUri.AbsolutePath}+{Name}+{cell.ID}";
            var cl = await CellLastValX.GetByID(path);
            return cl == null || string.IsNullOrEmpty(cl.Val) ? "" : cl.Val;
        }
        #endregion

        #region 切换数据源
        /// <summary>
        /// 切换数据源，Row或普通对象
        /// </summary>
        void OnDataChanged()
        {
            object data = Data;
            Row row = data as Row;

            if (data == null)
            {
                ClearValue(IsDirtyProperty);
                ClearValue(DataViewProperty);
            }
            else
            {
                // Data.IsChanged <=> Fv.IsDirty
                Binding bind = new Binding { Path = new PropertyPath("IsChanged") };
                if (row != null)
                {
                    bind.Source = row;
                    row.Changed += OnCellValueChanged;
                }
                else
                {
                    DataView = new ObjectView();
                    bind.Source = DataView;
                    DataView.Changed += OnPropertyValueChanged;
                }
                SetBinding(IsDirtyProperty, bind);
            }

            if (AutoCreateCell)
            {
                // 根据数据源自动生成格
                using (Items.Defer())
                {
                    Items.Clear();
                    _panel.Children.Clear();
                    if (row != null)
                    {
                        foreach (var dc in row.Cells)
                        {
                            FvCell cell = CreateCell(dc.Type, dc.ID);
                            cell.Owner = this;
                            cell.OnDataChanged(data);
                            Items.Add(cell);
                        }
                    }
                    else if (data != null)
                    {
                        PropertyInfo[] pis = data.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                        foreach (var pi in pis)
                        {
                            FvCell cell = CreateCell(pi.PropertyType, pi.Name);
                            cell.Owner = this;
                            cell.Title = pi.Name;
                            cell.OnDataChanged(data);
                            Items.Add(cell);
                        }
                    }
                }
            }
            else
            {
                // 只处理含ID的格，其它格怕有影响未设置DataContext！
                foreach (var cell in IDCells)
                {
                    cell.OnDataChanged(data);
                }
            }

            if (!IsReadOnly
                && data != null
                && (AutoFocus || (data is Row r && r.IsAdded)))
            {
                // 跳过不适合自动设置焦点的类型
                foreach (var cell in _panel.Children.OfType<FvCell>())
                {
                    if (cell != null
                        && !_noAutoFocus.Contains(cell.GetType())
                        && cell.ReceiveFocus())
                        break;
                }
            }

            // 切换数据源事件
            OnFvDataChanged();
        }

        void OnCellValueChanged(object sender, Cell e)
        {
            OnValueChanged(e);
        }

        void OnPropertyValueChanged(object sender, PropertyView e)
        {
            OnValueChanged(e);
        }

        static List<Type> _noAutoFocus = new List<Type> { typeof(CList), typeof(CTree), typeof(CIcon), typeof(CColor), typeof(CFile), typeof(CImage) };
        #endregion
    }
}