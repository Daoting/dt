#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-12-28 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 单元格数据接口
    /// </summary>
    public interface ICell
    {
        /// <summary>
        /// 属性 Val,IsChanged 变化事件
        /// </summary>
        event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 获取数据项名称
        /// </summary>
        string ID { get; }

        /// <summary>
        /// 获取数据项值的类型
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// 获取设置数据项值
        /// </summary>
        object Val { get; set; }

        /// <summary>
        /// 获取当前数据项是否已发生更改。
        /// </summary>
        bool IsChanged { get; set; }

        /// <summary>
        /// 获取该数据项未发生更改前的值
        /// </summary>
        object OriginalVal { get; }

        /// <summary>
        /// 提交自上次调用以来对该数据项进行的所有更改。
        /// </summary>
        void AcceptChanges();

        /// <summary>
        /// 回滚自该表加载以来或上次调用 AcceptChanges 以来对该数据项进行的所有更改。
        /// </summary>
        void RejectChanges();

        /// <summary>
        /// 获取当前数据项的值
        /// </summary>
        /// <typeparam name="T">将值转换为指定的类型</typeparam>
        /// <returns>指定类型的值</returns>
        T GetVal<T>();

        /// <summary>
        /// ID是否匹配给定列表中的任一名称，忽略大小写
        /// </summary>
        /// <param name="p_ids">一个或多个id名称</param>
        /// <returns>true 匹配任一</returns>
        bool IsID(params string[] p_ids);
    }
}