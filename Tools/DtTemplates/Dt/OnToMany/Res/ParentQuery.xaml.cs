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
    public sealed partial class $parentroot$Query : Tab
    {
        public $parentroot$Query()
        {
            InitializeComponent();
        }

         /// <summary>
        /// 查询事件
        /// </summary>
        public event EventHandler<QueryClause> Query
        {
            add { _fv.Query += value; }
            remove { _fv.Query -= value; }
        }

        protected override void OnInit(object p_params)
        {
            var row = new Row();
$querydata$
            _fv.Data = row;
        }

        $parentroot$Win _win => ($parentroot$Win)OwnWin;
    }
}
