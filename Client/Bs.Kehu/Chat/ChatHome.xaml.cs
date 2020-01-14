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
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Kehu
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ChatHome : UserControl
    {
        public ChatHome()
        {
            InitializeComponent();

            var tbl = new Table
            {
                { "icon" },
                { "name" },
                { "unread", typeof(bool) },
                { "stime" },
                { "msg" },
            };
            tbl.AddRow(new { icon = "ms-appx:///Bs.Kehu/Assets/u7878.png", name = "爸", unread = false, stime = "昨天", msg = "老爸加油" });
            tbl.AddRow(new { icon = "ms-appx:///Bs.Kehu/Assets/u7884.png", name = "妈", unread = true, stime = "36分钟前", msg = "完成拉伸脊柱空间运动疗法" });
            tbl.AddRow(new { icon = "ms-appx:///Bs.Kehu/Assets/u7878.png", name = "岳父", unread = true, stime = "1小时前", msg = "完成仰卧举腿运动疗法" });
            tbl.AddRow(new { icon = "ms-appx:///Bs.Kehu/Assets/u7884.png", name = "岳母", unread = false, stime = "周二", msg = "[语音]" });
            tbl.AddRow(new { icon = "ms-appx:///Bs.Kehu/Assets/u7878.png", name = "大爷", unread = false, stime = "昨天", msg = "长内容，可视化报表模板设计，报表预览时支持导出、打印、简单编辑，支持报表绘制过程脚本。业务系统基础编辑器，配合Form和Sheet使用，通过配置参数指定编辑器种类和行为，可动态设置参数" });
            tbl.AddRow(new { icon = "ms-appx:///Bs.Kehu/Assets/u7884.png", name = "二舅", unread = false, stime = "52分钟前", msg = "[照片]" });
            tbl.AddRow(new { icon = "ms-appx:///Bs.Kehu/Assets/u7878.png", name = "大姑", unread = false, stime = "昨天", msg = "好多了" });
            tbl.AddRow(new { icon = "ms-appx:///Bs.Kehu/Assets/u7884.png", name = "老叔", unread = true, stime = "10分钟前", msg = "拉的有点疼" });

            tbl.AddRow(new { icon = "ms-appx:///Bs.Kehu/Assets/u7878.png", name = "阿伟", unread = false });
            tbl.AddRow(new { icon = "ms-appx:///Bs.Kehu/Assets/u7884.png", name = "阿珍", unread = false,  });
            tbl.AddRow(new { icon = "ms-appx:///Bs.Kehu/Assets/u7878.png", name = "包拯", unread = false, });
            tbl.AddRow(new { icon = "ms-appx:///Bs.Kehu/Assets/u7884.png", name = "三叔", unread = false, });
            _lv.Data = tbl;
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            AtApp.OpenWin(typeof(ChatLetter));
        }
    }
}