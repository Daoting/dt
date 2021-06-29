#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core.EventBus
{
    /// <summary>
    /// 请求/响应模式的事件
    /// </summary>
    /// <typeparam name="TResponse">返回值类型</typeparam>
    public interface IRequest<out TResponse> : IEvent
    { }
}