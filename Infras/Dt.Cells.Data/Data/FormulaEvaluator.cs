#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using Dt.CalcEngine.Functions;
using System;
#endregion

namespace Dt.Cells.Data
{
    internal static class FormulaEvaluator
    {
        public static double FORECAST(ICalcEvaluator evaluator, int dest, object[,] values, object[,] indexes)
        {
            CalcArray array = evaluator.EvaluateFunction(new CalcForecastFunction(), new object[] { (int) dest, values, indexes }) as CalcArray;
            return (double) ((double) array.GetValue(0, 0));
        }

        public static double GROWTH(ICalcEvaluator evaluator, object[,] values, object[,] indexes, int dest)
        {
            CalcArray array = evaluator.EvaluateFunction(new CalcGrowthFunction(), new object[] { values, indexes, (int) dest }) as CalcArray;
            return (double) ((double) array.GetValue(0, 0));
        }

        public static double TREND(ICalcEvaluator evaluator, object[,] values, object[,] indexes, int dest)
        {
            CalcArray array = evaluator.EvaluateFunction(new CalcTrendFunction(), new object[] { values, indexes, (int) dest }) as CalcArray;
            return (double) ((double) array.GetValue(0, 0));
        }
    }
}

