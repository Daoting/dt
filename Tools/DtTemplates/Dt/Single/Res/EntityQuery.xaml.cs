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
    public sealed partial class $clsroot$Query : Mv
    {
        public $clsroot$Query()
        {
            InitializeComponent();
        }

        public event EventHandler<Row> Search;

        protected override void OnInit(object p_params)
        {

        }

        void OnSearch(object sender, RoutedEventArgs e)
        {
            var row = _fv.Row;
            Result = row;
            Search?.Invoke(this, row);

            if (!IsHome)
                Backward();
        }

        $clsroot$Win _win => ($clsroot$Win)_tab.OwnWin;
    }
}
