#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Domain
{
    /// <summary>
    /// 主键ID为long类型的mysql仓库
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class Repo<TEntity> : DbRepo<TEntity, long>
        where TEntity : class, IRoot<long>
    {
        
    }
}
