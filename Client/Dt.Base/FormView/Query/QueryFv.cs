#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-01-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using System.Text;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 查询面板
    /// </summary>
    public partial class QueryFv : Fv
    {
        #region 静态内容
        public readonly static DependencyProperty IsAndProperty = DependencyProperty.Register(
            "IsAnd",
            typeof(bool),
            typeof(QueryFv),
            new PropertyMetadata(true));
        #endregion

        #region 成员变量
        private BaseCommand _cmdQuery;
        private BaseCommand _cmdReset;
        #endregion

        #region 事件
        /// <summary>
        /// 查询事件
        /// </summary>
        public event EventHandler<QueryClause> Query;
        #endregion

        /// <summary>
        /// 获取设置筛选条件之间是否为【与】操作，默认true，false时【或】操作
        /// </summary>
        public bool IsAnd
        {
            get { return (bool)GetValue(IsAndProperty); }
            set { SetValue(IsAndProperty, value); }
        }

        public void OnQuery()
        {
            // 未订阅事件，无需构造查询条件
            if (Query == null || Data is not Row row)
                return;

            // 生成sql查询的where后面的条件子句
            StringBuilder sw = new StringBuilder();
            // 加无用的条件，方便字符串处理
            sw.Append(IsAnd ? "true" : "false");

            Dict dt = new Dict();
            foreach (var cell in IDCells)
            {
                // 过滤掉忽略的
                if (cell.Query == QueryType.Disable
                    || cell.QueryFlag == CompFlag.Ignore)
                    continue;

                var id = cell.ID;
                var c = row.Cells[id];

                // 忽略空字符串、空时间
                if ((c.Type == typeof(string) && c.GetVal<string>() == "")
                    || (c.Type == typeof(DateTime) && (DateTime)c.Val == default))
                    continue;

                // 单字段多条件查询，形如：xxx_min, xxx_max
                var name = id;
                int pos = id.LastIndexOf("_");
                if (pos > -1)
                    name = id.Substring(0, pos);

                var val = c.Val;
                sw.Append(IsAnd ? " and (" : " or (");
                switch (cell.QueryFlag)
                {
                    case CompFlag.Equal://相等 equal
                        dt[id] = val;
                        sw.Append(name);
                        sw.Append(" = @");
                        sw.Append(id);
                        break;
                    case CompFlag.Unequal://不等 unequal
                        dt[id] = val;
                        sw.Append(name);
                        sw.Append(" != @");
                        sw.Append(id);
                        break;
                    case CompFlag.Less://小于 less
                        dt[id] = val;
                        sw.Append(name);
                        sw.Append(" < @");
                        sw.Append(id);
                        break;
                    case CompFlag.Ceil://小于等于
                        dt[id] = val;
                        sw.Append(name);
                        sw.Append(" <= @");
                        sw.Append(id);
                        break;
                    case CompFlag.Greater://大于 more
                        dt[id] = val;
                        sw.Append(name);
                        sw.Append(" > @");
                        sw.Append(id);
                        break;
                    case CompFlag.Floor://大于等于 more equal
                        dt[id] = val;
                        sw.Append(name);
                        sw.Append(" >= @");
                        sw.Append(id);
                        break;
                    case CompFlag.StartsWith:// begin with
                        dt[id] = val.ToString() + "%";
                        sw.Append(name);
                        sw.Append(" like @");
                        sw.Append(id);
                        break;
                    case CompFlag.EndsWith:// finish with
                        dt[id] = "%" + val.ToString();
                        sw.Append(name);
                        sw.Append(" like @");
                        sw.Append(id);
                        break;
                    case CompFlag.Contains:// any where 
                        dt[id] = "%" + val.ToString() + "%";
                        sw.Append(name);
                        sw.Append(" like @");
                        sw.Append(id);
                        break;
                    default:
                        dt[id] = val;
                        sw.Append(" = @");
                        sw.Append(id);
                        break;
                }
                sw.Append(")");
            }

            Query(this, new QueryClause { Where = sw.ToString(), Params = dt });
        }

        #region 命令对象
        /// <summary>
        /// 获取查询命令
        /// </summary>
        public BaseCommand CmdQuery
        {
            get
            {
                if (_cmdQuery == null)
                    _cmdQuery = new BaseCommand(p_params => { OnQuery(); });
                return _cmdQuery;
            }
        }

        /// <summary>
        /// 获取重置命令
        /// </summary>
        public BaseCommand CmdReset
        {
            get
            {
                if (_cmdReset == null)
                {
                    _cmdReset = new BaseCommand(p_params =>
                    {
                        if (Data is Row row)
                            row.RejectChanges();
                    });
                }
                return _cmdReset;
            }
        }
        #endregion
    }
}