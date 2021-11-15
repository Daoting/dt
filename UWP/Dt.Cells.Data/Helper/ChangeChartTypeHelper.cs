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
    internal static class ChangeChartTypeHelper
    {
        public static SpreadDataSeries CreateDataSeries(SpreadChartType chartType)
        {
            switch (SpreadChartUtility.GetDataDimension(chartType))
            {
                case 1:
                    return new SpreadDataSeries();

                case 2:
                    return new SpreadXYDataSeries();

                case 3:
                    return new SpreadBubbleSeries();

                case 5:
                    return new SpreadOpenHighLowCloseSeries();
            }
            return new SpreadDataSeries();
        }
    }
}

