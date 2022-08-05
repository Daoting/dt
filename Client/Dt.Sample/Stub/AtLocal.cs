#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022/8/4 13:04:15 创建
******************************************************************************/
#endregion

namespace Dt.Sample
{
    /// <summary>
    /// 本地sqlite库，文件名 local.db
    /// </summary>
    public class AtLocal : SqliteProvider<Sqlite_local>
    {
        /// <summary>
        /// 查询本地库的字典值
        /// </summary>
        /// <param name="p_key"></param>
        /// <returns></returns>
        public static string GetDict(string p_key)
        {
            string val = _db.GetScalar<string>($"select val from LocalDict where key='{p_key}'");
            return val == null ? string.Empty : val;
        }

        /// <summary>
        /// 保存字典行
        /// </summary>
        /// <param name="p_key"></param>
        /// <param name="p_val"></param>
        public static void SaveDict(string p_key, string p_val)
        {
            _db.Execute($"insert OR REPLACE into LocalDict (key,val) values ('{p_key}','{p_val}')");
        }

        /// <summary>
        /// 删除指定字典行
        /// </summary>
        /// <param name="p_key"></param>
        public static void DeleteDict(string p_key)
        {
            _db.Execute($"delete from LocalDict where key='{p_key}'");
        }

    }

    public class Sqlite_local
    { }
}
