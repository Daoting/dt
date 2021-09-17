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
            _lv.Data = new Nl<MainInfo>
            {
                new MainInfo(Icons.书籍, "文章管理", typeof(PostWin), ""),
                new MainInfo(Icons.展开, "关键字", typeof(KeywordWin), ""),
                new MainInfo(Icons.文件, "文章专辑", typeof(AlbumWin), ""),
                new MainInfo(Icons.词典, "素材库", typeof(ResLibWin), ""),
                new MainInfo(Icons.留言, "评论管理", default(Type), ""),
            };
        }
    }
}
