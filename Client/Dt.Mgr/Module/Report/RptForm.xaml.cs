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
using Dt.Mgr.Rbac;
#endregion

namespace Dt.Mgr.Module
{
    public sealed partial class RptForm : Tab
    {
        #region 构造方法
        public RptForm()
        {
            InitializeComponent();
        }
        #endregion

        #region 公开
        public async void Update(long p_id)
        {
            var d = Data;
            if (d != null && d.ID == p_id)
                return;

            if (!await _fv.DiscardChanges())
                return;

            if (p_id > 0)
            {
                Data = await RptX.GetByID(p_id);
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

        public RptX Data
        {
            get { return _fv.Data.To<RptX>(); }
            private set { _fv.Data = value; }
        }
        #endregion

        #region 交互
        async void OnSave(object sender, Mi e)
        {
            if (await _fv.Data.To<RptX>().Save())
            {
                _win.List.Update();
            }
        }

        void OnAdd(object sender, Mi e)
        {
            Create();
        }

        async void OnDel(object sender, Mi e)
        {
            var d = _fv.Data.To<RptX>();
            if (d == null)
                return;

            if (!await Kit.Confirm("确认要删除吗？\r\n做个报表不容易，请慎重删除！"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (d.IsAdded)
            {
                Clear();
                return;
            }

            if (await RptX.DelByID(d.ID, true))
            {
                Clear();
                _win.List.Update();
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
        #endregion

        #region 内部
        async void Create()
        {
            _fv.Data = await RptX.New(
                Name: "新报表");
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        RptWin _win => (RptWin)OwnWin;
        #endregion
    }
}
