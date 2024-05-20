#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-03-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Report;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 临时空报表模板信息
    /// </summary>
    public class TempRptDesignInfo : RptDesignInfo
    {
        public TempRptDesignInfo()
        {
            Name = "临时模板 - " + Kit.NewGuid.Substring(0, 6);
            Root = new RptRoot();
            ShowNewFile = true;
            ShowOpenFile = true;
            ShowSave = true;
            AttachRootEvent();
        }

        public override Task<string> ReadTemplate()
        {
            return Task.FromResult("");
        }
    }
}