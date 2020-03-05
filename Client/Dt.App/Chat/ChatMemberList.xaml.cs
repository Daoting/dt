#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-08 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.ComponentModel;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Chat
{
    /// <summary>
    /// 聊天人员列表
    /// </summary>
    public sealed partial class ChatMemberList : UserControl
    {
        const string _refreshKey = "LastRefreshChatMember";

        public event EventHandler<long> ItemClick;

        public ChatMemberList()
        {
            InitializeComponent();

            // 按姓名排序
            _lv.SortDesc = new SortDescription("name", ListSortDirection.Ascending);
            Loaded += OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            // 超过10小时需要刷新
            bool refresh = true;
            string val = AtLocal.GetCookie(_refreshKey);
            if (!string.IsNullOrEmpty(val) && DateTime.TryParse(val, out var last))
                refresh = (AtSys.Now - last).TotalHours >= 10;

            if (refresh)
                RefreshList();
            else
                LoadLocalList();
        }

        async void RefreshList()
        {
            // 暂时取所有，后续增加好友功能
            var newTbl = await AtCm.Query("select * from cm_user");
            newTbl.Columns.Add(new Column("photo"));
            _lv.Data = newTbl;

            if (newTbl != null && newTbl.Count > 0)
            {
                foreach (Row row in newTbl)
                {
                    long id = row.ID;
                    string path = AtUser.GetPhotoPath(id);
                    var mem = AtLocal.GetFirst<ChatMember>("select id,mtime from ChatMember where id=@id", new Dict { { "id", id } });

                    // 有头像 且 本地无记录或最后修改时间不同时，下载头像文件
                    if (row.Bool("hasphoto")
                        && (mem == null || mem.Mtime != row.Date("mtime")))
                    {
                        await Downloader.GetAndCacheFile(path);
                    }

                    // 无头像文件显示缺省
                    row["photo"] = File.Exists(Path.Combine(AtLocal.CachePath, id + ".jpg")) ? path : AtUser.DefaultPhotoPath;
                }
            }

            // 将新列表缓存到本地库
            AtLocal.Execute("delete from ChatMember");
            if (newTbl != null && newTbl.Count > 0)
                AtLocal.Save(newTbl, "ChatMember");

            // 记录刷新时间
            AtLocal.SaveCookie(_refreshKey, AtSys.Now.ToString());
        }

        void LoadLocalList()
        {
            _lv.Data = AtLocal.Query(
                "select id, \n" +
                "       name, \n" +
                "       ( case hasphoto \n" +
                "           when 1 then 'sys/photo/' || id || '.jpg' \n" +
                "           else 'sys/photo/profilephoto.jpg' \n" +
                "         end ) as photo \n" +
                "from   chatmember");
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            ItemClick?.Invoke(this, e.Row.Long("id"));
        }
    }
}
