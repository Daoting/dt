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
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    internal class SheetSparklineGroupDateProvider : WorksheetSparklineData
    {
        public SheetSparklineGroupDateProvider(Dt.Cells.Data.SparklineGroup group)
        {
            this.SparklineGroup = group;
        }

        internal override Dt.Cells.Data.DataOrientation DataOrientation
        {
            get { return  this.SparklineGroup.DateAxisOrientation; }
        }

        internal override CalcExpression DataReference
        {
            get { return  this.SparklineGroup.DateAxisReference; }
        }

        internal override ICalcEvaluator Sheet
        {
            get
            {
                if ((this.SparklineGroup != null) && (this.SparklineGroup.SparklineGroupManager != null))
                {
                    return this.SparklineGroup.SparklineGroupManager.CalcEvaluator;
                }
                return null;
            }
        }

        internal override bool ShowHiddenData
        {
            get
            {
                if (this.SparklineGroup != null)
                {
                    return this.SparklineGroup.Setting.DisplayHidden;
                }
                return base.ShowHiddenData;
            }
        }

        public Dt.Cells.Data.SparklineGroup SparklineGroup { get; private set; }
    }
}

