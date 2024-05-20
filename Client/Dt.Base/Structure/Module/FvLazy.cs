#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-01-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 内部含Fv，数据源为Row或Entity的Tab
    /// </summary>
    class FvLazy
    {
        #region 变量
        readonly IFvHost _host;
        protected long? _lastID;
        bool _toggling;
        bool _needUpdate;
        #endregion

        public FvLazy(IFvHost p_host)
        {
            _host = p_host;
        }

        #region 外部方法
        public async Task OnUpdate(long? p_id)
        {
            _lastID = p_id;

            // 不可见
            if (!_host.IsOpened)
            {
                _needUpdate = true;
                // 虽不可见仍需更新关联视图
                _host.UpdateRelated(_lastID > 0 ? _lastID.Value : -1);
                return;
            }

            var d = _host.Fv.Row;

            // 清空
            if (_lastID == null)
            {
                // 新增的无需清空
                if (d != null && !d.IsAdded)
                    _host.Clear();
                return;
            }

            // 新增
            if (_lastID <= 0)
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

            // 当前存在数据
            if (d != null)
            {
                // id相同，不再重复加载
                if (d.ID == _lastID)
                    return;

                if (d.IsChanged && CheckChanges)
                {
                    // 屏蔽多次弹框
                    _toggling = true;
                    bool save = await Kit.Confirm("数据已修改，确认要保存吗？");
                    _toggling = false;
                    if (save)
                    {
                        // 保存失败，不继续
                        if (!await _host.OnSave())
                            return;
                    }
                }
            }

            try
            {
                _toggling = true;
                await _host.OnGet(_lastID.Value);

                // 切换太快，加载的并非最后切换
                d = _host.Fv.Row;
                if (d != null && d.ID != _lastID)
                {
                    //Kit.Debug("FvTab两次");
                    await Task.Delay(500);
                    await _host.OnGet(_lastID.Value);
                }
            }
            finally
            {
                _toggling = false;
            }
        }

        public async void OnOpen()
        {
            // 切换到可见时更新数据
            if (_needUpdate)
            {
                await OnUpdate(_lastID);
                _needUpdate = false;
            }
        }

        public Task<bool> OnClose()
        {
            if (CheckChanges)
            {
                // 提示是否放弃修改
                return _host.Fv.DiscardChanges();
            }
            return Task.FromResult(true);
        }
        #endregion

        #region 命令方法
        /// <summary>
        /// 增加
        /// </summary>
        public async void Add()
        {
            if (BeforeAdd != FvBeforeAdd.None)
            {
                var d = _host.Fv.Row;
                if (d != null && d.IsChanged)
                {
                    if (BeforeAdd == FvBeforeAdd.AutoSave
                        || await Kit.Confirm("数据已修改，确认要保存吗？"))
                    {
                        if (!await _host.OnSave())
                        {
                            // 保存失败，放弃增加
                            return;
                        }
                        // 刷新列表
                        _host.RefreshList(-1, FvRefreshList.Saved);
                    }
                }
            }
            await _host.OnAdd();
            _host.UpdateRelated(-1);
        }

        /// <summary>
        /// 保存
        /// </summary>
        public async void Save()
        {
            bool suc = await _host.OnSave();
            if (suc)
            {
                IsSaved = true;
                var d = _host.Fv.Row;
                if (d != null)
                {
                    _host.UpdateRelated(d.ID);
                    _host.RefreshList(d.ID, FvRefreshList.Saved);
                }
            }
        }

        /// <summary>
        /// 删除数据的默认流程
        /// </summary>
        public async void Delete()
        {
            if (_host.Fv.Data == null)
                return;

            if (!await Kit.Confirm("确认要删除吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (_host.Fv.Data is Row d && d.IsAdded)
            {
                _host.Clear();
                return;
            }
            bool suc = await _host.OnDel();
            if (suc)
            { 
                IsDeleted = true;
                _host.Clear();
                _host.RefreshList(-1, FvRefreshList.Deleted);
            }
        }

        /// <summary>
        /// 创建默认菜单
        /// </summary>
        /// <returns></returns>
        public Menu CreateMenu(Menu p_menu)
        {
            Menu menu = p_menu?? new Menu();
            Mi mi = new Mi
            {
                ID = "增加",
                Icon = Icons.加号,
                ShowInPhone = VisibleInPhone.Icon,
                ShowBtn = true,
            };
            mi.Call += Add;
            mi.BtnCall += ShowSetting;
            ToolTipService.SetToolTip(mi, "Ctrl + N");
            menu.Items.Add(mi);

            mi = new Mi
            {
                ID = "保存",
                Icon = Icons.保存,
                ShowInPhone = VisibleInPhone.Icon
            };
            mi.Call += Save;
            mi.SetBinding(Mi.IsEnabledProperty, new Binding { Path = new PropertyPath("IsDirty"), Source = _host.Fv });
            ToolTipService.SetToolTip(mi, "Ctrl + S");
            menu.Items.Add(mi);

            mi = new Mi
            {
                ID = "删除",
                Icon = Icons.删除,
                ShowInPhone = VisibleInPhone.Icon
            };
            mi.Call += Delete;
            menu.Items.Add(mi);

            return menu;
        }

        public void ShowSetting()
        {
            var dlg = new Dlg { Title = "增加前选项", IsPinned = true, ShowVeil = true };
            StackPanel sp = new StackPanel { Margin = new Thickness(20), Spacing = 20 };
            var btn = new RadioButton { Content = "增加前自动保存已修改的数据" };
            if (BeforeAdd == FvBeforeAdd.AutoSave)
                btn.IsChecked = true;
            btn.Checked += (s, e) =>
            {
                BeforeAdd = FvBeforeAdd.AutoSave;
                dlg.Close();
            };
            sp.Children.Add(btn);

            btn = new RadioButton { Content = "增加前提示是否保存已修改的数据" };
            if (BeforeAdd == FvBeforeAdd.Confirm)
                btn.IsChecked = true;
            btn.Checked += (s, e) =>
            {
                BeforeAdd = FvBeforeAdd.Confirm;
                dlg.Close();
            };
            sp.Children.Add(btn);

            btn = new RadioButton { Content = "直接增加,不检查原数据是否已修改" };
            if (BeforeAdd == FvBeforeAdd.None)
                btn.IsChecked = true;
            btn.Checked += (s, e) =>
            {
                BeforeAdd = FvBeforeAdd.None;
                dlg.Close();
            };
            sp.Children.Add(btn);

            dlg.Content = sp;
            dlg.Show();
        }

        /// <summary>
        /// 是否曾经保存成功
        /// </summary>
        public bool IsSaved { get; set; }

        /// <summary>
        /// 是否曾经删除成功
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 增加前选项：自动保存已修改的数据、提示、不检查
        /// </summary>
        public FvBeforeAdd BeforeAdd { get; set; }

        /// <summary>
        /// 切换数据源或关闭前是否检查、提示旧数据已修改
        /// </summary>
        public bool CheckChanges { get; set; } = true;
        #endregion
    }
}
