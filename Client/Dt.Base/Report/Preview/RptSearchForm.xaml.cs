#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class RptSearchForm : Mv, IRptSearchForm
    {
        readonly RptInfo _info;

        public RptSearchForm(RptInfo p_info)
        {
            InitializeComponent();
            _info = p_info;
            _info.Root.Params.LoadFvCells(_fv);
        }

        /// <summary>
        /// 查询事件
        /// </summary>
        public event EventHandler<RptInfo> Query;

        void OnQuery(object sender, Mi e)
        {
            _info.UpdateParams(_fv.Row);
            Query?.Invoke(this, _info);
        }

        void OnResetParams(object sender, Mi e)
        {
            _fv.Row.RejectChanges();
        }
    }
}
