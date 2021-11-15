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
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.App.File
{
    public sealed partial class SelectFileDlg : Dlg, ISelectFileDlg
    {
        public SelectFileDlg()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 已选择的文件列表，每个字符串为独立的文件描述json，如：["v0/52/37/142888904373956608.xlsx","12","xlsx文件",8153,"daoting","2020-10-29 15:09"]
        /// </summary>
        public List<string> SelectedFiles { get; set; }

        public bool IsMultiSelection { get; private set; }

        public string TypeFilter { get; private set; }

        /// <summary>
        /// 显示文件选择对话框
        /// </summary>
        /// <param name="p_isMultiSelection">是否允许多选</param>
        /// <param name="p_typeFilter">按文件扩展名过滤</param>
        /// <returns></returns>
        public async Task<bool> Show(bool p_isMultiSelection, string p_typeFilter)
        {
            IsMultiSelection = p_isMultiSelection;
            TypeFilter = p_typeFilter;
            if (!Kit.IsPhoneUI)
            {
                Width = 400;
                Height = 600;
            }

            LoadMv(new SelectLibPage(this));
            return await ShowAsync();
        }
    }
}
