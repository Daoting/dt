#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-16 创建
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
    public sealed partial class RptForm : Mv
    {
        public RptForm()
        {
            InitializeComponent();
            Menu["保存"].Bind(IsEnabledProperty, _fv, "IsDirty");
        }

        public async void Update(long p_id)
        {
            if (await _fv.DiscardChanges())
                _fv.Data = await AtCm.First<RptObj>("报表-ID", new { id = p_id });
        }

        public void Clear()
        {
            _fv.Data = null;
        }

        async void OnSave(object sender, Mi e)
        {
            if (await AtCm.Save(_fv.Data.To<RptObj>()))
            {
                _win.List.Update();
                AtCm.PromptForUpdateModel();
            }
        }

        async void OnAdd(object sender, Mi e)
        {
            _fv.Data = new RptObj(
                ID: await AtCm.NewID(),
                Name: "新报表");
        }

        async void OnDel(object sender, Mi e)
        {
            var d = _fv.Data.To<RptObj>();
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

            if (await AtCm.DelByID<RptObj>(d.ID))
            {
                Clear();
                _win.List.Update();
                AtCm.PromptForUpdateModel();
            }
        }

        async void OnEditTemp(object sender, RoutedEventArgs e)
        {
            RptObj rpt = _fv.Data.To<RptObj>();
            if (rpt != null)
            {
                if (rpt.IsAdded || rpt.IsChanged)
                {
                    if (await AtCm.Save(rpt, false))
                    {
                        _win.List.Update();
                    }
                    else
                    {
                        Kit.Warn("自动保存失败！");
                        return;
                    }
                }
                _ = AtRpt.ShowDesign(new AppRptDesignInfo(rpt));
            }
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        RptWin _win => (RptWin)_tab.OwnWin;
    }
}
