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
    public partial class RptViewRptInoForm : Form
    {
        RptViewParamsDlg _owner;

        public RptViewRptInoForm(RptViewParamsDlg p_owner)
        {
            InitializeComponent();
            _owner = p_owner;
            BeforeAdd = BeforeAddOption.None;
            CheckChanges = false;
        }

        protected override Task OnAdd()
        {
            var r = _owner.RptTbl.AddRow();
            r["id"] = Kit.NewID;
            _fv.Data = r;
            return Task.CompletedTask;
        }

        protected override Task OnGet()
        {
            _fv.Data = (from row in _owner.RptTbl
                        where row.ID == _args.ID
                        select row).FirstOrDefault();
            return Task.CompletedTask;
        }
        
        protected override void OnClear()
        {
            if (_fv.Data is Row row)
            {
                _owner.DeleteRpt(row);
            }
            _fv.Data = null;
        }
        
        async void OnDefault()
        {
            if (_fv.Data is Row r)
            {
                var txt = await SelectRptList.ShowDlg();
                if (!string.IsNullOrEmpty(txt))
                    r["uri"] = txt;
            }
        }

        void OnContent()
        {
            AddTemplate("ms-appx:///程序集名/子路径/模板名称.rpt");
        }

        void OnEmbedded()
        {
            AddTemplate("embedded://程序集名/完整路径.模板名称.rpt");
        }

        void OnLocal()
        {
            AddTemplate("local://sqlite库名/模板名称");
        }
        
        void AddTemplate(string p_temp)
        {
            if (_fv.Data is Row r)
            {
                r["uri"] = p_temp;
            }
        }
    }
}