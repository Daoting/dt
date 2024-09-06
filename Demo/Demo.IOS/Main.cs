using Dt.Core;
using System;
using UIKit;

namespace Demo
{
    public class EntryPoint
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