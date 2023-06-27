#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr.Rbac;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Module
{
    public partial class OptionGroupOptionList : Tab
    {
        #region 构造方法
        public OptionGroupOptionList()
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
                _lv.Data = await OptionX.Query($"SELECT a.*,b.Name as GroupName FROM cm_option a, cm_option_group b where a.GroupID=b.ID and a.GroupID={_parentID} order by Dispidx");
            }
            else
            {
                _lv.Data = null;
            }
        }
        #endregion

        #region 交互
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
                _form = new OptionGroupOptionForm();
            _win.ChildForm.Toggle(_form);
            await _form.Update(p_id, _parentID);
        }
        
        async void OnDel(object sender, Mi e)
        {
            if (!await Kit.Confirm("确认要删除选择的数据吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (_lv.SelectionMode == Base.SelectionMode.Multiple)
            {
                var ls = _lv.SelectedItems.Cast<OptionX>().ToList();
                if (await ls.Delete(false))
                {
                    Refresh();
                    RbacDs.PromptForUpdateModel("基础选项删除成功");
                }
                else
                {
                    Kit.Warn("基础选项删除失败！");
                }
            }
            else
            {
                if (await e.Data.To<OptionX>().Delete(false))
                {
                    Refresh();
                    RbacDs.PromptForUpdateModel("基础选项删除成功");
                }
                else
                {
                    Kit.Warn("基础选项删除失败！");
                }
            }
            _win.ChildForm.BackToHome();
        }

        void OnMoveUp(object sender, Mi e)
        {
            var src = e.Data.To<OptionX>();
            int index = _lv.Data.IndexOf(src);
            if (index > 0)
                Exchange(src, _lv.Data[index - 1].To<OptionX>());
        }

        void OnMoveDown(object sender, Mi e)
        {
            var src = e.Data.To<OptionX>();
            int index = _lv.Data.IndexOf(src);
            if (index < _lv.Data.Count - 1 && index >= 0)
                Exchange(src, _lv.Data[index + 1].To<OptionX>());
        }

        async void Exchange(OptionX p_src, OptionX p_tgt)
        {
            var tbl = await Table<OptionX>.Create();

            var save = (OptionX)tbl.AddRow(new { ID = p_src.ID });
            save.AcceptChanges();
            save.Dispidx = p_tgt.Dispidx;

            save = (OptionX)tbl.AddRow(new { ID = p_tgt.ID });
            save.AcceptChanges();
            save.Dispidx = p_src.Dispidx;

            if (await tbl.Save(false))
            {
                Refresh();
                RbacDs.PromptForUpdateModel("调序成功");
            }
            else
            {
                Kit.Warn("调序失败！");
            }
        }
        #endregion

        #region 选择
        void OnSelectAll(object sender, Mi e)
        {
            _lv.SelectAll();
        }

        void OnMultiMode(object sender, Mi e)
        {
            _lv.SelectionMode = Base.SelectionMode.Multiple;
            Menu.HideExcept("删除", "全选", "取消");
        }

        void OnCancelMulti(object sender, Mi e)
        {
            _lv.SelectionMode = Base.SelectionMode.Single;
            Menu.ShowExcept("删除", "全选", "取消");
        }
        #endregion

        #region 内部
        OptionGroupWin _win => (OptionGroupWin)OwnWin;
        long _parentID;
        OptionGroupOptionForm _form;
        #endregion
    }
}