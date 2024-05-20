#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-05 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo
{
    [View("用户")]
    public partial class 用户Win : Win
    {
        public 用户Win()
        {
            InitializeComponent();
            MainForm = new 用户Form { OwnWin = this };
        }

        public 用户List MainList => _mainList;

        public 用户Form MainForm { get; }

        public 用户角色List 角色List => _角色List;

        public 用户Query Query => _query;

        void OnQuery(QueryClause e)
        {
            _mainList.OnSearch(e);
        }
    }
}