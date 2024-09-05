#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-12-24 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Threading.Tasks;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 页面参数
    /// </summary>
    internal class PhonePageParameter
    {
        public PhonePageParameter(IPhonePage p_content, TaskCompletionSource<bool> p_taskSource)
        {
            Content = p_content;
            TaskSource = p_taskSource;
        }

        /// <summary>
        /// 页面内容
        /// </summary>
        public IPhonePage Content;

        /// <summary>
        /// 可等待页面关闭的任务
        /// </summary>
        public TaskCompletionSource<bool> TaskSource;
    }
}