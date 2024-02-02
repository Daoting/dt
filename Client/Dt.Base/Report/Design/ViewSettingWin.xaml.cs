#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-10-10 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class ViewSettingWin : Win
    {
        RptDesignInfo _info;

        public ViewSettingWin(RptDesignInfo p_info)
        {
            InitializeComponent();
            _info = p_info;
            _info.TemplateChanged += (s, e) => LoadSetting();
            _info.Saved += OnSaved;
            LoadSetting();
        }

        void OnSaved(object sender, EventArgs e)
        {
            _info.Root.ViewSetting.Data.AcceptChanges();
        }

        void LoadSetting()
        {
            _fv.Data = _info.Root.ViewSetting.Data;
        }
    }
}
