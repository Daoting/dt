#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    internal class StrokeDashHelper
    {
        internal static DoubleCollection GetStrokeDashes(StrokeDashType dashType)
        {
            switch (dashType)
            {
                case StrokeDashType.None:
                    return null;

                case StrokeDashType.Hair:
                {
                    DoubleCollection doubles = new DoubleCollection();
                    doubles.Add(1.0);
                    doubles.Add(1.0);
                    return doubles;
                }
                case StrokeDashType.Dot:
                {
                    DoubleCollection doubles2 = new DoubleCollection();
                    doubles2.Add(2.0);
                    doubles2.Add(2.0);
                    return doubles2;
                }
                case StrokeDashType.Dash:
                {
                    DoubleCollection doubles3 = new DoubleCollection();
                    doubles3.Add(4.0);
                    doubles3.Add(2.0);
                    return doubles3;
                }
                case StrokeDashType.DashDot:
                {
                    DoubleCollection doubles4 = new DoubleCollection();
                    doubles4.Add(4.0);
                    doubles4.Add(2.0);
                    doubles4.Add(2.0);
                    doubles4.Add(2.0);
                    return doubles4;
                }
                case StrokeDashType.LongDash:
                {
                    DoubleCollection doubles5 = new DoubleCollection();
                    doubles5.Add(8.0);
                    doubles5.Add(2.0);
                    return doubles5;
                }
                case StrokeDashType.LongDashDot:
                {
                    DoubleCollection doubles6 = new DoubleCollection();
                    doubles6.Add(8.0);
                    doubles6.Add(2.0);
                    doubles6.Add(2.0);
                    doubles6.Add(2.0);
                    return doubles6;
                }
                case StrokeDashType.LongDashDotDot:
                {
                    DoubleCollection doubles7 = new DoubleCollection();
                    doubles7.Add(8.0);
                    doubles7.Add(2.0);
                    doubles7.Add(2.0);
                    doubles7.Add(2.0);
                    doubles7.Add(2.0);
                    doubles7.Add(2.0);
                    return doubles7;
                }
                case StrokeDashType.Thin:
                {
                    DoubleCollection doubles8 = new DoubleCollection();
                    doubles8.Add(1.0);
                    doubles8.Add(0.0);
                    return doubles8;
                }
            }
            return null;
        }
    }
}

