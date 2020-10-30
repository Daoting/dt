#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.App.File
{
    public sealed partial class SelectFileDlg : Dlg
    {
        public SelectFileDlg()
        {
            InitializeComponent();
        }

        public List<string> SelectedFiles { get; set; }

        public bool IsMultiSelection { get; private set; }

        public string TypeFilter { get; private set; }

        public async Task<bool> Show(bool p_isMultiSelection, string p_typeFilter)
        {
            IsMultiSelection = p_isMultiSelection;
            TypeFilter = p_typeFilter;
            if (!AtSys.IsPhoneUI)
            {
                Width = 400;
                Height = 600;
            }

            Content = new SelectLibPage(this);
            return await ShowAsync();
        }
    }
}
