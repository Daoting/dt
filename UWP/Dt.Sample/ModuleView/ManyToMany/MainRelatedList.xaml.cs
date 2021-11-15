#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Dt.Sample.ModuleView
{
    public sealed partial class MainRelatedList : Mv
    {
        long _id;

        public MainRelatedList()
        {
            InitializeComponent();
            Menu["移除"].Bind(IsEnabledProperty, _lv, "HasSelected");
        }

        public void Update(long p_id)
        {
            _id = p_id;
            Menu["添加"].IsEnabled = true;
            Refresh();
        }

        public void Clear()
        {
            _id = -1;
            Menu["添加"].IsEnabled = false;
            _lv.Data = null;
        }

        void Refresh()
        {
            //_lv.Data = await AtCm.Query("a-关联b", new { id = _id });
        }

        void OnAdd(object sender, Mi e)
        {
            //var dlg = new SelectUserDlg();
            //if (await dlg.Show(_id, e))
            //{
            //    List<long> users = new List<long>();
            //    foreach (var row in dlg.SelectedItems.OfType<Row>())
            //    {
            //        users.Add(row.ID);
            //    }
            //    if (users.Count > 0 && await AtCm.AddRoleUser(_id, users))
            //        Refresh();
            //}
        }

        void OnRemove(object sender, Mi e)
        {
            DoRemove(_lv.SelectedRows);
        }

        void OnRemove2(object sender, Mi e)
        {
            if (_lv.SelectionMode == SelectionMode.Multiple)
                DoRemove(_lv.SelectedRows);
            else
                DoRemove(new List<Row> { e.Row });
        }

        void DoRemove(IEnumerable<Row> p_rows)
        {
            
        }

        void OnSelectAll(object sender, Mi e)
        {
            _lv.SelectAll();
        }

        void OnMultiMode(object sender, Mi e)
        {
            _lv.SelectionMode = SelectionMode.Multiple;
            Menu.Hide("添加", "选择");
            Menu.Show("移除", "全选", "取消");
        }

        void OnCancelMulti(object sender, Mi e)
        {
            _lv.SelectionMode = SelectionMode.Single;
            Menu.Show("添加", "选择");
            Menu.Hide("移除", "全选", "取消");
        }

        MainWin _win => (MainWin)_tab.OwnWin;
    }
}
