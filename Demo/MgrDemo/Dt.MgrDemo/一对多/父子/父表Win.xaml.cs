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
    [View("父表")]
    public partial class 父表Win : Win
    {
        public 父表Win()
        {
            InitializeComponent();
            ParentForm = new 父表Form { OwnWin = this };
        }

        public 父表List ParentList => _parentList;

        public 父表Form ParentForm { get; }

        public 父表大儿List 大儿List => _大儿List;

        public 父表小儿List 小儿List => _小儿List;

        public 父表Query Query => _query;

        void OnQuery(QueryClause e)
        {
            _parentList.OnSearch(e);
        }
    }
}