#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-24 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Mgr.Rbac;
using Dt.Mgr.Workflow;
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 流程描述信息，加载流程表单的参数
    /// </summary>
    public class WfFormInfo
    {
        #region 成员变量
        static readonly Dictionary<long, WfdPrcX> _prcDefs = new Dictionary<long, WfdPrcX>();
        #endregion

        #region 属性
        /// <summary>
        /// 获取当前状态名称（即活动名称）
        /// </summary>
        public string State => AtvDef.Name;

        /// <summary>
        /// 业务数据主键，也是流程实例主键
        /// </summary>
        public long ID => PrcInst.ID;

        /// <summary>
        /// 是否为新表单
        /// </summary>
        public bool IsNew => PrcInst.IsAdded;

        /// <summary>
        /// 表单是否只读状态
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// 获取流程模板定义
        /// </summary>
        public WfdPrcX PrcDef { get; private set; }

        /// <summary>
        /// 获取当前活动定义
        /// </summary>
        public WfdAtvX AtvDef { get; private set; }

        /// <summary>
        /// 获取流程实例
        /// </summary>
        public WfiPrcX PrcInst { get; private set; }

        /// <summary>
        /// 获取当前活动实例
        /// </summary>
        public WfiAtvX AtvInst { get; private set; }

        /// <summary>
        /// 获取当前工作项
        /// </summary>
        public WfiItemX WorkItem { get; private set; }

        /// <summary>
        /// 后续活动
        /// </summary>
        public Table<WfdAtvX> NextAtvs { get; private set; }

        /// <summary>
        /// 获取是否为回退活动
        /// </summary>
        public bool IsRollback
        {
            get { return WorkItem != null && WorkItem.AssignKind == WfiItemAssignKind.追回; }
        }

        /// <summary>
        /// 是否为开始活动工作项
        /// </summary>
        public bool IsStartItem
        {
            get { return AtvDef.Type == WfdAtvType.Start; }
        }

        /// <summary>
        /// 后续活动是否为结束活动(完成)
        /// </summary>
        public bool IsLastAtv => NextAtvs != null && NextAtvs.Count == 1 && NextAtvs[0].Type == WfdAtvType.Finish;

        /// <summary>
        /// 获取后续活动的接收者列表
        /// </summary>
        public AtvRecvs NextRecvs { get; internal set; }

        /// <summary>
        /// 流程表单界面
        /// </summary>
        internal WfForm Form { get; set; }
        #endregion

        #region 内部方法
        /// <summary>
        /// 创建迁移实例
        /// </summary>
        /// <param name="p_tatvid">目标活动模板标识</param>
        /// <param name="p_tatviid">目标活动实例标识</param>
        /// <param name="p_date">创建时间</param>
        /// <param name="p_rollback">是否回退</param>
        /// <returns></returns>
        internal async Task<WfiTrsX> CreateAtvTrs(long p_tatvid, long p_tatviid, DateTime p_date, bool p_rollback)
        {
            long trsdid = await WfdDs.GetWfdTrsID(PrcInst.PrcdID, AtvInst.AtvdID, p_tatvid, p_rollback);
            Throw.If(trsdid == 0, "未找到流程迁移模板");

            return await WfiTrsX.New(
                TrsdID: trsdid,
                SrcAtviID: AtvInst.ID,
                TgtAtviID: p_tatviid,
                IsRollback: p_rollback,
                Ctime: p_date);
        }

        /// <summary>
        /// 加载流程定义
        /// </summary>
        /// <param name="p_prcID">流程标识</param>
        /// <returns></returns>
        internal static async Task<WfdPrcX> GetPrcDef(long p_prcID)
        {
            WfdPrcX def;
            if (!_prcDefs.TryGetValue(p_prcID, out def))
            {
                def = await WfdPrcX.GetByID(p_prcID);
                _prcDefs[p_prcID] = def;
            }
            return def;
        }

        /// <summary>
        /// 加载流程定义
        /// </summary>
        /// <param name="p_prcName"></param>
        /// <returns></returns>
        internal static async Task<WfdPrcX> GetPrcDef(string p_prcName)
        {
            WfdPrcX def = (from pr in _prcDefs
                           where pr.Value.Name == p_prcName
                           select pr.Value).FirstOrDefault();
            if (def == null)
            {
                def = await WfdPrcX.GetByKey("name", p_prcName);
                if (def != null)
                    _prcDefs[def.ID] = def;
            }
            return def;
        }

        /// <summary>
        /// 是否允许回退
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> AllowRollback()
        {
            if (AtvDef.Type == WfdAtvType.Start || AtvInst.InstCount > 1)
                return false;
            var pre = await AtvInst.GetRollbackAtv();
            return pre != null;
        }

        internal void CloseForm()
        {
            Form.Close();
        }
        #endregion

        #region 初始化
        /// <summary>
        /// 1. itemID > 0 时，其余两项无效，以当前工作项为标准
        /// 2. prciID > 0 时，以该流程实例的最后工作项为标准
        /// 3. 提供流程名称时，创建新工作项、流程实例、起始活动实例
        /// </summary>
        /// <param name="p_itemID">工作项标识</param>
        /// <param name="p_prciID">流程实例标识</param>
        /// <param name="p_prcName">流程名称</param>
        internal async Task Init(long p_itemID, long p_prciID, string p_prcName)
        {
            if (p_itemID > 0)
            {
                // 当前工作项
                WorkItem = await WfiItemX.GetByID(p_itemID);
                AtvInst = await WfiAtvX.First($"where id={WorkItem.AtviID}");
                AtvDef = await WfdAtvX.GetByID(AtvInst.AtvdID);
                PrcInst = await WfiPrcX.First($"where id={AtvInst.PrciID}");
                PrcDef = await GetPrcDef(PrcInst.PrcdID);
            }
            else if (p_prciID > 0)
            {
                // 根据流程实例id获取最后工作项
                WorkItem = await WfiItemX.GetLastItem(p_prciID);
                AtvInst = await WfiAtvX.First($"where id={WorkItem.AtviID}");
                AtvDef = await WfdAtvX.GetByID(AtvInst.AtvdID);
                PrcInst = await WfiPrcX.First($"where id={p_prciID}");
                PrcDef = await GetPrcDef(PrcInst.PrcdID);
            }
            else if (!string.IsNullOrEmpty(p_prcName))
            {
                // 创建新工作项、流程实例、起始活动实例
                PrcDef = await GetPrcDef(p_prcName);
                AtvDef = await WfdAtvX.First($"where prc_id={PrcDef.ID} and type=1");

                PrcInst = await WfiPrcX.New(
                    PrcdID: PrcDef.ID,
                    Name: PrcDef.Name);

                AtvInst = await WfiAtvX.New(
                    PrciID: PrcInst.ID,
                    AtvdID: AtvDef.ID,
                    InstCount: 1);

                WorkItem = await WfiItemX.New(
                    AtviID: AtvInst.ID,
                    AssignKind: WfiItemAssignKind.起始指派,
                    IsAccept: true,
                    Status: WfiItemStatus.活动,
                    UserID: Kit.UserID,
                    SenderID: Kit.UserID,
                    Sender: Kit.UserName);
            }
            else
            {
                Throw.Msg("WfFormInfo 实例的所有标识都无效！");
            }

            // 获得所有后续活动，包括同步
            NextAtvs = await WfdAtvX.GetNextAtv(AtvDef.ID);

            // 确定表单是否可编辑
            if (WorkItem.Status == WfiItemStatus.活动
                && (WorkItem.UserID == Kit.UserID
                    || (WorkItem.RoleID.HasValue && await RbacDs.ExistsUserRole(Kit.UserID, WorkItem.RoleID.Value))))
            {
                IsReadOnly = false;
            }
            else
            {
                IsReadOnly = true;
            }

            // 自动签收
            if (!IsNew
                && !IsReadOnly
                && AtvDef.AutoAccept
                && !WorkItem.IsAccept)
            {
                WorkItem.IsAccept = true;
                WorkItem.AcceptTime = Kit.Now;
                if (await WorkItem.Save(false))
                    Kit.Msg("已自动签收！");
            }
        }
        #endregion
    }
}
