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
    public sealed partial class $parentroot$$childroot$Form : Tab
    {
        #region 构造方法
        public $parentroot$$childroot$Form()
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
        #endregion

        #region 内部
        async void Create()
        {
            Data = await $entity$.New($parentidprop$: _parentID);
        }

        async void Save()
        {
            if (await Data.Save())
            {
                _win.$childroot$List.Refresh();
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
                _win.$childroot$List.Refresh();
            }
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        $entity$ Data
        {
            get { return _fv.Data.To<$entity$>(); }
            set { _fv.Data = value; }
        }

        $parentroot$Win _win => OwnWin as $parentroot$Win;
        long _parentID;
        #endregion
    }
}
