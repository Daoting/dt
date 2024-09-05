#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-07 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Workflow
{
    public partial class WfiPrcX
    {
        public static async Task<WfiPrcX> New(
            long PrcdID = default,
            string Name = default,
            WfiPrcStatus Status = default,
            int Dispidx = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            long id = await NewID();
            return new WfiPrcX(id, PrcdID, Name, Status, Dispidx, Ctime, Mtime);
        }

        public static async Task<Table<WfiPrcX>> Search(long p_prcdid, DateTime p_start, DateTime p_end, int p_status, string p_title)
        {
            var sql = Sql查询实例;
            var db = await At.GetDbType();
            if (db == DatabaseType.Oracle)
                sql = sql.Replace("@", ":");

            return await Query(sql, new
            {
                prcdid = p_prcdid,
                start = p_start,
                end = p_end,
                status = p_status,
                title = p_title,
            });
        }

        protected override void InitHook()
        {
        }

        #region Sql
        const string Sql查询实例 = @"
select * from cm_wfi_prc
where
	(prcd_id = @prcdid or @prcdid = 0)
	and ( @status > 2 or status = @status )
	and ( @title = '' or name = @title )
	and ( @start < '1900-01-01' or Mtime >= @start )
	and ( @end < '1900-01-01' or Mtime <= @end )
order by
	dispidx
";
        #endregion
    }
}