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
    public partial class PubPostX
    {
        public static async Task<PubPostX> New()
        {
            return new PubPostX(
                ID: await NewID(),
                Title: "新文章",
                TempType: 0,
                Dispidx: await NewSeq("Dispidx"),
                CreatorID: Kit.UserID,
                Creator: Kit.UserName,
                Ctime: Kit.Now);
        }

        protected override void InitHook()
        {
            OnSaving(() =>
            {
                Throw.IfEmpty(Title, "标题不可为空！");
                Throw.If(IsPublish && string.IsNullOrEmpty(Content), "发布的文章内容不能为空");
                return Task.CompletedTask;
            });

            OnDeleting(() =>
            {
                Throw.If(IsPublish, "已发布的文章不可删除");
                return Task.CompletedTask;
            });

            OnDeleted(async () =>
            {
                if (!string.IsNullOrEmpty(Url))
                {
                    // 删除html文件
                    await AtFsm.DeleteFile($"g/{Url}");
                }
            });
        }
    }

    public enum PostTempType
    {
        普通,
        封面标题混合
    }
}