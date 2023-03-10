#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Module
{
    public partial class PubCommentX
    {
        public static async Task<PubCommentX> New(
            long PostID = default,
            string Content = default,
            long UserID = default,
            string UserName = default,
            DateTime Ctime = default,
            bool IsSpam = default,
            long? ParentID = default,
            int Support = default,
            int Oppose = default)
        {
            long id = await NewID();
            return new PubCommentX(id, PostID, Content, UserID, UserName, Ctime, IsSpam, ParentID, Support, Oppose);
        }

        protected override void InitHook()
        {
        }
    }
}