#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#endregion

namespace $rootnamespace$
{
    public sealed partial class $childroot$Form : Tab
    {
        public $childroot$Form()
        {
            InitializeComponent();
        }

        public async void Update(long p_id)
        {
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

        async void Create()
        {
            Data = await $entity$.New();
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
                _win.$childroot$List.Update();
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
                _win.$childroot$List.Update();
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

        $parentroot$Win _win => ($parentroot$Win)OwnWin;
    }
}
