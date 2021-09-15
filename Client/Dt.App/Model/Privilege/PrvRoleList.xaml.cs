#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
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
    public sealed partial class PrvRoleList : Mv
    {
        string _id;

        public PrvRoleList()
        {
            InitializeComponent();
            Menu["移除"].Bind(IsEnabledProperty, _lv, "HasSelected");
        }

        public void Update(string p_id)
        {
            _id = p_id;
            Menu["添加"].IsEnabled = true;
            Refresh();
        }

        public void Clear()
        {
            _id = null;
            Menu["添加"].IsEnabled = false;
            _lv.Data = null;
        }

        async void Refresh()
        {
            _lv.Data = await AtCm.Query("权限-关联角色", new { prvid = _id });
        }

        async void OnAdd(object sender, Mi e)
        {
            SelectRolesDlg dlg = new SelectRolesDlg();
            if (await dlg.Show(RoleRelations.Prv, _id, e))
            {
                List<RolePrvObj> ls = new List<RolePrvObj>();
                foreach (var row in dlg.SelectedItems.OfType<Row>())
                {
                    ls.Add(new RolePrvObj(row.ID, _id));
                }
                if (ls.Count > 0 && await AtCm.BatchSave(ls))
                {
                    Refresh();
                    await AtCm.DeleteDataVer(ls.Select(rm => rm.RoleID).ToList(), "privilege");
                }
            }
        }

        void OnRemove(object sender, Mi e)
        {
            DoRemove(_lv.SelectedRows);
        }

        void OnRemove2(object sender, Mi e)
        {
            if (_lv.SelectionMode == Base.SelectionMode.Multiple)
                DoRemove(_lv.SelectedRows);
            else
                DoRemove(new List<Row> { e.Row });
        }

        async void DoRemove(IEnumerable<Row> p_rows)
        {
            List<RolePrvObj> ls = new List<RolePrvObj>();
            foreach (var row in p_rows)
            {
                ls.Add(new RolePrvObj(row.Long("roleid"), _id));
            }
            if (ls.Count > 0 && await AtCm.BatchDelete(ls))
            {
                Refresh();
                await AtCm.DeleteDataVer(ls.Select(rm => rm.RoleID).ToList(), "privilege");
            }
        }

        void OnSelectAll(object sender, Mi e)
        {
            _lv.SelectAll();
        }

        void OnMultiMode(object sender, Mi e)
        {
            _lv.SelectionMode = Base.SelectionMode.Multiple;
            Menu.Hide("添加", "选择");
            Menu.Show("移除", "全选", "取消");
        }

        void OnCancelMulti(object sender, Mi e)
        {
            _lv.SelectionMode = Base.SelectionMode.Single;
            Menu.Show("添加", "选择");
            Menu.Hide("移除", "全选", "取消");
        }

        PrvWin _win => (PrvWin)_tab.OwnWin;
    }
}
