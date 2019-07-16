#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dts.Core
{
    /// <summary>
    /// 实体类，主键任意也可以是组合主键，为方便集成尽量使用<see cref="IEntity{TKey}"/>
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// 返回实体的主键列表
        /// </summary>
        /// <returns></returns>
        object[] GetKeys();
    }

    /// <summary>
    /// 实体类，只包含"Id"主键
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam>
    public interface IEntity<TKey> : IEntity
    {
        /// <summary>
        /// 实体的唯一主键
        /// </summary>
        TKey Id { get; set; }
    }
}
