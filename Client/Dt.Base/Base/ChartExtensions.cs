#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base;
using Dt.Charts;
using Dt.Core;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Chart扩展方法
    /// </summary>
    public static class ChartExtensions
    {
        public static bool LoadData(this Chart p_chart, Table p_tbl, string p_s, string p_x, string p_y, string p_z)
        {
            if (p_chart == null || p_tbl == null || string.IsNullOrEmpty(p_x) || string.IsNullOrEmpty(p_y))
                return false;

            ChartType chartType = p_chart.ChartType;

            if (chartType == ChartType.Gantt)
            {
                if (string.IsNullOrEmpty(p_z))
                    return false;

                LoadGantt(p_chart, p_tbl, p_x, p_y, p_z, p_s);
            }
            else
            {
                if (string.IsNullOrEmpty(p_s))
                    LoadTable(p_chart, p_tbl, p_x, p_y);
                else
                    LoadMatrix(p_chart, p_tbl, p_x, p_s, p_y);
            }

            return true;
        }


        public static void LoadGantt(this Chart p_chart, Table p_tbl, string p_colX, string p_colY, string p_colZ, string p_colS = null)
        {
            if (p_chart == null || p_tbl == null)
                return;

            Binding bindX, bindY, bindZ;
            HighLowSeries ds;
            p_chart.BeginUpdate();
            ChartData data = p_chart.Data;
            data.Children.Clear();
            data.ItemNameBinding = null;
            data.ItemsSource = p_tbl;
            Size symSize = new Size(30, 30);
            if (string.IsNullOrEmpty(p_colS))
            {
                ds = new HighLowSeries();
                bindX = new Binding();
                bindX.Path = new PropertyPath($"Cells[{p_colX}].Val");
                ds.XValueBinding = bindX;

                bindY = new Binding();
                bindY.Path = new PropertyPath($"Cells[{p_colY}].Val");
                ds.LowValueBinding = bindY;

                bindZ = new Binding();
                bindZ.Path = new PropertyPath($"Cells[{p_colZ}].Val");
                ds.HighValueBinding = bindZ;

                ds.SymbolSize = symSize;
                data.Children.Add(ds);
            }
            else
            {
                string[] names = (from t in p_tbl
                                  select t.Str(p_colS)
                                 ).Distinct().ToArray();
                foreach (string sn in names)
                {
                    ds = new HighLowSeries() { Label = sn };

                    ds.XValuesSource = (from t in p_tbl
                                        where t.Str(p_colS) == sn
                                        select t.Str(p_colX));
                    ds.LowValuesSource = (from t in p_tbl
                                          where t.Str(p_colS) == sn
                                          select t.Str(p_colY));
                    ds.HighValuesSource = (from t in p_tbl
                                           where t.Str(p_colS) == sn
                                           select t.Str(p_colZ));
                    ds.SymbolSize = symSize;
                    data.Children.Add(ds);
                }
            }

            p_chart.EndUpdate();
        }

        /// <summary>
        /// 加载表格数据源图表
        /// </summary>
        /// <param name="p_chart"></param>
        /// <param name="p_tbl">数据源</param>
        /// <param name="p_colX">x轴取值列名</param>
        /// <param name="p_colY">y轴取值列名</param>
        public static void LoadTable(this Chart p_chart, Table p_tbl, string p_colX, string p_colY)
        {
            if (!string.IsNullOrEmpty(p_colY))
                LoadTable(p_chart, p_tbl, p_colX, new List<string> { p_colY });
        }

        /// <summary>
        /// 加载表格数据源图表
        /// </summary>
        /// <param name="p_chart"></param>
        /// <param name="p_tbl">数据源</param>
        /// <param name="p_colX">x轴取值列名</param>
        /// <param name="p_colY">y轴系列取值列名，每个列名对应一系列</param>
        public static void LoadTable(this Chart p_chart, Table p_tbl, string p_colX, List<string> p_colY)
        {
            if (p_chart == null
                || p_tbl == null
                || string.IsNullOrEmpty(p_colX)
                || !p_tbl.Columns.Contains(p_colX)
                || p_colY == null
                || p_colY.Count == 0)
                return;

            Binding bind;
            DataSeries ds;
            p_chart.BeginUpdate();
            ChartData data = p_chart.Data;
            data.Children.Clear();
            data.ItemNameBinding = null;

            data.ItemsSource = p_tbl;
            bind = new Binding();
            bind.Path = new PropertyPath($"Cells[{p_colX}].Val");
            data.ItemNameBinding = bind;

            foreach (string name in p_colY)
            {
                ds = new DataSeries();
                ds.Label = name;
                bind = new Binding();
                bind.Path = new PropertyPath($"Cells[{name}].Val");
                ds.ValueBinding = bind;
                data.Children.Add(ds);
            }
            p_chart.EndUpdate();
        }

        /// <summary>
        /// 加载矩阵数据源图表，动态生成系列
        /// </summary>
        /// <param name="p_chart"></param>
        /// <param name="p_tbl">数据源</param>
        /// <param name="p_colX">x轴取值列名</param>
        /// <param name="p_colSeries">系列对应的列名</param>
        /// <param name="p_colY">y轴取值列名</param>
        public static void LoadMatrix(this Chart p_chart, Table p_tbl, string p_colX, string p_colSeries, string p_colY)
        {
            if (p_chart == null || p_tbl == null)
                return;

            Table tbl = p_tbl.CreateMatrix(p_colX, p_colSeries, p_colY);
            if (tbl == null)
                return;

            Binding bind;
            DataSeries ds;
            p_chart.BeginUpdate();
            ChartData data = p_chart.Data;
            data.Children.Clear();
            data.ItemNameBinding = null;

            data.ItemsSource = tbl;
            bind = new Binding();
            bind.Path = new PropertyPath($"Cells[{p_colX}].Val");
            data.ItemNameBinding = bind;

            for (int i = 1; i < tbl.Columns.Count; i++)
            {
                string name = tbl.Columns[i].ID;
                ds = new DataSeries();
                ds.Label = name;
                bind = new Binding();
                bind.Path = new PropertyPath($"Cells[{name}].Val");
                ds.ValueBinding = bind;
                data.Children.Add(ds);
            }
            p_chart.EndUpdate();
        }
    }
}
