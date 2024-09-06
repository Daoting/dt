#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-04-19 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
#endregion

namespace Demo.UI
{
    public partial class RptTabDemo : Win
    {
        public RptTabDemo()
        {
            InitializeComponent();
        }

        async void OnLocal(object sender, RoutedEventArgs e)
        {
            // 表中模板不存在时先插入
            var da = new AgentInfo(AccessType.Local, "rptdemo").GetAccessInfo().GetDa();
            var cnt = await da.GetScalar<int>($"select count(*) from OmReport where name='综合'");
            if (cnt == 0)
            {
                string define = null;
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Demo.UI/Files/Content/zh.rpt"));
                using (var stream = await file.OpenStreamForReadAsync())
                using (var reader = new StreamReader(stream))
                {
                    define = reader.ReadToEnd();
                }
                var entity = await OmReportX.New("综合", define);
                await entity.Save(false);
            }

            _rpt.LoadReport(new RptInfo { Uri = "local://rptdemo/综合" });
        }

        void OnContent(object sender, RoutedEventArgs e)
        {
            _rpt.LoadReport(new RptInfo { Uri = "ms-appx:///Demo.UI/Files/Content/zh.rpt" });
        }

        void OnEmbedded(object sender, RoutedEventArgs e)
        {
            _rpt.LoadReport(new RptInfo { Uri = "embedded://Demo.UI/Demo.UI.Files.Embed.模板.综合.rpt" });
        }

        void OnNoMenu(object sender, RoutedEventArgs e)
        {
            _rpt.LoadReport(new RptInfo { Uri = "embedded://Demo.UI/Demo.UI.Files.Embed.模板.无工具栏.rpt" });
        }

        void OnContextMenu(object sender, RoutedEventArgs e)
        {
            _rpt.LoadReport(new RptInfo { Uri = "embedded://Demo.UI/Demo.UI.Files.Embed.模板.右键菜单.rpt" });
        }

        void OnCustomMenu(object sender, RoutedEventArgs e)
        {
            _rpt.LoadReport(new RptInfo { Uri = "embedded://Demo.UI/Demo.UI.Files.Embed.模板.交互脚本.rpt" });
        }
    }
}