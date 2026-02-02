#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.UI
{
    public partial class TvViewSelector : Win
    {
        public TvViewSelector()
        {
            InitializeComponent();

            _tv.View = new TvItemSelector
            {
                Folder = (DataTemplate)Resources["Folder"],
                File = (DataTemplate)Resources["File"],
            };
            _tv.Data = TvData.GetTbl();
        }
    }

#if WIN
    [WinRT.GeneratedBindableCustomProperty]
#else
    [Microsoft.UI.Xaml.Data.Bindable]
#endif
    public partial class TvItemSelector : DataTemplateSelector
    {
        public DataTemplate Folder { get; set; }
        public DataTemplate File { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (((TvItem)item).Children.Count > 0)
                return Folder;
            return File;
        }
    }
}