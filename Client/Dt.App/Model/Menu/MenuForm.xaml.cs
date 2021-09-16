#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.App;
using Dt.Base;
using Dt.Core;
using Dt.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.App.Model
{
    public sealed partial class MenuForm : Mv
    {
        MenuObj _curItem;

        public MenuForm()
        {
            InitializeComponent();
            Menu["保存"].Bind(IsEnabledProperty, _fv, "IsDirty");
        }

        public async void Update(long p_id)
        {
            if (!await _fv.DiscardChanges())
                return;

            if (p_id > 0)
            {
                _curItem = await AtCm.First<MenuObj>("菜单-id菜单项", new { id = p_id });
                _fv.Data = _curItem;
                UpdateRelated(_curItem);
            }
            else
            {
                Clear();
            }
        }

        public void Clear()
        {
            _curItem = null;
            _fv.Data = null;
            ClearRelated();
        }

        void OnAddMi(object sender, Mi e)
        {
            AddMenu(false);
        }

        void OnAddGroup(object sender, Mi e)
        {
            AddMenu(true);
        }

        async void AddMenu(bool p_isGroup)
        {
            MenuObj m = new MenuObj(
                ID: await AtCm.NewID(),
                Name: p_isGroup ? "新组" : "新菜单",
                Icon: p_isGroup ? "文件夹" : "文件",
                IsGroup: p_isGroup,
                ParentID: _curItem != null ? (long?)_curItem.ID : null,
                Dispidx: await AtCm.NewSeq("sq_menu"),
                Ctime: Kit.Now,
                Mtime: Kit.Now);
            m.AddCell("parentname", _curItem?.Name);
            _fv.Data = m;
            ClearRelated();
        }

        async void OnSave(object sender, Mi e)
        {
            var d = _fv.Data.To<MenuObj>();
            if (await AtCm.Save(d))
            {
                _win.List.Update();
                UpdateRelated(d);
                AtCm.PromptForUpdateModel();
            }
        }

        async void OnDel(object sender, Mi e)
        {
            var d = _fv.Data.To<MenuObj>();
            if (d == null)
                return;

            if (!await Kit.Confirm("确认要删除吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (d.IsAdded)
            {
                Clear();
                return;
            }

            if (await AtCm.Delete(d))
            {
                Clear();
                _win.List.Update();
                AtCm.PromptForUpdateModel();
            }
        }

        void OnOpen(object sender, Mi e)
        {
            var row = _fv.Data.To<MenuObj>();
            OmMenu menu = new OmMenu(
                ID: row.ID,
                Name: row.Name,
                Icon: row.Icon,
                ViewName: row.ViewName,
                Params: row.Params);
            MenuKit.OpenMenu(menu);
        }

        async void OnLoadTreeGroup(object sender, AsyncEventArgs e)
        {
            using (e.Wait())
            {
                ((CTree)sender).Data = await AtCm.Query<MenuObj>("菜单-分组树");
            }
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        void OnFvDataChanged(object sender, object e)
        {
            var m = e as MenuObj;
            if (m == null)
            {
                // 根节点
                Menu.HideExcept("新建");
                _fv.HideExcept("name", "icon", "parentname");
            }
            else if (m.IsGroup)
            {
                Menu.ShowExcept("打开菜单");
                _fv.HideExcept("name", "icon", "parentname");
            }
            else
            {
                Menu.ShowExcept("新建");
                _fv.ShowExcept();
            }
        }

        void UpdateRelated(MenuObj p_mi)
        {
            if (p_mi.IsGroup)
                _win.RoleList.Clear();
            else
                _win.RoleList.Update(p_mi.ID);
        }

        void ClearRelated()
        {
            _win.RoleList.Clear();
        }

        MenuWin _win => (MenuWin)_tab.OwnWin;
    }
}
