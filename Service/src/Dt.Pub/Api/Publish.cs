#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.IO;
using System.Threading.Tasks;
#endregion

namespace Dt.Pub
{
    /// <summary>
    /// 发布服务Api
    /// </summary>
    [Api]
    public class Publish : BaseApi
    {
        const string _template =
            "<!DOCTYPE html>\n" +
            "<html>\n" +
            "<head>\n" +
            "    <title>{0}</title>\n" +
            "    <link rel=\"stylesheet\" href=\"../css/froala_editor.pkgd.min.css\">\n" +
            "</head>\n" +
            "<body>\n" +
            "    <div class=\"fr-element fr-view\">\n" +
            "        {1}\n" +
            "    </div>\n" +
            "</body>\n" +
            "</html>";

        public async Task<string> SavePost(Post p_post)
        {
            if (p_post == null)
                return "待保存的文章对象为null";

            if (!p_post.IsAdded && !p_post.IsChanged)
                return "文章对象不需要保存";

            if (!p_post.IsAdded
                && (p_post.Cells["title"].IsChanged || p_post.Cells["content"].IsChanged))
            {
                if (!string.IsNullOrEmpty(p_post.Url))
                {
                    // 删除旧文件
                }

                string url = null;
                if (!string.IsNullOrEmpty(p_post.Title) && !string.IsNullOrEmpty(p_post.Content))
                {
                    url = await BuildHtmlPage(p_post.Title, p_post.Content, Glb.Now.ToString("yyyyMM"));
                }
                p_post.SetUrl(url);
            }
            
            Repo<Post> repo = new Repo<Post>();
            bool suc = await repo.Save(p_post);
            return "";
        }

        /// <summary>
        /// 创建静态页面
        /// </summary>
        /// <param name="p_title">页面标题</param>
        /// <param name="p_content">页面内容</param>
        /// <returns>返回静态页面的路径，生成失败返回null</returns>
        public Task<string> CreatePage(string p_title, string p_content)
        {
            return BuildHtmlPage(p_title, p_content, Glb.Now.ToString("yyyyMM"));
        }

        /// <summary>
        /// 创建测试页面
        /// </summary>
        /// <param name="p_title">页面标题</param>
        /// <param name="p_content">页面内容</param>
        /// <returns>返回页面路径，生成失败返回null</returns>
        public Task<string> CreateTestPage(string p_title, string p_content)
        {
            return BuildHtmlPage(p_title, p_content, "test");
        }

        async Task<string> BuildHtmlPage(string p_title, string p_content, string p_folder)
        {
            if (string.IsNullOrEmpty(p_title)
                || string.IsNullOrEmpty(p_content)
                || string.IsNullOrEmpty(p_folder))
                return null;

            string pageContent = string.Format(_template, p_title, p_content);
            string pageName = $"{Id.New()}.html";
            try
            {
                // BaseDirectory程序集所在的目录，不可用Directory.GetCurrentDirectory()！
                string path = Path.Combine(AppContext.BaseDirectory, "wwwroot", p_folder);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                using (var stream = File.Create(Path.Combine(path, pageName)))
                using (var sw = new StreamWriter(stream))
                {
                    await sw.WriteAsync(pageContent);
                    await sw.FlushAsync();
                }
            }
            catch
            {
                return null;
            }
            return $"{p_folder}/{pageName}";
        }
    }
}
