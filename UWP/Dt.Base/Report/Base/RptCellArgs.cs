#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base.Report;
using Dt.Core;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 单元格脚本参数
    /// </summary>
    public class RptCellArgs
    {
        readonly RptTextInst _inst;

        internal RptCellArgs(RptTextInst p_inst)
        {
            _inst = p_inst;
        }

        /// <summary>
        /// 单元格在报表模板的行索引
        /// </summary>
        public int Row
        {
            get { return _inst.Item.Row; }
        }

        /// <summary>
        /// 单元格在报表模板的列索引
        /// </summary>
        public int Col
        {
            get { return _inst.Item.Col; }
        }

        /// <summary>
        /// 获取对应的数据行
        /// </summary>
        public Row Data
        {
            get { return _inst.Data; }
        }

        /// <summary>
        /// 获取单元格内容字符串
        /// </summary>
        public string Text
        {
            get { return _inst.Text; }
        }
    }
}
