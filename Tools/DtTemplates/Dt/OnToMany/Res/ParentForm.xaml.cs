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
    public sealed partial class $parentroot$Form : Tab
    {
        #region 构造方法
        public $parentroot$Form()
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

        public $entity$ Data
        {
            get { return _fv.Data.To<$entity$>(); }
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

        #endregion

        #region 内部
        async void Create()
        {
            Data = await $entity$.New();
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
$relatedupdate$
            _win.ChildForm.BackToHome();
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        $parentroot$Win _win => OwnWin as $parentroot$Win;
        #endregion
    }
}
