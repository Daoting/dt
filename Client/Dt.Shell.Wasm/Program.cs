using System;

namespace Dt.Shell
{
	public class Program
	{
		static App _app;

		public static void Main(string[] args)
		{
			Windows.UI.Xaml.Application.Start(_ => _app = new App());
		}
	}
}
