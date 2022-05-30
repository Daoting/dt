#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI.Notifications;
#if !WASM
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;
#endif
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 跨平台工具集：选择图片、视频、音频文件，拍照、录像、录音
    /// </summary>
    public partial class Kit
    {
        #region 打开文件
        /// <summary>
        /// 默认关联程序打开文件，wasm未实现
        /// </summary>
        /// <param name="p_filePath">文件完整路径</param>
        public static Task OpenFile(string p_filePath)
        {
#if WASM
            return Task.CompletedTask;
#else
            // 默认关联程序打开
            return Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(p_filePath)
            });
#endif
        }
        #endregion

        #region 分享
        /// <summary>
        /// 分享文字内容
        /// </summary>
        /// <param name="p_content"></param>
        /// <param name="p_title"></param>
        /// <param name="p_uri"></param>
        /// <returns></returns>
        public static Task ShareText(string p_content, string p_title = null, string p_uri = null)
        {
#if WASM
            // https://platform.uno/docs/articles/features/windows-applicationmodel-datatransfer.html?q=ShareUI
            
            var dtm = DataTransferManager.GetForCurrentView();
            TypedEventHandler<DataTransferManager, DataRequestedEventArgs> handler = null;
            handler = delegate (DataTransferManager sender, DataRequestedEventArgs args)
            {
                args.Request.Data.Properties.Title = string.IsNullOrEmpty(p_title) ? "分享内容" : p_title;
                //args.Request.Data.Properties.Description = "分享内容";

                args.Request.Data.SetText(p_content);
                if (!string.IsNullOrEmpty(p_uri))
                    args.Request.Data.SetWebLink(new Uri(p_uri));

                dtm.DataRequested -= handler;
            };
            dtm.DataRequested += handler;

            DataTransferManager.ShowShareUI();
            return Task.CompletedTask;
#else
            var request = new ShareTextRequest
            {
                Text = p_content,
                Subject = string.IsNullOrEmpty(p_title) ? "分享内容" : p_title
            };
            if (!string.IsNullOrEmpty(p_uri))
                request.Uri = p_uri;
            return Share.RequestAsync(request);
#endif
        }

        /// <summary>
        /// 分享文件，wasm未实现
        /// </summary>
        /// <param name="p_filePath"></param>
        /// <param name="p_title"></param>
        /// <returns></returns>
        public static Task ShareFile(string p_filePath, string p_title = null)
        {
#if WASM
            return Task.CompletedTask;
#else
            return Share.RequestAsync(new ShareFileRequest
            {
                File = new ShareFile(p_filePath),
                Title = string.IsNullOrEmpty(p_title) ? "分享文件" : p_title
            });
#endif
        }
        #endregion

        #region 系统通知
        /// <summary>
        /// 显示系统通知，iOS只有app在后台或关闭时才显示！其他平台始终显示
        /// </summary>
        /// <param name="p_title">标题</param>
        /// <param name="p_content">内容</param>
        /// <param name="p_startInfo">点击通知的启动参数</param>
        public static void Toast(string p_title, string p_content, AutoStartInfo p_startInfo = null)
        {
#if !WASM
            BgJob.Toast(p_title, p_content, p_startInfo);
#endif
        }

        /// <summary>
        /// 更新磁贴内容，最多支持四行信息
        /// </summary>
        /// <param name="p_msgs"></param>
        public static void Tile(params string[] p_msgs)
        {
#if WIN
            // 最多支持四行信息！
            int cnt = p_msgs.Length > 4 ? 4 : p_msgs.Length;
            if (cnt == 0)
                return;

            Windows.Data.Xml.Dom.XmlDocument xml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text03);
            Windows.Data.Xml.Dom.XmlNodeList nodes = xml.GetElementsByTagName("text");
            for (uint i = 0; i < cnt; i++)
            {
                nodes.Item(i).InnerText = p_msgs[i];
            }
            TileUpdateManager.CreateTileUpdaterForApplication().Update(new TileNotification(xml));
#elif IOS
            throw new NotImplementedException();
#elif ANDROID
            throw new NotImplementedException();
#elif WASM
            throw new NotImplementedException();
#endif
        }

        /// <summary>
        /// 更新磁贴数字
        /// </summary>
        /// <param name="p_num"></param>
        public static void Tile(double p_num)
        {
#if WIN
            Windows.Data.Xml.Dom.XmlDocument xml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Block);
            Windows.Data.Xml.Dom.XmlNodeList nodes = xml.GetElementsByTagName("text");
            nodes.Item(0).InnerText = p_num.ToString();
            TileUpdateManager.CreateTileUpdaterForApplication().Update(new TileNotification(xml));
#elif IOS
            throw new NotImplementedException();
#elif ANDROID
            throw new NotImplementedException();
#elif WASM
            throw new NotImplementedException();
#endif
        }
#endregion
    }
}