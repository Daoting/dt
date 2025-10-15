#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-27 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 默认首页Api
    /// </summary>
    [Api(IsTest = true)]
    public class HomeApi : DomainSvc
    {
        public List<string> GetInitInfo()
        {
            var ls = new List<string>();
            ls.Add($"{Kit.AppName}");
            ls.Add(GetPkg());
            // 版本号
            var ver = typeof(HomeApi).Assembly.GetName().Version.ToString(3);
            ls.Add(ver);
            ls.Add(GetSvcs());
            ls.Add(GetModelInfo());
            return ls;
        }

        public bool UpdatePkg()
        {
            return Pkg.UpdatePkgVer();
        }
        
        string GetPkg()
        {
            var sb = new StringBuilder("<div class=\"row\">");

            sb.Append("<div class=\"cell\">Win10+</div>");
            var x64 = Pkg.WinAppVer.Str("x64");
            var arm64 = Pkg.WinAppVer.Str("arm64");
            if (x64 == "" && arm64 == "")
            {
                sb.Append("<div class=\"cell\">无安装包</div>");
            }
            else
            {
                if (x64 == "")
                {
                    sb.Append("<div class=\"cell\">无x64安装包</div>");
                }
                else
                {
                    sb.Append($"<div class=\"cell\"><a href=\"./pkg/win/{x64}\">{x64}</a></div>");
                }

                if (arm64 == "")
                {
                    sb.Append("<div class=\"cell\">无arm64安装包</div>");
                }
                else
                {
                    sb.Append($"<div class=\"cell\"><a href=\"./pkg/win/{arm64}\">{arm64}</a></div>");
                }

                if (!string.IsNullOrEmpty(Pkg.WinCerFile))
                    sb.Append($"<div class=\"cell\"><a href=\"./pkg/win/{Pkg.WinCerFile}\">下载证书</a></div>");
            }
            sb.Append("</div>");

            sb.Append("<div class=\"row\"><div class=\"cell\" style=\"margin: 0 1.5rem 0 0;\">手机</div>");
            if (string.IsNullOrEmpty(Pkg.ApkFile))
                sb.Append("<div class=\"cell\">无安卓安装包</div>");
            else
                sb.Append($"<div class=\"cell\"><a href=\"./pkg/android/{Pkg.ApkFile}\">{Pkg.ApkFile}</a></div>");
            sb.Append("</div>");

            sb.Append("<div class=\"row\"><div class=\"cell\" style=\"margin: 0 1.5rem 0 0;\">桌面</div>");
            sb.Append("<div class=\"cell\"><a href=\"./pkg/win/install.cer\">Win7-</a></div>");
            sb.Append("<div class=\"cell\"><a href=\"./pkg/win/install.cer\">Linux</a></div>");
            sb.Append("<div class=\"cell\"><a href=\"./pkg/win/install.cer\">Mac</a></div>");
            sb.Append("</div>");

            sb.Append("<div class=\"row\"><div class=\"cell\" style=\"margin: 0 1.5rem 0 0;\">Web</div><div class=\"cell\"><a href=\"./wasm\" target=\"blank\">打开Web应用</a></div></div>");

            return sb.ToString();
        }

        string GetSvcs()
        {
            if (Kit.IsSingletonSvc)
                return "<div class=\"row\"><div class=\"cell\"><a href=\"./.admin\" target=\"blank\">单体服务 API</a></div></div>";

            var sb = new StringBuilder("<div class=\"row\">");

            var req = Kit.HttpContext.Request;
            string urlCm = req.Scheme + "://" + req.Host;
            var match = Regex.Match(urlCm, @"^http[s]?://[^\s:/]+");
            string prefix = "";
            if (match.Success)
                prefix = match.Value;

            sb.Append("<div class=\"cell\"><a href=\"./.admin\" target=\"blank\">cm API</a></div>");
            var urls = (Dict)SysKernel.Config["SvcUrls"];
            foreach (var item in urls)
            {
                var url = item.Value as string;
                if (url.StartsWith("*"))
                    url = prefix + url.Substring(1).TrimEnd('\\');
                sb.Append($"<div class=\"cell\"><a href=\"{url}/.admin\" target=\"blank\">{item.Key} API</a></div>");
            }
            sb.Append("</div>");
            return sb.ToString();
        }

        string GetModelInfo()
        {
            if (!SysKernel.Config.ContainsKey("SqliteVer"))
            {
                return "<div class=\"row\"><div class=\"cell\" style=\"color: red;\">当前无模型文件，请点击右上角的【更新所有】生成模型文件！</div></div>";
            }

            var sb = new StringBuilder("<div class=\"row\">");
            var model = Kit.GetService<SqliteFileHandler>();
            foreach (var file in model.GetAllFile())
            {
                sb.Append($"<div class=\"cell\"><a onclick=\"updateModel('{file}')\" href=\"javascript:void(0);\">更新{file}</a></div>");
            }

            sb.Append("</div>");
            return sb.ToString();
        }
    }
}
