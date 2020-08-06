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
    internal class DataSeiresDataProvider : WorksheetSeriesDataProvider
    {
        IDataSeries _dataSeries;

        public DataSeiresDataProvider(IDataSeries dataSeries)
        {
            this._dataSeries = dataSeries;
        }

        public override Dt.Cells.Data.DataOrientation DataOrientation
        {
            get
            {
                if (this._dataSeries.DataOrientation.HasValue)
                {
                    return this._dataSeries.DataOrientation.Value;
                }
                return Dt.Cells.Data.DataOrientation.Vertical;
            }
        }

        public override CalcExpression DataReference
        {
            get { return  this._dataSeries.DataReference; }
        }
    }
}

