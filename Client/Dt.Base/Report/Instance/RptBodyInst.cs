#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 内容实例
    /// </summary>
    internal class RptBodyInst
    {
        #region 成员变量
        readonly RptPart _item;
        readonly List<RptItemInst> _children;
        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        public RptBodyInst(RptPart p_item)
        {
            _item = p_item;
            _children = new List<RptItemInst>();
        }
        #endregion

        /// <summary>
        /// 添加子项
        /// </summary>
        /// <param name="p_item"></param>
        public void AddChild(RptItemInst p_item)
        {
            _children.Add(p_item);
        }

        /// <summary>
        /// 输出所有报表项
        /// </summary>
        public void Output()
        {
            SortItems();
            foreach (RptItemInst item in _children)
            {
                item.Output();
            }
        }

        #region 内部方法
        /// <summary>
        /// 按照位置整理出每个报表项的左上位置列表
        /// 两报表项之间位置关系有三种：
        /// 1. 无影响
        /// 2. 垂直依赖
        /// 3. 水平依赖
        /// </summary>
        void SortItems()
        {
            RptItemBase item;
            RptItemInst itemInst;
            List<RptItemInst> topList = new List<RptItemInst>();
            List<RptItemInst> leftList = new List<RptItemInst>();

            for (int i = 0; i < _children.Count; i++)
            {
                item = _children[i].Item;
                itemInst = _children[i];
                topList.Clear();
                leftList.Clear();

                // 查找所有上侧左侧的报表项
                for (int j = 0; j < _children.Count; j++)
                {
                    RptItemBase tgt = _children[j].Item;
                    RptItemInst tgtInst = _children[j];
                    Type tp = tgtInst.GetType();
                    if ((tgt.Row + tgt.RowSpan - 1) < item.Row && item.IsCrossWithColumns(tgt.Col, tgt.ColSpan))
                        topList.Add(tgtInst);
                    else if ((tgt.Col + tgt.ColSpan - 1) < item.Col && item.IsCrossWithRows(tgt.Row, tgt.RowSpan))
                        leftList.Add(tgtInst);
                }

                if (topList.Count > 0)
                    itemInst.TopItems = GetTopLeftItems(topList, true);

                if (leftList.Count > 0)
                    itemInst.LeftItems = GetTopLeftItems(leftList, false);
            }
        }

        /// <summary>
        /// 去除重复记录
        /// </summary>
        /// <param name="p_list"></param>
        /// <param name="p_isTop"></param>
        /// <returns></returns>
        List<RptItemInst> GetTopLeftItems(List<RptItemInst> p_list, bool p_isTop)
        {
            // 查找重复记录
            List<RptItemInst> tempList = new List<RptItemInst>();
            foreach (RptItemInst inst in p_list)
            {
                List<RptItemInst> topLefts = p_isTop ? inst.TopItems : inst.LeftItems;
                if (topLefts == null || topLefts.Count == 0)
                    continue;

                foreach (RptItemInst inst2 in topLefts)
                {
                    if (p_list.Contains(inst2) && !tempList.Contains(inst2))
                    {
                        tempList.Add(inst2);
                    }
                }
            }

            List<RptItemInst> result = new List<RptItemInst>();
            foreach (RptItemInst inst in p_list)
            {
                // 去除重复记录
                if (!tempList.Contains(inst))
                    result.Add(inst);
            }
            return result;
        }
        #endregion
    }
}
