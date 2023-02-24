#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-24 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo.一对多
{
    public sealed partial class 父表大儿Form : Tab
    {
        long _parentID;

        public 父表大儿Form()
        {
            InitializeComponent();
        }

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
                Data = await 大儿X.GetByID(p_id);
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

        async void Create()
        {
            Data = await 大儿X.New(ParentID: _parentID);
        }

        void OnSave(object sender, Mi e)
        {
            Save();
        }

        void OnAdd(object sender, Mi e)
        {
            Create();
        }

        async void Save()
        {
            if (await Data.Save())
            {
                _win.大儿List.Refresh();
            }
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

            if (await d.Delete())
            {
                Clear();
                _win.大儿List.Refresh();
            }
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        大儿X Data
        {
            get { return _fv.Data.To<大儿X>(); }
            set { _fv.Data = value; }
        }

        父表Win _win => (父表Win)OwnWin;
    }
}
