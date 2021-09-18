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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.Sample.ModuleView.OneToMany1
{
    [View("ShoppingWin")]
    public partial class ShoppingWin : Win
    {
        public ShoppingWin()
        {
            InitializeComponent();
        }

        public ShoppingList List => _list;

        public ShoppingForm Form => _form;

        public ShoppingGoodsList GoodsList => _goodsList;

    }
}