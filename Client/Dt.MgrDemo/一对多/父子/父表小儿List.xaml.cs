#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-12-11 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo
{
    public partial class 父表小儿List : Tab
    {
        #region 构造方法
        public 父表小儿List()
        {
            InitializeComponent();
        }
        #endregion

        #region 公开
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
                _lv.Data = await 小儿X.Query($"where parent_id={_parentID}");
            }
            else
            {
                _lv.Data = null;
            }
        }
        #endregion

        #region 交互
        void OnAdd(Mi e)
        {
            ShowForm(-1);
        }

        void OnItemClick(ItemClickArgs e)
        {
            if (_lv.SelectionMode != SelectionMode.Multiple)
                ShowForm(e.Row.ID);
        }

        async void ShowForm(long p_id)
        {
            if (_form == null)
                _form = new 父表小儿Form();
            _win.ChildForm.Toggle(_form);
            await _form.Update(p_id, _parentID);
        }
        
        async void OnDel(Mi e)
        {
            if (!await Kit.Confirm("确认要删除选择的数据吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (_lv.SelectionMode == SelectionMode.Multiple)
            {
                var ls = _lv.SelectedItems.Cast<小儿X>().ToList();
                if (await ls.Delete())
                    Refresh();
            }
            else if (await e.Data.To<小儿X>().Delete())
            {
                Refresh();
            }
            _win.ChildForm.BackToHome();
        }
        #endregion

        #region 选择
        void OnSelectAll(Mi e)
        {
            _lv.SelectAll();
        }

        void OnMultiMode(Mi e)
        {
            _lv.SelectionMode = SelectionMode.Multiple;
            Menu.HideExcept("删除", "全选", "取消");
        }

        void OnCancelMulti(Mi e)
        {
            _lv.SelectionMode = SelectionMode.Single;
            Menu.ShowExcept("删除", "全选", "取消");
        }
        #endregion

        #region 内部
        父表Win _win => OwnWin as 父表Win;
        long _parentID;
        父表小儿Form _form;
        #endregion
    }
}