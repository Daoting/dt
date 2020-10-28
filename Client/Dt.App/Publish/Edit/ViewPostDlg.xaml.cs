#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-06-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using Xamarin.Essentials;
#endregion

namespace Dt.App.Publish
{
    public partial class ViewPostDlg : Dlg
    {
        public ViewPostDlg(PostMgr p_owner)
        {
            InitializeComponent();

            if (!AtSys.IsPhoneUI)
            {
                ShowWinVeil = true;
                Height = AtApp.ViewHeight - 140;
                Width = Math.Min(900, AtApp.ViewWidth - 200);
            }
            Title = p_owner.CurrentPost.Title;
            _wv.Source = new Uri($"{AtSys.Stub.ServerUrl.TrimEnd('/')}/pub/{p_owner.CurrentPost.Url}");
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