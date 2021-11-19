#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Dt.Base;
using Dt.Core;
using Dt.Core.Rpc;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
#endregion

namespace Dt.Sample
{
    public sealed partial class SearchMvWin : Win
    {
        public SearchMvWin()
        {
            InitializeComponent();
        }

        void OnSearch(object sender, string e)
        {
            _si.DoSearch(e);
        }
    }
}