#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Tools;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public partial class MenuList : LvTab
    {
        #region 变量
        long? _parentID;
        #endregion

        #region 构造
        public MenuList()
        {
            InitializeComponent();
        }
        #endregion

        #region 公开
        public void Update(long? p_parentID)
        {
            _parentID = p_parentID;
            _ = Refresh();
        }
        #endregion

        #region 重写
        protected override Lv Lv => _lv;

        protected override async Task Query()
        {
            if (_parentID == null)
            {
                _lv.Data = null;
            }
            else if (_parentID > 0)
            {
                _lv.Data = await MenuX.Query($"where parent_id={_parentID} order by dispidx");
            }
            else
            {
                _lv.Data = await MenuX.Query("where parent_id is null order by dispidx");
            }
        }
        #endregion

        #region 交互
        async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OwnWin is MenuWin win)
            {
                await win.Form.Update(_lv.SelectedRow?.ID);
            }
        }

        async void OnItemDbClick(object e)
        {
            if (OwnWin is MenuWin win)
            {
                var m = e.To<MenuX>();
                if (m.IsGroup)
                {
                    win.Tree.SelectByID(m.ID);
                }
                else
                {
                    await win.Form.Open(m.ID);
                }
            }
        }

        void AddMi()
        {
            if (OwnWin is MenuWin win)
            {
                win.Form.OpenAdd(false);
            }
        }

        void AddGroup()
        {
            if (OwnWin is MenuWin win)
            {
                win.Form.OpenAdd(true);
            }
        }

        async void OnEdit(Mi e)
        {
            if (OwnWin is MenuWin win)
            {
                await win.Form.Open(e.Row?.ID);
            }
        }

        async void OnDel(Mi e)
        {
            var d = e.Data.To<MenuX>();
            if (!await Kit.Confirm($"确认要删除【{d.Name}】吗？\r\n删除后不可恢复，请谨慎删除！"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (await d.Delete())
            {
                if (d.IsGroup && OwnWin is MenuWin win)
                    await win.Tree.Refresh(null);
                await Refresh();
            }
        }

        void OnOpen(Mi e)
        {
            if (OwnWin is MenuWin win)
            {
                var m = e.Data.To<MenuX>();
                if (m.IsGroup)
                {
                    win.Tree.SelectByID(m.ID);
                }
                else
                {
                    OmMenu menu = new OmMenu(
                        ID: m.ID,
                        Name: m.Name,
                        Icon: m.Icon,
                        ViewName: m.ViewName,
                        Params: m.Params);
                    MenuDs.OpenMenu(menu);
                }
            }
        }

        void OnMoveUp(Mi e)
        {
            var src = e.Data.To<MenuX>();
            int index = _lv.Data.IndexOf(src);
            if (index > 0)
                Exchange(src, (MenuX)_lv.Data[index - 1]);
        }

        void OnMoveDown(Mi e)
        {
            var src = e.Data.To<MenuX>();
            int index = _lv.Data.IndexOf(src);
            if (index < _lv.Data.Count - 1)
                Exchange(src, (MenuX)_lv.Data[index + 1]);
        }

        async void Exchange(MenuX src, MenuX tgt)
        {
            if (await ExchangeDispidx(src, tgt))
            {
                await Refresh();
            }
            else
            {
                Kit.Warn("菜单调序失败！");
            }
        }

        void OnRefresh(Mi e)
        {
            RefreshSqliteWin.UpdateSqliteFile("menu");
        }

        /// <summary>
        /// 互换两行的显示位置，确保包含 id,dispidx 列
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_src"></param>
        /// <param name="p_tgt"></param>
        /// <returns>true 互换成功</returns>
        public static Task<bool> ExchangeDispidx<TEntity>(TEntity p_src, TEntity p_tgt)
            where TEntity : Entity
        {
            var tbl = new Table<TEntity> { { "id", typeof(long) }, { "dispidx", typeof(int) } };

            var save = tbl.AddRow(new { id = p_src.ID });
            save.AcceptChanges();
            save["dispidx"] = p_tgt["dispidx"];

            save = tbl.AddRow(new { id = p_tgt.ID });
            save.AcceptChanges();
            save["dispidx"] = p_src["dispidx"];

            return tbl.Save(false);
        }
        #endregion
    }
}