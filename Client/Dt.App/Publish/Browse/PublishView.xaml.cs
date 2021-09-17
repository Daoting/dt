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
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Publish
{
    [View("发布")]
    public partial class PublishView : Win
    {
        public PublishView() : this(-1)
        {
        }

        public PublishView(long p_postID)
        {
            InitializeComponent();
            _tab.Content = new PostList(this, p_postID);
        }
    }
}