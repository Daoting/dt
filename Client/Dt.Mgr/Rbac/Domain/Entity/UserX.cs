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
            return new UserX(id, Name: "新用户");
        }

        public static async Task<UserX> CreateByPhone(string p_phone)
        {
            return new UserX(
                ID: await NewID(),
                Phone: p_phone,
                Name: p_phone,
                Pwd: Kit.GetMD5(p_phone.Substring(p_phone.Length - 4)));
        }

        protected override void InitHook()
        {
            OnSaving(async () =>
            {
                if (IsAdded || Cells["Name"].IsChanged)
                    Throw.IfEmpty(Name, "名称不可为空！");

                if (IsAdded || Cells["Name"].IsChanged)
                    Throw.IfEmpty(Phone, "手机号不可为空！");

                Throw.If(!Regex.IsMatch(Phone, "^1[34578]\\d{9}$"), "手机号码错误！");

                if ((IsAdded || Cells["phone"].IsChanged)
                    && await AtCm.GetScalar<int>("用户-重复手机号", new { phone = Phone }) > 0)
                {
                    Throw.Msg("手机号码重复！");
                }

                if (IsAdded)
                {
                    // 初始密码为手机号后4位
                    Pwd = Kit.GetMD5(Phone.Substring(Phone.Length - 4));
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

            // 清除以手机号为键名的缓存
            OnSaved(async () => await this.ClearCache("phone"));

            OnDeleted(async () =>
            {
                await this.ClearCache("phone");
                // 清除用户的数据版本号
                RbacDs.DelUserDataVer(ID);
            });
        }
    }
}