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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 当前登录用户相关信息
    /// </summary>
    public partial class Kit
    {
        #region 用户信息
        /// <summary>
        /// 用户ID
        /// </summary>
        public static long UserID { get; private set; } = -1;

        /// <summary>
        /// 姓名
        /// </summary>
        public static string UserName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public static string UserPhone { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public static string UserPhoto { get; set; }

        /// <summary>
        /// 是否已登录
        /// </summary>
        public static bool IsLogon => UserID > 0;

        /// <summary>
        /// 成功登录后事件
        /// </summary>
        public static event Action LoginSuc;

        /// <summary>
        /// 登录后初始化用户信息
        /// </summary>
        /// <param name="p_result"></param>
        public static void InitUser(LoginResult p_result)
        {
            UserID = p_result.UserID;
            UserPhone = p_result.Phone;
            UserName = p_result.Name;
            UserPhoto = p_result.Photo;

            BaseRpc.RefreshHeader();
            if (p_result.Contains("Version"))
                UpdateDataVersion(p_result.Version);
            LoginSuc?.Invoke();
        }

        /// <summary>
        /// 注销时清空用户信息
        /// </summary>
        public static void ResetUser()
        {
            UserID = -1;
            UserName = null;
            UserPhone = null;
            UserPhoto = null;

            BaseRpc.RefreshHeader();
        }

        static void UpdateDataVersion(string p_ver)
        {
            if (!string.IsNullOrEmpty(p_ver))
            {
                var ls = p_ver.Split(',');
                var tbl = AtState.Query("select id,ver from DataVersion");
                if (tbl != null && tbl.Count > 0)
                {
                    foreach (var row in tbl)
                    {
                        if (!ls.Contains($"{row[0]}+{row[1]}"))
                        {
                            // 删除版本号，未实际删除缓存数据，待下次用到时获取新数据！
                            AtState.Exec($"delete from DataVersion where id='{row.Str(0)}'");
                        }
                    }
                }
            }
            else
            {
                // 所有缓存数据失效
                AtState.Exec("delete from DataVersion");
            }
        }
        #endregion

        #region 头像路径
        /// <summary>
        /// 缺省头像文件的路径
        /// </summary>
        public const string DefaultUserPhoto = "photo/profilephoto.jpg";
        #endregion

        #region 权限
        /// <summary>
        /// 判断当前登录用户是否具有指定权限
        /// </summary>
        /// <param name="p_id">权限ID</param>
        /// <returns>true 表示有权限</returns>
        public static async Task<bool> HasPrv(string p_id)
        {
            int cnt = AtState.GetScalar<int>("select count(*) from DataVersion where id='privilege'");
            if (cnt == 0)
            {
                // 查询服务端
                Dict dt = await new UnaryRpc(
                    "cm",
                    "UserRelated.GetPrivileges",
                    UserID
                ).Call<Dict>();

                // 记录版本号
                var ver = new DataVersion(ID: "privilege", Ver: dt.Str("ver"));
                await AtState.Save(ver, false);

                // 清空旧数据
                AtState.Exec("delete from UserPrivilege");
                // 插入新数据
                var ls = (List<string>)dt["result"];
                if (ls != null && ls.Count > 0)
                {
                    List<Dict> dts = new List<Dict>();
                    foreach (var prv in ls)
                    {
                        dts.Add(new Dict { { "prv", prv } });
                    }
                    AtState.BatchExec("insert into UserPrivilege (prv) values (:prv)", dts);
                }
            }

            return AtState.GetScalar<int>($"select count(*) from UserPrivilege where Prv='{p_id}'") > 0;
        }
        #endregion

        #region 用户参数
        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_paramID"></param>
        /// <returns></returns>
        public static async Task<T> GetParam<T>(string p_paramID)
        {
            await InitParams();

            var row = AtState.First($"select val from UserParams where id='{p_paramID}'");
            Throw.IfNull(row, $"无参数【{p_paramID}】");

            string val = row.Str(0);
            if (string.IsNullOrEmpty(val))
                return default;

            Type type = typeof(T);
            if (type == typeof(string))
                return (T)(object)val;

            object result;
            try
            {
                result = Convert.ChangeType(val, type);
            }
            catch
            {
                throw new Exception(string.Format("参数【{0}】的值转换异常：无法将【{1}】转换到【{2}】类型！", p_paramID, val, type));
            }
            return (T)result;
        }

        static async Task InitParams()
        {
            int cnt = AtState.GetScalar<int>("select count(*) from DataVersion where id='params'");
            if (cnt > 0)
                return;

            // 查询服务端
            Dict dt = await new UnaryRpc(
                    "cm",
                    "UserRelated.GetParams",
                    UserID
                ).Call<Dict>();

            // 记录版本号
            var ver = new DataVersion(ID: "params", Ver: dt.Str("ver"));
            await AtState.Save(ver, false);

            // 清空旧数据
            AtState.Exec("delete from UserParams");

            // 插入新数据
            var tbl = (Table)dt["result"];
            if (tbl != null && tbl.Count > 0)
            {
                List<Dict> dts = new List<Dict>();
                foreach (var row in tbl)
                {
                    dts.Add(new Dict { { "id", row.Str(0) }, { "val", row.Str(1) } });
                }
                AtState.BatchExec("insert into UserParams (id,val) values (:id, :val)", dts);
            }
        }
        #endregion
    }
}
