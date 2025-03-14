#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-03-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using System.Reflection;
#endregion

namespace Dt.Base
{
    public static class FvDesignKit
    {
        /// <summary>
        /// 加载单元格属性面板
        /// </summary>
        /// <param name="p_tgtCell">目标单元格</param>
        /// <param name="p_fv">属性面板</param>
        public static void LoadCellProps(object p_tgtCell, Fv p_fv)
        {
            if (p_tgtCell == null || p_fv == null)
                return;

            if (p_fv.Data == p_tgtCell)
                return;

            // 同类型直接切换数据源
            if (p_fv.Data != null && p_fv.Data.GetType() == p_tgtCell.GetType())
            {
                p_fv.Data = p_tgtCell;
                return;
            }

            var items = p_fv.Items;
            using (items.Defer())
            {
                items.Clear();

                if (p_tgtCell is FvCell fc)
                {
                    FvCell cell = new CTip();
                    cell.ID = "ID";
                    items.Add(cell);

                    cell = new CText();
                    cell.ID = "Title";
                    cell.Title = "标题";
                    items.Add(cell);

                    List<FvCell> bs = new List<FvCell>();
                    foreach (var info in GetCellProps(fc.GetType()))
                    {
                        cell = Fv.CreateCell(info.Info.PropertyType, info.Info.Name);
                        cell.Title = info.Title;
                        if (info.Info.PropertyType == typeof(bool))
                        {
                            cell.ShowTitle = false;
                            cell.ColSpan = 0.5;
                            bs.Add(cell);
                        }
                        else
                        {
                            cell.TitleWidth = 240;
                            items.Add(cell);
                        }
                    }

                    if (bs.Count > 0)
                    {
                        foreach (var item in bs)
                        {
                            items.Add(item);
                        }
                    }
                }
                else
                {
                    FvCell cell = new CText();
                    cell.ID = "Title";
                    cell.Title = "标题";
                    items.Add(cell);

                    cell = new CNum { IsInteger = true };
                    cell.ID = "RowSpan";
                    cell.Title = "占用行数";
                    items.Add(cell);

                    cell = new CNum();
                    cell.ID = "ColSpan";
                    cell.Title = "列宽占比0~1";
                    items.Add(cell);
                }
            }
            p_fv.Data = p_tgtCell;
        }

        public static IEnumerable<CellPropertyInfo> GetCellProps(Type p_type)
        {
            if (p_type == null)
                return null;
            
            if (!_cellProps.TryGetValue(p_type, out List<CellPropertyInfo> props))
            {
                props = new List<CellPropertyInfo>();
                var obj = Activator.CreateInstance(p_type);
                foreach (var info in p_type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    var attr = (CellParamAttribute)info.GetCustomAttribute(typeof(CellParamAttribute), false);
                    if (attr != null)
                        props.Add(new CellPropertyInfo(info, info.GetValue(obj), attr.Title));
                }
                _cellProps[p_type] = props;
            }
            return props.Concat(_baseProps);
        }
        
        static FvDesignKit()
        {
            var normal = new FvCell();
            foreach (var info in typeof(FvCell).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                var attr = (CellParamAttribute)info.GetCustomAttribute(typeof(CellParamAttribute), false);
                if (attr != null)
                    _baseProps.Add(new CellPropertyInfo(info, info.GetValue(normal), attr.Title));
            }
        }

        static readonly Dictionary<Type, List<CellPropertyInfo>> _cellProps = new Dictionary<Type, List<CellPropertyInfo>>();
        static readonly List<CellPropertyInfo> _baseProps = new List<CellPropertyInfo>();
    }

    public class CellPropertyInfo
    {
        public CellPropertyInfo(PropertyInfo p_info, object p_defVal, string p_title)
        {
            Info = p_info;
            DefaultValue = p_defVal;
            Title = p_title;
        }

        public PropertyInfo Info { get; }

        public object DefaultValue { get; }

        public string Title { get; }
    }
}