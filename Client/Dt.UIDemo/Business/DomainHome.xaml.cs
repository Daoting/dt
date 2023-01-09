#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dt.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.UIDemo
{
    [View("业务样例")]
    public partial class DomainHome : Win
    {
        public DomainHome()
        {
            InitializeComponent();
            _nav.Data = new Nl<Nav>
            {
                new Nav("实体业务校验", typeof(HookWin), Icons.传真),
            };
        }

    }
}