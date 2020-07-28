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
    public partial class LvListSelector : Win
    {
        public LvListSelector()
        {
            InitializeComponent();

            _lv.View = new ListItemSelector
            {
                Male = (DataTemplate)Resources["Male"],
                Lady = (DataTemplate)Resources["Lady"],
            };
            _lv.Data = SampleData.CreatePersonsTbl(50);
        }
    }

    public class ListItemSelector : DataTemplateSelector
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