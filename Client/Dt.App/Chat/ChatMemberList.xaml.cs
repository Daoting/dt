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
            string val = AtState.GetCookie(_refreshKey);
            if (!string.IsNullOrEmpty(val) && DateTime.TryParse(val, out var last))
                refresh = (Kit.Now - last).TotalHours >= 10;

            if (refresh)
                RefreshList();
            else
                LoadLocalList();
        }

        async void RefreshList()
        {
            // 暂时取所有，后续增加好友功能
            var tbl = await AtCm.Query<ChatMember>("select id,name,phone,sex,(case photo when '' then 'photo/profilephoto.jpg' else photo end) as photo, mtime from cm_user");
            _lv.Data = tbl;

            // 将新列表缓存到本地库
            AtState.Exec("delete from ChatMember");
            if (tbl != null && tbl.Count > 0)
            {
                foreach (var r in tbl)
                {
                    r.IsAdded = true;
                }
                await AtState.BatchSave(tbl, false);
            }

            // 记录刷新时间
            AtState.SaveCookie(_refreshKey, Kit.Now.ToString());
        }

        void LoadLocalList()
        {
            _lv.Data = AtState.Query("select * from chatmember");
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            ItemClick?.Invoke(this, e.Row.ID);
        }
    }
}
