#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Cells.Data;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml;
using Windows.UI;
using Windows.UI.Xaml.Media;

#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 报表元素基类
    /// </summary>
    internal abstract class RptItemInst
    {
        #region 成员变量
        protected RptItemBase _item;
        protected RptItemInst _parent;
        protected List<RptItemInst> _leftItems;
        protected List<RptItemInst> _topItems;
        protected RptRegion _region;
        protected bool _outputted;
        #endregion

        #region 构造方法
        public RptItemInst(RptItemBase p_item)
        {
            _item = p_item;
            Visible = true;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取报表实例
        /// </summary>
        public RptRootInst Inst
        {
            get { return _item.Root.Inst; }
        }

        /// <summary>
        /// 获取报表项实例对应的模板项
        /// </summary>
        public RptItemBase Item
        {
            get { return _item; }
        }
        
        /// <summary>
        /// 获取设置父项
        /// </summary>
        public RptItemInst Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        /// <summary>
        /// 获取设置在其上部的报表项
        /// </summary>
        public List<RptItemInst> TopItems
        {
            get { return _topItems; }
            set { _topItems = value; }
        }

        /// <summary>
        /// 获取设置在其左侧的报表项
        /// </summary>
        public List<RptItemInst> LeftItems
        {
            get { return _leftItems; }
            set { _leftItems = value; }
        }

        /// <summary>
        /// 获取设置报表项区域
        /// </summary>
        public RptRegion Region
        {
            get { return _region; }
            set { _region = value; }
        }

        /// <summary>
        /// 获取设置报表项是否可见
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// 是否正在输出中
        /// </summary>
        public bool Outputted
        {
            get { return _outputted; }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 输出报表项
        /// </summary>
        public void Output()
        {
            if (!_outputted)
            {
                RefreshPosition();
                DoOutput();
                _outputted = true;
            }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 输出报表项内容
        /// </summary>
        protected virtual void DoOutput()
        {
        }

        /// <summary>
        /// 计算元素的位置信息
        /// </summary>
        void RefreshPosition()
        {
            if (_region != null)
                return;

            _region = new RptRegion();

            // 垂直位置
            if (_topItems != null && _topItems.Count > 0)
            {
                // 上部存在元素
                foreach (var inst in _topItems)
                {
                    if (!inst.Outputted)
                        inst.Output();
                }

                // 最靠近的上部元素
                var lastTop = _topItems[0];
                if (_topItems.Count > 1)
                {
                    for (int i = 1; i < _topItems.Count; i++)
                    {
                        var inst = _topItems[i];
                        if (inst.Region.Row + inst.Region.RowSpan > lastTop.Region.Row + lastTop.Region.RowSpan)
                            lastTop = inst;
                    }
                }

                // 模板设计时与上部元素的行距
                int num = _item.Row - (lastTop.Item.Row + lastTop.Item.RowSpan);
                _region.Row = (lastTop.Region.Row + lastTop.Region.RowSpan) + num;
            }
            else if (_parent != null)
            {
                // 报表项内部元素
                _region.Row = _parent.Region.Row + (_item.Row - _parent.Item.Row);
            }
            else
            {
                // 报表项
                _region.Row = _item.Row;
            }

            // 水平位置
            if (_leftItems != null && _leftItems.Count != 0)
            {
                foreach (var inst in _leftItems)
                {
                    if (!inst.Outputted)
                        inst.Output();
                }

                var lastLeft = _leftItems[0];
                if (_leftItems.Count > 1)
                {
                    for (int i = 1; i < _leftItems.Count; i++)
                    {
                        var inst = _leftItems[i];
                        if ((inst.Region.Col + inst.Region.ColSpan) > (lastLeft.Region.Col + lastLeft.Region.ColSpan))
                            lastLeft = inst;
                    }
                }

                int num3 = _item.Col - (lastLeft.Item.Col + lastLeft.Item.ColSpan);
                _region.Col = (lastLeft.Region.Col + lastLeft.Region.ColSpan) + num3;
            }
            else if (_parent != null)
            {
                _region.Col = _parent.Region.Col + (_item.Col - _parent.Item.Col);
            }
            else
            {
                _region.Col = _item.Col;
            }

            _region.RowSpan = _item.RowSpan;
            _region.ColSpan = _item.ColSpan;
        }
        #endregion
    }
}
