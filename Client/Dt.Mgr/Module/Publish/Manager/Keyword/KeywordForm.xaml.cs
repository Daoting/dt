#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.Mgr.Module
{
    public sealed partial class KeywordForm : Tab
    {
        public KeywordForm()
        {
            InitializeComponent();
        }

        public async void Update(string p_id)
        {
            var d = Data;
            if (d != null && d.ID == p_id)
                return;

            if (!await _fv.DiscardChanges())
                return;

            if (p_id != null)
            {
                Data = await PubKeywordX.GetByID(p_id);
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

        public PubKeywordX Data
        {
            get { return _fv.Data.To<PubKeywordX>(); }
            private set { _fv.Data = value; }
        }

        void Create()
        {
            _fv.Data = PubKeywordX.New();
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
            var d = _fv.Data.To<PubKeywordX>();
            if (d == null)
                return;

            if (!d.IsAdded
                && d.IsChanged
                && await IsUsed(d.GetOriginalVal<string>("id")))
            {
                d.RejectChanges();
                return;
            }

            if (await d.Save())
            {
                _win?.List.Update();
            }
        }

        async void OnDel(object sender, Mi e)
        {
            var d = _fv.Data.To<PubKeywordX>();
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

            d.RejectChanges();
            if (await IsUsed(d.ID))
                return;

            if (await d.Delete())
            {
                _win?.List.Update();
                Clear();
            }
        }

        async Task<bool> IsUsed(string p_keyword)
        {
            int cnt = await AtCm.GetScalar<int>("发布-关键字引用数", new { keyword = p_keyword });
            if (cnt > 0)
            {
                Kit.Warn("关键字已被引用，无法修改或删除！");
                return true;
            }
            return false;
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        KeywordWin _win => (KeywordWin)OwnWin;
    }
}
