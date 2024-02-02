#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-01 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 内部含Fv，数据源为Row或Entity的Dlg
    /// 主要为减少无用的数据查询
    /// </summary>
    public abstract partial class FvDlg : Dlg, IFvHost
    {
        #region 构造方法
        readonly FvLazy _lazy;
        public FvDlg()
        {
            _lazy = new FvLazy(this);
            IsPinned = true;
            MinHeight = 300;
            Loaded += (s, e) => _lazy.OnOpen();
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 更新数据源
        /// <para>1. 不可见不查询，切换到可见时再查询</para>
        /// <para>2. 上次查询未结束，放弃后续的查询</para>
        /// </summary>
        /// <param name="p_id">null清空；大于0加载；≤0新增</param>
        /// <returns></returns>
        public Task Update(long? p_id)
        {
            return _lazy.OnUpdate(p_id);
        }

        /// <summary>
        /// 打开对话框，更新数据源
        /// </summary>
        /// <param name="p_id">
        /// <para>null只打开对话框，不更新数据；</para>
        /// <para>大于0更新数据源；</para>
        /// <para>≤0新增</para>
        /// </param>
        /// <returns></returns>
        public Task Open(long? p_id)
        {
            Show();
            
            if (p_id == null)
                return Task.CompletedTask;
            return _lazy.OnUpdate(p_id);
        }
        
        /// <summary>
        /// 所属Win
        /// </summary>
        public Win OwnWin { get; set; }
        #endregion

        #region 重写
        protected override Task<bool> OnClosing(bool p_result)
        {
            // 提示是否放弃修改
            return _lazy.OnClose();
        }
        #endregion

        #region 虚方法
        /// <summary>
        /// 内部Fv
        /// </summary>
        protected abstract Fv Fv { get; }

        /// <summary>
        /// 增加前选项：自动保存已修改的数据、提示、不检查
        /// </summary>
        protected FvBeforeAdd BeforeAdd { get => _lazy.BeforeAdd; set => _lazy.BeforeAdd = value; }

        /// <summary>
        /// 增加
        /// </summary>
        protected abstract Task OnAdd();

        /// <summary>
        /// 加载数据源
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        protected abstract Task OnGet(long p_id);

        /// <summary>
        /// 保存数据
        /// </summary>
        protected abstract Task<bool> OnSave();

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        protected virtual Task OnDel()
        {
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// 清空数据源
        /// </summary>
        protected abstract void Clear();

        /// <summary>
        /// 更新关联视图
        /// </summary>
        /// <param name="p_id"></param>
        protected virtual void UpdateRelated(long p_id)
        { }
        #endregion

        #region 命令方法
        /// <summary>
        /// 增加
        /// </summary>
        protected void Add()
        {
            _lazy.Add();
        }

        /// <summary>
        /// 保存
        /// </summary>
        protected void Save()
        {
            _lazy.Save();
        }

        /// <summary>
        /// 删除数据的默认流程
        /// </summary>
        protected virtual void Delete()
        {
            _lazy.Delete();
        }
        
        /// <summary>
        /// 创建默认菜单
        /// </summary>
        /// <returns></returns>
        protected Menu CreateMenu()
        {
            return _lazy.CreateMenu();
        }
        
        protected void ShowSetting()
        {
            _lazy.ShowSetting();
        }
        #endregion

        #region IFvHost
        bool IFvHost.IsOpened => IsOpened;
        
        Fv IFvHost.Fv => Fv;
        
        Task IFvHost.OnAdd() => OnAdd();

        Task IFvHost.OnGet(long p_id) => OnGet(p_id);

        Task<bool> IFvHost.OnSave() => OnSave();

        Task IFvHost.OnDel() => OnDel();

        void IFvHost.Clear() => Clear();

        void IFvHost.UpdateRelated(long p_id) => UpdateRelated(p_id);
        #endregion
    }
}
