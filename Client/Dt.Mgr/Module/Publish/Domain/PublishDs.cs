#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Module
{
    class PublishDs : DomainSvc<PublishDs, AtCm.Info>
    {
        /// <summary>
        /// 保存文章，返回文章地址
        /// </summary>
        /// <param name="p_post"></param>
        /// <returns></returns>
        public static async Task<bool> SavePost(PubPostX p_post)
        {
            Throw.If(!p_post.IsAdded && !p_post.IsChanged, "文章对象不需要保存");

            string oldUrl = null;

            // 标题或内容变化时需要重新生成html
            if (p_post.Cells["title"].IsChanged || p_post.Cells["content"].IsChanged)
            {
                oldUrl = p_post.Url;
                if (!string.IsNullOrEmpty(p_post.Title) && !string.IsNullOrEmpty(p_post.Content))
                {
                    p_post.Url = await BuildHtmlPage(p_post.Title, p_post.Content, Kit.Now.ToString("yyyyMM"));
                }
            }

            bool suc = await p_post.Save();
            if (suc && oldUrl != null)
            {
                // 删除旧文件
                await AtFsm.DeleteFile($"g/{oldUrl}");
            }
            return suc;
        }

        /// <summary>
        /// 创建静态页面
        /// </summary>
        /// <param name="p_title">页面标题</param>
        /// <param name="p_content">页面内容</param>
        /// <returns>返回静态页面的路径，生成失败返回null</returns>
        public static Task<string> CreatePage(string p_title, string p_content)
        {
            return BuildHtmlPage(p_title, p_content, Kit.Now.ToString("yyyyMM"));
        }

        async static Task<string> BuildHtmlPage(string p_title, string p_content, string p_folder)
        {
            if (string.IsNullOrEmpty(p_title)
                || string.IsNullOrEmpty(p_content)
                || string.IsNullOrEmpty(p_folder))
                return null;

            string pageContent = string.Format(_template, p_title, p_content);
            string pageName = await PubPostX.NewID() + ".html";
            var result = await AtFsm.SaveFile($"g/{p_folder}/{pageName}", pageContent);
            if (string.IsNullOrEmpty(result))
                return $"{p_folder}/{pageName}";
            return null;
        }

        const string _template =
            "<!DOCTYPE html>\n" +
            "<html>\n" +
            "<head>\n" +
            "    <meta charset=\"utf-8\">\n" +
            "    <title>{0}</title>\n" +
            "    <link rel=\"stylesheet\" href=\"../froala.css\">\n" +
            "</head>\n" +
            "<body>\n" +
            "    <div class=\"fr-element fr-view\">\n" +
            "        {1}\n" +
            "    </div>\n" +
            "</body>\n" +
            "</html>";
    }
}