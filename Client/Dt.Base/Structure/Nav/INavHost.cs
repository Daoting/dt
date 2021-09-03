#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-10-22 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 支持内部导航的宿主接口，如Tab Dlg都支持内部导航
    /// </summary>
    public interface INavHost
    {
        /// <summary>
        /// 向前导航到新内容
        /// </summary>
        /// <param name="p_content"></param>
        void Forward(Nav p_content);

        /// <summary>
        /// 向后导航到上一内容
        /// </summary>
        void Backward();
    }
}
