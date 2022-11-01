#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace $ext_safeprojectname$
{
    [PushApi]
    public class PushApi
    {
        public void Hello(string p_msg)
        {
            Kit.Msg($"【收到服务端推送】\r\n{p_msg}");
        }
    }
}
