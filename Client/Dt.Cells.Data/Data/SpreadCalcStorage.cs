#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using Dt.CalcEngine.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    internal class SpreadCalcStorage : ICalcStorage<CalcLocalIdentity, CalcExpression>, IEnumerable<KeyValuePair<CalcLocalIdentity, CalcExpression>>, IEnumerable
    {
        Dictionary<CalcLocalIdentity, CalcExpression> _storage = new Dictionary<CalcLocalIdentity, CalcExpression>();

        internal void Add(CalcLocalIdentity id, CalcExpression exp)
        {
            this._storage[id] = exp;
        }

        public IEnumerator<KeyValuePair<CalcLocalIdentity, CalcExpression>> GetEnumerator()
        {
            foreach (KeyValuePair<CalcLocalIdentity, CalcExpression> iteratorVariable0 in this._storage)
            {
                yield return new KeyValuePair<CalcLocalIdentity, CalcExpression>(iteratorVariable0.Key, iteratorVariable0.Value);
            }
        }


        public void RemoveAt(CalcLocalIdentity id)
        {
            this._storage.Remove(id);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (KeyValuePair<CalcLocalIdentity, CalcExpression> iteratorVariable0 in this._storage)
            {
                yield return new KeyValuePair<CalcLocalIdentity, CalcExpression>(iteratorVariable0.Key, iteratorVariable0.Value);
            }
        }


        public int Count
        {
            get { return  this._storage.Count; }
        }

        public CalcExpression this[CalcLocalIdentity id]
        {
            get
            {
                CalcExpression expression;
                this._storage.TryGetValue(id, out expression);
                return expression;
            }
            set { this._storage[id] = value; }
        }

    }
}

