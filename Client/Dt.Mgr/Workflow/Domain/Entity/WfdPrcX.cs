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
    public partial class WfdPrcX
    {
        public static async Task<WfdPrcX> New()
        {
            var prc = new WfdPrcX(
                ID: await NewID(),
                Name: "新流程",
                IsLocked: true,
                Dispidx: await NewSeq("Dispidx"),
                Ctime: Kit.Now);

            prc.Atvs = await Table<WfdAtvX>.Create();
            prc.Trss = await Table<WfdTrsX>.Create();
            prc.AtvRoles = await Table<WfdAtvRoleX>.Create();
            prc.AttachEvent();
            return prc;
        }

        public static async Task<WfdPrcX> Get(long p_id)
        {
            var prc = await WfdPrcX.First($"where id={p_id}");
            prc.Atvs = await WfdAtvX.Query("cm_流程_编辑活动模板", new { p_prcid = p_id });
            prc.Trss = await WfdTrsX.Query($"where prcid={p_id}");
            prc.AtvRoles = await WfdAtvRoleX.Query($"select a.*,b.name as role from cm_wfd_atv_role a,cm_role b where a.roleid=b.id and atvid in (select id from cm_wfd_atv where prcid={p_id})");
            prc.AttachEvent();
            return prc;
        }

        public event EventHandler Modified;

        public Table<WfdAtvX> Atvs { get; private set; }

        public Table<WfdTrsX> Trss { get; private set; }

        public Table<WfdAtvRoleX> AtvRoles { get; private set; }

        public bool IsModified
        {
            get
            {
                // Atvs和Trss集合变化会触发CmdChanged，无需重复判断
                return IsChanged
                    || Atvs.IsChanged
                    || Trss.ExistDeleted
                    || Trss.ExistAdded
                    || AtvRoles.ExistDeleted
                    || AtvRoles.ExistAdded;
            }
        }

        protected override void InitHook()
        {
            OnDeleting(async () =>
            {
                int cnt = await WfiPrcX.GetCount($"where PrcdID={ID}");
                Throw.If(cnt > 0, "已有流程实例，禁止删除！");
            });
        }

        void AttachEvent()
        {
            Changed += (s, e) => OnModified();
            Atvs.RecordDeleted();
            Atvs.Changed += (s, e) => OnModified();

            Trss.RecordDeleted();
            Trss.CollectionChanged += (s, e) => OnModified();

            AtvRoles.RecordDeleted();
            AtvRoles.CollectionChanged += (s, e) => OnModified();
        }

        void OnModified()
        {
            Modified?.Invoke(this, EventArgs.Empty);
        }
    }
}