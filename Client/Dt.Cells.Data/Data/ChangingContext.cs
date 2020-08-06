#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using Dt.CalcEngine.Expressions;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Cells.Data
{
    internal class ChangingContext
    {
        Dictionary<CalcLocalIdentity, Tuple<CalcLocalIdentity, CalcExpression>> _changedFormulas;
        Dictionary<CalcLocalIdentity, CalcLocalIdentity> _changingIdentities;
        public Dictionary<IFormulaOperatorSource, Dictionary<CalcLocalIdentity, Tuple<CalcLocalIdentity, CalcExpression>>> _extChangedFormulas;
        Dictionary<IFormulaOperatorSource, Dictionary<CalcLocalIdentity, CalcLocalIdentity>> _invalidateIdentities;

        public void AddExtChangedFormula(IFormulaOperatorSource manager, CalcLocalIdentity id, CalcLocalIdentity oldId, CalcExpression expr)
        {
            Dictionary<CalcLocalIdentity, Tuple<CalcLocalIdentity, CalcExpression>> dictionary;
            if (!this.ExtChangedFormulas.TryGetValue(manager, out dictionary))
            {
                dictionary = new Dictionary<CalcLocalIdentity, Tuple<CalcLocalIdentity, CalcExpression>>();
                this.ExtChangedFormulas[manager] = dictionary;
            }
            dictionary[id] = new Tuple<CalcLocalIdentity, CalcExpression>(oldId, expr);
        }

        public void InvalidateFormula(IFormulaOperatorSource manager, CalcLocalIdentity id)
        {
            Dictionary<CalcLocalIdentity, CalcLocalIdentity> dictionary;
            if (!this.InvalidateIdentities.TryGetValue(manager, out dictionary))
            {
                dictionary = new Dictionary<CalcLocalIdentity, CalcLocalIdentity>();
                this.InvalidateIdentities[manager] = dictionary;
            }
            dictionary[id] = id;
        }

        public Dictionary<CalcLocalIdentity, Tuple<CalcLocalIdentity, CalcExpression>> ChangedFormulas
        {
            get
            {
                if (this._changedFormulas == null)
                {
                    this._changedFormulas = new Dictionary<CalcLocalIdentity, Tuple<CalcLocalIdentity, CalcExpression>>();
                }
                return this._changedFormulas;
            }
        }

        public Dictionary<CalcLocalIdentity, CalcLocalIdentity> ChangingIdentities
        {
            get
            {
                if (this._changingIdentities == null)
                {
                    this._changingIdentities = new Dictionary<CalcLocalIdentity, CalcLocalIdentity>();
                }
                return this._changingIdentities;
            }
        }

        public Dictionary<IFormulaOperatorSource, Dictionary<CalcLocalIdentity, Tuple<CalcLocalIdentity, CalcExpression>>> ExtChangedFormulas
        {
            get
            {
                if (this._extChangedFormulas == null)
                {
                    this._extChangedFormulas = new Dictionary<IFormulaOperatorSource, Dictionary<CalcLocalIdentity, Tuple<CalcLocalIdentity, CalcExpression>>>();
                }
                return this._extChangedFormulas;
            }
        }

        public Dictionary<IFormulaOperatorSource, Dictionary<CalcLocalIdentity, CalcLocalIdentity>> InvalidateIdentities
        {
            get
            {
                if (this._invalidateIdentities == null)
                {
                    this._invalidateIdentities = new Dictionary<IFormulaOperatorSource, Dictionary<CalcLocalIdentity, CalcLocalIdentity>>();
                }
                return this._invalidateIdentities;
            }
        }
    }
}

