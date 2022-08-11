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
    public sealed partial class $entityname$Form : Mv
    {
        public $entityname$Form()
        {
            InitializeComponent();
            Menu["保存"].Bind(IsEnabledProperty, _fv, "IsDirty");
        }

        public async void Update(long p_id)
        {
            if (!await _fv.DiscardChanges())
                return;

            if (p_id > 0)
            {
                _fv.Data = await $agent$.First<$entityname$Obj>("$entitytitle$-编辑", new { id = p_id });
            }
            else
            {
                Create();
            }
        }

        public void Clear()
        {
            _fv.Data = null;
        }

        async void Create()
        {
            _fv.Data = new $entityname$Obj(
                ID: await $agent$.NewID());
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
            var d = _fv.Data.To<$entityname$Obj>();
            if (await $agent$.Save(d))
            {
                _win.List.Update();
            }
        }

        async void OnDel(object sender, Mi e)
        {
            var d = _fv.Data.To<$entityname$Obj>();
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

            if (await $agent$.Delete(d))
            {
                Clear();
                _win.List.Update();
            }
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        $entityname$Win _win => ($entityname$Win)_tab.OwnWin;
    }
}
