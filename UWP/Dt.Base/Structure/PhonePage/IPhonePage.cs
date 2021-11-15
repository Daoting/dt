#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-09-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Threading.Tasks;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 可承载在PhonePage页面中的内容需要实现的接口
    /// </summary>
    public interface IPhonePage
    {
        /// <summary>
        /// 关闭或后退之前，返回false表示禁止关闭
        /// </summary>
        /// <returns>true 表允许关闭</returns>
        Task<bool> OnClosing();

        /// <summary>
        /// 关闭或后退之后
        /// </summary>
        void OnClosed();
    }
}