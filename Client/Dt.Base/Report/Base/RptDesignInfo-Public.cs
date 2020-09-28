#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-25 创建
**************************************************************************/
#endregion

#region 命名空间
using System.Threading.Tasks;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 报表设计的描述信息
    /// </summary>
    public abstract partial class RptDesignInfo
    {
        /// <summary>
        /// 获取设置报表名称，作为唯一标识识别窗口用
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 读取模板内容
        /// </summary>
        /// <returns></returns>
        public abstract Task<string> ReadTemplate();

        /// <summary>
        /// 保存模板内容
        /// </summary>
        /// <param name="p_xml"></param>
        public abstract void SaveTemplate(string p_xml);

    }
}