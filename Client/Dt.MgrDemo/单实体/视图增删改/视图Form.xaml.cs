#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo
{
    public sealed partial class 视图Form : Tab
    {
        #region 构造方法
        public 视图Form()
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
                Data = await ChildViewX.GetByID(p_id);
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

        public ChildViewX Data
        {
            get { return _fv.Data.To<ChildViewX>(); }
            private set { _fv.Data = value; }
        }
        #endregion

        #region 内部
        async void Create()
        {
            Data = await ChildViewX.New();
        }

        async void Save()
        {
            if (await Data.Save())
            {
                _win.List.Update();
            }
        }

        async void Delete()
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

        视图Win _win => OwnWin as 视图Win;
        #endregion
    }
}
