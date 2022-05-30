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
using System.Collections;
using System.Globalization;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Checks whether the value is refers to any error value except #N/A.
    /// </summary>
    public class CalcIsErrFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process an error.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process an error; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsError(int i)
        {
            return true;
        }

        /// <summary>
        /// Checks whether the value is refers to any error value except #N/A.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: value.
        /// </para>
        /// <para>
        /// Value is the value you want tested. Value can be a blank (empty cell),
        /// error, logical, text, number, or reference value, or a name referring
        /// to any of these, that you want to test.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Boolean" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            if (ArrayHelper.IsArrayOrRange(args[0]))
            {
                IEnumerator enumerator = CalcConvert.ToEnumerator(args[0], false);
                while (enumerator.MoveNext())
                {
                    if (this.IsErr(enumerator.Current))
                    {
                        return (bool) true;
                    }
                }
            }
            return (bool) this.IsErr(args[0]);
        }

        private bool IsErr(object obj)
        {
            if ((obj is CalcError) && (((CalcError) obj) != CalcErrors.NotAvailable))
            {
                return true;
            }
            if (!(obj is string))
            {
                return false;
            }
            CalcError error = CalcErrors.Parse((string) (obj as string), CultureInfo.InvariantCulture);
            return ((error != null) && (error != CalcErrors.NotAvailable));
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
                return 1;
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
                return "ISERR";
            }
        }
    }
}

