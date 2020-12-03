#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Model;
using Dt.Core.Rpc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        /// 头像
        /// </summary>
        public static string Photo { get; set; }

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
            Photo = p_info.Str("photo");

            BaseRpc.RefreshHeader();
            UpdateDataVersion(p_info.Str("ver"));
        }

        /// <summary>
        /// 注销时清空用户信息
        /// </summary>
        public static void Reset()
        {
            ID = -1;
            Name = null;
            Phone = null;
            Photo = null;

            BaseRpc.RefreshHeader();
        }

        static void UpdateDataVersion(string p_ver)
        {
            if (!string.IsNullOrEmpty(p_ver))
            {
                var ls = p_ver.Split(',');
                var tbl = AtLocal.Query("select id,ver from DataVersion");
                if (tbl != null && tbl.Count > 0)
                {
                    foreach (var row in tbl)
                    {
                        if (!ls.Contains($"{row[0]}+{row[1]}"))
                        {
                            // 删除版本号，未实际删除缓存数据，待下次用到时获取新数据！
                            AtLocal.Execute($"delete from DataVersion where id='{row.Str(0)}'");
                        }
                    }
                }
            }
            else
            {
                // 所有缓存数据失效
                AtLocal.Execute("delete from DataVersion");
            }
        }
        #endregion

        #region 头像路径
        /// <summary>
        /// 缺省头像文件的路径
        /// </summary>
        public const string DefaultPhoto = "photo/profilephoto.jpg";
        #endregion

        #region 权限
        /// <summary>
        /// 判断当前登录用户是否具有指定权限
        /// </summary>
        /// <param name="p_id">权限ID</param>
        /// <returns>true 表示有权限</returns>
        public static async Task<bool> HasPrv(string p_id)
        {
            int cnt = AtLocal.GetScalar<int>("select count(*) from DataVersion where id='privilege'");
            if (cnt == 0)
            {
                // 查询服务端
                Dict dt = await new UnaryRpc(
                    "cm",
                    "UserRelated.GetPrivileges",
                    ID
                ).Call<Dict>();

                // 记录版本号
                var ver = new DataVersion
                {
                    ID = "privilege",
                    Ver = dt.Str("ver"),
                };
                AtLocal.Save(ver);

                // 清空旧数据
                AtLocal.Execute("delete from UserPrivilege");
                // 插入新数据
                var ls = (List<string>)dt["result"];
                if (ls != null && ls.Count > 0)
                {
                    List<Dict> dts = new List<Dict>();
                    foreach (var prv in ls)
                    {
                        dts.Add(new Dict { { "prv", prv } });
                    }
                    AtLocal.BatchExecute("insert into UserPrivilege (prv) values (:prv)", dts);
                }
            }

            return AtLocal.GetScalar<int>($"select count(*) from UserPrivilege where Prv='{p_id}'") > 0;
        }
        #endregion
    }
}
