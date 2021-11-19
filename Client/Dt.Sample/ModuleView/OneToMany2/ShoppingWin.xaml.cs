#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.Sample.ModuleView.OneToMany2
{
    [View("ShoppingWin")]
    public partial class ShoppingWin : Win
    {
        public ShoppingWin()
        {
            InitializeComponent();
        }

        public ShoppingList List => _list;

        public ShoppingGoodsList GoodsList => _goodsList;

    }
}