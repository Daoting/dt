#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-07-28
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 依赖注入的全局服务对象
    /// </summary>
    public partial class Kit
    {
        /// <summary>
        /// 在全局服务容器中获取指定类型的服务对象，服务类型不存在时返回null，不抛异常
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>服务对象</returns>
        public static T GetService<T>() => _svcProvider.GetService<T>();

        /// <summary>
        /// 在全局服务容器中获取指定类型的服务对象，服务类型不存在时抛异常
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>服务对象</returns>
        public static T GetRequiredService<T>() => _svcProvider.GetRequiredService<T>();

        /// <summary>
        /// 在全局服务容器中获取指定类型的服务对象，服务类型不存在时返回null，不抛异常
        /// </summary>
        /// <param name="p_svcType"></param>
        /// <returns>服务对象</returns>
        public static object GetService(Type p_svcType) => _svcProvider.GetService(p_svcType);

        /// <summary>
        /// 在全局服务容器中获取指定类型的服务对象，服务类型不存在时抛异常
        /// </summary>
        /// <param name="p_svcType"></param>
        /// <returns>服务对象</returns>
        public static object GetRequiredService(Type p_svcType) => _svcProvider.GetRequiredService(p_svcType);

        /// <summary>
        /// 在全局服务容器中获取指定类型的所有服务对象
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>所有服务对象</returns>
        public static IEnumerable<T> GetServices<T>() => _svcProvider.GetServices<T>();

        /// <summary>
        /// 在全局服务容器中获取指定类型的所有服务对象
        /// </summary>
        /// <param name="p_svcType">服务类型</param>
        /// <returns>所有服务对象</returns>
        public static IEnumerable<object> GetServices(Type p_svcType) => _svcProvider.GetServices(p_svcType);
    }
}