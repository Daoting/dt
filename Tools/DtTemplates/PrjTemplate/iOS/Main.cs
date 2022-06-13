namespace $ext_safeprojectname$
{
    public class EntryPoint
    {
        static void Main(string[] args)
        {
            try
            {
                UIKit.UIApplication.Main(args, null, typeof(App));
            }
            catch (Exception ex)
            {
                Kit.OnIOSUnhandledException(ex);
            }
        }
    }
}