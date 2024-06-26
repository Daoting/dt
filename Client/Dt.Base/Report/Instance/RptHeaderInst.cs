﻿#region 文件描述
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
    /// 页眉实例
    /// </summary>
    public class RptHeaderInst
    {
        readonly RptPart _item;
        readonly List<RptTextInst> _children;

        /// <summary>
        /// 
        /// </summary>
        public RptHeaderInst(RptPart p_item)
        {
            _item = p_item;
            _children = new List<RptTextInst>();
        }

        /// <summary>
        /// 添加子项
        /// </summary>
        /// <param name="p_item"></param>
        public void AddChild(RptTextInst p_item)
        {
            _children.Add(p_item);
        }

        /// <summary>
        /// 输出到指定页面
        /// </summary>
        /// <param name="p_page"></param>
        public void Output(RptPage p_page)
        {
            foreach (RptTextInst item in _children)
            {
                item.OutputHeader(p_page);
            }
        }

        /// <summary>
        /// 克隆整个区域内容
        /// </summary>
        /// <returns></returns>
        public RptHeaderInst Clone()
        {
            RptHeaderInst inst = new RptHeaderInst(_item);
            foreach (RptTextInst item in _children)
            {
                inst.AddChild(item.Clone());
            }
            return inst;
        }
    }
}
