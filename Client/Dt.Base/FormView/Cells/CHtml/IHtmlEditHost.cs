#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-11-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Threading.Tasks;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Html编辑器宿主接口
    /// </summary>
    public interface IHtmlEditHost
    {
        /// <summary>
        /// 初始Html内容
        /// </summary>
        string CurrentHtml { get; }

        /// <summary>
        /// 保存Html
        /// </summary>
        /// <param name="p_html"></param>
        /// <returns></returns>
        Task<bool> SaveHtml(string p_html);
    }
}
