#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Charts
{
    internal interface IAxis
    {
        void AddLabels(double[] vals, object[] lbls);
        void ClearLabels();
        string Format(double val);
        string Format(object val);
        double FromData(double val);
        int GetAnnoNumber();
        double ToData(double val);

        string AnnoFormat { get; }

        Dt.Charts.AxisType AxisType { get; }

        bool IsTime { get; }

        double LogBase { get; }

        double MajorUnit { get; }

        double Max { get; set; }

        double Min { get; set; }

        bool Visible { get; set; }
    }
}

