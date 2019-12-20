#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
#endregion

namespace Dt.Charts
{
    internal class Themes
    {
        private static Dictionary<ChartTheme, ResourceDictionary> _dict = new Dictionary<ChartTheme, ResourceDictionary>();

        public static ResourceDictionary GetThemes(ChartTheme theme)
        {
            if ((theme == ChartTheme.None) || (theme == ChartTheme.Custom))
            {
                return null;
            }
            if (!_dict.ContainsKey(theme))
            {
                _dict.Add(theme, LoadThemeFromResources(theme));
            }
            return _dict[theme];
        }

        public static bool IsStandard(ResourceDictionary rd)
        {
            return ((rd != null) && _dict.ContainsValue(rd));
        }

        private static ResourceDictionary LoadThemeFromResources(ChartTheme theme)
        {
            return new ResourceDictionary
            {
                Source = new Uri(string.Format("ms-appx:///Dt.Charts/Themes/{0}.xaml", theme.ToString()))
            };
        }
    }
}

