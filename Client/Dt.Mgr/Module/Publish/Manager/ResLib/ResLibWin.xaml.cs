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
#endregion

namespace Dt.Mgr.Module
{
    public partial class ResLibWin : Win
    {
        public ResLibWin()
        {
            InitializeComponent();
            LoadContent();
        }

        void LoadContent()
        {
            var setting = new FileMgrSetting
            {
                AllowEdit = async () => await LobKit.HasPermission("素材库管理"),
                SaveHistory = false,
            };
            _tab.NaviParams = new ResFileMgr { Setting = setting };
        }
    }
}