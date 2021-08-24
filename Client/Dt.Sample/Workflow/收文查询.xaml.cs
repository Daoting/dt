#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.App;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class 收文查询 : Win
    {
        public 收文查询()
        {
            InitializeComponent();

            _fv.Data = new 收文(ID: 0);
            AtWf.AddMenu(_lv);
        }

        async void OnSearch(object sender, Mi e)
        {
            _lv.Data = await AtCm.GetAll<收文>();
            NaviTo("结果");
        }

    }
}