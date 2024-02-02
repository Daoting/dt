
namespace Dt.Demo
{
	public class Program
	{
		static App _app;

		public static void Main(string[] args)
		{
            // 默认privider为 SQLite3Provider_e_sqlite3
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
            Microsoft.UI.Xaml.Application.Start(_ => _app = new App());
		}
	}
}
