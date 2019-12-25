#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
#endregion

namespace Dt.Charts
{
    [ContentProperty(Name="SeriesBindings")]
    public class ChartBindings
    {
        List<Binding> _seriesBindings = new List<Binding>();

        public Binding ItemNameBinding { get; set; }

        public List<Binding> SeriesBindings
        {
            get { return  _seriesBindings; }
        }

        public Binding XBinding { get; set; }
    }
}

