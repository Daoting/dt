﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-26 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 领域服务的抽象基类
    /// </summary>
    /// <typeparam name="TDomainSvc">当前领域服务的类型，保证静态变量属于各自的领域服务类型</typeparam>
    public abstract class DomainSvc<TDomainSvc>
        where TDomainSvc : class
    {
        /**********************************************************************************************************************************************************/
        // 泛型类型：
        // 对象是类的实例，提供具体类型参数的泛型类是泛型类型的实例
        // 若将cm服务的类型作为类型参数，如 AbcDs : DomainSvc<AtCm.Info>
        // 则AbcDs是该泛型基类的实例类，泛型基类中保证有一套只属于AbcDs类的静态变量实例！
        // 因此类型参数相同的泛型类的静态成员相同
        /***********************************************************************************************************************************************************/

        /// <summary>
        /// 日志对象，日志属性中包含来源
        /// </summary>
        protected static readonly ILogger _log = Log.ForContext("src", typeof(TDomainSvc).FullName);
    }
}
