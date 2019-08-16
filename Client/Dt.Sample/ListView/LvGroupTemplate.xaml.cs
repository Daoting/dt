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

            _lv.View = new TraceItemSelector
            {
#if UWP
                Male = (DataTemplate)Resources["Male"],
                Lady = (DataTemplate)Resources["Lady"],
#else
                Male = StaticResources.Male,
                Lady = StaticResources.Lady,
#endif
            };
            _lv.Data = SampleData.CreatePersonsTbl(50);
        }
    }

    public class TraceItemSelector : DataTemplateSelector
    {
        public DataTemplate Male { get; set; }
        public DataTemplate Lady { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (((LvItem)item).Row.Str("xb") == "男")
                return Male;
            return Lady;
        }
    }
}