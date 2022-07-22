#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using System;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Temporary object used during parsing
    /// </summary>
    internal class UndefinedFunction : CalcFunction
    {
        private readonly string _name;

        public UndefinedFunction(string name)
        {
            this._name = name;
        }

        public override object Evaluate(object[] args)
        {
            return CalcErrors.Name;
        }

        public override int MaxArgs
        {
            get
            {
                return 0xff;
            }
        }

        public override int MinArgs
        {
            get
            {
                return 0;
            }
        }

        public override string Name
        {
            get
            {
                return this._name;
            }
        }
    }
}

