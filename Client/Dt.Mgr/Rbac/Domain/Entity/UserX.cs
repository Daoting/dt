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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Rbac
{
    public partial class UserX
    {
        public static async Task<UserX> New()
        {
            long id = await NewID();
            return new UserX(id);
        }

        public static async Task<UserX> CreateByPhone(string p_phone)
        {
            return new UserX(
                ID: await NewID(),
                Phone: p_phone,
                Name: p_phone,
                Pwd: Kit.GetMD5(p_phone.Substring(p_phone.Length - 4)));
        }

        public static Task<Table<UserX>> ExistsInGroup(long p_groupID)
        {
            return Query($"where exists (select user_id from cm_user_group b where a.id=b.user_id and group_id={p_groupID})");
        }

        protected override void InitHook()
        {
            OnSaving(async () =>
            {
                if (Acc == "" && Phone == "")
                {
                    Throw.Msg("账号和手机号不可同时为空！");
                }

                if (Acc != ""
                    && (IsAdded || cAcc.IsChanged))
                {
                    if (await GetCount($"where acc='{Acc}'") > 0)
                        Throw.Msg("账号重复！", cAcc);
                }

                if (Phone != ""
                    && (IsAdded || cPhone.IsChanged))
                {
                    if (!Regex.IsMatch(Phone, "^1[34578]\\d{9}$"))
                        Throw.Msg("手机号码错误！", cPhone);

                    if (await GetCount($"where phone='{Phone}'") > 0)
                        Throw.Msg("手机号码重复！", cPhone);
                }

                if (IsAdded)
                {
                    // 初始密码为4个1
                    Pwd = Kit.GetMD5("1111");
                    Ctime = Mtime = Kit.Now;
                }
                else
                {
                    Mtime = Kit.Now;
                }
            });

            OnDeleting(() =>
            {
                Throw.If(ID < 1000, "系统用户无法删除！");
                return Task.CompletedTask;
            });

            OnDeleted(() =>
            {
                // 清除用户的数据版本号
                RbacDs.DelUserDataVer(ID);
                return Task.CompletedTask;
            });
        }
    }
}