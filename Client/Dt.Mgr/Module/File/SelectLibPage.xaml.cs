#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-11-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Module
{
    /// <summary>
    /// 选择库
    /// </summary>
    public sealed partial class SelectLibPage : Tab
    {
        readonly SelectFileDlg _owner;

        public SelectLibPage(SelectFileDlg p_owner)
        {
            InitializeComponent();
            _owner = p_owner;
        }

        async void OnPublicFile(object sender, RoutedEventArgs e)
        {
            var setting = new FileMgrSetting { AllowEdit = await LobKit.HasPermission("公共文件管理") };
            Forward(new SelectFilePage(new PubFileMgr { Setting = setting }, _owner));
        }

        void OnMyFile(object sender, RoutedEventArgs e)
        {
            Forward(new SelectFilePage(new MyFileMgr { Setting = new FileMgrSetting { AllowEdit = true } }, _owner));
        }

        async void OnResFile(object sender, RoutedEventArgs e)
        {
            var setting = new FileMgrSetting { AllowEdit = await LobKit.HasPermission("素材库管理") };
            Forward(new SelectFilePage(new ResFileMgr { Setting = setting }, _owner));
        }
        
        void OnSearch(object sender, Mi e)
        {
            Forward(new SelectSearchPage(_owner));
        }
    }
}
