#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.App;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endregion

namespace $rootnamespace$
{
    public sealed partial class $maincls$$childcls$Form : Mv
    {
        public $maincls$$childcls$Form()
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
                _fv.Data = await AtCm.First<$childcls$Obj > ("$childtitle$-编辑", new { id = p_id });
            }
            else
            {
                Create();
            }
        }

        async void Create()
        {
            _fv.Data = new $childcls$Obj(
                ID: await AtCm.NewID());
        }

        void OnAdd(object sender, Mi e)
        {
            Create();
        }

        async void OnSave(object sender, Mi e)
        {
            var d = _fv.Data.To<$childcls$Obj > ();
            bool isNew = d.IsAdded;
            if (await AtCm.Save(d))
            {
                _win.List.Update();
                if (isNew)
                {
                }
            }
        }

        async void OnDel(object sender, Mi e)
        {
            var d = _fv.Data.To<$childcls$Obj > ();
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

        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        $maincls$Win _win => ($maincls$Win)_tab.OwnWin;
    }
}
