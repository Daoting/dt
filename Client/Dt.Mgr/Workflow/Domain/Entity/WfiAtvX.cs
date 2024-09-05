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
    public partial class WfiAtvX
    {
        public static async Task<WfiAtvX> New(
            long PrciID = default,
            long AtvdID = default,
            WfiAtvStatus Status = default,
            int InstCount = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            return new WfiAtvX(
                ID: await NewID(),
                PrciID: PrciID,
                AtvdID: AtvdID,
                Status: Status,
                InstCount: InstCount,
                Ctime: Ctime,
                Mtime: Mtime);
        }

        /// <summary>
        /// 判断当前活动是否完成，发送者是否为当前活动的最后一个发送者
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsFinished()
        {
            if (InstCount == 1)
                return true;

            int count = await WfiItemX.GetCount($"where atvi_id={ID} and status=1");
            return (count + 1) >= InstCount;
        }

        /// <summary>
        /// 结束当前活动
        /// </summary>
        public void Finished()
        {
            Status = WfiAtvStatus.结束;
            Mtime = Kit.Now;
        }

        /// <summary>
        /// 获得当前活动的回退活动
        /// </summary>
        /// <returns></returns>
        public async Task<WfiAtvX> GetRollbackAtv()
        {
            // 回退活动实例
            var atv = await WfiAtvX.First($"where prci_id={PrciID} and exists (select tgt_atv_id from cm_wfd_trs b where src_atv_id={AtvdID} and b.is_rollback='1' and a.atvd_id=b.tgt_atv_id) order by mtime desc");
            if (atv != null && atv.Status != WfiAtvStatus.同步)
            {
                // 存在同步的活动，不允许进行回退。(优先级大于设置的可以回退)
                return atv;
            }
            return null;
        }
    }
}