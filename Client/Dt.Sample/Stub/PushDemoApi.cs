#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr;
#endregion

namespace Dt.Sample
{
    [PushApi]
    public class PushDemoApi
    {
        public void Hello(string p_msg)
        {
            Kit.Msg($"【收到服务端推送】\r\n{p_msg}");
        }
    }
}