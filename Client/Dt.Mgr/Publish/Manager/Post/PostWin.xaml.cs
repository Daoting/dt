#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.Mgr.Publish
{
    public partial class PostWin : Win
    {
        public PostWin()
        {
            InitializeComponent();
        }

        public PostxList List => _list;

        public PostForm Form => _form;

        public PostKeywordList KeywordList => _keywordList;

        public PostAlbumList AlbumList => _albumList;

    }
}