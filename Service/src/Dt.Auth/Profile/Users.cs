#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Auth
{
    /// <summary>
    /// 账号
    /// </summary>
    public static class Users
    {
        const string _sqlID = "select * from auth_user where id=@id";
        const string _sqlPhone = "select * from auth_user where phone=@phone";
        static Dictionary<long, User> _idDict = new Dictionary<long, User>();
        static Dictionary<string, User> _phoneDict = new Dictionary<string, User>();

        public static async Task<User> GetUserByID(long p_id)
        {
            User user;
            if (_idDict.TryGetValue(p_id, out user))
                return user;

            user = await new Db().First<User>(_sqlID, new { id = p_id });
            if (user != null)
                CacheUser(user);
            return user;
        }

        public static async Task<User> GetUserByPhone(string p_phone)
        {
            if (string.IsNullOrEmpty(p_phone))
                return null;

            User user;
            if (_phoneDict.TryGetValue(p_phone, out user))
                return user;

            user = await new Db().First<User>(_sqlPhone, new { phone = p_phone });
            if (user != null)
                CacheUser(user);
            return user;
        }

        internal static void CacheUser(User p_user)
        {
            if (p_user == null || string.IsNullOrEmpty(p_user.Phone))
                return;

            if (!_idDict.ContainsKey(p_user.ID))
                _idDict.Add(p_user.ID, p_user);
            if (!_phoneDict.ContainsKey(p_user.Phone))
                _phoneDict.Add(p_user.Phone, p_user);
        }
    }
}
