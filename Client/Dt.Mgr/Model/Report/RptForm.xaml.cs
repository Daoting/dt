#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-16 创建
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

namespace Dt.Mgr.Model
{
    public sealed partial class RptForm : Tab
    {
        public RptForm()
        {
            InitializeComponent();
        }

        public async void Update(long p_id)
        {
            if (await _fv.DiscardChanges())
                _fv.Data = await AtCm.First<RptX>("报表-ID", new { id = p_id });
        }

        public void Clear()
        {
            _fv.Data = null;
        }

        async void OnSave(object sender, Mi e)
        {
            if (await _fv.Data.To<RptX>().Save())
            {
                _win.List.Update();
                LobKit.PromptForUpdateModel();
            }
        }

        async void OnAdd(object sender, Mi e)
        {
            _fv.Data = await RptX.New(
                Name: "新报表");
        }

        async void OnDel(object sender, Mi e)
        {
            var d = _fv.Data.To<RptX>();
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

            if (await RptX.DelByID(d.ID))
            {
                Clear();
                _win.List.Update();
                LobKit.PromptForUpdateModel();
            }
        }

        async void OnEditTemp(object sender, RoutedEventArgs e)
        {
            RptX rpt = _fv.Data.To<RptX>();
            if (rpt != null)
            {
                if (rpt.IsAdded || rpt.IsChanged)
                {
                    if (await rpt.Save(false))
                    {
                        _win.List.Update();
                    }
                    else
                    {
                        Kit.Warn("自动保存失败！");
                        return;
                    }
                }
                _ = Rpt.ShowDesign(new AppRptDesignInfo(rpt));
            }
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        RptWin _win => (RptWin)OwnWin;
    }
}
