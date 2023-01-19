#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-19 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 虚拟实体基类，多个一对一实体的组合实体
    /// </summary>
    public abstract class VirEntity : Entity
    {
        /// <summary>
        /// 获取内部所有实体
        /// </summary>
        /// <returns></returns>
        public abstract List<Entity> GetEntities();
    }
}
