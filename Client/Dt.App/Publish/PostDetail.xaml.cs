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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Essentials;
#endregion

namespace Dt.App.Publish
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
            _wv.Source = new Uri($"{AtSys.Stub.ServerUrl.TrimEnd('/')}/pub/g/{p_row.Str("url")}");
        }

        async void OnShare(object sender, Mi e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Subject = "福祉堂",
                Text = Title,
                Uri = _wv.Source.ToString()
            });
        }
    }
}