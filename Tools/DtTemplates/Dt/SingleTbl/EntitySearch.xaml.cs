#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.App;
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

namespace $rootnamespace$
{
    public sealed partial class $entityname$Search : Mv
    {
        public $entityname$Search()
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

        $entityname$Win _win => ($entityname$Win)_tab.OwnWin;
    }
}
