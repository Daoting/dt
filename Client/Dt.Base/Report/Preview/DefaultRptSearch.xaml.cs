#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-18 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base.Report
{
    public sealed partial class DefaultRptSearch : RptSearchTab
    {
        public DefaultRptSearch(RptInfo p_info) : base(p_info)
        {
            InitializeComponent();
            _info.Root.Params.LoadFvCells(_fv);
            _fv.Data = _row;
        }
    }
}