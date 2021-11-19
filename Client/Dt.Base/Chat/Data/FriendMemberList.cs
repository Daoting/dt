#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using Dt.Core.Sqlite;
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.Base.Chat
{
    /// <summary>
    /// 好友列表
    /// </summary>
    public class FriendMemberList
    {
        const string _refreshKey = "LastRefreshChatMember";

        /// <summary>
        /// 更新好友列表，默认超过10小时需要刷新
        /// </summary>
        /// <returns></returns>
        public static async Task Refresh()
        {
            if (!NeedRefresh())
                return;

            // 暂时取所有，后续增加好友功能
            var tbl = await new UnaryRpc(
                "cm",
                "Da.Query",
                "select id,name,phone,sex,(case photo when '' then 'photo/profilephoto.jpg' else photo end) as photo, mtime from cm_user"
            ).Call<Table<ChatMember>>();

            // 将新列表缓存到本地库
            AtState.Exec("delete from ChatMember");
            if (tbl != null && tbl.Count > 0)
            {
                foreach (var r in tbl)
                {
                    r.IsAdded = true;
                }
                await AtState.BatchSave(tbl, false);
            }

            // 记录刷新时间
            AtState.SaveCookie(_refreshKey, Kit.Now.ToString());
        }

        static bool NeedRefresh()
        {
            // 超过10小时需要刷新
            bool refresh = true;
            string val = AtState.GetCookie(_refreshKey);
            if (!string.IsNullOrEmpty(val) && DateTime.TryParse(val, out var last))
                refresh = (Kit.Now - last).TotalHours >= 10;
            return refresh;
        }
    }
}
