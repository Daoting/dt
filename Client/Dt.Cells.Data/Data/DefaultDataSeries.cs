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
    internal class DefaultDataSeries : IDataSeries
    {
        Dt.Cells.Data.DataOrientation? _dataOrientation;
        CalcExpression _dataReference;
        bool _displayHiddenData;
        Dt.Cells.Data.EmptyValueStyle _emptyValueStyle;
        ICalcEvaluator _evaluator;

        public DefaultDataSeries(CalcExpression dataReference, Dt.Cells.Data.DataOrientation dataOrientation, bool displayHiddenData, Dt.Cells.Data.EmptyValueStyle emptyValueStyle, ICalcEvaluator evalutor)
        {
            this._dataReference = dataReference;
            this._dataOrientation = new Dt.Cells.Data.DataOrientation?(dataOrientation);
            this._displayHiddenData = displayHiddenData;
            this._emptyValueStyle = emptyValueStyle;
            this._evaluator = evalutor;
        }

        public Dt.Cells.Data.DataOrientation? DataOrientation
        {
            get { return  this._dataOrientation; }
        }

        public CalcExpression DataReference
        {
            get { return  this._dataReference; }
        }

        public bool DisplayHiddenData
        {
            get { return  this._displayHiddenData; }
        }

        public Dt.Cells.Data.EmptyValueStyle EmptyValueStyle
        {
            get { return  this._emptyValueStyle; }
        }

        public ICalcEvaluator Evaluator
        {
            get { return  this._evaluator; }
        }
    }
}

