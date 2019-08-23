#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Bs.Mgr
{
    /// <summary>
    /// 认证服务的公共Api代理类
    /// </summary>
    public partial class AtAuth : SrvAgent<Auth> { }

    /// <summary>
    /// 内核模型服务的公共Api代理类
    /// </summary>
    public class AtCm : SrvAgent<Cm> { }

    /// <summary>
    /// 认证服务，只为规范服务名称
    /// </summary>
    public class Auth { }

    /// <summary>
    /// 内核模型服务，只为规范服务名称
    /// </summary>
    public class Cm { }
}
