#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Dt.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.App.Home
{
    /// <summary>
    /// 我的
    /// </summary>
    public sealed partial class MyMain : UserControl
    {
        public MyMain()
        {
            InitializeComponent();
            Init();
        }

        async void Init()
        {
            _tbName.Text = AtUser.Name;
            _tbPhone.Text = AtUser.Phone.Substring(0, 3) + "****" + AtUser.Phone.Substring(7, 4);
            string photo = AtUser.HasPhoto ? AtUser.GetPhotoPath(AtUser.ID) : AtUser.DefaultPhotoPath;
            await ImgKit.LoadImage(photo, _img);
        }

        void OnExit(object sender, RoutedEventArgs e)
        {
            AtSys.Logout();
        }

        async void OnClearLocalFile(object sender, RoutedEventArgs e)
        {
            if (await AtKit.Confirm("清除缓存后再次用到时需要重新下载，建议存储空间充足时不必清除。\r\n确认要清除吗？"))
            {
                AtLocal.ClearAllFiles();
                AtKit.Msg("清除完毕！");
            }
        }

        async void OnEditInfo(object sender, EventArgs e)
        {
            if (await new MyInfoDlg().ShowAsync())
            {

            }
        }
    }
}
