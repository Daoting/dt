
namespace Dt.Sample
{
	public class Program
	{
		static App _app;

		public static void Main(string[] args)
		{
			Microsoft.UI.Xaml.Application.Start(_ => _app = new App());
		}
	}
}
