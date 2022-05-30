#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-06-08 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Publish
{
    public partial class PostDetail : Win
    {
        public PostDetail()
        {
            InitializeComponent();
        }

        public void Refresh(Row p_row)
        {
            _tab.Title = p_row.Str("title");
            // Android ios上WebView的 https 需要有效证书，临时用http！
            _wv.Source = new Uri($"{Kit.GetSvcUrl("fsm").Replace("https:", "http:")}/drv/g/{p_row.Str("url")}");
        }

        async void OnShare(object sender, Mi e)
        {
            await Kit.ShareText(Title, "福祉堂", _wv.Source.ToString());
        }
    }
}