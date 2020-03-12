#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using System.Collections.Generic;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 当前登录用户相关信息
    /// </summary>
    public static class AtUser
    {
        #region 用户信息
        /// <summary>
        /// 用户ID
        /// </summary>
        public static long ID { get; private set; } = -1;

        /// <summary>
        /// 姓名
        /// </summary>
        public static string Name { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public static string Phone { get; set; }

        /// <summary>
        /// 是否有头像，头像在Fsm，路径sys/photo/id.jpg
        /// </summary>
        public static bool HasPhoto { get; set; }

        /// <summary>
        /// 是否已登录
        /// </summary>
        public static bool IsLogon => ID > 0;

        /// <summary>
        /// 登录后初始化用户信息
        /// </summary>
        /// <param name="p_info"></param>
        public static void Init(Dict p_info)
        {
            ID = p_info.Long("userid");
            Phone = p_info.Str("phone");
            Name = p_info.Str("name");
            HasPhoto = p_info.Bool("hasphoto");

            if (p_info.TryGetValue("roles", out var r))
                InitRoles((string)r);
            BaseRpc.RefreshHeader();
        }

        /// <summary>
        /// 注销时清空用户信息
        /// </summary>
        public static void Reset()
        {
            ID = -1;
            Name = null;
            Phone = null;
            HasPhoto = false;

            BaseRpc.RefreshHeader();
        }
        #endregion

        #region 头像路径
        /// <summary>
        /// 缺省头像文件的路径
        /// </summary>
        public const string DefaultPhotoPath = "photo/profilephoto.jpg";

        /// <summary>
        /// 获取用户头像文件的路径
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns></returns>
        public static string GetPhotoPath(long p_userID)
        {
            return $"sys/photo/{p_userID}.jpg";
        }
        #endregion

        #region 角色
        /// <summary>
        /// 用户角色列表
        /// </summary>
        public static List<long> Roles { get; private set; }

        /// <summary>
        /// 用在sql中的角色串，格式 1,2,3
        /// </summary>
        public static string SqlRoles { get; private set; }

        static void InitRoles(string p_roles)
        {
            // 任何人角色ID
            if (string.IsNullOrEmpty(p_roles))
                p_roles = "1";

            List<long> ls = new List<long>();
            long roleid;
            foreach (string id in p_roles.Split(','))
            {
                if (long.TryParse(id, out roleid))
                    ls.Add(roleid);
            }
            Roles = ls;
            SqlRoles = p_roles;
        }
        #endregion

        #region 权限
        static List<string> _prvs;

        /// <summary>
        /// 获取当前登录用户的权限列表
        /// </summary>
        public static List<string> Prvs
        {
            get
            {
                if (_prvs == null)
                {
                    _prvs = new List<string>();
                    foreach (var rp in AtLocal.QueryModel(string.Format("select PrvID from roleprv where roleid in ({0})", SqlRoles)))
                    {
                        _prvs.Add(rp.Str("PrvID"));
                    }
                }
                return _prvs;
            }
        }

        /// <summary>
        /// 判断当前登录用户是否具有指定权限
        /// </summary>
        /// <param name="p_id">权限ID</param>
        /// <returns>true 表示有权限</returns>
        public static bool HasPrv(string p_id)
        {
            return Prvs.Contains(p_id);
        }
        #endregion
    }
}
