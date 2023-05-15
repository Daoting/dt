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
using System.Linq;
using System.Text;
#endregion

namespace Dt.Mgr.Module
{
    /// <summary>
    /// 文件
    /// </summary>
    [View(LobViews.文件)]
    public partial class FileHome : Win
    {
        public FileHome()
        {
            InitializeComponent();
            LoadContent();
        }

        void LoadContent()
        {
            var setting = new FileMgrSetting
            {
                AllowEdit = async () => await Kit.HasPermission("公共文件管理"),
                OnOpenedFile = LoadHistory,
            };
            _tabPub.NaviParams = new PubFileMgr { Setting = setting };

            setting = new FileMgrSetting
            {
                AllowEdit = () => Task.FromResult(true),
                OnOpenedFile = LoadHistory,
            };
            _tabMy.NaviParams = new MyFileMgr { Setting = setting };

            LoadHistory();
        }

        void LoadHistory()
        {
            Kit.RunAsync(async () =>
            {
                var ls = await AtLob.Each<ReadFileHistoryX>("select info from ReadFileHistory order by LastReadTime desc limit 20");
                StringBuilder sb = new StringBuilder();
                foreach (var file in ls)
                {
                    if (!string.IsNullOrEmpty(file.Info))
                    {
                        if (sb.Length > 0)
                            sb.Append(",");
                        sb.Append(file.Info.Substring(1, file.Info.Length - 2));
                    }
                }
                if (sb.Length > 0)
                {
                    sb.Insert(0, "[");
                    sb.Append("]");
                    _fl.Data = sb.ToString();
                }
                else
                {
                    _fl.Data = null;
                }
            });
        }

        async void OnClearHis(object sender, Mi e)
        {
            if (_fl.Items.Count() == 0)
                return;

            if (await Kit.Confirm("确认要清空历史记录吗？"))
            {
                await AtLob.Exec("delete from ReadFileHistory");
                _fl.Data = null;
            }
        }

        async void OnDeleteHis(object sender, Mi e)
        {
            if (await Kit.Confirm("确认要删除当前历史记录吗？"))
            {
                if (await AtLob.Exec("delete from ReadFileHistory where info like @info", new Dict { { "info", $"[[\"{((FileItem)e.DataContext).ID}%" } }) > 0)
                    LoadHistory();
            }
        }
    }
}