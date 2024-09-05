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
    public partial class WfdTrsX
    {
        public static async Task<WfdTrsX> New(
            long PrcID = default,
            long SrcAtvID = default,
            long TgtAtvID = default,
            bool IsRollback = default,
            long? TrsID = default)
        {
            return new WfdTrsX(
                ID: await NewID(),
                PrcID: PrcID,
                SrcAtvID: SrcAtvID,
                TgtAtvID: TgtAtvID,
                IsRollback: IsRollback,
                TrsID: TrsID);
        }

        public static Task<Table<WfdTrsX>> QueryWithAtvName(long p_prcID)
        {
            return Query(string.Format(_sqlAtvName, p_prcID));
        }

        #region sql
        const string _sqlAtvName = @"
SELECT a.name src_atv,
	c.name tgt_atv,
	is_rollback 
FROM
	cm_wfd_atv a,
	cm_wfd_trs b,
	cm_wfd_atv c 
WHERE
	a.ID = b.src_atv_id 
	AND b.tgt_atv_id = c.ID
	AND b.prc_id={0}
";
        #endregion
    }
}