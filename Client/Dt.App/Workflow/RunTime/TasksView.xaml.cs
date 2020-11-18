#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
#endregion

namespace Dt.App.Workflow
{
    [View("任务")]
    public partial class TasksView : Win
    {
        public TasksView()
        {
            InitializeComponent();
        }

        void OnStart(object sender, RoutedEventArgs e)
        {
            new StartWorkflow().Open();
        }
    }
}