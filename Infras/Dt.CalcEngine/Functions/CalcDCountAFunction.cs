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
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Counts the cells that contain numbers in a field (column) of 
    /// records in a list or database that match conditions that you specify.
    /// </summary>
    /// <remarks>
    /// The field argument is optional. If field is <see cref="T:Dt.CalcEngine.CalcMissingArgument" />, 
    /// DCOUNTA counts all records in the database that match the criteria.
    /// </remarks>
    public class CalcDCountAFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process an array arguments.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process an array arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsArray(int i)
        {
            return (i != 1);
        }

        /// <summary>
        /// Indicates whether the Evaluate method can process references.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process references; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsReference(int i)
        {
            return (i != 1);
        }

        /// <summary>
        /// Counts the cells that contain numbers in a field (column) of
        /// records in a list or database that match conditions that you specify.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: database, field, criteria.
        /// </para>
        /// <para>
        /// Database is the range of cells that makes up the list or database.
        /// A database is a list of related data in which rows of related
        /// information are records, and columns of data are fields.
        /// The first row of the list contains labels for each column.
        /// </para>
        /// <para>
        /// Field indicates which column is used in the function.
        /// Enter the column label enclosed between double quotation marks,
        /// such as "Age" or "Yield," or a number (without quotation marks)
        /// that represents the position of the column within the list:
        /// 1 for the first column, 2 for the second column, and so on.
        /// </para>
        /// <para>
        /// Criteria is the range of cells that contains the conditions you
        /// specify. You can use any range for the criteria argument,
        /// as long as it includes at least one column label and at least
        /// one cell below the column label in which you specify a condition for the column.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            if (((args[0] == null) || (args[1] == null)) || (args[2] == null))
            {
                throw new ArgumentNullException();
            }
            CalcArray database = CalcConvert.ToArray(args[0]);
            object field = args[1];
            CalcArray criteria = CalcConvert.ToArray(args[2]);
            double num = 0.0;
            if (CalcHelper.ArgumentExists(args, 1))
            {
                IEnumerator enumerator = new DatabaseEnumerator(database, field, criteria);
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current != null)
                    {
                        num++;
                    }
                }
            }
            else
            {
                IEnumerator enumerator2 = new DatabaseEnumerator(database, criteria);
                while (enumerator2.MoveNext())
                {
                    num++;
                }
            }
            return CalcConvert.ToResult(num);
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
                return 3;
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
                return 3;
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
                return "DCOUNTA";
            }
        }
    }
}

