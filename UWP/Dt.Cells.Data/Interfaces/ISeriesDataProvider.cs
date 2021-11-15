#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Expressions;
using System;
#endregion

namespace Dt.Cells.Data
{
    internal interface ISeriesDataProvider
    {
        event EventHandler DataChanged;

        string GetText(int valueIndex);
        string GetText(int seriesIndex, int valueIndex);
        object GetValue(int valueIndex);
        object GetValue(int seriesIndex, int valueIndex);
        bool IsSeriesVisible(int seriesIndex);
        bool IsValueVisible(int valueIndex);
        bool IsValueVisible(int seriesIndex, int valueIndex);

        Dt.Cells.Data.DataOrientation DataOrientation { get; }

        CalcExpression DataReference { get; }

        int SeriesCount { get; }

        int ValuesCount { get; }
    }
}

