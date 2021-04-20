#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-04-16 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text.Json;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 本地sqlite状态库
    /// </summary>
    public class AtState : SqliteProvider<Sqlite_state>
    {
        /// <summary>
        /// 查询本地存储的Cookie值
        /// </summary>
        /// <param name="p_key"></param>
        /// <returns></returns>
        public static string GetCookie(string p_key)
        {
            string val = _db.GetScalar<string>($"select val from ClientCookie where key='{p_key}'");
            return val == null ? string.Empty : val;
        }

        /// <summary>
        /// 保存Cookie的键值
        /// </summary>
        /// <param name="p_key"></param>
        /// <param name="p_val"></param>
        public static void SaveCookie(string p_key, string p_val)
        {
            _db.Execute($"insert OR REPLACE into ClientCookie (key,val) values ('{p_key}','{p_val}')");
        }

        /// <summary>
        /// 删除指定Cookie
        /// </summary>
        /// <param name="p_key"></param>
        public static void DeleteCookie(string p_key)
        {
            _db.Execute($"delete from ClientCookie where key='{p_key}'");
        }

        /// <summary>
        /// 查询自启动信息
        /// </summary>
        /// <returns></returns>
        internal static AutoStartInfo GetAutoStart()
        {
            try
            {
                string json = _db.GetScalar<string>("select val from ClientCookie where key='AutoStart'");
                return JsonSerializer.Deserialize<AutoStartInfo>(json);
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 保存自启动信息
        /// </summary>
        /// <param name="p_info"></param>
        internal static void SaveAutoStart(AutoStartInfo p_info)
        {
            if (p_info != null)
            {
                string json = JsonSerializer.Serialize(p_info, JsonOptions.UnsafeSerializer);
                _db.Execute($"insert OR REPLACE into ClientCookie (key,val) values ('AutoStart','{json}')");
            }
        }

        /// <summary>
        /// 删除自启动信息
        /// </summary>
        internal static void DelAutoStart()
        {
            _db.Execute("delete from ClientCookie where key='AutoStart'");
        }
    }

    /// <summary>
    /// Sqlite_为前缀，后面为文件名
    /// </summary>
    public class Sqlite_state
    { }
}
