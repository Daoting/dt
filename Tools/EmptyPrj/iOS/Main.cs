﻿namespace Dt.Lib
{
    public class Application
    {
        static void Main(string[] args)
        {
            try
            {
                UIApplication.Main(args, null, typeof(App));
            }
            catch (Exception ex)
            {
                Kit.OnIOSUnhandledException(ex);
            }
        }
    }
}