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
    public partial class WfdPrcObj
    {
        public static async Task<WfdPrcObj> New()
        {
            var prc = new WfdPrcObj(
                ID: await NewID(),
                Name: "新流程",
                IsLocked: true,
                Dispidx: await NewSeq("Dispidx"),
                Ctime: Kit.Now);

            prc.Atvs = Table<WfdAtvObj>.Create();
            prc.Trss = Table<WfdTrsObj>.Create();
            prc.AtvRoles = Table<WfdAtvRoleObj>.Create();
            prc.AttachEvent();
            return prc;
        }

        public static async Task<WfdPrcObj> Get(long p_id)
        {
            var dt = new Dict { { "prcid", p_id } };
            var prc = await AtCm.First<WfdPrcObj>("流程-编辑流程模板", dt);
            prc.Atvs = await AtCm.Query<WfdAtvObj>("流程-编辑活动模板", dt);
            prc.Trss = await AtCm.Query<WfdTrsObj>("流程-编辑迁移模板", dt);
            prc.AtvRoles = await AtCm.Query<WfdAtvRoleObj>("流程-编辑活动授权", dt);
            prc.AttachEvent();
            return prc;
        }

        public event EventHandler Modified;

        public Table<WfdAtvObj> Atvs { get; private set; }

        public Table<WfdTrsObj> Trss { get; private set; }

        public Table<WfdAtvRoleObj> AtvRoles { get; private set; }

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
            Atvs.StartRecordDelRows();
            Atvs.Changed += (s, e) => OnModified();

            Trss.StartRecordDelRows();
            Trss.CollectionChanged += (s, e) => OnModified();

            AtvRoles.StartRecordDelRows();
            AtvRoles.CollectionChanged += (s, e) => OnModified();
        }

        void OnModified()
        {
            Modified?.Invoke(this, EventArgs.Empty);
        }
    }
}