#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.App.Publish
{
    public partial class KeywordWin : Win
    {
        public KeywordWin()
        {
            InitializeComponent();
        }

        public KeywordList List => _list;

        public KeywordForm Form => _form;
    }
}