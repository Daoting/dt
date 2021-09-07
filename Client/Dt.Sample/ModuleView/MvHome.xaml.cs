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
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public sealed partial class MvHome : Win
    {
        public MvHome()
        {
            InitializeComponent();
            _lv.Data = new Nl<MainInfo>
            {
                new MainInfo { Title = "Tab内导航", Type = typeof(MvNavi) },
            };
        }
    }
}
