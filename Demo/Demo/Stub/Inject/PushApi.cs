#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024/5/20 10:39:08 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Demo
{
    [PushApi]
    public class PushApi
    {
        public void Hello(string p_msg)
        {
            Kit.Msg($"【收到服务端推送】\r\n{p_msg}\r\n当前位置：PushApi.Hello");
        }
    }
}
