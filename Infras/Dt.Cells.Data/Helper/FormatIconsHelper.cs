#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
#endregion

namespace Dt.Cells.Data
{
    internal static class FormatIconsHelper
    {
        static string[,] _cachedIconNames;
        static string[,] _cachedImageSources = new string[20, 5];
        static Dictionary<string, Stream> _cachedResource = new Dictionary<string, Stream>();

        static FormatIconsHelper()
        {
            string[,] strArray = new string[20, 5];
            strArray[0, 0] = "ArrowRedDown.png";
            strArray[0, 1] = "ArrowYellowRight.png";
            strArray[0, 2] = "ArrowBlueUp.png";
            strArray[1, 0] = "ArrowGrayDown.png";
            strArray[1, 1] = "ArrowGrayRight.png";
            strArray[1, 2] = "ArrowGrayUp.png";
            strArray[2, 0] = "TriangleRedDown.png";
            strArray[2, 1] = "TriangleYellow.png";
            strArray[2, 2] = "TriangleGreenUp.png";
            strArray[3, 0] = "Star0.png";
            strArray[3, 1] = "Star1.png";
            strArray[3, 2] = "Star2.png";
            strArray[4, 0] = "RedFlag.png";
            strArray[4, 1] = "YellowFlag.png";
            strArray[4, 2] = "GreenFlag.png";
            strArray[5, 0] = "RedCircle.png";
            strArray[5, 1] = "YellowCircle.png";
            strArray[5, 2] = "GreenCircle.png";
            strArray[6, 0] = "RedTraficLight.png";
            strArray[6, 1] = "YellowTraficLight.png";
            strArray[6, 2] = "GreenTraficLight.png";
            strArray[7, 0] = "RedDiamond.png";
            strArray[7, 1] = "YellowTrangle.png";
            strArray[7, 2] = "GreenCircle.png";
            strArray[8, 0] = "RedCrossSymbol.png";
            strArray[8, 1] = "YellowExclamationSymbol.png";
            strArray[8, 2] = "GreenCheckSymbol.png";
            strArray[9, 0] = "RedCross.png";
            strArray[9, 1] = "YellowExclamation.png";
            strArray[9, 2] = "GreenCheck.png";
            strArray[10, 0] = "ArrowRedDown.png";
            strArray[10, 1] = "Arrow45YellowDown.png";
            strArray[10, 2] = "Arrow45YellowUp.png";
            strArray[10, 3] = "ArrowBlueUp.png";
            strArray[11, 0] = "ArrowGrayDown.png";
            strArray[11, 1] = "Arrow45GrayDown.png";
            strArray[11, 2] = "Arrow45GrayUp.png";
            strArray[11, 3] = "ArrowGrayUp.png";
            strArray[12, 0] = "BlackFillCircles.png";
            strArray[12, 1] = "GrayFillCicle.png";
            strArray[12, 2] = "PinkFillCircle.png";
            strArray[12, 3] = "RedFillCircle.png";
            strArray[13, 0] = "Rating1.png";
            strArray[13, 1] = "Rating2.png";
            strArray[13, 2] = "Rating3.png";
            strArray[13, 3] = "Rating4.png";
            strArray[14, 0] = "BlackCircle.png";
            strArray[14, 1] = "RedCircle.png";
            strArray[14, 2] = "YellowCircle.png";
            strArray[14, 3] = "GreenCircle.png";
            strArray[15, 0] = "ArrowRedDown.png";
            strArray[15, 1] = "Arrow45YellowDown.png";
            strArray[15, 2] = "ArrowYellowRight.png";
            strArray[15, 3] = "Arrow45YellowUp.png";
            strArray[15, 4] = "ArrowBlueUp.png";
            strArray[0x10, 0] = "ArrowGrayDown.png";
            strArray[0x10, 1] = "Arrow45GrayDown.png";
            strArray[0x10, 2] = "ArrowGrayRight.png";
            strArray[0x10, 3] = "Arrow45GrayUp.png";
            strArray[0x10, 4] = "ArrowGrayUp.png";
            strArray[0x11, 0] = "Rating0.png";
            strArray[0x11, 1] = "Rating1.png";
            strArray[0x11, 2] = "Rating2.png";
            strArray[0x11, 3] = "Rating3.png";
            strArray[0x11, 4] = "Rating4.png";
            strArray[0x12, 0] = "Quarter0.png";
            strArray[0x12, 1] = "Quarter1.png";
            strArray[0x12, 2] = "Quarter2.png";
            strArray[0x12, 3] = "Quarter3.png";
            strArray[0x12, 4] = "Quarter4.png";
            strArray[0x13, 0] = "Box0.png";
            strArray[0x13, 1] = "Box1.png";
            strArray[0x13, 2] = "Box2.png";
            strArray[0x13, 3] = "Box3.png";
            strArray[0x13, 4] = "Box4.png";
            _cachedIconNames = strArray;
        }

        internal static Stream GetResource(IconSetType iconType, int iconIndex)
        {
            int num = (int) iconType;
            string str = _cachedImageSources[num, iconIndex];
            if (str == null)
            {
                str = _cachedIconNames[num, iconIndex];
                _cachedImageSources[num, iconIndex] = str;
            }
            if (_cachedResource.ContainsKey(str))
            {
                return _cachedResource[str];
            }
            Assembly assembly = IntrospectionExtensions.GetTypeInfo((Type) typeof(FormatIconsHelper)).Assembly;
            string[] manifestResourceNames = assembly.GetManifestResourceNames();
            Stream manifestResourceStream = null;
            string str2 = null;
            foreach (string str3 in manifestResourceNames)
            {
                if (!string.IsNullOrEmpty(str3) && str3.Contains(str))
                {
                    str2 = str3;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(str2))
            {
                manifestResourceStream = assembly.GetManifestResourceStream(str2);
            }
            _cachedResource.Add(str, manifestResourceStream);
            return manifestResourceStream;
        }
    }
}

