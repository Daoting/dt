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
    public sealed partial class 父表Form : Tab
    {
        #region 构造方法
        public 父表Form()
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
                Data = await 父表X.GetByID(p_id);
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

        public 父表X Data
        {
            get { return _fv.Data.To<父表X>(); }
            private set { _fv.Data = value; }
        }
        #endregion

        #region 交互
        void OnAdd(Mi e)
        {
            Create();
        }

        void OnSave(Mi e)
        {
            Save();
        }

        #endregion

        #region 内部
        async void Create()
        {
            Data = await 父表X.New();
            UpdateRelated(-1);
        }

        async void Save()
        {
            var d = Data;
            bool isNew = d.IsAdded;
            if (await d.Save())
            {
                _win.ParentList.Update();
                if (isNew)
                {
                    UpdateRelated(d.ID);
                }
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
                _win.ParentList.Update();
            }
        }

        void UpdateRelated(long p_id)
        {
            _win.小儿List.Update(p_id);
            _win.大儿List.Update(p_id);
            _win.ChildForm.BackToHome();
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        父表Win _win => OwnWin as 父表Win;
        #endregion
    }
}
