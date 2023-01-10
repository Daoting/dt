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

namespace Dt.Mgr.Domain
{
    public partial class PubPostObj
    {
        public static async Task<PubPostObj> New()
        {
            return new PubPostObj(
                ID: await NewID(),
                Title: "新文章",
                TempType: 0,
                Dispidx: await NewSeq("Dispidx"),
                CreatorID: Kit.UserID,
                Creator: Kit.UserName,
                Ctime: Kit.Now);
        }

        public bool IsValid()
        {
            Throw.IfEmpty(Title, "标题不可为空！");
            Throw.If(IsPublish && string.IsNullOrEmpty(Content), "发布的文章内容不能为空");
            return true;
        }

        protected override void InitHook()
        {
            OnChanging<bool>(nameof(IsPublish), v =>
            {
                Throw.If(v && string.IsNullOrEmpty(Content), "发布的文章内容不能为空");
            });

            OnDeleting(() =>
            {
                Throw.If(IsPublish, "已发布的文章不可删除");
                return Task.CompletedTask;
            });
        }
    }

    public enum PostTempType
    {
        普通,
        封面标题混合
    }
}