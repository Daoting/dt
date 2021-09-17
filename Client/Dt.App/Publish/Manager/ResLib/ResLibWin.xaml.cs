#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.App.File;
using Dt.Base;
using Dt.Core;
#endregion

namespace Dt.App.Publish
{
    public partial class ResLibWin : Win
    {
        public ResLibWin()
        {
            InitializeComponent();
            LoadContent();
        }

        async void LoadContent()
        {
            var setting = new FileMgrSetting
            {
                AllowEdit = await Kit.HasPrv("素材库管理"),
                SaveHistory = false,
            };
            _tab.Content = new FolderPage(new ResFileMgr { Setting = setting });
        }
    }
}