#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
#endregion

namespace Dt.Cells.UndoRedo
{
    internal class GenerateNameHelper
    {
        const string DefaultPrefixChartName = "Chart";
        const string DefaultPrefixFloatingObjectName = "FloatingObject";
        const string DefaultPrefixPictureName = "Pictrue";
        const string DefaultPrefixSurfaceChartName = "SurfaceChart";

        public static string GenerateChartName(Worksheet worksheet)
        {
            int count = worksheet.Charts.Count;
            while (worksheet.FindChart("Chart" + ((int) count)) != null)
            {
                count++;
            }
            return ("Chart" + ((int) count));
        }

        public static string GenerateFloatingObjectName(Worksheet worksheet)
        {
            int count = worksheet.FloatingObjects.Count;
            while (worksheet.FindFloatingObject("FloatingObject" + ((int) count)) != null)
            {
                count++;
            }
            return ("FloatingObject" + ((int) count));
        }

        public static string GeneratePictureName(Worksheet worksheet)
        {
            int count = worksheet.Pictures.Count;
            while (worksheet.FindPicture("Pictrue" + ((int) count)) != null)
            {
                count++;
            }
            return ("Pictrue" + ((int) count));
        }
    }
}

