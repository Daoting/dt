#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-05-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Crud
{
    public partial class CPickDemo : Win
    {
        public CPickDemo()
        {
            InitializeComponent();
            _fv.Data = new Row
            {
                { "menu", typeof(string) },
                { "parentid", typeof(long) },
                { "parent", typeof(string) },
                { "child1", typeof(string) },
                { "child2", typeof(string) },
                { "localmenu", typeof(string) },
            };
        }

    }
}