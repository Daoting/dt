#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Provides a container for a collection of SpreadDataSeries.
    /// </summary>
    public class DataSeriesCollection : SpreadChartElementCollection<SpreadDataSeries>
    {
        internal DataSeriesCollection(SpreadChartBase chart, ChartArea area) : base(chart, area)
        {
        }
    }
}

