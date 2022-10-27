namespace Dt.Agent
{
    /// <summary>
    /// 发布服务Api
    /// </summary>
    public partial class AtCm
    {
        /// <summary>
        /// 保存文章，返回文章地址
        /// </summary>
        /// <param name="p_post"></param>
        /// <returns></returns>
        public static Task<string> SavePost(Row p_post)
        {
            return Kit.Rpc<string>(
                "cm",
                "Publish.SavePost",
                p_post
            );
        }

        /// <summary>
        /// 创建静态页面
        /// </summary>
        /// <param name="p_title">页面标题</param>
        /// <param name="p_content">页面内容</param>
        /// <returns>返回静态页面的路径，生成失败返回null</returns>
        public static Task<string> CreatePage(string p_title, string p_content)
        {
            return Kit.Rpc<string>(
                "cm",
                "Publish.CreatePage",
                p_title,
                p_content
            );
        }

        /// <summary>
        /// 创建测试页面
        /// </summary>
        /// <param name="p_title">页面标题</param>
        /// <param name="p_content">页面内容</param>
        /// <returns>返回页面路径，生成失败返回null</returns>
        public static Task<string> CreateTestPage(string p_title, string p_content)
        {
            return Kit.Rpc<string>(
                "cm",
                "Publish.CreateTestPage",
                p_title,
                p_content
            );
        }
    }
}
