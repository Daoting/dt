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
    [View("实体")]
    public partial class 实体Win : Win
    {
        public 实体Win()
        {
            InitializeComponent();
            Form = new 实体Form { OwnWin = this };
        }

        public 实体List List => _list;

        public 实体Form Form { get; }

        public 实体Query Query => _query;

        void OnQuery(QueryClause e)
        {
            _list.OnSearch(e);
        }
    }
}