#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-08-05 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
#endregion

namespace Dt.Sample
{
    /// <summary>
    /// 服务端推送的处理Api
    /// </summary>
    public class PushApi : IPushApi
    {
        public void Hello(string p_msg)
        {
            Kit.Msg($"【收到服务端推送】\r\n{p_msg}");
        }
    }
}
