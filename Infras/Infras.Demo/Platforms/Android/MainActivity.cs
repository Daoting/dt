#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2017-12-06 ����
******************************************************************************/
#endregion

#region ��������
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using System;
#endregion

namespace Infras.Demo
{
    [Activity(
        MainLauncher = true,
        // ������ģʽ�ı䡢��Ļ��С�仯�����иı䶼����������activity
        ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
        // ���������
        WindowSoftInputMode = SoftInput.AdjustNothing | SoftInput.StateHidden
    )]
    public class MainActivity : Microsoft.UI.Xaml.ApplicationActivity
    {
    }

    [Application(
       Label = "@string/ApplicationName",
       Icon = "@mipmap/icon",
       LargeHeap = true,
       HardwareAccelerated = true,
       Theme = "@style/AppTheme"
   )]
    public class Application : Microsoft.UI.Xaml.NativeApplication
    {
        public Application(IntPtr javaReference, JniHandleOwnership transfer)
            : base(() => new Infras.Demo.App(), javaReference, transfer)
        {
        }
    }
}

