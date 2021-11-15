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
using System.Linq;
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
    /// 报表输出页面，输出时布局用
    /// </summary>
    internal class RptPage
    {
        RptRootInst _root;

        public RptPage(int p_x, int p_y, RptRootInst p_root)
        {
            _root = p_root;
            X = p_x;
            Y = p_y;
            HeaderItems = new List<RptTextInst>();
            Items = new List<RptOutputInst>();
            FooterItems = new List<RptTextInst>();
        }

        /// <summary>
        /// 获取当前页水平分页页码
        /// </summary>
        public int X { get; }

        /// <summary>
        /// 获取当前页垂直分页页码
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// 获取页眉区域的输出项
        /// </summary>
        public List<RptTextInst> HeaderItems { get; }

        /// <summary>
        /// 获取内容区域的输出项
        /// </summary>
        public List<RptOutputInst> Items { get; }

        /// <summary>
        /// 获取页脚区域的输出项
        /// </summary>
        public List<RptTextInst> FooterItems { get; }

        /// <summary>
        /// 获取设置页面行定义
        /// </summary>
        public PageDefine Rows
        {
            get { return _root.Rows[Y]; }
        }

        /// <summary>
        /// 获取设置页面列定义
        /// </summary>
        public PageDefine Cols
        {
            get { return _root.Cols[X]; }
        }

        /// <summary>
        /// 获取当前页面的页号，只有全部输出后渲染时才准确！
        /// </summary>
        public string PageNum { get; set; }

        /// <summary>
        /// 当前行是否有定义
        /// </summary>
        /// <returns></returns>
        public bool IsRowHasDefine()
        {
            return _root.Pages.Any(itm => itm.X != X && itm.Y == Y);
        }

        /// <summary>
        /// 当前列是否有定义
        /// </summary>
        /// <returns></returns>
        public bool IsColHasDefine()
        {
            return _root.Pages.Any(itm => itm.X == X && itm.Y != Y);
        }

        /// <summary>
        /// 添加要输出的报表项
        /// </summary>
        /// <param name="p_item"></param>
        public void AddItem(RptOutputInst p_item)
        {
            Items.Add(p_item);
            p_item.Page = this;
        }

        /// <summary>
        /// 更新当前页面的页号，只有全部输出后渲染时才准确！
        /// </summary>
        public void UpdatePageNum()
        {
            PageNum = (Y * _root.PageCols + X + 1).ToString();
        }
    }
}
