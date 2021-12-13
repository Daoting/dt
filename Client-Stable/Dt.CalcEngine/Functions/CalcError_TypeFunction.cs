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
    /// Returns a number corresponding to one of the error values
    /// or returns the #N/A error if no error exists.
    /// </summary>
    /// <remarks>
    /// You can use ERROR.TYPE in an IF function to test for an error
    /// value and return a text string, such as a message, instead of 
    /// the error value.
    /// </remarks>
    public class CalcError_TypeFunction : CalcBuiltinFunction
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
        /// Returns a number corresponding to one of the error values
        /// or returns the #N/A error if no error exists.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: error_val.
        /// </para>
        /// <para>
        /// Error_val is the error value whose identifying number you want
        /// to find. Although error_val can be the actual error value,
        /// it will usually be a reference to a cell containing a formula
        /// that you want to test.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Int32" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            CalcError error = args[0] as CalcError;
            if (error != null)
            {
                switch (error.ErrorCode)
                {
                    case 0:
                        return (int) 1;

                    case 7:
                        return (int) 2;

                    case 15:
                        return (int) 3;

                    case 0x17:
                        return (int) 4;

                    case 0x1d:
                        return (int) 5;

                    case 0x24:
                        return (int) 6;

                    case 0x2a:
                        return (int) 7;
                }
            }
            return CalcErrors.NotAvailable;
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
                return "ERROR.TYPE";
            }
        }
    }
}

