#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-12-23 创建
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

namespace Dt.Sample
{
    public partial class DemoHookWin : Win
    {
        public DemoHookWin()
        {
            InitializeComponent();
        }

        public DemoHookList List => _list;

        public DemoHookForm Form => _form;
    }
}