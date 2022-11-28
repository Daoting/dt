#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Windows.ApplicationModel.DataTransfer;
#endregion

namespace Dt.Sample
{
    public class MyRptDesignInfo : RptDesignInfo
    {
        public override Task<string> ReadTemplate()
        {
            return Task.Run(() =>
            {
                using (var stream = typeof(RptDemo).Assembly.GetManifestResourceStream($"Dt.Sample.Report.模板.{Name}.xml"))
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
            Kit.Msg("已保存到剪切板！");
        }
    }

    public class MyRptInfo : RptInfo
    {
        public override Task<string> ReadTemplate()
        {
            return Task.Run(() =>
            {
                using (var stream = typeof(RptDemo).Assembly.GetManifestResourceStream($"Dt.Sample.Report.模板.{Name}.xml"))
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            });
        }
    }
}