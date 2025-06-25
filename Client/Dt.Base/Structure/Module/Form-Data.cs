#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 表单对话框
    /// </summary>
    public partial class Form : Dlg
    {
        #region 变量
        protected LvMsgArgs _args;
        protected readonly Locker _locker = new Locker();
        bool _toggling;
        bool _needUpdate;
        #endregion

        #region 构造方法
        public Form()
        {
            IsPinned = true;
            Loaded += (s, e) => OnLoaded();
        }
        #endregion

        #region 事件
        /// <summary>
        /// 更新列表事件，实体成功保存、删除后：
        /// <para>1. win模式无遮罩，实时触发</para>
        /// <para>2. phone模式或有遮罩时，对话框关闭后触发</para>
        /// </summary>
        public event Action<UpdateListArgs> UpdateList;

        /// <summary>
        /// 实体增加、切换、删除、保存后更新关联视图的事件
        /// <para>1. 对话框不可见时始终触发</para>
        /// <para>2. win模式无遮罩时触发</para>
        /// </summary>
        public event Action<UpdateRelatedArgs> UpdateRelated;

        /// <summary>
        /// 数据源修改状态变化事件
        /// </summary>
        public event Action<bool> Dirty;
        #endregion

        #region 外部
        /// <summary>
        /// 更新数据源，FvArgs.Action 控制行为
        /// </summary>
        /// <param name="p_args">null清空；ID大于0更新数据源；≤0新增</param>
        /// <returns></returns>
        public virtual Task Query(LvMsgArgs p_args)
        {
            if (_args == p_args)
                return Task.CompletedTask;

            _args = p_args;
            if (_args != null)
            {
                if (_args.Action == FormAction.Open)
                {
                    Show();
                }
                else if (_args.Action == FormAction.Close)
                {
                    Close();
                    return Task.CompletedTask;
                }
            }
            return Refresh();
        }

        /// <summary>
        /// 更新数据源，控制是否同步打开对话框
        /// <para>1. 不可见不查询，切换到可见时再查询</para>
        /// <para>2. 上次查询未结束，放弃后续的查询</para>
        /// </summary>
        /// <param name="p_id">大于0更新数据源；≤0新增</param>
        /// <param name="p_openDlg">是否打开对话框</param>
        /// <returns></returns>
        public Task Query(long p_id, bool p_openDlg = true)
        {
            return Query(new LvMsgArgs
            {
                ID = p_id,
                Action = p_openDlg ? FormAction.Open : FormAction.None
            });
        }

        /// <summary>
        /// 刷新数据
        /// <para>1. 不可见不查询但更新关联视图，可见时再查询</para>
        /// <para>2. 上次查询未结束，放弃后续的查询</para>
        /// </summary>
        /// <returns></returns>
        public async Task Refresh()
        {
            long? lastID = (_args == null || _args.Action == FormAction.Clear) ? null : _args.ID;

            // 不可见
            if (!IsOpened)
            {
                _needUpdate = true;
                // 虽不可见仍需更新关联视图
                UpdateRelatedInternal(lastID > 0 ? lastID.Value : -1, null, UpdateRelatedEvent.DlgClosed);
                return;
            }

            _needUpdate = false;
            var d = MainFv.Row;

            // 与当前数据id相同，不再重复加载
            if (d != null && d.ID == lastID)
                return;

            // 清空
            if (lastID == null)
            {
                // 新增的无需清空
                if (d != null && !d.IsAdded)
                {
                    OnClear();
                    UpdateRelatedInternal(-1, null, UpdateRelatedEvent.Clear);
                }
                return;
            }

            // 新增
            if (lastID <= 0)
            {
                Add();
                return;
            }

            // 屏蔽两种情况：
            // 1. 因切换太快上次未完成加载
            // 2. 数据已修改提示保存时，屏蔽多次弹框
            if (_toggling)
            {
                //Kit.Debug("FvTab跳过");
                return;
            }

            // 当前数据需要保存
            if (d != null && IsDirty && CheckChanges)
            {
                // 屏蔽多次弹框
                _toggling = true;
                bool save = await Kit.Confirm("数据已修改，确认要保存吗？");
                _toggling = false;
                if (save)
                {
                    // 保存失败，不继续
                    if (!await SaveInternal())
                        return;
                }
            }

            try
            {
                _toggling = true;
                await GetInternal();

                // 切换太快，加载的并非最后切换
                d = MainFv.Row;
                if (d != null && d.ID != lastID)
                {
                    //Kit.Debug("FvTab两次");
                    await Task.Delay(500);
                    await GetInternal();
                }
            }
            finally
            {
                _toggling = false;
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 表单是否成功提交过数据(包括增删改)，常用作判断是否需要刷新列表
        /// </summary>
        public bool IsModified => IsSaved || IsDeleted;

        /// <summary>
        /// 表单是否成功保存过数据
        /// </summary>
        public bool IsSaved { get; private set; }

        /// <summary>
        /// 表单是否成功删除过数据
        /// </summary>
        public bool IsDeleted { get; private set; }

        /// <summary>
        /// 表单及子列表是否只读
        /// </summary>
        public bool IsReadOnly
        {
            get { return MainFv == null ? true : MainFv.IsReadOnly; }
            set
            {
                if (MainFv == null)
                    return;

                MainFv.IsReadOnly = value;
                if (Items.Count > 0)
                {
                    foreach (var fi in Items)
                    {
                        fi.IsReadOnly = value;
                    }
                }
            }
        }
        #endregion

        #region 重写
        protected override async Task<bool> OnClosing(bool p_result)
        {
            if (CheckChanges && IsDirty)
            {
                // 提示是否放弃修改
                if (await Kit.Confirm("数据未保存，确认要丢弃所有修改吗？"))
                {
                    _args = null;
                    MainFv.Data = null;

                    // 清空子列表
                    if (Items.Count > 0)
                    {
                        foreach (var item in Items)
                        {
                            item.ClearData();
                        }
                    }
                    return true;
                }
                return false;
            }
            return true;
        }

        protected override void OnClosed(bool p_result)
        {
            // phone模式或有遮罩时，对话框关闭后触发
            if (IsModified
                && (Kit.IsPhoneUI || ShowVeil))
            {
                OnUpdateList(new UpdateListArgs
                {
                    ID = -1,
                    Event = UpdateListEvent.DlgClosed
                });
            }
        }
        #endregion

        #region 虚方法
        /// <summary>
        /// 增加实体
        /// </summary>
        protected virtual Task OnAdd() => Task.CompletedTask;

        /// <summary>
        /// 加载实体数据源
        /// </summary>
        /// <returns></returns>
        protected virtual Task OnGet() => Task.CompletedTask;

        /// <summary>
        /// 增加子实体
        /// </summary>
        /// <param name="p_fv"></param>
        /// <returns></returns>
        protected virtual Task OnAddChild(Fv p_fv) => Task.CompletedTask;

        /// <summary>
        /// 清空数据源
        /// </summary>
        protected virtual void OnClear()
        {
            MainFv.Data = null;

            // 清空子列表
            if (Items.Count > 0)
            {
                foreach (var item in Items)
                {
                    item.ClearData();
                }
            }
        }

        /// <summary>
        /// 实体保存、删除后更新列表，也可附加 UpdateList 事件
        /// </summary>
        /// <param name="p_args"></param>
        protected virtual void OnUpdateList(UpdateListArgs p_args)
        {
            UpdateList?.Invoke(p_args);
        }

        /// <summary>
        /// 实体增加、切换、删除、保存后更新关联视图，也可附加 UpdateRelated 事件
        /// </summary>
        /// <param name="p_args"></param>
        protected virtual void OnUpdateRelated(UpdateRelatedArgs p_args)
        {
            UpdateRelated?.Invoke(p_args);
        }

        /// <summary>
        /// 每次更新数据源 (OnAdd OnGet) 前调用，如初始化参数
        /// </summary>
        protected virtual Task OnLoading()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 每次数据源更新 (OnAdd OnGet) 完毕后调用，可处理：
        /// <para>1. 菜单状态</para>
        /// <para>2. 表单是否只读</para>
        /// <para>3. 根据状态控制表单的显示项等</para>
        /// <para>未用 OnLoaded 因容易重名！</para>
        /// </summary>
        protected virtual Task OnLoad()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 主Fv数据源切换、删除或保存后的处理
        /// </summary>
        protected virtual void OnFvDataChanged()
        {
        }

        /// <summary>
        /// 获取Fv当前实体的数据访问对象
        /// </summary>
        /// <returns></returns>
        async Task<IDataAccess> GetDataAccess()
        {
            if (MainFv.Data is Entity entity)
            {
                Type tp = entity.GetType();
                if (Entity.IsVirEntity(tp))
                {
                    var vm = await VirEntitySchema.Get(tp);
                    return vm.AccessInfo.GetDa();
                }
                
                var model = await EntitySchema.Get(tp);
                return model.AccessInfo.GetDa();
            }
            return At.AccessInfo.GetDa();
        }
        #endregion

        #region 增加
        /// <summary>
        /// 增加命令
        /// </summary>
        protected void Add()
        {
            _locker.Call(async () =>
            {
                if (BeforeAdd != BeforeAddOption.None)
                {
                    var d = MainFv.Row;
                    if (d != null && d.IsChanged)
                    {
                        if (BeforeAdd == BeforeAddOption.AutoSave
                            || await Kit.Confirm("数据已修改，确认要保存吗？"))
                        {
                            if (!await SaveInternal())
                            {
                                // 保存失败，放弃增加
                                return;
                            }
                            // 刷新列表
                            UpdateListInternal(d, UpdateListEvent.Saved);
                        }
                    }
                }

                await OnLoading();
                await OnAdd();

                // OnAdd中可能关闭Form
                if (IsOpened)
                {
                    await OnLoad();

                    // 清空子列表
                    if (Items.Count > 0)
                    {
                        foreach (var item in Items)
                        {
                            item.ClearData();
                        }
                    }
                    UpdateRelatedInternal(-1, MainFv.Row, UpdateRelatedEvent.Add);
                }
            });
        }
        #endregion

        #region 保存
        /// <summary>
        /// 保存命令
        /// </summary>
        protected virtual void Save()
        {
            _locker.Call(async () =>
            {
                bool suc = await SaveInternal();
                if (suc)
                {
                    IsSaved = true;

                    // Entity类型已统一在Saved事件中处理
                    if (MainFv.Data is not Entity && MainFv.Data is Row d)
                    {
                        UpdateRelatedInternal(d.ID, d, UpdateRelatedEvent.Saved);
                        UpdateListInternal(d, UpdateListEvent.Saved);
                        OnFvDataChanged();
                    }
                }
                OnSaved(suc);
            });
        }

        /// <summary>
        /// 保存实体数据的完整过程
        /// </summary>
        /// <param name="w">实体写入器，所有需要增删改的实体在一个事务内保存到db</param>
        /// <param name="p_isCommit">是否提交事务，如流程表单保存时需要在同一事务保存流程相关数据，不能提交</param>
        /// <param name="p_isNotify">是否提醒保存结果</param>
        /// <returns></returns>
        public async Task<bool> DoSave(IEntityWriter w, bool p_isCommit, bool p_isNotify)
        {
            if (!await OnSave(w))
                return false;

            if (IsEntityData())
            {
                var entity = MainFv.Data as Entity;
                await w.Save(entity);

                var ls = GetDirtyItemsData();
                if (ls.Count > 0)
                {
                    foreach (var tbl in ls)
                    {
                        await w.Save(tbl);
                    }
                }
                
                if (p_isCommit)
                    return await w.Commit(p_isNotify);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 保存实体数据，根据返回值确定是否继续执行默认保存过程，使用场景：
        /// <para>1. 数据校验，失败时Throw.Msg抛出异常提示信息，若只单实体校验最好放在Entity的Hook方法中</para>
        /// <para>2. 可通过 IEntityWriter 增加额外需要保存的实体，确保一个事务内保存</para>
        /// <para>3. 完全自定义保存过程，返回false</para>
        /// </summary>
        /// <param name="w">实体写入器，所有需要增删改的实体在一个事务内保存到db</param>
        /// <returns>true继续执行默认保存过程，false放弃后续保存过程</returns>
        protected virtual Task<bool> OnSave(IEntityWriter w)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// 保存命令执行后调用，如保存成功后关闭窗口
        /// </summary>
        /// <param name="p_suc">是否保存成功</param>
        protected virtual void OnSaved(bool p_suc)
        {
        }
        
        /// <summary>
        /// 保存实体数据
        /// </summary>
        /// <returns></returns>
        async Task<bool> SaveInternal()
        {
            // 实体写入器，所有需要增删改的实体在一个事务内保存到db
            var da = await GetDataAccess();
            var w = new EntityWriter(da);
            return await DoSave(w, true, true);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除命令
        /// </summary>
        protected virtual void Delete()
        {
            if (MainFv.Data == null)
                return;

            _locker.Call(async () =>
            {
                if (!await Kit.Confirm("确认要删除吗？"))
                {
                    Kit.Msg("已取消删除！");
                    return;
                }

                Row d = MainFv.Row;
                if (d != null && d.IsAdded)
                {
                    OnClear();
                    return;
                }

                bool suc = await OnDelete();
                if (suc)
                {
                    IsDeleted = true;
                    UpdateRelatedInternal(-1, d, UpdateRelatedEvent.Deleted);
                    UpdateListInternal(d, UpdateListEvent.Deleted);
                }
                OnDeleted(suc);
            });
        }

        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<bool> OnDelete()
        {
            if (IsEntityData())
            {
                var ls = new List<Table>();
                if (Items.Count > 0)
                {
                    foreach (var fi in Items)
                    {
                        if (fi.Lv.Data is Table tbl)
                            ls.Add(tbl);
                    }
                }

                var entity = MainFv.Data as Entity;
                if (ls.Count == 0)
                    return await entity.Delete();

                var da = await GetDataAccess();
                var w = new EntityWriter(da);
                // 先删子实体
                foreach (var tbl in ls)
                {
                    await w.Delete(tbl);
                }
                await w.Delete(entity);
                return await w.Commit();
            }
            return false;
        }

        /// <summary>
        /// 删除命令执行后调用
        /// </summary>
        /// <param name="p_suc">是否删除成功</param>
        protected virtual void OnDeleted(bool p_suc)
        {
        }
        #endregion

        #region 创建菜单
        /// <summary>
        /// 添加默认菜单项：增加、保存、删除，若提供Menu则添加默认项，否则创建新菜单
        /// </summary>
        /// <param name="p_menu">若提供Menu则添加默认项，否则创建新菜单</param>
        /// <param name="add">是否添加"增加"菜单项</param>
        /// <param name="save">是否添加"保存"菜单项</param>
        /// <param name="del">是否添加"删除"菜单项</param>
        /// <returns></returns>
        protected Menu CreateMenu(Menu p_menu = null, bool add = true, bool save = true, bool del = true)
        {
            Menu menu = p_menu ?? new Menu();
            Mi mi = Mi.增加(call: Add);
            mi.ShowBtn = true;
            mi.BtnCall += ShowSetting;
            ToolTipService.SetToolTip(mi, "Ctrl + N");
            menu.Items.Add(mi);

            mi = Mi.保存(call: Save);
            mi.SetBinding(Mi.IsEnabledProperty, new Binding { Path = new PropertyPath("IsDirty"), Source = this });
            ToolTipService.SetToolTip(mi, "Ctrl + S");
            menu.Items.Add(mi);

            menu.Items.Add(Mi.删除(call: Delete));

            return menu;
        }
        #endregion

        #region 选项
        /// <summary>
        /// 显示选项
        /// </summary>
        protected void ShowSetting()
        {
            var dlg = new Dlg { Title = "增加前选项", IsPinned = true, ShowVeil = true };
            StackPanel sp = new StackPanel { Margin = new Thickness(20), Spacing = 20 };
            var btn = new RadioButton { Content = "增加前自动保存已修改的数据" };
            if (BeforeAdd == BeforeAddOption.AutoSave)
                btn.IsChecked = true;
            btn.Checked += (s, e) =>
            {
                BeforeAdd = BeforeAddOption.AutoSave;
                dlg.Close();
            };
            sp.Children.Add(btn);

            btn = new RadioButton { Content = "增加前提示是否保存已修改的数据" };
            if (BeforeAdd == BeforeAddOption.Confirm)
                btn.IsChecked = true;
            btn.Checked += (s, e) =>
            {
                BeforeAdd = BeforeAddOption.Confirm;
                dlg.Close();
            };
            sp.Children.Add(btn);

            btn = new RadioButton { Content = "直接增加,不检查原数据是否已修改" };
            if (BeforeAdd == BeforeAddOption.None)
                btn.IsChecked = true;
            btn.Checked += (s, e) =>
            {
                BeforeAdd = BeforeAddOption.None;
                dlg.Close();
            };
            sp.Children.Add(btn);

            dlg.Content = sp;
            dlg.Show();
        }
        #endregion

        #region 内部方法
        async Task GetInternal()
        {
            if (_args != null)
            {
                await OnLoading();
                await OnGet();

                // OnGet中可能关闭Form
                if (IsOpened)
                {
                    await OnLoad();
                    UpdateRelatedInternal(_args.ID, MainFv.Row, UpdateRelatedEvent.Loaded);
                }
            }
        }

        void UpdateListInternal(Row p_data, UpdateListEvent p_fire)
        {
            // win模式无遮罩，实时触发
            if (!Kit.IsPhoneUI && !ShowVeil)
            {
                OnUpdateList(new UpdateListArgs
                {
                    ID = p_data?.ID,
                    Data = p_data,
                    Event = p_fire
                });
            }
        }

        void UpdateRelatedInternal(long p_id, Row p_data, UpdateRelatedEvent p_event)
        {
            // 对话框不可见、win模式无遮罩触发
            if (p_event == UpdateRelatedEvent.DlgClosed
                || (!Kit.IsPhoneUI && !ShowVeil))
            {
                OnUpdateRelated(new UpdateRelatedArgs
                {
                    ID = p_id,
                    Data = p_data,
                    Event = p_event,
                });
            }
        }

        internal Task AddChild(Fv p_fv)
        {
            return OnAddChild(p_fv);
        }

        async void OnLoaded()
        {
            // 切换到可见时更新数据
            if (_needUpdate)
            {
                await Refresh();
                _needUpdate = false;
            }

            // 恢复无修改状态
            IsSaved = false;
            IsDeleted = false;

            // 父子表单时指定对话框大小
            if (!Kit.IsPhoneUI
                && Items.Count > 0
                && double.IsNaN(Width))
            {
                // 父子表单，固定宽度，避免切换内容时动态调整宽度
                Width = DesiredSize.Width;
            }
        }
        #endregion
    }
}
