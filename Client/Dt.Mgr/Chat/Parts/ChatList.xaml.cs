#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-08 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Threading.Tasks;
using Windows.System.Threading;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
#endregion

namespace Dt.Mgr.Chat
{
    /// <summary>
    /// 聊天目录
    /// </summary>
    public sealed partial class ChatList : UserControl
    {
        public event Action<long> ItemClick;

        public ChatList()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await LoadData();
            ChatDs.StateChanged += OnStateChanged;
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // 页面卸载停止接收新信息
            ChatDs.StateChanged -= OnStateChanged;
        }

        void OnStateChanged(long p_otherID)
        {
            _ = LoadData();
        }

        /// <summary>
        /// 每次加载时刷新目录
        /// </summary>
        async Task LoadData()
        {
            _lv.Data = await AtLob.Query(
                "select l.*, \n" +
                "       m.photo \n" +
                "from   (select id, \n" +
                "               otherid, \n" +
                "               othername, \n" +
                "               unread, \n" +
                "               Max(stime) stime, \n" +
                "               ( case lettertype \n" +
                "                   when 0 then content \n" +
                "                   when 1 then '【文件】' \n" +
                "                   when 2 then '【照片】' \n" +
                "                   when 3 then '【语音】' \n" +
                "                   when 4 then '【视频】' \n" +
                "                   when 5 then '【链接】' \n" +
                "                   when 6 then '【撤回了一条消息】' \n" +
                "                 end )    as msg \n" +
                "        from   letter \n" +
                "        where  loginid = @loginid \n" +
                "        group  by otherid) l \n" +
                "       left join chatmember m \n" +
                "              on l.otherid = m.id \n" +
                "order  by stime desc",
                new Dict { { "loginid", Kit.UserID } });
        }

        void OnItemClick(ItemClickArgs e)
        {
            ItemClick?.Invoke(e.Row.Long("otherid"));
        }

        async void OnDelMsg(Mi e)
        {
            Row row = e.Row;
            if (await Kit.Confirm($"确认要清空与{row.Str("othername")}的聊天记录吗？"))
            {
                await AtLob.Exec($"delete from letter where otherid={row.Str("otherid")} and loginid={Kit.UserID}");
                await LoadData();
            }
        }
    }
}
