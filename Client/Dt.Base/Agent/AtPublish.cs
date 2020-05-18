#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-05-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using System.Threading.Tasks;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 发布服务Api代理类（自动生成）
    /// </summary>
    public static class AtPublish
    {
        #region Publish
        /// <summary>
        /// 创建静态页面
        /// </summary>
        /// <param name="p_title">页面标题</param>
        /// <param name="p_content">页面内容</param>
        /// <returns>返回静态页面的路径，生成失败返回null</returns>
        public static Task<string> CreatePage(string p_title, string p_content)
        {
            return new UnaryRpc(
                "pub",
                "Publish.CreatePage",
                p_title,
                p_content
            ).Call<string>();
        }

        /// <summary>
        /// 创建测试页面
        /// </summary>
        /// <param name="p_title">页面标题</param>
        /// <param name="p_content">页面内容</param>
        /// <returns>返回页面路径，生成失败返回null</returns>
        public static Task<string> CreateTestPage(string p_title, string p_content)
        {
            return new UnaryRpc(
                "pub",
                "Publish.CreateTestPage",
                p_title,
                p_content
            ).Call<string>();
        }
        #endregion
    }
}
