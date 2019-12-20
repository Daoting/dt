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
    internal class SheetSparklineDataProvider : WorksheetSparklineData
    {
        public SheetSparklineDataProvider(Dt.Cells.Data.Sparkline sparkline)
        {
            this.Sparkline = sparkline;
        }

        internal override Dt.Cells.Data.DataOrientation DataOrientation
        {
            get
            {
                if (this.Sparkline != null)
                {
                    return this.Sparkline.DataOrientation;
                }
                return Dt.Cells.Data.DataOrientation.Vertical;
            }
        }

        internal override CalcExpression DataReference
        {
            get
            {
                if (this.Sparkline != null)
                {
                    return this.Sparkline.DataReference;
                }
                return null;
            }
        }

        internal override ICalcEvaluator Sheet
        {
            get
            {
                if (((this.Sparkline != null) && (this.Sparkline.Group != null)) && (this.Sparkline.Group.SparklineGroupManager != null))
                {
                    return this.Sparkline.Group.SparklineGroupManager.CalcEvaluator;
                }
                return null;
            }
        }

        internal override bool ShowHiddenData
        {
            get
            {
                if (this.Sparkline != null)
                {
                    return this.Sparkline.Setting.DisplayHidden;
                }
                return base.ShowHiddenData;
            }
        }

        public Dt.Cells.Data.Sparkline Sparkline { get; private set; }
    }
}

