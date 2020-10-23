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
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.File
{
    /// <summary>
    /// 文件
    /// </summary>
    [View("文件")]
    public partial class FileHome : Win
    {
        public FileHome()
        {
            InitializeComponent();
            _tabPub.Content = new FolderPage(new PubFileMgr());
            _tabMy.Content = new FolderPage(new MyFileMgr());
        }
    }
}