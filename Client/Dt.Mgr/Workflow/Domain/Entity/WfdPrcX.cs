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

            prc.Atvs = Table<WfdAtvX>.Create();
            prc.Trss = Table<WfdTrsX>.Create();
            prc.AtvRoles = Table<WfdAtvRoleX>.Create();
            prc.AttachEvent();
            return prc;
        }

        public static async Task<WfdPrcX> Get(long p_id)
        {
            var dt = new Dict { { "prcid", p_id } };
            var prc = await AtCm.First<WfdPrcX>("流程-编辑流程模板", dt);
            prc.Atvs = await AtCm.Query<WfdAtvX>("流程-编辑活动模板", dt);
            prc.Trss = await AtCm.Query<WfdTrsX>("流程-编辑迁移模板", dt);
            prc.AtvRoles = await AtCm.Query<WfdAtvRoleX>("流程-编辑活动授权", dt);
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
                int cnt = await AtCm.GetScalar<int>("流程-流程实例数", new { PrcdID = ID });
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