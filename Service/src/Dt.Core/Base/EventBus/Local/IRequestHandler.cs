#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Threading.Tasks;
#endregion

namespace Dt.Core.EventBus
{
    /// <summary>
    /// 请求/响应模式的事件处理接口
    /// </summary>
    /// <typeparam name="TRequest">事件处理的输入类型</typeparam>
    /// <typeparam name="TResponse">事件处理的输出类型</typeparam>
    public interface IRequestHandler<TRequest, TResponse> : IEventHandler
        where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="p_request">请求内容</param>
        /// <returns>返回响应值</returns>
        Task<TResponse> Handle(TRequest p_request);
    }
}