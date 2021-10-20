#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-10-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
#endregion

namespace Dt.App.Publish
{
    [View("发布管理")]
    public sealed partial class PublishMgr : Win
    {
        public PublishMgr()
        {
            InitializeComponent();
            _nav.Data = new Nl<Nav>
            {
                new Nav("文章管理", typeof(PostWin), Icons.书籍),
                new Nav("关键字", typeof(KeywordWin), Icons.展开),
                new Nav("文章专辑", typeof(AlbumWin), Icons.文件),
                new Nav("素材库", typeof(ResLibWin), Icons.词典),
                new Nav("评论管理", default(Type), Icons.留言),
            };
        }
    }
}
