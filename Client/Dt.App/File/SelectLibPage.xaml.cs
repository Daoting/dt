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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.File
{
    /// <summary>
    /// 选择库
    /// </summary>
    public sealed partial class SelectLibPage : UserControl, INaviContent
    {
        readonly SelectFileDlg _owner;

        public SelectLibPage(SelectFileDlg p_owner)
        {
            InitializeComponent();
            _owner = p_owner;
        }

        async void OnPublicFile(object sender, RoutedEventArgs e)
        {
            var setting = new FileMgrSetting { AllowEdit = await AtUser.HasPrv("公共文件管理") };
            _host.NaviTo(new SelectFilePage(new PubFileMgr { Setting = setting }, _owner));
        }

        void OnMyFile(object sender, RoutedEventArgs e)
        {
            _host.NaviTo(new SelectFilePage(new MyFileMgr { Setting = new FileMgrSetting { AllowEdit = true } }, _owner));
        }

        async void OnResFile(object sender, RoutedEventArgs e)
        {
            var setting = new FileMgrSetting { AllowEdit = await AtUser.HasPrv("素材库管理") };
            _host.NaviTo(new SelectFilePage(new ResFileMgr { Setting = setting }, _owner));
        }
        
        void OnSearch(object sender, Mi e)
        {
            _host.NaviTo(new SelectSearchPage(_owner));
        }

        #region INaviContent
        INaviHost _host;

        void INaviContent.AddToHost(INaviHost p_host)
        {
            _host = p_host;
            _host.Title = "选择文件位置";

            var menu = new Menu();
            Mi mi = new Mi { ID = "搜索", Icon = Icons.搜索, ShowInPhone = VisibleInPhone.Icon };
            mi.Click += OnSearch;
            menu.Items.Add(mi);
            _host.Menu = menu;
        }
        #endregion

    }
}
