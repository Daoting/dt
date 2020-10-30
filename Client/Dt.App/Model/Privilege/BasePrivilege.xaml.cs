#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Dt.Core.Model;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
#endregion

namespace Dt.App.Model
{
    [View("基础权限")]
    public partial class BasePrivilege : Win
    {
        readonly Repo<Prv> _prv = new Repo<Prv>();

        public BasePrivilege()
        {
            InitializeComponent();
            LoadAll();
        }

        async void LoadAll()
        {
            _lvPrv.Data = await _prv.Query("权限-所有");
        }

        void OnNaviToSearch(object sender, RoutedEventArgs e)
        {
            NaviTo("查找权限");
        }

        async void OnSearch(object sender, string e)
        {
            if (e == "#全部")
            {
                LoadAll();
            }
            else if (!string.IsNullOrEmpty(e))
            {
                _lvPrv.Data = await _prv.Query("权限-模糊查询", new { id = $"%{e}%" });
            }
            NaviTo("权限列表");
        }

        async void OnAddPrv(object sender, Mi e)
        {
            if (await new EditPrvDlg().Show(null))
                LoadAll();
        }

        async void OnEditPrv(object sender, Mi e)
        {
            if (await new EditPrvDlg().Show(e.Data.To<Prv>().ID))
                LoadAll();
        }

        async void OnDelPrv(object sender, Mi e)
        {
            var prv = e.Data.To<Prv>();
            if (!await AtKit.Confirm($"确认要删除[{prv.ID}]吗？"))
            {
                AtKit.Msg("已取消删除！");
                return;
            }

            if (await _prv.Delete(prv))
                LoadAll();
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
                RefreshRelation(e.Data.To<Prv>().ID);

            NaviTo("授权角色,授权用户");
        }

        async void RefreshRelation(string p_prvid)
        {
            _lvUser.Data = await AtCm.Query("权限-关联用户", new { prvid = p_prvid });
            _lvRole.Data = await AtCm.Query("权限-关联角色", new { prvid = p_prvid });
        }

        void OnDataChanged(object sender, object e)
        {
            _lvUser.Data = null;
            _lvRole.Data = null;
        }

        async void OnAddRole(object sender, Mi e)
        {
            string prvID = _lvPrv.SelectedItem.To<Prv>().ID;
            SelectRolesDlg dlg = new SelectRolesDlg();
            
            if (await dlg.Show(RoleRelations.Prv, prvID, e))
            {
                List<RolePrv> ls = new List<RolePrv>();
                foreach (var row in dlg.SelectedItems.OfType<Row>())
                {
                    ls.Add(new RolePrv(row.ID, prvID));
                }
                if (ls.Count > 0 && await new Repo<RolePrv>().BatchSave(ls))
                    RefreshRelation(prvID);
            }
        }

        async void OnRemoveRole(object sender, Mi e)
        {
            string prvID = _lvPrv.SelectedItem.To<Prv>().ID;
            var rp = new RolePrv(_lvRole.SelectedRow.Long("roleid"), prvID);
            rp.AcceptChanges();
            if (await new Repo<RolePrv>().Delete(rp))
                RefreshRelation(prvID);
        }
    }
}