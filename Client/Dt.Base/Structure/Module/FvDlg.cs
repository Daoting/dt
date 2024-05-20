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
        #region 变量
        readonly FvLazy _lazy;
        Win _ownWin;
        #endregion

        #region 构造方法
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
        /// 打开对话框，更新数据源，关闭时返回true表示成功提交过数据(包括增删改)
        /// </summary>
        /// <param name="p_id">null只打开对话框，不更新数据；大于0更新数据源；≤0新增</param>
        /// <returns>true 成功提交过数据(包括增删改)，常用来判断关闭后是否需要刷新列表</returns>
        public async Task<bool> Open(long? p_id)
        {
            // 非null更新数据源
            if (p_id != null)
                await _lazy.OnUpdate(p_id);
            
            await ShowAsync();
            return IsModified;
        }

        /// <summary>
        /// 表单是否成功提交过数据(包括增删改)，常用作判断是否需要刷新列表
        /// </summary>
        public bool IsModified => _lazy.IsSaved || _lazy.IsDeleted;

        /// <summary>
        /// 表单是否成功保存过数据
        /// </summary>
        public bool IsSaved => _lazy.IsSaved;

        /// <summary>
        /// 表单是否成功删除过数据
        /// </summary>
        public bool IsDeleted => _lazy.IsDeleted;

        /// <summary>
        /// 所属Win
        /// </summary>
        public Win OwnWin
        {
            get { return _ownWin; }
            set
            {
                if (_ownWin != value)
                {
                    if (_ownWin != null)
                        _ownWin.Closed -= OnOwnWinClosed;
                    _ownWin = value;
                    _ownWin.Closed += OnOwnWinClosed;
                }
            }
        }

        void OnOwnWinClosed(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        #region 重写
        protected override Task<bool> OnClosing(bool p_result)
        {
            // 提示是否放弃修改
            return _lazy.OnClose();
        }

        protected override void OnClosed(bool p_result)
        {
            ((IFvHost)this).RefreshList(null, FvRefreshList.DlgClosed);
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
        /// 切换数据源或关闭前是否检查、提示旧数据已修改
        /// </summary>
        protected bool CheckChanges { get => _lazy.CheckChanges; set => _lazy.CheckChanges = value; }

        /// <summary>
        /// 刷新列表的时机，默认保存或删除后实时刷新列表
        /// </summary>
        protected RefreshListOption RefreshListOption { get; set; }

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
        protected virtual Task<bool> OnSave()
        {
            if (Fv.Data is Entity entity)
                return entity.Save();
            return Task.FromResult(false);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        protected virtual Task<bool> OnDel()
        {
            if (Fv.Data is Entity entity)
                return entity.Delete();
            return Task.FromResult(false);
        }

        /// <summary>
        /// 清空数据源
        /// </summary>
        protected virtual void Clear()
        {
            Fv.Data = null;
        }

        /// <summary>
        /// 更新列表
        /// </summary>
        /// <param name="p_id"></param>
        protected virtual void RefreshList(long? p_id)
        { }
        
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
        /// 添加默认菜单项，若提供Menu则添加默认项
        /// </summary>
        /// <param name="p_menu">若提供Menu则添加默认项，否则创建新菜单</param>
        /// <returns></returns>
        protected Menu CreateMenu(Menu p_menu = null)
        {
            return _lazy.CreateMenu(p_menu);
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

        async Task IFvHost.OnGet(long p_id)
        {
            await OnGet(p_id);
            UpdateRelated(p_id);
        }

        Task<bool> IFvHost.OnSave() => OnSave();

        async Task<bool> IFvHost.OnDel()
        {
            bool suc = await OnDel();
            if (suc)
            {
                Clear();
                RefreshList(-1);
            }
            return suc;
        }

        void IFvHost.Clear()
        {
            Clear();
            UpdateRelated(-1);
        }

        void IFvHost.RefreshList(long? p_id, FvRefreshList p_fire)
        {
            if (RefreshListOption == RefreshListOption.None)
                return;
            
            if ((RefreshListOption == RefreshListOption.Realtime && p_fire != FvRefreshList.DlgClosed)
                || (RefreshListOption == RefreshListOption.DlgClosed && p_fire == FvRefreshList.DlgClosed))
            {
                RefreshList(p_id);
            }
        }
        
        void IFvHost.UpdateRelated(long p_id) => UpdateRelated(p_id);
        #endregion
    }
}
