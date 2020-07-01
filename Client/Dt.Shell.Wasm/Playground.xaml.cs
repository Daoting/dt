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

        async void OnTest(object sender, RoutedEventArgs e)
        {
            throw new Exception("test Exception");
        }

    }
}
