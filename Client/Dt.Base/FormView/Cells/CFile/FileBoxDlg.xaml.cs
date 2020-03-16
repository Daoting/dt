#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base.FormView
{
    public partial class FileBoxDlg : Dlg
    {
        CFile _owner;

        public FileBoxDlg(CFile p_owner)
        {
            InitializeComponent();
            _owner = p_owner;
        }

        public void ShowDlg()
        {
            string path = null;
            object data = _owner.GetVal();
            if (data != null)
                path = data.ToString();
            _fl.Data = path;

            Show();
        }

        void OnUploadFinished(FileList sender, bool suc)
        {
            if (suc)
            {
                Close();
            }
        }
    }
}
