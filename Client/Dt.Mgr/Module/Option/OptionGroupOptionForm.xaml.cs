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
    public sealed partial class OptionGroupOptionForm : Tab
    {
        #region 构造方法
        public OptionGroupOptionForm()
        {
            InitializeComponent();
        }
        #endregion

        #region 公开
        public async Task Update(long p_id, long p_parentID)
        {
            var d = Data;
            if (d != null && d.ID == p_id)
                return;

            if (!await _fv.DiscardChanges())
                return;

            _parentID = p_parentID;
            if (p_id > 0)
            {
                Data = await OptionX.GetByID(p_id);
            }
            else
            {
                Create();
            }
        }

        public void Clear()
        {
            Data = null;
        }
        #endregion

        #region 交互
        void OnAdd(object sender, Mi e)
        {
            Create();
        }

        void OnSave(object sender, Mi e)
        {
            Save();
        }

        async void OnDel(object sender, Mi e)
        {
            var d = Data;
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

            if (await d.Delete(false))
            {
                Clear();
                _win.OptionList.Refresh();
                RbacDs.PromptForUpdateModel("基础选项删除成功");
            }
            else
            {
                Kit.Warn("基础选项删除失败！");
            }
        }
        #endregion

        #region 内部
        async void Create()
        {
            Data = await OptionX.New(GroupID: _parentID);
        }

        async void Save()
        {
            if (await Data.Save(false))
            {
                _win.OptionList.Refresh();
                RbacDs.PromptForUpdateModel("基础选项保存成功");
            }
            else
            {
                Kit.Warn("基础选项保存失败！");
            }
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        OptionX Data
        {
            get { return _fv.Data.To<OptionX>(); }
            set { _fv.Data = value; }
        }

        OptionGroupWin _win => (OptionGroupWin)OwnWin;
        long _parentID;
        #endregion
    }
}
