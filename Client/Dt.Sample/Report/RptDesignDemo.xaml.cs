#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.Report;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class RptDesignDemo : Win
    {
        public RptDesignDemo()
        {
            InitializeComponent();
            AttachEvent();
        }

        void AttachEvent()
        {
            foreach (var item in _fv.Items)
            {
                if (item is Panel pnl)
                {
                    foreach (Button btn in pnl.Children.OfType<Button>())
                    {
                        btn.Click += OnBtnClick;
                    }
                }
            }
        }

        void OnBtnClick(object sender, RoutedEventArgs e)
        {
            string name = ((Button)sender).Content.ToString();
            AtApp.OpenWin(typeof(RptDesignHome), name, Icons.Excel, new MyRptDesignInfo { Name = name });
        }
    }

    public class MyRptDesignInfo : RptDesignInfo
    {
        public override Task<string> ReadTemplate()
        {
            return Task.Run(() =>
            {
                using (var stream = typeof(RptDesignDemo).Assembly.GetManifestResourceStream($"Dt.Sample.Report.模板.{Name}.xml"))
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            });
        }

        public override void SaveTemplate(string p_xml)
        {
            DataPackage data = new DataPackage();
            data.SetText(p_xml);
            Clipboard.SetContent(data);
            AtKit.Msg("已保存到剪切板！");
        }
    }
}