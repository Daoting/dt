#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-07-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Reflection;
#endregion

namespace Dt.Base.ListView
{
    public partial class CombinedFilterDlg : Dlg
    {
        FilterCfg _cfg;

        public CombinedFilterDlg()
        {
            InitializeComponent();
            IsPinned = true;
            _fv.Query += OnQuery;
        }

        public void ShowDlg(FilterCfg p_cfg)
        {
            _cfg = p_cfg;
            _cfg.Host.Refresh();
            _cfg.MyFilter = DoFilter;

            LoadCols();
            if (!Kit.IsPhoneUI)
            {
                MaxHeight = Kit.ViewHeight - 100;
                Width = 450;
            }
            Show();
        }

        bool DoFilter(object p_obj, string p_txt)
        {
            var data = _fv.Row;
            var row = p_obj as Row;

            foreach (var cell in _fv.IDCells)
            {
                // 过滤掉忽略的
                if (cell == null
                    || cell.QueryFlag == CompFlag.Ignore)
                    continue;

                var id = cell.ID;
                var c = data.Cells[id];
                var tp = c.Type;
                var filterVal = c.Val;

                // 忽略空字符串
                if (tp == typeof(string) && c.GetVal<string>() == "" && cell.QueryFlag != CompFlag.Null)
                    continue;

                // 忽略空时间
                if (tp == typeof(DateTime) && (DateTime)filterVal == default)
                    continue;

                // 可null值类型
                bool nullable = tp.IsGenericType && tp.GetGenericTypeDefinition() == typeof(Nullable<>);
                if (nullable)
                {
                    // 实际值类型
                    tp = tp.GetGenericArguments()[0];
                }

                // 单字段多条件查询，形如：xxx_min, xxx_max
                var name = id;
                int pos = id.LastIndexOf("_");
                if (pos > -1)
                    name = id.Substring(0, pos);

                // 数据源的列值
                object srcVal;
                if (row != null)
                {
                    srcVal = row[name];
                }
                else if (GetPropInfo(name) is PropertyInfo pi)
                {
                    srcVal = pi.GetValue(p_obj, null);
                }
                else
                {
                    continue;
                }

                // 列值null
                if (srcVal == null)
                {
                    if (cell.QueryFlag == CompFlag.Null)
                        continue;
                    return false;
                }

                // 过滤值null
                if (filterVal == null)
                {
                    // 忽略
                    if (cell.QueryFlag != CompFlag.Null)
                        continue;

                    // 空串 为 空值
                    if (tp == typeof(string) && (string)srcVal == "")
                        continue;
                    return false;
                }

                // srcVal filterVal 都不为null
                switch (cell.QueryFlag)
                {
                    case CompFlag.Equal: // 等于
                        if (!Convert.ChangeType(srcVal, tp).Equals(Convert.ChangeType(filterVal, tp)))
                            return false;
                        break;

                    case CompFlag.Unequal: // 不等于
                        if (Convert.ChangeType(srcVal, tp).Equals(Convert.ChangeType(filterVal, tp)))
                            return false;
                        break;

                    case CompFlag.Less: // 小于
                        if (tp == typeof(int))
                        {
                            if ((int)srcVal >= (int)filterVal)
                                return false;
                        }
                        else if (tp == typeof(long))
                        {
                            if ((long)srcVal >= (long)filterVal)
                                return false;
                        }
                        else if (tp == typeof(double))
                        {
                            if ((double)srcVal >= (double)filterVal)
                                return false;
                        }
                        else if (tp == typeof(float))
                        {
                            if ((float)srcVal >= (float)filterVal)
                                return false;
                        }
                        else if (tp == typeof(DateTime))
                        {
                            if ((DateTime)srcVal >= (DateTime)filterVal)
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                        break;

                    case CompFlag.Ceil: // 小于等于
                        if (tp == typeof(int))
                        {
                            if ((int)srcVal > (int)filterVal)
                                return false;
                        }
                        else if (tp == typeof(long))
                        {
                            if ((long)srcVal > (long)filterVal)
                                return false;
                        }
                        else if (tp == typeof(double))
                        {
                            if ((double)srcVal > (double)filterVal)
                                return false;
                        }
                        else if (tp == typeof(float))
                        {
                            if ((float)srcVal > (float)filterVal)
                                return false;
                        }
                        else if (tp == typeof(DateTime))
                        {
                            if ((DateTime)srcVal > (DateTime)filterVal)
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                        break;

                    case CompFlag.Greater: // 大于
                        if (tp == typeof(int))
                        {
                            if ((int)srcVal <= (int)filterVal)
                                return false;
                        }
                        else if (tp == typeof(long))
                        {
                            if ((long)srcVal <= (long)filterVal)
                                return false;
                        }
                        else if (tp == typeof(double))
                        {
                            if ((double)srcVal <= (double)filterVal)
                                return false;
                        }
                        else if (tp == typeof(float))
                        {
                            if ((float)srcVal <= (float)filterVal)
                                return false;
                        }
                        else if (tp == typeof(DateTime))
                        {
                            if ((DateTime)srcVal <= (DateTime)filterVal)
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                        break;

                    case CompFlag.Floor: // 大于等于
                        if (tp == typeof(int))
                        {
                            if ((int)srcVal < (int)filterVal)
                                return false;
                        }
                        else if (tp == typeof(long))
                        {
                            if ((long)srcVal < (long)filterVal)
                                return false;
                        }
                        else if (tp == typeof(double))
                        {
                            if ((double)srcVal < (double)filterVal)
                                return false;
                        }
                        else if (tp == typeof(float))
                        {
                            if ((float)srcVal < (float)filterVal)
                                return false;
                        }
                        else if (tp == typeof(DateTime))
                        {
                            if ((DateTime)srcVal < (DateTime)filterVal)
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                        break;

                    case CompFlag.StartsWith:
                        if (srcVal is string str)
                        {
                            if (!str.StartsWith((string)filterVal))
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                        break;

                    case CompFlag.EndsWith:
                        if (srcVal is string src)
                        {
                            if (!src.EndsWith((string)filterVal))
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                        break;

                    case CompFlag.Contains:
                        if (srcVal is string srcStr)
                        {
                            if (srcStr.IndexOf((string)filterVal) == -1)
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                        break;

                    case CompFlag.Null:
                        return false;

                    default:
                        break;
                }
            }
            return true;
        }

        void LoadCols()
        {
            var fvData = new Row();
            object data = _cfg.FirstRow;
            if (data == null)
                return;

            Row row = data as Row;
            foreach (var r in _cfg.AllCols)
            {
                if (!r.Bool("ischecked"))
                    continue;

                Type tpOriginal = null;
                string id = r.Str("id");
                if (row != null)
                {
                    if (row.Contains(id))
                        tpOriginal = row.Cells[id].Type;
                }
                else
                {
                    var prop = data.GetType().GetProperty(id, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase);
                    if (prop != null)
                        tpOriginal = prop.PropertyType;
                }
                if (tpOriginal == null)
                    continue;

                // 字段可能为null
                Type tp = tpOriginal;
                if (tp.IsGenericType && tp.GetGenericTypeDefinition() == typeof(Nullable<>))
                    tp = tp.GetGenericArguments()[0];
                
                if (tp.IsEnum)
                {
                    // 枚举 CList
                    var ls = new CList
                    {
                        ID = id,
                        Title = r.Str("title"),
                        Query = QueryType.Editable,
                        QueryFlag = CompFlag.Equal,
                    };
                    _fv.Items.Add(ls);
                    fvData.Add(id, tpOriginal);
                    continue;
                }

                if (tp == typeof(bool))
                {
                    var b = new CBool
                    {
                        ID = id,
                        Title = r.Str("title"),
                        Query = QueryType.Editable,
                        QueryFlag = CompFlag.Equal,
                    };
                    _fv.Items.Add(b);
                    fvData.Add(id, tpOriginal);
                }
                else if (tp == typeof(int) || tp == typeof(long))
                {
                    var bar = new CBar
                    {
                        Title = r.Str("title"),
                    };
                    _fv.Items.Add(bar);

                    var num = new CNum
                    {
                        ID = id + "_min",
                        IsInteger = true,
                        Query = QueryType.Editable,
                        QueryFlag = CompFlag.Floor,
                        ShowTitle = false,
                        ColSpan = 0.5,
                    };
                    _fv.Items.Add(num);
                    fvData.Add(id + "_min", tpOriginal);

                    num = new CNum
                    {
                        ID = id + "_max",
                        IsInteger = true,
                        Query = QueryType.Editable,
                        QueryFlag = CompFlag.Ceil,
                        ShowTitle = false,
                        ColSpan = 0.5,
                    };
                    _fv.Items.Add(num);
                    fvData.Add(id + "_max", tpOriginal);
                }
                else if (tp == typeof(float) || tp == typeof(double))
                {
                    var bar = new CBar
                    {
                        Title = r.Str("title"),
                    };
                    _fv.Items.Add(bar);

                    var num = new CNum
                    {
                        ID = id + "_min",
                        Query = QueryType.Editable,
                        QueryFlag = CompFlag.Floor,
                        ShowTitle = false,
                        ColSpan = 0.5,
                    };
                    _fv.Items.Add(num);
                    fvData.Add(id + "_min", tpOriginal);

                    num = new CNum
                    {
                        ID = id + "_max",
                        Query = QueryType.Editable,
                        QueryFlag = CompFlag.Ceil,
                        ShowTitle = false,
                        ColSpan = 0.5,
                    };
                    _fv.Items.Add(num);
                    fvData.Add(id + "_max", tpOriginal);
                }
                else if (tp == typeof(DateTime))
                {
                    var bar = new CBar
                    {
                        Title = r.Str("title"),
                    };
                    _fv.Items.Add(bar);

                    var num = new CDate
                    {
                        ID = id + "_min",
                        Query = QueryType.Editable,
                        QueryFlag = CompFlag.Floor,
                        ShowTitle = false,
                        ColSpan = 0.5,
                    };
                    _fv.Items.Add(num);
                    fvData.Add(id + "_min", tpOriginal);

                    num = new CDate
                    {
                        ID = id + "_max",
                        Query = QueryType.Editable,
                        QueryFlag = CompFlag.Ceil,
                        ShowTitle = false,
                        ColSpan = 0.5,
                    };
                    _fv.Items.Add(num);
                    fvData.Add(id + "_max", tpOriginal);
                }
                else
                {
                    var txt = new CText
                    {
                        ID = id,
                        Title = r.Str("title"),
                        Query = QueryType.Editable,
                        QueryFlag = CompFlag.Contains,
                    };
                    _fv.Items.Add(txt);
                    fvData.Add(id, tpOriginal);
                }
            }
            _fv.Data = fvData;
        }

        PropertyInfo GetPropInfo(string p_name)
        {
            return (from pi in _cfg.LastCols.OfType<PropertyInfo>()
                    where pi.Name.Equals(p_name, StringComparison.OrdinalIgnoreCase)
                    select pi).FirstOrDefault();
        }

        void OnQuery(QueryClause clause)
        {
            _cfg.Host.Refresh();
        }

        protected override void OnClosed(bool p_result)
        {
            _cfg.MyFilter = null;
        }
    }
}