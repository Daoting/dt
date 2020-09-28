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
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class RptPreviewDemo : Win
    {
        public RptPreviewDemo()
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
                        if (btn.Content.ToString() != "报表组")
                            btn.Click += OnBtnClick;
                    }
                }
            }
        }

        void OnBtnClick(object sender, RoutedEventArgs e)
        {
            string name = ((Button)sender).Content.ToString();
            AtRpt.Show(new MyRptInfo { Name = name });
        }

        void OnRptGroup(object sender, RoutedEventArgs e)
        {
            //Table tbl = new Table();
            //using (var stream = typeof(RptDesignDemo).Assembly.GetManifestResourceStream($"Dt.Sample.Report.数据源.{_tb.Text}.xml"))
            //{
            //    using (XmlReader reader = XmlReader.Create(stream, AtKit.ReaderSettings))
            //    {
            //        tbl.ReadRpcXml(reader);
            //    }
            //}


            //using (var stream = new MemoryStream())
            //{
            //    using (var writer = new Utf8JsonWriter(stream, JsonOptions.IndentedWriter))
            //    {
            //        JsonRpcSerializer.Serialize(tbl, writer);
            //    }
            //    var str = Encoding.UTF8.GetString(stream.ToArray());
            //    DataPackage data = new DataPackage();
            //    data.SetText(str);
            //    Clipboard.SetContent(data);
            //    AtKit.Msg("已保存到剪切板！");
            //}
        }
    }

    public class MyRptInfo : RptInfo
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

    }

    public class MyRptScript : RptScript
    {
        public override Task<Table> GetData(string p_name)
        {
            return Task.Run(() =>
            {
                using (var stream = typeof(RptDesignDemo).Assembly.GetManifestResourceStream($"Dt.Sample.Report.数据源.{p_name}.json"))
                {
                    return Table.Create(stream);
                }
            });
        }

        public override void InitMenu(Menu p_menu)
        {
            p_menu.Items.Add(new Mi { ID = "显示网格", IsCheckable = true, Cmd = View.CmdGridLine });
            p_menu.Items.Add(new Mi { ID = "显示列头", IsCheckable = true, Cmd = View.CmdColHeader });
            p_menu.Items.Add(new Mi { ID = "显示行头", IsCheckable = true, Cmd = View.CmdRowHeader });
        }
    }
}