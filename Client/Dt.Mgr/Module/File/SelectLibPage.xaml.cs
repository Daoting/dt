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

        void OnPublicFile(object sender, RoutedEventArgs e)
        {
            var setting = new FileMgrSetting { AllowEdit = async () => await Per.系统预留.文件管理.公共文件增删 };
            Forward(new SelectFilePage(new PubFileMgr { Setting = setting }, _owner));
        }

        void OnMyFile(object sender, RoutedEventArgs e)
        {
            Forward(new SelectFilePage(new MyFileMgr { Setting = new FileMgrSetting { AllowEdit = () => Task.FromResult(true) } }, _owner));
        }

        void OnResFile(object sender, RoutedEventArgs e)
        {
            var setting = new FileMgrSetting { AllowEdit = async () => await Per.系统预留.文件管理.素材库增删 };
            Forward(new SelectFilePage(new ResFileMgr { Setting = setting }, _owner));
        }
        
        void OnSearch(object sender, Mi e)
        {
            Forward(new SelectSearchPage(_owner));
        }
    }
}
