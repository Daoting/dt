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
    /// Returns <see langword="true" /> if all its arguments are <see langword="true" />; 
    /// returns <see langword="false" /> if one or more argument is <see langword="false" />.
    /// </summary>
    public class CalcAndFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Determines whether the function accepts array values
        /// for the specified argument.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// <see langword="true" /> if the function accepts array values
        /// for the specified argument; <see langword="false" /> otherwise.
        /// </returns>
        public override bool AcceptsArray(int i)
        {
            return true;
        }

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
        /// Returns <see langword="true" /> if all its arguments are <see langword="true" />;
        /// returns <see langword="false" /> if one or more argument is <see langword="false" />.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 255 items: logical, [logica2], [logica3], ..
        /// </para>
        /// <para>
        /// logical, [logica2], [logica3], ... are 1 to 255 conditions you want to test that can be either <see langword="true" />
        /// or <see langword="false" />.
        /// </para>
        /// </param>
        /// <returns>
        /// A <see cref="T:System.Boolean" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            bool flag = true;
            foreach (object obj2 in args)
            {
                for (int i = 0; i < ArrayHelper.GetLength(obj2, 0); i++)
                {
                    object obj3 = ArrayHelper.GetValue(obj2, i, 0);
                    if (obj3 != null)
                    {
                        flag &= CalcConvert.ToBool(obj3);
                    }
                    else
                    {
                        return (bool) false;
                    }
                }
            }
            return (bool) flag;
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
                return 0xff;
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
                return "AND";
            }
        }
    }
}

