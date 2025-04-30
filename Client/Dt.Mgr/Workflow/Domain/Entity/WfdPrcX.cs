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
            prc.Atvs = await WfdAtvX.Query($"where prc_id={p_id}");
            prc.Trss = await WfdTrsX.Query($"where prc_id={p_id}");
            prc.AtvRoles = await WfdAtvRoleX.Query($"select a.*,b.name as role from cm_wfd_atv_role a,cm_role b where a.role_id=b.id and atv_id in (select id from cm_wfd_atv where prc_id={p_id})");
            prc.AttachEvent();
            return prc;
        }

        public event Action Modified;

        public Table<WfdAtvX> Atvs { get; private set; }

        public Table<WfdTrsX> Trss { get; private set; }

        public Table<WfdAtvRoleX> AtvRoles { get; private set; }

        public bool IsModified
        {
            get
            {
                // Atvs和Trss集合变化会触发CmdChanged，无需重复判断
                return IsChanged
                    || Atvs.IsDirty
                    || Trss.IsDirty
                    || AtvRoles.IsDirty;
            }
        }

        protected override void InitHook()
        {
            OnSaving(async () =>
            {
                Throw.IfEmpty(Name, "流程名称不可为空！", cName);

                if ((IsAdded || Cells["name"].IsChanged)
                    && await GetCount($"where name='{Name}'") > 0)
                {
                    Throw.Msg("流程名称重复！", cName);
                }

                if (!IsAdded && Cells["name"].IsChanged)
                {
                    if (!await Kit.Confirm("流程名称可能被流程表单引用，\r\n确认要修改吗？"))
                        Throw.Msg("已取消保存！");
                }
                
                if (IsAdded)
                {
                    Ctime = Mtime = Kit.Now;
                }
                else
                {
                    Mtime = Kit.Now;
                }
            });
            
            OnDeleting(async () =>
            {
                int cnt = await WfiPrcX.GetCount($"where prcd_id={ID}");
                Throw.If(cnt > 0, "已有流程实例，禁止删除！");
            });
        }

        void AttachEvent()
        {
            Changed += (s, e) => OnModified();
            Atvs.LockCollection();
            Atvs.Changed += (s, e) => OnModified();

            Trss.LockCollection();
            Trss.CollectionChanged += (s, e) => OnModified();

            AtvRoles.LockCollection();
            AtvRoles.CollectionChanged += (s, e) => OnModified();
        }

        void OnModified()
        {
            Modified?.Invoke();
        }
    }
}