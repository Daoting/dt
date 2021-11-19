#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using System.Collections.Generic;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 报表项容器实例
    /// </summary>
    internal class RptItemPartInst : RptItemInst
    {
        protected readonly List<RptTextInst> _children;

        public RptItemPartInst(RptItemBase p_item)
            : base(p_item)
        {
            _children = new List<RptTextInst>();
        }

        /// <summary>
        /// 获取设置当前过滤串字典
        /// </summary>
        public Dictionary<string, string> Filter { get; set; }

        /// <summary>
        /// 添加子项
        /// </summary>
        /// <param name="p_item"></param>
        public void AddChild(RptTextInst p_item)
        {
            _children.Add(p_item);
            p_item.Parent = this;
            p_item.Filter = Filter;
        }

        /// <summary>
        /// 输出报表项内容
        /// </summary>
        protected override void DoOutput()
        {
            foreach (RptTextInst item in _children)
            {
                item.Output();
            }
        }
    }
}
