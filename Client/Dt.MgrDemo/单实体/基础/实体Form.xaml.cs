#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-16 创建
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
        #region 构造方法
        public 实体Form()
        {
            InitializeComponent();
        }
        #endregion

        #region 公开
        public async Task Update(long p_id)
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

        public 基础X Data
        {
            get { return _fv.Data.To<基础X>(); }
            private set { _fv.Data = value; }
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

            if (await d.Delete())
            {
                Clear();
                _win.List.Update();
            }
        }
        #endregion

        #region 内部
        async void Create()
        {
            Data = await 基础X.New();
        }

        async void Save()
        {
            if (await Data.Save())
            {
                _win.List.Update();
            }
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        实体Win _win => OwnWin as 实体Win;
        #endregion
    }
}
