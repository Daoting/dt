#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-07-28 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Entry
{
    class ReceiveShare : IReceiveShare
    {
        /// <summary>
        /// 接收分享内容
        /// </summary>
        /// <param name="p_info">分享内容描述</param>
        public void OnReceive(ShareInfo p_info)
        {
            Kit.OpenWin(Type.GetType("Demo.UI.ReceiveShareWin,Demo.UI"), "接收分享", Icons.分享, p_info);
        }
    }
}
