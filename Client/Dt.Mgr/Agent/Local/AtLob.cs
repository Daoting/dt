#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 本地业务库
    /// </summary>
    public class AtLob : SqliteProvider<Sqlite_lob>
    {
        /// <summary>
        /// 查询指定表的所有列
        /// </summary>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        public static IEnumerable<OmColumn> EachColumns(string p_tblName)
        {
            return _db.ForEach<OmColumn>($"select * from OmColumn where tabname='{p_tblName}'");
        }

    }

    public class Sqlite_lob
    { }
}
