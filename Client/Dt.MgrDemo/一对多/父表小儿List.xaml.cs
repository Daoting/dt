#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo.一对多
{
    public partial class 父表小儿List : Tab
    {
        long _parentID;
        父表小儿Form _form;

        public 父表小儿List()
        {
            InitializeComponent();
        }

        public void Update(long p_parentID)
        {
            _parentID = p_parentID;
            Menu["增加"].IsEnabled = _parentID > 0;
            Refresh();
            _win.ChildForm.BackToHome();
        }

        public async void Refresh()
        {
            if (_parentID > 0)
            {
                _lv.Data = await 小儿X.Query("GroupID=@ParentID", new Dict { { "ParentID", _parentID.ToString() } });
            }
            else
            {
                _lv.Data = null;
            }
        }

        void OnAdd(object sender, Mi e)
        {
            ShowForm(-1);
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            if (_lv.SelectionMode != Base.SelectionMode.Multiple)
                ShowForm(e.Row.ID);
        }

        async void ShowForm(long p_id)
        {
            if (_form == null)
                _form = new 父表小儿Form();
            _win.ChildForm.Toggle(_form);
            await _form.Update(p_id, _parentID);
        }

        #region 删除
        async void OnDel(object sender, Mi e)
        {
            if (!await Kit.Confirm("确认要删除选择的数据吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (_lv.SelectionMode == Base.SelectionMode.Multiple)
            {
                var ls = _lv.SelectedItems.Cast<小儿X> ().ToList();
                if (await ls.Delete())
                    Refresh();
            }
            else if (await e.Data.To<小儿X> ().Delete())
            {
                Refresh();
            }
            _win.ChildForm.BackToHome();
        }

        void OnSelectAll(object sender, Mi e)
        {
            _lv.SelectAll();
        }

        void OnMultiMode(object sender, Mi e)
        {
            _lv.SelectionMode = Base.SelectionMode.Multiple;
            Menu.Hide("增加", "选择");
            Menu.Show("删除", "全选", "取消");
        }

        void OnCancelMulti(object sender, Mi e)
        {
            _lv.SelectionMode = Base.SelectionMode.Single;
            Menu.Show("增加", "选择");
            Menu.Hide("删除", "全选", "取消");
        }
        #endregion

        父表Win _win => (父表Win)OwnWin;
    }
}