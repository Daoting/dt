#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Caches;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm
{
    public class UserRoleCache
    {
        readonly StringCache _cache = new StringCache("ur");

        public async Task<string> GetRoles(long p_userID)
        {
            string roles = await _cache.Get<string>(p_userID.ToString());
            if (string.IsNullOrEmpty(roles))
            {
                var ls = await Bag.Dp.EachItem<long>("select roleid from cm_userrole where userid=@userid", new { userid = p_userID });
                StringBuilder sb = new StringBuilder();
                // 任何人
                sb.Append("1");
                foreach (var roleid in ls)
                {
                    sb.Append(",");
                    sb.Append(roleid.ToString());
                }
                roles = sb.ToString();
                await _cache.Set(p_userID.ToString(), roles);
            }
            return roles;
        }

        public Task<bool> Remove(long p_userID)
        {
            return _cache.Remove(p_userID.ToString());
        }
    }
}
