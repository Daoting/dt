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

namespace Dt.Base.Chat
{
    /// <summary>
    /// 聊天人员列表
    /// </summary>
    public sealed partial class ChatMemberList : UserControl
    {
        public event EventHandler<long> ItemClick;

        public ChatMemberList()
        {
            InitializeComponent();

            // 按姓名排序
            _lv.SortDesc = new SortDescription("name", ListSortDirection.Ascending);
            Loaded += OnLoaded;
        }

        async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            await FriendMemberList.Refresh();
            LoadLocalList();
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
