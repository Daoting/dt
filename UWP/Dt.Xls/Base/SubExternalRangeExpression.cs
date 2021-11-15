#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls
{
    internal class SubExternalRangeExpression
    {
        private ExternalRangeExpression _expr1;
        private ExternalRangeExpression _expr2;
        private string _op;

        public SubExternalRangeExpression(string op, ExternalRangeExpression expr1, ExternalRangeExpression expr2)
        {
            this._op = op;
            this._expr1 = expr1;
            this._expr2 = expr2;
        }

        public ExternalRangeExpression ExternalReference1
        {
            get { return  this._expr1; }
        }

        public ExternalRangeExpression ExternalReference2
        {
            get { return  this._expr2; }
        }

        public string RangeOperator
        {
            get { return  this._op; }
        }
    }
}

