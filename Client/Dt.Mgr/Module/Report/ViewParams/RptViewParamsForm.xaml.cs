#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-05-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Module
{
    public partial class RptViewParamsForm : Form
    {
        RptViewParamsDlg _owner;
        
        public RptViewParamsForm(RptViewParamsDlg p_owner)
        {
            InitializeComponent();
            _owner = p_owner;
            BeforeAdd = BeforeAddOption.None;
            CheckChanges = false;
        }
        
        protected override Task OnAdd()
        {
            var r = _owner.ParamsTbl.NewRow();
            r["rptid"] = _owner.SelectedRpt.ID;
            _fv.Data = r;
            _owner.ParamsTbl.Add(r);
            return Task.CompletedTask;
        }

        protected override Task OnGet()
        {
            _fv.Data = (from row in _owner.ParamsTbl
                        where row.ID == _args.ID
                        select row).FirstOrDefault();
            return Task.CompletedTask;
        }

        protected override void OnClear()
        {
            if (_fv.Data is Row row)
            {
                _owner.ParamsTbl.Remove(row);
            }
            _fv.Data = null;
        }
    }
}