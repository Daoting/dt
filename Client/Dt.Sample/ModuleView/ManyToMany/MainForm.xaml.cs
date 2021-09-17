#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.App;
using Dt.App.Model;
using Dt.Base;
using Dt.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
#endregion

namespace Dt.Sample.ModuleView
{
    public sealed partial class MainForm : Mv
    {
        public MainForm()
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
                _fv.Data = await AtCm.First<RoleObj>("角色-编辑", new { id = p_id });
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
            _fv.Data = new RoleObj(
                ID: await AtCm.NewID(),
                Name: "新角色");

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

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        async void Save()
        {
            var d = _fv.Data.To<RoleObj>();
            bool isNew = d.IsAdded;
            if (await AtCm.Save(d))
            {
                _win.List.Update();
                if (isNew)
                {
                    UpdateRelated(d.ID);
                }
            }
        }

        async void OnDel(object sender, Mi e)
        {
            var d = _fv.Data.To<RoleObj>();
            if (d == null)
                return;

            if (!await Kit.Confirm($"确认要删除[{d.Name}]吗？"))
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

        void UpdateRelated(long p_id)
        {
            _win.RelatedList.Update(p_id);
        }

        void ClearRelated()
        {
            _win.RelatedList.Clear();
        }

        MainWin _win => (MainWin)_tab.OwnWin;
    }
}
