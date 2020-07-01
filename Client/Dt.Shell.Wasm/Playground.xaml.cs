using Dt.App;
using Dt.Core;
using Dt.Core.Rpc;
using Dt.Core.Sqlite;
using System;
using System.Buffers.Binary;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Dt.Shell
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame. 
    /// </summary>
    public sealed partial class Playground : Page
	{
		public Playground()
		{

			this.InitializeComponent();

		}

        void OnTest(object sender, RoutedEventArgs e)
        {
            
            //try
            //{
            //    //var dt = await AtCm.GetConfig();

            //    var _client = new HttpClient();

            //    byte[] data = RpcKit.GetCallBytes("Entry.GetConfig", null);
            //    byte[] end = new byte[data.Length + 5];
            //    data.CopyTo(end, 5);
            //    var l = BitConverter.GetBytes(data.Length);
            //    l.CopyTo(end, 1);
            //    using (var request = CreateRequestMessage())
            //    {
            //        request.Content = new ByteArrayContent(end);
            //        HttpResponseMessage response;
            //        try
            //        {
            //            response = await _client.SendAsync(request);
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine(ex.Message);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
        }

        private void OnTest1(object sender, RoutedEventArgs e)
        {
            try
            {
                
                var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                Console.WriteLine(path);
                Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                var _stateDb = new SqliteConnectionEx("Data Source=" + Path.Combine(path, "test.db"));
                _stateDb.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        HttpRequestMessage CreateRequestMessage()
        {
            // 使用http2协议Post方法
            return new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Version = new Version(2, 0),
                RequestUri = new Uri($"http://localhost/baisui/cm/.c"),
            };
        }
    }

    class PushStreamContent : HttpContent
    {
        private readonly Func<Stream, Task> _onStreamAvailable;

        public PushStreamContent(Func<Stream, Task> onStreamAvailable)
        {
            _onStreamAvailable = onStreamAvailable;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            // 此处返回的Task未结束前一直可以写入流，实现客户端推送流功能！
            return _onStreamAvailable(stream);
        }

        protected override bool TryComputeLength(out long length)
        {
            // 设置内容长度未知
            length = -1;
            return false;
        }
    }
}
