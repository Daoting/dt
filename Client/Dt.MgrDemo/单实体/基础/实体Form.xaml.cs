#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo.单实体
{
    public sealed partial class 实体Form : Tab
    {
        public 实体Form()
        {
            InitializeComponent();
        }

        public async void Update(long p_id)
        {
            var d = Data;
            if (d != null && d.ID == p_id)
                return;

            if (!await _fv.DiscardChanges())
                return;

            if (p_id > 0)
            {
                Data = await 基础X.GetByID(p_id);
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
            Data = await 基础X.New();
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
                _win.List.Update();
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
                _win.List.Update();
            }
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        基础X Data
        {
            get { return _fv.Data.To<基础X>(); }
            set { _fv.Data = value; }
        }

        实体Win _win => (实体Win)OwnWin;
    }
}
