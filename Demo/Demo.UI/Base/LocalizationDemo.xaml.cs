#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Microsoft.UI.Xaml;
#endregion

namespace Demo.UI
{
    public partial class LocalizationDemo : Win
    {
        public LocalizationDemo()
        {
            InitializeComponent();
        }

        void GetLocalStr(object sender, RoutedEventArgs e)
        {
            //string str = "Name".Loc();
            //str += "\r\n" + "LocBar.Title".Loc();
            //str += "\r\n" + "LocFormat".LocFormat(23, 99);
            //Kit.Msg(str);
        }
    }
    
    //static class Localizer
    //{
    //    static readonly ResourceStringLocalizer _loc = new ResourceStringLocalizer(typeof(Localizer).Assembly.GetName().Name);

    //    public static LocalizedString Loc(this string p_key) => _loc[p_key];

    //    public static LocalizedString LocFormat(this string p_name, params object[] p_args) => _loc[p_name, p_args];
    //}
}