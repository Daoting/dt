#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Xls
{
    internal static class ExcelSystemColor
    {
        private static Dictionary<string, GcColor> _systemColors = new Dictionary<string, GcColor>();

        private static void InitSystemColorTable()
        {
            _systemColors.Add("3DDKSHADOW", GcSystemColors.GetSystemColor(GcSystemColorIndex.ThreeDDarkShadow));
            _systemColors.Add("3DLIGHT", GcSystemColors.GetSystemColor(GcSystemColorIndex.ThreeDLight));
            _systemColors.Add("ACTIVEBORDER", GcSystemColors.GetSystemColor(GcSystemColorIndex.ActiveBorder));
            _systemColors.Add("ACTIVECAPTION", GcSystemColors.GetSystemColor(GcSystemColorIndex.ActiveCaption));
            _systemColors.Add("APPWORKSPACE", GcSystemColors.GetSystemColor(GcSystemColorIndex.AppWorkspace));
            _systemColors.Add("BACKGROUND", GcSystemColors.GetSystemColor(GcSystemColorIndex.Desktop));
            _systemColors.Add("BTNFACE", GcSystemColors.GetSystemColor(GcSystemColorIndex.ButtonFace));
            _systemColors.Add("BTNHIGHLIGHT", GcSystemColors.GetSystemColor(GcSystemColorIndex.ButtonHightlight));
            _systemColors.Add("BTNSHADOW", GcSystemColors.GetSystemColor(GcSystemColorIndex.ButtonShadow));
            _systemColors.Add("BTNTEXT", GcSystemColors.GetSystemColor(GcSystemColorIndex.ButtonText));
            _systemColors.Add("CAPTIONTEXT", GcSystemColors.GetSystemColor(GcSystemColorIndex.ActiveCaptionText));
            _systemColors.Add("GRAYTEXT", GcSystemColors.GetSystemColor(GcSystemColorIndex.GrayText));
            _systemColors.Add("HIGHLIGHT", GcSystemColors.GetSystemColor(GcSystemColorIndex.Highlight));
            _systemColors.Add("HIGHLIGHTTEXT", GcSystemColors.GetSystemColor(GcSystemColorIndex.HighlightText));
            _systemColors.Add("INACTIVEBORDER", GcSystemColors.GetSystemColor(GcSystemColorIndex.InactiveBorder));
            _systemColors.Add("INACTIVECAPTION", GcSystemColors.GetSystemColor(GcSystemColorIndex.InactiveCaption));
            _systemColors.Add("INACTIVECAPTIONTEXT", GcSystemColors.GetSystemColor(GcSystemColorIndex.InactiveCaptionText));
            _systemColors.Add("INFOBK", GcSystemColors.GetSystemColor(GcSystemColorIndex.Info));
            _systemColors.Add("INFOTEXT", GcSystemColors.GetSystemColor(GcSystemColorIndex.InfoText));
            _systemColors.Add("MENU", GcSystemColors.GetSystemColor(GcSystemColorIndex.Menu));
            _systemColors.Add("GRADIENTACTIVECAPTION", GcSystemColors.GetSystemColor(GcSystemColorIndex.GradientActiveCaption));
            _systemColors.Add("HOTLIGHT", GcSystemColors.GetSystemColor(GcSystemColorIndex.HotTrack));
            _systemColors.Add("GRADIENTINACTIVECAPTION", GcSystemColors.GetSystemColor(GcSystemColorIndex.GradientInactiveCaption));
            _systemColors.Add("MENUBAR", GcSystemColors.GetSystemColor(GcSystemColorIndex.MenuBar));
            _systemColors.Add("MENUHIGHLIGHT", GcSystemColors.GetSystemColor(GcSystemColorIndex.MenuHighlight));
            _systemColors.Add("MENUTEXT", GcSystemColors.GetSystemColor(GcSystemColorIndex.MenuText));
            _systemColors.Add("SCROLLBAR", GcSystemColors.GetSystemColor(GcSystemColorIndex.ScrollBar));
            _systemColors.Add("WINDOW", GcSystemColors.GetSystemColor(GcSystemColorIndex.Window));
            _systemColors.Add("WINDOWFRAME", GcSystemColors.GetSystemColor(GcSystemColorIndex.WindowFrame));
            _systemColors.Add("WINDOWTEXT", GcSystemColors.GetSystemColor(GcSystemColorIndex.WindowText));
        }

        public static bool TryGetSystemColor(string name, out GcColor color)
        {
            if (_systemColors.Count == 0)
            {
                InitSystemColorTable();
            }
            return _systemColors.TryGetValue(name.ToUpperInvariant(), out color);
        }
    }
}

