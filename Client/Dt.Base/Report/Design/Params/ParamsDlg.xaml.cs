#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-10-10 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base.Report
{
    public partial class ParamsDlg : Dlg
    {
        public ParamsDlg()
        {
            InitializeComponent();
            if (!Kit.IsPhoneUI)
            {
                IsPinned = true;
                Width = 800;
                Height = 600;
            }
        }

        public void ShowDlg(RptDesignInfo p_info)
        {
            Info = p_info;

            List = new ParamsList(this);
            Form = new ParamsForm(this);
            var xaml = new ParamsXaml(this);
            
            LoadTabs(new List<Tab> { List, xaml });
            Show();
        }

        public RptDesignInfo Info { get; private set; }

        public ParamsList List { get; private set; }

        public ParamsForm Form { get; private set; }
    }
}
