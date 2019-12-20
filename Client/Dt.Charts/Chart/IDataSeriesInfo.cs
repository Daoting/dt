#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.ComponentModel;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Charts
{
    [EditorBrowsable((EditorBrowsableState) EditorBrowsableState.Never)]
    public interface IDataSeriesInfo
    {
        string[] GetDataNames();
        ValueCoordinate[] GetValueCoordinates();
        double[,] GetValues();
        void SetResolvedValues(int index, object[] values);

        object Connection { get; set; }

        string Label { get; set; }

        Binding[] MemberPaths { get; }

        object Symbol { get; set; }
    }
}

