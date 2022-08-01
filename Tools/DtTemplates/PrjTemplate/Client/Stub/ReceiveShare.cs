#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace $ext_safeprojectname$
{
    public class ReceiveShare : IReceiveShare
    {
        /// <summary>
        /// 接收分享内容
        /// </summary>
        /// <param name="p_info">分享内容描述</param>
        public void OnReceive(ShareInfo p_info)
        {
            //Kit.OpenWin(typeof(ReceiveShareWin), "接收分享", Icons.分享, p_info);
        }
    }
}
