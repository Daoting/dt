#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 文本项实例
    /// </summary>
    internal class RptTextInst : RptOutputInst
    {
        #region 构造方法
        public RptTextInst(RptItemBase p_item)
            : base(p_item)
        {
        }
        #endregion

        /// <summary>
        /// 获取最终输出的字符串
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 获取设置当前过滤串字典
        /// </summary>
        public Dictionary<string, string> Filter { get; set; }

        /// <summary>
        /// 获取文本项对应的数据行，脚本用
        /// </summary>
        public Core.Row Data { get; private set; }

        /// <summary>
        /// 输出到指定页的页眉
        /// </summary>
        /// <param name="p_page"></param>
        public void OutputHeader(RptPage p_page)
        {
            _page = p_page;
            p_page.HeaderItems.Add(this);
            ParseValue();
        }

        /// <summary>
        /// 输出到指定页的页脚
        /// </summary>
        /// <param name="p_page"></param>
        public void OutputFooter(RptPage p_page)
        {
            _page = p_page;
            p_page.FooterItems.Add(this);
            ParseValue();
        }

        /// <summary>
        /// 输出报表项内容
        /// </summary>
        protected override void DoOutput()
        {
            RptText item = _item as RptText;
            RptRootInst root = Inst;
            root.OutputItem(this);
            ParseValue();
            if (!item.AutoHeight || _region.RowSpan > 1)
                return;

            // 处理自动行高
            double height = 0;
            Kit.RunSync(() =>
            {
                // 测量文本的实际高度
                TextBlock tb = new TextBlock();
                tb.TextWrapping = item.WordWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
                tb.FontFamily = new FontFamily(item.FontFamily);
                tb.FontSize = item.FontSize;
                if (item.Bold)
                    tb.FontWeight = FontWeights.Bold;
                if (item.Italic)
                    tb.FontStyle = FontStyle.Italic;
                tb.Text = Text;
                tb.Width = Width - 8 - item.Margin * 2;
                tb.Measure(new Size(double.MaxValue, double.MaxValue));
                height = Math.Ceiling(tb.ActualHeight) + 4;
            });
            root.SyncRowHeight(this, height);
        }

        /// <summary>
        /// 复制报表项
        /// </summary>
        /// <returns></returns>
        public RptTextInst Clone()
        {
            return new RptTextInst(_item);
        }

        /// <summary>
        /// 获取最终输出的字符串，替换总页数或页号占位符
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            if (((RptText)_item).ExistPlaceholder)
            {
                // 解析时已将页号、总页数转为内置符号
                Text = Text.Replace("$$", Inst.Pages.Count.ToString()).Replace("##", _page.PageNum);
            }
            return Text;
        }

        #region 内部方法
        /// <summary>
        /// 解析文本内容
        /// </summary>
        void ParseValue()
        {
            RptText item = _item as RptText;
            List<RptExpression> exps = item.Expressions;
            if (exps == null || exps.Count == 0)
            {
                if (item.IsScriptRender)
                {
                    // 通过外部脚本绘制单元格内容和样式
                    string tbl = null;
                    if (item.Parent is RptTblPartRow tblRow)
                    {
                        tbl = tblRow.Table.Tbl;
                    }
                    else if (item.Parent is RptMtxRow ib && ib.Parent is RptMatrix mtx)
                    {
                        // 矩阵其他位置无当前数据行概念，需要数据源可在脚本中通过 info.GetData 获取
                        tbl = mtx.Tbl;
                    }

                    RptData src;
                    if (!string.IsNullOrEmpty(tbl) && (src = Inst.Info.GetData(tbl).Result) != null)
                        Data = src.CurrentRow;
                }
                else
                {
                    // 静态文本
                    Text = item.Val;
                }
                return;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var exp in exps)
            {
                string col = exp.VarName;
                if (exp.Func == RptExpFunc.Param)
                {
                    Dict dtParams = Inst.Info.Params;
                    if (dtParams.ContainsKey(col))
                    {
                        object val = dtParams[col];
                        var row = item.Root.Params[col];
                        if (row != null
                            && row.Str("type") == "DateTime"
                            && val != null)
                        {
                            // 根据格的设置格式化日期
                            DateTime date = (DateTime)dtParams[col];
                            string ct = row.Str("ct");
                            string format = "yyyy-MM-dd";
                            if (!string.IsNullOrEmpty(ct))
                            {
                                Match match = Regex.Match(ct, "Format=\"(\\S*)\"");
                                if (match.Success && match.Groups.Count == 2)
                                    format = match.Groups[1].Value;
                            }
                            sb.Append(date.ToString(format));
                        }
                        else
                        {
                            sb.Append(val);
                        }
                    }
                }
                else if (exp.Func == RptExpFunc.Global)
                {
                    switch (col)
                    {
                        case "页号":
                            // 输出过程中
                            sb.Append("##");
                            break;
                        case "总页数":
                            // 占位符，最后渲染时才知道总页数
                            sb.Append("$$");
                            break;
                        case "水平页号":
                            sb.Append(_page.X + 1);
                            break;
                        case "垂直页号":
                            sb.Append(_page.Y + 1);
                            break;
                        case "报表名称":
                            sb.Append(Inst.Info.Name);
                            break;
                        case "日期":
                            sb.Append(DateTime.Now.ToString("yyyy-MM-dd"));
                            break;
                        case "时间":
                            sb.Append(DateTime.Now.ToString("HH:mm"));
                            break;
                        case "日期时间":
                            sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            break;
                    }
                }
                else if (exp.Func == RptExpFunc.Unknown)
                {
                    sb.Append(col);
                }
                else
                {
                    RptData src;
                    string tbl = exp.DataName;
                    if (string.IsNullOrEmpty(tbl) || (src = Inst.Info.GetData(tbl).Result) == null)
                        continue;

                    Data = src.CurrentRow;
                    switch (exp.Func)
                    {
                        case RptExpFunc.Val:
                            if (Data != null)
                                sb.Append(Data.Str(col));
                            break;

                        case RptExpFunc.Avg:
                            sb.Append(GetRows(src.Data).Average((row) => row.Double(col)));
                            break;

                        case RptExpFunc.Max:
                            sb.Append(GetRows(src.Data).Max((row) => row.Double(col)));
                            break;

                        case RptExpFunc.Min:
                            sb.Append(GetRows(src.Data).Min((row) => row.Double(col)));
                            break;

                        case RptExpFunc.Sum:
                            sb.Append(GetRows(src.Data).Sum((row) => row.Double(col)));
                            break;

                        case RptExpFunc.Count:
                            sb.Append(GetRows(src.Data).Count());
                            break;

                        case RptExpFunc.Index:
                            sb.Append(src.Current);
                            break;
                    }
                }
            }
            Text = sb.ToString();
        }

        /// <summary>
        /// 获取符合条件的数据行
        /// </summary>
        /// <param name="p_tbl"></param>
        /// <returns></returns>
        IEnumerable<Core.Row> GetRows(Table p_tbl)
        {
            if (Filter == null || Filter.Count == 0)
                return p_tbl;

            List<Core.Row> rows = new List<Core.Row>();
            foreach (var dr in p_tbl)
            {
                bool isOk = true;
                foreach (var filter in Filter)
                {
                    if (dr.Str(filter.Key) != filter.Value)
                    {
                        isOk = false;
                        break;
                    }
                }
                if (isOk)
                    rows.Add(dr);
            }
            return rows;
        }
        #endregion
    }
}
