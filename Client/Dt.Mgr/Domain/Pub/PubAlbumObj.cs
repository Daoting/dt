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
    public partial class PubAlbumObj
    {
        public static async Task<PubAlbumObj> New(string Name = "新专辑")
        {
            long id = await NewID();
            return new PubAlbumObj(id, Name, Kit.UserName, Kit.Now);
        }

        protected override void InitHook()
        {
            OnSaving(() =>
            {
                Throw.IfEmpty(Name, "名称不可为空！");
                return Task.CompletedTask;
            });
        }
    }
}