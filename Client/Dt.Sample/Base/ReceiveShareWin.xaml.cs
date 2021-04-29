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
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class ReceiveShareWin : Win
    {
        ShareInfo _info;

        public ReceiveShareWin(ShareInfo p_info)
        {
            InitializeComponent();
            _info = p_info;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("分享类型：" + _info.DataType.ToString());
            if (!string.IsNullOrEmpty(_info.Content))
                sb.AppendLine("文本内容：" + _info.Content);
            if (!string.IsNullOrEmpty(_info.FileName))
                sb.AppendLine("文件名：" + _info.FileName);

            _tb.Text = sb.ToString();
        }

        void OnEnd(object sender, RoutedEventArgs e)
        {
            _info.ShareCompleted();
        }
    }
}