#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Dts.Core
{
    /// <summary>
    /// 聚合根类，主键任意也可以是组合主键，为方便集成尽量使用<see cref="IAggregateRoot{TKey}"/>
    /// </summary>
    public interface IAggregateRoot : IEntity
    {
    }

    /// <summary>
    /// 聚合根类，只包含"Id"主键
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam>
    public interface IAggregateRoot<TKey> : IEntity<TKey>, IAggregateRoot
    {
    }
}
