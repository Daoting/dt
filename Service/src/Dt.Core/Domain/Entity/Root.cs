#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
#endregion

namespace Dt.Core.Domain
{
    /// <summary>
    /// 包含"ID"主键的聚合根基类
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class Root<TKey> : Entity<TKey>, IRoot<TKey>
    {
        
    }

    /// <summary>
    /// 主键ID为long类型的聚合根基类
    /// </summary>
    public abstract class Root : Root<long>
    {

    }
}
