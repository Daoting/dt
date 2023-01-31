#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-26 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 领域服务的抽象基类
    /// </summary>
    public abstract class DomainSvc<TDomainSvc, TEntityAccess>
        where TDomainSvc : class
        where TEntityAccess : class
    {
        /// <summary>
        /// 获取领域层数据访问对象
        /// </summary>
        protected readonly IEntityAccess _ea;

        public DomainSvc()
        {
            if (!typeof(TEntityAccess).IsSubclassOf(typeof(EntityAccess<>)))
                throw new Exception("TEntityAccess参数需继承自EntityAccess<>");

            var prop = typeof(TEntityAccess).GetProperty("Ea", BindingFlags.Public | BindingFlags.Static);
            _ea = (IEntityAccess)prop.GetValue(null);
        }

        /// <summary>
        /// 获取新的实体写入器
        /// </summary>
        protected IEntityWriter NewWriter => new EntityWriter();

        /// <summary>
        /// 单例对象
        /// </summary>
        public static readonly TDomainSvc Me = (TDomainSvc)Activator.CreateInstance(typeof(TDomainSvc), true);
    }
}
