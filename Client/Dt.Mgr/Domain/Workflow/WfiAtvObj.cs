#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Domain
{
    public partial class WfiAtvObj
    {
        public static async Task<WfiAtvObj> New(
            long PrciID = default,
            long AtvdID = default,
            WfiAtvStatus Status = default,
            int InstCount = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            return new WfiAtvObj(
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

            int count = await AtCm.GetScalar<int>("流程-工作项个数", new { atviid = ID });
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
        public async Task<WfiAtvObj> GetRollbackAtv()
        {
            Dict dt = new Dict();
            dt["prciid"] = PrciID;
            dt["SrcAtvID"] = AtvdID;

            var atv = await AtCm.First<WfiAtvObj>("流程-回退活动实例", dt);
            if (atv != null && atv.Status != WfiAtvStatus.同步)
            {
                // 存在同步的活动，不允许进行回退。(优先级大于设置的可以回退)
                return atv;
            }
            return null;
        }
    }
}