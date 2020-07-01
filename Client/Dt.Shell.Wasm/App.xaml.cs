using Dt.Base;
using Dt.Core;
using Dt.Fz;
using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Dt.Shell
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
	{
		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			this.InitializeComponent();
			AtSys.Startup(new Stub());
		}

		protected override void OnLaunched(LaunchActivatedEventArgs e)
		{
			Console.WriteLine("launched!");
			ApplicationView.GetForCurrentView().Title = AtSys.Stub.Title;
			AtApp.Run(e);
		}
	}
}
