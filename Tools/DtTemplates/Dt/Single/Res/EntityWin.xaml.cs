#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#endregion

namespace $rootnamespace$
{
    [View("$clsroot$Win")]
    public partial class $clsroot$Win : Win
    {
        public $clsroot$Win()
        {
            InitializeComponent();
        }

        public $clsroot$List List => _list;

        public $clsroot$Form Form => _form;

        public SearchMv Search => _search;

        public $clsroot$Query Query => _query;

        void OnSearch(object sender, object e)
        {
            _list.OnSearch(e);
        }
    }
}