#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace $rootnamespace$
{
    public sealed partial class $clsroot$Form : Tab
    {
        #region 构造方法
        public $clsroot$Form()
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
                Data = await $entity$.GetByID(p_id);
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

        public $entity$ Data
        {
            get { return _fv.Data.To<$entity$>(); }
            private set { _fv.Data = value; }
        }
        #endregion

        #region 内部
        async void Create()
        {
            Data = await $entity$.New();
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

        $clsroot$Win _win => OwnWin as $clsroot$Win;
        #endregion
    }
}
