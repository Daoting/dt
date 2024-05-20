#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Module
{
    public partial class OptionGroupOptionList : LvTab
    {
        #region 变量
        long _parentID;
        OptionGroupOptionForm _form;
        #endregion

        #region 构造
        public OptionGroupOptionList()
        {
            InitializeComponent();
            _lv.AddMultiSelMenu(Menu);
        }
        #endregion

        #region 公开
        public void Update(long p_parentID)
        {
            if (_parentID == p_parentID)
                return;
            
            _parentID = p_parentID;
            Menu["增加"].IsEnabled = _parentID > 0;
            _ = Refresh();
        }
        #endregion

        #region 重写
        protected override Lv Lv => _lv;

        protected override async Task Query()
        {
            if (_parentID > 0)
            {
                _lv.Data = await OptionX.Query($"SELECT a.*,b.Name as GroupName FROM cm_option a, cm_option_group b where a.group_id=b.ID and a.group_id={_parentID} order by Dispidx");
            }
            else
            {
                _lv.Data = null;
            }
        }
        #endregion

        #region 交互
        void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_lv.SelectionMode != SelectionMode.Multiple
                && _form != null)
            {
                _form.Update(_lv.SelectedRow?.ID, _parentID);
            }
        }

        async void OnItemDbClick(object e)
        {
            if (_lv.SelectionMode != SelectionMode.Multiple)
            {
                await Form.Open(_lv.SelectedRow?.ID, _parentID);
            }
        }

        async void OnAdd(Mi e)
        {
            await Form.Open(-1, _parentID);
        }

        async void OnEdit(Mi e)
        {
            await Form.Open(e.Row?.ID, _parentID);
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
                var ls = _lv.SelectedItems.Cast<OptionX>().ToList();
                if (await ls.Delete())
                {
                    await Refresh();
                }
            }
            else if (await e.Data.To<OptionX>().Delete())
            {
                await Refresh();
            }
        }

        OptionGroupOptionForm Form
        {
            get
            {
                if (_form == null)
                    _form = new OptionGroupOptionForm { OwnWin = OwnWin };
                return _form;
            }
        }
        #endregion
    }
}