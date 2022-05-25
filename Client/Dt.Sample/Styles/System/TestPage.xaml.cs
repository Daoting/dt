#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
#endregion

namespace Dt.Sample
{
    public sealed partial class TestPage : Page
    {
        public TestPage()
        {
            Console.WriteLine("******************Before TestPage");
            InitializeComponent();
            Console.WriteLine("******************TestPage");
        }

    }
}