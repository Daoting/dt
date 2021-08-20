#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class LvGroupTemplate : Win
    {
        public LvGroupTemplate()
        {
            InitializeComponent();

            _lv.GroupName = "bumen";
            _lv.GroupContext = typeof(MyGroupContext);
            _lv.Data = SampleData.CreatePersonsTbl(100);
        }
    }

    public class MyGroupContext : GroupContext
    {
        public double Sum => SumDouble("shengao");

        public double Average => AverageDouble("shengao");

        public double Max => MaxDouble("shengao");

        public double Min => MinDouble("shengao");
    }
}