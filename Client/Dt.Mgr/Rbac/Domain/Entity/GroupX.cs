#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Rbac
{
    public partial class GroupX
    {
        public static async Task<GroupX> New(
            string Name = default,
            string Note = default)
        {
            return new GroupX(
                ID: await NewID(),
                Name: "新分组",
                Note: Note);
        }

        /// <summary>
        /// 分组_关联用户
        /// </summary>
        /// <param name="p_groupid"></param>
        /// <returns></returns>
        public static Task<List<long>> GetRelationUserIDs(long p_groupid)
        {
            return AtCm.FirstCol<long>($"select id,name,phone from cm_user a where exists (select user_id from cm_user_group b where a.id=b.user_id and group_id={p_groupid})");
        }

        protected override void InitHook()
        {
            OnSaving(() =>
            {
                Throw.IfEmpty(Name, "分组名称不可为空！");
                return Task.CompletedTask;
            });

            OnDeleting(async () =>
            {
                Throw.If(ID < 1000, "系统分组无法删除！");

                // 清除关联用户的数据版本号，没放在 OnDeleted 处理因为cm_user_group有级联删除
                var users = await GetRelationUserIDs(ID);
                RbacDs.DelUserDataVer(users);
            });
        }
    }
}