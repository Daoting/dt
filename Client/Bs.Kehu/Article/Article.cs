#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-12-10
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.Docking;
using Dt.Base.FormView;
using Dt.Core;
using Dt.Core.Model;
using Dt.Kehu;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
#endregion

namespace Dt.Kehu
{

    public class Article
    {
        public long ID { get; set; }

        public long AuthorID { get; set; }

        public string AuthorIcon { get; set; }

        public string AuthorName { get; set; }

        public string JobTitle { get; set; }

        public string PubDate { get; set; }

        public string Title { get; set; }

        public string Img { get; set; }

        public string ShareCnt { get; set; }

        public string CommentCnt { get; set; }

        public string LikeCnt { get; set; }

    }
}
