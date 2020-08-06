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
using Dt.CalcEngine.Functions;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Cells.Data
{
    internal class WorkbookCalcSource : ICalcSource, IEqualityComparer<ICalcSource>
    {
        Workbook _workbook;

        public WorkbookCalcSource(Workbook workbook)
        {
            if (workbook == null)
            {
                throw new ArgumentNullException("workbook");
            }
            this._workbook = workbook;
        }

        public bool Equals(ICalcSource x, ICalcSource y)
        {
            throw new InvalidOperationException();
        }

        public int GetColumnCount()
        {
            throw new InvalidOperationException();
        }

        public CalcExpression GetDefinedName(string name, int row, int column)
        {
            if ((this._workbook != null) && (this._workbook.Names != null))
            {
                NameInfo info = this._workbook.Names.Find(name);
                if ((info != null) && (info.Expression != null))
                {
                    return info.Expression;
                }
            }
            return null;
        }

        public CalcEvaluatorContext GetEvaluatorContext(CalcLocalIdentity baseAddress)
        {
            throw new InvalidOperationException();
        }

        public CalcFunction GetFunction(string functionName)
        {
            throw new InvalidOperationException();
        }

        public int GetHashCode(ICalcSource obj)
        {
            throw new InvalidOperationException();
        }

        public CalcParserContext GetParserContext(CalcLocalIdentity baseAddress)
        {
            throw new InvalidOperationException();
        }

        public object GetReference(CalcLocalIdentity id)
        {
            throw new InvalidOperationException();
        }

        public int GetRowCount()
        {
            throw new InvalidOperationException();
        }

        public object GetValue(CalcLocalIdentity id)
        {
            throw new InvalidOperationException();
        }

        public void SetValue(CalcLocalIdentity id, object value)
        {
            throw new InvalidOperationException();
        }

        public string Name
        {
            get { return  this._workbook.Name; }
        }
    }
}

