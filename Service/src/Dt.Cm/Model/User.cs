#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 账号
    /// </summary>
    public static class Users
    {
        const string _sqlID = "select * from cm_user where id=@id";
        static Dictionary<long, User> _idDict = new Dictionary<long, User>();

        public static async Task<User> GetUser(long p_id)
        {
            User user;
            if (_idDict.TryGetValue(p_id, out user))
                return user;

            user = await new Db().First<User>(_sqlID, new { id = p_id });
            if (user != null)
                CacheUser(user);
            return user;
        }

        internal static void CacheUser(User p_user)
        {
            if (p_user != null && !_idDict.ContainsKey(p_user.ID))
                _idDict.Add(p_user.ID, p_user);
        }
    }

    /// <summary>
    /// 用户(扩展表)
    /// </summary>
    public class User
    {
        /// <summary>
        /// 用户标识
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号，唯一
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public bool Sex { get; set; }
    }
}
