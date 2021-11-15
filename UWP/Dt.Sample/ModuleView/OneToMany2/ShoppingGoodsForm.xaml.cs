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
    public sealed partial class ShoppingGoodsForm : Mv
    {
        long _parentID;

        public ShoppingGoodsForm()
        {
            InitializeComponent();
            Menu["保存"].Bind(IsEnabledProperty, _fv, "IsDirty");
        }

        public async void Update(long p_id, long p_parentID)
        {
            if (!await _fv.DiscardChanges())
                return;

            _parentID = p_parentID;
            if (p_id > 0)
            {
                _fv.Data = await AtCm.First<GoodsObj>("select * from oa_goods where id=@id", new { id = p_id });
            }
            else
            {
                Create();
            }
        }

        async void Create()
        {
            _fv.Data = new GoodsObj(
                ID: await AtCm.NewID(),
                ParentID: _parentID);
        }

        void OnAdd(object sender, Mi e)
        {
            Create();
        }

        async void OnSave(object sender, Mi e)
        {
            var d = _fv.Data.To<GoodsObj>();
            if (await AtCm.Save(d))
            {
                Result = true;
            }
        }

        async void OnDel(object sender, Mi e)
        {
            var d = _fv.Data.To<GoodsObj>();
            if (d == null)
                return;

            if (!await Kit.Confirm("确认要删除吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (d.IsAdded)
            {
                _fv.Data = null;
                return;
            }

            if (await AtCm.Delete(d))
            {
                Result = true;
                _fv.Data = null;
            }
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }
    }
}
