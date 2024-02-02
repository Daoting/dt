#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo
{
    [View("虚拟")]
    public partial class 虚拟Win : Win
    {
        public 虚拟Win()
        {
            InitializeComponent();
            Form = new 虚拟Form { OwnWin = this };
        }

        public 虚拟List List => _list;

        public 虚拟Form Form { get; }

        public 虚拟Query Query => _query;

        void OnQuery(QueryClause e)
        {
            _list.OnSearch(e);
        }
    }
}