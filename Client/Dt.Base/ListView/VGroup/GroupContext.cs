#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-06-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 分组模板的数据上下文
    /// </summary>
    public class GroupContext
    {
        /// <summary>
        /// 当前分组的所有数据行
        /// </summary>
        public IList Rows { get; internal set; }

        /// <summary>
        /// 分组标题
        /// </summary>
        public string Title => Rows?.ToString();

        /// <summary>
        /// 分组总行数
        /// </summary>
        public int Count => Rows.Count;

        /// <summary>
        /// 列求和
        /// </summary>
        /// <param name="p_id">列名或属性名</param>
        /// <returns></returns>
        public int SumInt(string p_id)
        {
            return Each<int>(p_id).Sum();
        }

        /// <summary>
        /// 列求和
        /// </summary>
        /// <param name="p_id">列名或属性名</param>
        /// <returns></returns>
        public double SumDouble(string p_id)
        {
            return Each<double>(p_id).Sum();
        }

        /// <summary>
        /// 列求和
        /// </summary>
        /// <param name="p_id">列名或属性名</param>
        /// <returns></returns>
        public long SumLong(string p_id)
        {
            return Each<long>(p_id).Sum();
        }

        /// <summary>
        /// 列求平均值
        /// </summary>
        /// <param name="p_id">列名或属性名</param>
        /// <returns></returns>
        public double AverageInt(string p_id)
        {
            return Each<int>(p_id).Average();
        }

        /// <summary>
        /// 列求平均值
        /// </summary>
        /// <param name="p_id">列名或属性名</param>
        /// <returns></returns>
        public double AverageDouble(string p_id)
        {
            return Each<double>(p_id).Average();
        }

        /// <summary>
        /// 列求平均值
        /// </summary>
        /// <param name="p_id">列名或属性名</param>
        /// <returns></returns>
        public double AverageLong(string p_id)
        {
            return Each<long>(p_id).Average();
        }

        /// <summary>
        /// 求列最大值
        /// </summary>
        /// <param name="p_id">列名或属性名</param>
        /// <returns></returns>
        public int MaxInt(string p_id)
        {
            return Each<int>(p_id).Max();
        }

        /// <summary>
        /// 求列最大值
        /// </summary>
        /// <param name="p_id">列名或属性名</param>
        /// <returns></returns>
        public double MaxDouble(string p_id)
        {
            return Each<double>(p_id).Max();
        }

        /// <summary>
        /// 求列最大值
        /// </summary>
        /// <param name="p_id">列名或属性名</param>
        /// <returns></returns>
        public long MaxLong(string p_id)
        {
            return Each<long>(p_id).Max();
        }

        /// <summary>
        /// 求列最小值
        /// </summary>
        /// <param name="p_id">列名或属性名</param>
        /// <returns></returns>
        public int MinInt(string p_id)
        {
            return Each<int>(p_id).Min();
        }

        /// <summary>
        /// 求列最小值
        /// </summary>
        /// <param name="p_id">列名或属性名</param>
        /// <returns></returns>
        public double MinDouble(string p_id)
        {
            return Each<double>(p_id).Min();
        }

        /// <summary>
        /// 求列最小值
        /// </summary>
        /// <param name="p_id">列名或属性名</param>
        /// <returns></returns>
        public long MinLong(string p_id)
        {
            return Each<long>(p_id).Min();
        }

        /// <summary>
        /// 枚举分组行的某列值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_id"></param>
        /// <returns></returns>
        public IEnumerable<T> Each<T>(string p_id)
        {
            if (Rows == null || Rows.Count == 0)
                yield return default(T);

            var tp = Rows[0].GetType();
            PropertyInfo pi = null;
            if (tp != typeof(Row) && !tp.IsSubclassOf(typeof(Entity)))
            {
                // 分组属性
                pi = tp.GetProperty(p_id, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (pi == null)
                    yield return default(T);
            }

            foreach (object row in Rows)
            {
                if (pi == null)
                {
                    // Row
                    yield return ((Row)row).GetVal<T>(p_id);
                }
                else
                {
                    // 普通对象
                    yield return (T)pi.GetValue(row);
                }
            }
        }
    }
}
