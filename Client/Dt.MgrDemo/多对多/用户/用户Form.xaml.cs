#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-06-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo.多对多
{
    public sealed partial class 用户Form : Tab
    {
        #region 构造方法
        public 用户Form()
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
                Data = await 用户X.GetByID(p_id);
                UpdateRelated(p_id);
            }
            else
            {
                Create();
            }
        }

        public void Clear()
        {
            Data = null;
            UpdateRelated(-1);
        }

        public 用户X Data
        {
            get { return _fv.Data.To<用户X>(); }
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
                _win.MainList.Update();
            }
        }
        #endregion

        #region 内部
        async void Create()
        {
            Data = await 用户X.New();
            UpdateRelated(-1);
        }

        async void Save()
        {
            var d = Data;
            bool isNew = d.IsAdded;
            if (await d.Save())
            {
                _win.MainList.Update();
                if (isNew)
                {
                    UpdateRelated(d.ID);
                }
            }
        }

        void UpdateRelated(long p_id)
        {
            _win.角色List.Update(p_id);
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        用户Win _win => OwnWin as 用户Win;
        #endregion
    }
}
