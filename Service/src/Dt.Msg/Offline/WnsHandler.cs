#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Serilog;
using System;
using System.IO;
using System.Net;
using System.Web;
#endregion

namespace Dt.Msg
{
    /*
    /// <summary>
    /// WNS推送类
    /// </summary>
    public class WnsHandler
    {
        string _wnsToken;
        bool _gettingToken;

        public WnsHandler()
        {
            GetWnsToken();
            if (!string.IsNullOrEmpty(_wnsToken))
                Log.Information("已获取WNS访问令牌");
        }

        /// <summary>
        /// Post到WNS，实现toast推送
        /// </summary>
        /// <param name="p_uri">设备通道Uri</param>
        /// <param name="p_data">推送内容</param>
        /// <returns>通道 URI 是否有效，无效时后续删除</returns>
        public async Task<bool> PostToWns(string p_uri, string p_data)
        {
            bool valid = true;
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-WNS-Type", "wns/toast");
                    client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", _wnsToken));
                    client.DefaultRequestHeaders.Add("Content-Type", "text/xml");
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(p_uri),
                        Content = new StringContent(p_data),
                    };

                    // 侦听来自 WNS 确认收到通知的响应
                    var response = await client.SendAsync(request).ConfigureAwait(false);
                }
            }
            catch (WebException webException)
            {
                HttpStatusCode status = ((HttpWebResponse)webException.Response).StatusCode;
                if (status == HttpStatusCode.Unauthorized)
                {
                    // 你出示的访问令牌已过期。获取新的令牌，然后尝试再次发送通知。
                    // 由于你的已缓存访问令牌将在 24 小时后过期，你一天至少可以从 WNS 收到一次此响应。建议你实现最大重试策略。
                    if (!_gettingToken)
                    {
                        _gettingToken = true;
                        GetWnsToken();
                        _gettingToken = false;
                        _ = PostToWns(p_uri, p_data);
                    }
                    else
                    {
                        Log.Warning("Wns令牌已过期，无法获取新令牌！");
                    }
                }
                else if (status == HttpStatusCode.Gone
                    || status == HttpStatusCode.NotFound
                    || status == HttpStatusCode.Forbidden)
                {
                    // 此通道 URI 不在有效。从数据库删除此通道以防止进一步尝试向其发送通知。
                    // 此用户下次启动你的应用时，请求新的 WNS 通道。应用应当检测到它的通道已更改，这应当触发该应用向应用服务器发送新通道 URI。
                    valid = false;
                }
                else if (status == HttpStatusCode.NotAcceptable)
                {
                    // WNS 正在对此通道限流。实现可成倍减少发送的通知量的重试策略，以防止再次限流。
                    // 此外，重新思考导致你的通知被限流的情况。通过将发送的通知限定为增添真正价值的通知，你将提供更丰富的用户体验。
                    Log.Warning("Toast推送被限流！");
                }
                else
                {
                    string[] output = {
                                       status.ToString(),
                                       webException.Response.Headers["X-WNS-Debug-Trace"],
                                       webException.Response.Headers["X-WNS-Error-Description"],
                                       webException.Response.Headers["X-WNS-Msg-ID"],
                                       webException.Response.Headers["X-WNS-Status"]
                                   };
                    Log.Warning("Toast推送异常：" + string.Join(" | ", output));
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Toast推送未知异常");
            }
            return valid;
        }

        // 在仪表板中注册应用以使用 WNS。你的应用服务器必须使用仪表板提供的特定凭据进行身份验证和发送通知。
        // 在每次启动应用时请求一个通道。通道 URL 可能过期，而且每次请求时不保证相同。如果返回的通道 URL 与使用的 URL 不同，则更新你在应用服务器中的引用。
        // 验证通道 URL 是否来自 WNS。请不要尝试将通知推送到非 WNS 服务。确保你的通道 URL 使用域 "notify.windows.com"（Windows 或 Windows Phone）或者 "s.notify.live.net"（仅限 Windows Phone）。
        // 始终保护对应用服务器的通道注册回调。当你的应用接收到其通道 URL 并将其发送到你的应用服务器时，它应该以安全方式进行操作。为用于接收和发送通道 URL 的机制进行身份验证和加密。
        // 同时将通道 URL 和设备 ID 发送到你的应用服务，以便应用服务器跟踪已将 URL 分配到哪些设备。如果某个 URL 发生改变，应用服务器可以替换与该设备 ID 相关联的旧 URL。
        // 重用访问令牌。因为你的访问令牌可用于发送多个通知，所以你的服务器应该缓存该访问令牌，以免在每次要发送通知时重新进行身份验证。如果令牌已过期，则你的应用服务器将收到一个错误，你应该对你的应用服务器进行身份验证并重试该通知。
        // 不要与任何人共享你的程序包安全标识符(PKSID) 和密钥。将这些凭据安全地存储在你的应用服务器上。如果你认为你的密钥已泄露，请生成一个新密钥。定期生成新密钥，让别有用心的人难以下手。

        /// <summary>
        /// 获取Wns访问令牌
        /// </summary>
        void GetWnsToken()
        {
            _wnsToken = null;
            // WNS获取令牌时的凭据，在【仪表盘->搬运工->推送通知->Live 服务网站(链接)】中获取
            var urlEncodedSecret = HttpUtility.UrlEncode(Kit.GetCfg<string>("UwpClientSecret"));
            var urlEncodedSid = HttpUtility.UrlEncode(Kit.GetCfg<string>("UwpPkgSID"));
            string result = null;
            var body = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=notify.windows.com", urlEncodedSid, urlEncodedSecret);
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri("https://login.live.com/accesstoken.srf"),
                        Content = new StringContent(body),
                    };
                    var response = await client.SendAsync(request).ConfigureAwait(false);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "获取WNS访问令牌时异常");
                return;
            }

            // 形如：{"token_type":"bearer","access_token":"xxx=","expires_in":86400}
            string[] strs;
            string[] token;
            if (!string.IsNullOrEmpty(result)
                && (strs = result.Split(',')).Length == 3
                && (token = strs[1].Split(':')).Length == 2)
            {
                _wnsToken = token[1].Trim('\"');
            }
            else
            {
                Log.Warning("WNS令牌格式错误");
            }
        }
    }
    */
}
