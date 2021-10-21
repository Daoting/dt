#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.App.Model;
using Dt.Base;
using Dt.Core;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Workflow
{
    /// <summary>
    /// 活动授权
    /// </summary>
    public sealed partial class AtvRole : UserControl
    {
        long _atvID;

        public AtvRole()
        {
            InitializeComponent();
            _lv.Filter = OnFilter;
        }

        public void LoadRoles(long p_atvID, Table<WfdAtvroleObj> p_atvRoles)
        {
            _atvID = p_atvID;
            _lv.Data = p_atvRoles;
            _lv.Refresh();
        }

        /// <summary>
        /// 数据行过滤
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        bool OnFilter(object row)
        {
            return ((WfdAtvroleObj)row).AtvID == _atvID;
        }

        async void OnAddRole(object sender, RoutedEventArgs e)
        {
            var dlg = new SelectRolesDlg();
            if (await dlg.Show(RoleRelations.WfAtv, _atvID.ToString(), (Button)sender))
            {
                foreach (var row in dlg.SelectedItems.OfType<Row>())
                {
                    var ar = new WfdAtvroleObj(
                        AtvID: _atvID,
                        RoleID: row.ID);
                    ar.AddCell("role", row.Str("name"));
                    _lv.Data.Add(ar);
                }
            }
        }

        void OnDelete(object sender, Mi e)
        {
            _lv.Data.Remove(e.Data);
        }
    }
}
