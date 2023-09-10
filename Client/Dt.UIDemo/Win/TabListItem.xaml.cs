#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.FormView;
using Dt.Core;
using System;
using Windows.Foundation;
using Windows.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
#endregion

namespace Dt.UIDemo
{
    public partial class TabListItem : Tab
    {
        public TabListItem()
        {
            InitializeComponent();
        }

        protected override void OnFirstLoaded()
        {
            Result = new Random().Next(100);
            Title = "Tab " + Result.ToString();
            _tb.Text = Title;
        }

    }
}