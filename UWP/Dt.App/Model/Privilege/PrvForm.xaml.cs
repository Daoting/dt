#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-15 创建
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

namespace Dt.App.Model
{
    public sealed partial class PrvForm : Mv
    {
        public PrvForm()
        {
            InitializeComponent();
            Menu["保存"].Bind(IsEnabledProperty, _fv, "IsDirty");
        }

        public async void Update(string p_id)
        {
            if (!await _fv.DiscardChanges())
                return;

            if (!string.IsNullOrEmpty(p_id))
            {
                _fv.Data = await AtCm.GetByID<PrvObj>(p_id);
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

        void Create()
        {
            _fv.Data = new PrvObj("");
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
            var d = _fv.Data.To<PrvObj>();
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
            var d = _fv.Data.To<PrvObj>();
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

            if (await AtCm.Delete(d))
            {
                _win.List.Update();
                ClearRelated();
            }
        }

        void UpdateRelated(string p_id)
        {
            _win.RoleList.Update(p_id);
            _win.UserList.Update(p_id);
        }

        void ClearRelated()
        {
            _win.RoleList.Clear();
            _win.UserList.Clear();
        }

        PrvWin _win => (PrvWin)_tab.OwnWin;
    }
}
