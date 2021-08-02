#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-08-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 向导页面基类
    /// </summary>
    public partial class GuidePage : UserControl
    {
        readonly TaskCompletionSource<bool> _taskSrc = new TaskCompletionSource<bool>();

        /// <summary>
        /// 关闭任务
        /// </summary>
        internal Task AsyncTask => _taskSrc.Task;

        /// <summary>
        /// 关闭向导页面
        /// </summary>
        /// <returns></returns>
        public void Close()
        {
            _taskSrc?.SetResult(true);
        }
    }
}