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
    /// <summary>
    /// Represents Excel calc errors
    /// </summary>
    public class ExcelCalcError
    {
        private string _text;
        /// <summary>
        /// Argument or function not available error
        /// </summary>
        public static readonly ExcelCalcError ArgumentOrFunctionNotAvailable = new ExcelCalcError("#N/A");
        /// <summary>
        /// Divide by zero error
        /// </summary>
        public static readonly ExcelCalcError DivideByZero = new ExcelCalcError("#DIV/0!");
        /// <summary>
        /// Getting Data error
        /// </summary>
        public static readonly ExcelCalcError FAILEDTOGETDATA = new ExcelCalcError("#GETTING_DATA");
        /// <summary>
        /// Wrong reference error
        /// </summary>
        public static readonly ExcelCalcError IllegalOrDeletedCellReference = new ExcelCalcError("#REF!");
        /// <summary>
        /// Null reference error
        /// </summary>
        public static readonly ExcelCalcError InterSectionOfTwoCellRangesIsEmpty = new ExcelCalcError("#NULL!");
        /// <summary>
        /// Unknown Error
        /// </summary>
        public static readonly ExcelCalcError UnKnownError = new ExcelCalcError("Unknown");
        /// <summary>
        /// Value range overflow error
        /// </summary>
        public static readonly ExcelCalcError ValueRangeOverflow = new ExcelCalcError("#NUM!");
        /// <summary>
        /// Name error
        /// </summary>
        public static readonly ExcelCalcError WrongFunctionOrRangeName = new ExcelCalcError("#NAME?");
        /// <summary>
        /// Wrong type or value error
        /// </summary>
        public static readonly ExcelCalcError WrongTypeOfOperand = new ExcelCalcError("#VALUE!");

        private ExcelCalcError(string text)
        {
            this._text = text;
        }

        /// <summary>
        /// Determine whether the specified object is the same CalcError object.
        /// </summary>
        /// <param name="obj">The other object used to compare</param>
        /// <returns>True if specified object is the same CalcError, otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            return ((obj is ExcelCalcError) && (this._text == ((ExcelCalcError) obj)._text));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this._text.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the error.
        /// </summary>
        /// <returns>String representation of the error</returns>
        public override string ToString()
        {
            return this._text;
        }
    }
}

