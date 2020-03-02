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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 聊天目录
    /// </summary>
    public sealed partial class ChatList : UserControl
    {
        public event EventHandler<long> ItemClick;

        public ChatList()
        {
            InitializeComponent();
            Loaded += (sender, e) => LoadData();
        }

        /// <summary>
        /// 每次加载时刷新目录
        /// </summary>
        void LoadData()
        {
            _lv.Data = AtLocal.Query<Letter>(
                "select *" +
                "  from (select id," +
                "               otherid," +
                "               othername," +
                "               unread," +
                "               max(stime) stime," +
                "               (case LetterType" +
                "                 when 0 then content" +
                "                 when 1 then '【文件】'" +
                "                 when 2 then '【照片】'" +
                "                 when 3 then '【语音】'" +
                "                 when 4 then '【视频】'" +
                "                 when 5 then '【链接】'" +
                "                 when 6 then '【撤回了一条消息】'" +
                "               end) as msg" +
                "          from Letter" +
                "         where loginid = @loginid" +
                "         group by OtherID)" +
                " order by stime desc",
                new Dict { { "loginid", AtUser.ID } });
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {

        }
    }
}
