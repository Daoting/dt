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
    /// Returns the row number of a reference.
    /// </summary>
    public class CalcRowFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Determines whether the function accepts CalcReference values
        /// for the specified argument.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// <see langword="true" /> if the function accepts CalcReference values
        /// for the specified argument; <see langword="false" /> otherwise.
        /// </returns>
        public override bool AcceptsReference(int i)
        {
            return true;
        }

        /// <summary>
        /// Returns the row number of a reference.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 0 - 1 item: [reference].
        /// </para>
        /// <para>
        /// [Reference] is the cell or range of cells for which you want the row number.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Int32" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            return EvaluateImp(args, null);
        }

        /// <summary>
        /// Evaluates the function with the given arguments.
        /// </summary>
        /// <param name="args">Arguments for the function evaluation</param>
        /// <param name="context">Context in which the evaluation occurs</param>
        /// <returns>
        /// Result of the function applied to the arguments
        /// </returns>
        public override object Evaluate(object[] args, object context)
        {
            base.CheckArgumentsLength(args);
            return EvaluateImp(args, context);
        }

        private static object EvaluateImp(object[] args, object context)
        {
            CalcEvaluatorContext objA = context as CalcEvaluatorContext;
            if (object.ReferenceEquals(objA, null))
            {
                return CalcErrors.Value;
            }
            bool arrayFormulaMode = objA.ArrayFormulaMode;
            CalcReference reference = CalcHelper.ArgumentExists(args, 0) ? (args[0] as CalcReference) : (objA.GetReference(new CalcRangeIdentity(objA.Row, objA.Column, objA.RowCount, objA.ColumnCount)) as CalcReference);
            if ((reference == null) || (reference.RangeCount != 1))
            {
                return CalcErrors.Value;
            }
            if (arrayFormulaMode)
            {
                return new CellInfoReference(reference.GetSource(), reference.GetRow(0), reference.GetColumn(0), reference.GetRowCount(0), reference.GetColumnCount(0), CellInfoReference.CellInfoType.Row);
            }
            return (int) (reference.GetRow(0) + 1);
        }

        /// <summary>
        /// Determines whether the evaluation of the function is dependent
        /// on the context in which the evaluation occurs.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> the evaluation of the function is dependent on the context;
        /// <see langword="false" /> otherwise.
        /// </returns>
        public override bool IsContextSensitive
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the maximum number of arguments for the function.
        /// </summary>
        /// <value>
        /// The maximum number of arguments for the function.
        /// </value>
        public override int MaxArgs
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the minimum number of arguments for the function.
        /// </summary>
        /// <value>
        /// The minimum number of arguments for the function.
        /// </value>
        public override int MinArgs
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets The name of the function.
        /// </summary>
        /// <value>
        /// The name of the function.
        /// </value>
        public override string Name
        {
            get
            {
                return "ROW";
            }
        }
    }
}

