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
    public sealed partial class $maincls$Form : Mv
    {
        public $maincls$Form()
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
                _fv.Data = await $agent$.First<$maincls$Obj>("$maintitle$-编辑", new { id = p_id });
                UpdateRelated(p_id);
            }
            else
            {
                Create();
            }
        }

        public void Clear()
        {
            _fv.Data = null;
            ClearRelated();
        }

        async void Create()
        {
            _fv.Data = new $maincls$Obj(
                ID: await $agent$.NewID());

            ClearRelated();
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
            var d = _fv.Data.To<$maincls$Obj>();
            bool isNew = d.IsAdded;
            if (await $agent$.Save(d))
            {
                _win?.List.Update();
                Result = true;
                if (isNew)
                {
                    UpdateRelated(d.ID);
                }
            }
        }

        async void OnDel(object sender, Mi e)
        {
            var d = _fv.Data.To<$maincls$Obj>();
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
                Result = true;
                Clear();
            }
        }

        void UpdateRelated(long p_id)
        {
$relatedupdate$
        }

        void ClearRelated()
        {
$relatedclear$
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        $maincls$Win _win => ($maincls$Win)_tab.OwnWin;
    }
}
