#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace $rootnamespace$
{
    public sealed partial class $clsroot$Query : Tab
    {
        public $clsroot$Query()
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

        $clsroot$Win _win => ($clsroot$Win)OwnWin;
    }
}
