#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-18 创建
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

namespace Dt.Sample.ModuleView.OneToMany2
{
    public sealed partial class ShoppingForm : Mv
    {
        public ShoppingForm()
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
                _fv.Data = await AtCm.First<ShoppingObj>("select * from oa_shopping where id=@id", new { id = p_id });
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
            _fv.Data = new ShoppingObj(
                ID: await AtCm.NewID());

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
            var d = _fv.Data.To<ShoppingObj>();
            bool isNew = d.IsAdded;
            if (await AtCm.Save(d))
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
            var d = _fv.Data.To<ShoppingObj>();
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
                Result = true;
                Clear();
            }
        }

        void UpdateRelated(long p_id)
        {
            _win?.GoodsList.Update(p_id);
        }

        void ClearRelated()
        {
            _win?.GoodsList.Clear();
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        ShoppingWin _win => (ShoppingWin)_tab.OwnWin;
    }
}
