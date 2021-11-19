#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-11-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Workflow
{
    public partial class WfdPrcObj
    {
        public static async Task<WfdPrcObj> New()
        {
            var prc = new WfdPrcObj(
                ID: await AtCm.NewID(),
                Name: "新流程",
                IsLocked: true,
                Dispidx: await AtCm.NewSeq("sq_wfd_prc"),
                Ctime: Kit.Now);

            prc.Atvs = Table<WfdAtvObj>.Create();
            prc.Trss = Table<WfdTrsObj>.Create();
            prc.AtvRoles = Table<WfdAtvroleObj>.Create();
            prc.AttachEvent();
            return prc;
        }

        public static async Task<WfdPrcObj> Get(long p_id)
        {
            var dt = new Dict { { "prcid", p_id } };
            var prc = await AtCm.First<WfdPrcObj>("流程-编辑流程模板", dt);
            prc.Atvs = await AtCm.Query<WfdAtvObj>("流程-编辑活动模板", dt);
            prc.Trss = await AtCm.Query<WfdTrsObj>("流程-编辑迁移模板", dt);
            prc.AtvRoles = await AtCm.Query<WfdAtvroleObj>("流程-编辑活动授权", dt);
            prc.AttachEvent();
            return prc;
        }

        public event EventHandler Modified;

        public Table<WfdAtvObj> Atvs { get; private set; }

        public Table<WfdTrsObj> Trss { get; private set; }

        public Table<WfdAtvroleObj> AtvRoles { get; private set; }

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

        async Task OnDeleting()
        {
            int cnt = await AtCm.GetScalar<int>("流程-流程实例数", new { PrcdID = ID });
            Throw.If(cnt > 0, "已有流程实例，禁止删除！");
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

    #region 自动生成
    [Tbl("cm_wfd_prc")]
    public partial class WfdPrcObj : Entity
    {
        #region 构造方法
        WfdPrcObj() { }

        public WfdPrcObj(
            long ID,
            string Name = default,
            string Diagram = default,
            bool IsLocked = default,
            bool Singleton = default,
            string FormType = default,
            string ListType = default,
            string Note = default,
            int Dispidx = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            AddCell<long>("ID", ID);
            AddCell<string>("Name", Name);
            AddCell<string>("Diagram", Diagram);
            AddCell<bool>("IsLocked", IsLocked);
            AddCell<bool>("Singleton", Singleton);
            AddCell<string>("FormType", FormType);
            AddCell<string>("ListType", ListType);
            AddCell<string>("Note", Note);
            AddCell<int>("Dispidx", Dispidx);
            AddCell<DateTime>("Ctime", Ctime);
            AddCell<DateTime>("Mtime", Mtime);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 流程名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 流程图
        /// </summary>
        public string Diagram
        {
            get { return (string)this["Diagram"]; }
            set { this["Diagram"] = value; }
        }

        /// <summary>
        /// 锁定标志，0表未锁定；1表锁定，不能创建流程实例，已启动的流程实例继续执行
        /// </summary>
        public bool IsLocked
        {
            get { return (bool)this["IsLocked"]; }
            set { this["IsLocked"] = value; }
        }

        /// <summary>
        /// 同一时刻只允许有一个激活的流程实例，0表非单实例，1表单实例
        /// </summary>
        public bool Singleton
        {
            get { return (bool)this["Singleton"]; }
            set { this["Singleton"] = value; }
        }

        /// <summary>
        /// 表单类型
        /// </summary>
        public string FormType
        {
            get { return (string)this["FormType"]; }
            set { this["FormType"] = value; }
        }

        /// <summary>
        /// 表单查询类型
        /// </summary>
        public string ListType
        {
            get { return (string)this["ListType"]; }
            set { this["ListType"] = value; }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Note
        {
            get { return (string)this["Note"]; }
            set { this["Note"] = value; }
        }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)this["Dispidx"]; }
            set { this["Dispidx"] = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["Ctime"]; }
            set { this["Ctime"] = value; }
        }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["Mtime"]; }
            set { this["Mtime"] = value; }
        }
        #endregion
    }
    #endregion
}