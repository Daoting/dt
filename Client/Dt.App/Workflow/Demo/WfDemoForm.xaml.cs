#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Workflow
{
    public partial class WfDemoForm : Win, IWfForm
    {

        public WfDemoForm(WfFormInfo p_info)
        {
            InitializeComponent();
            _tab.Menu = p_info.GetDefaultMenu(_fv);
            //p_info.ApplyMenuCmd(_tab.Menu, _fv);

            var r = new Row();
            r.AddCell("txt", "abc");
            _fv.Data = r;
        }

        public Task<bool> Save()
        {
            return Task.FromResult(true);
        }

        public Task<bool> Delete()
        {
            throw new NotImplementedException();
        }

        public string GetPrcName()
        {
            return "请假单";
        }
    }
}