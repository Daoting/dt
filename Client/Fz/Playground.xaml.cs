using Dt.Core;
using Dt.Core.Rpc;
using Dt.Core.Sqlite;
using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Dt.Fz
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
            //var dt = DateTime.Now;
            //var cfg = await new UnaryRpc("cm", "Entry.GetConfig").Call<Dict>();
            //Console.WriteLine(cfg.Str("ver"));

#if WASM
            try
            {
                var prv = new SQLite3P2();
                SQLitePCL.raw.SetProvider(prv);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //
            //try
            //{
            //    var _stateDb = new SqliteConnectionEx("Data Source=local.db");
            //    _stateDb.Open();


            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
#endif
        }

        ResponseReader _reader;
        async void OnServerStream(object sender, RoutedEventArgs e)
        {
            _reader = await new ServerStreamRpc(
                "cm",
                "TestRpc.OnServerStream",
                "hello"
            ).Call();
            while (await _reader.MoveNext())
            {
                Console.WriteLine($"收到：{_reader.Val<string>()}");
            }
            Console.WriteLine("结束");
        }

        void OnStopStream(object sender, RoutedEventArgs e)
        {
            if (_reader != null)
                _reader.Close();
        }


        async void OnClientStream(object sender, RoutedEventArgs e)
        {
            var writer = await new ClientStreamRpc(
                "cm",
                "TestRpc.OnClientStream",
                "hello"
            ).Call();

            int i = 0;
            while (true)
            {
                var msg = $"hello {i++}";
                if (!await writer.Write(msg) || i > 50)
                    break;
                Console.WriteLine(msg);
                await Task.Delay(1000);
            }
        }
    }
}
