#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Functions;
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 
    /// </summary>
    internal class FormulaFunction
    {
        /// <summary>
        /// 
        /// </summary>
        public FormulaFunction()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="function"></param>
        public FormulaFunction(CalcFunction function)
        {
            Function = function;
            Name = function.Name;
            MinArgs = function.MinArgs;
            MaxArgs = function.MaxArgs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CalcFunction Function { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MaxArgs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MinArgs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string[] Param { get; set; }
    }
}

