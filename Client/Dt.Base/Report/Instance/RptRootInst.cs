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
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 报表模板根元素
    /// </summary>
    internal class RptRootInst
    {
        #region 成员变量
        /// <summary>
        /// 页面之间的缝隙，为避免打印时分页边框不正常！
        /// </summary>
        public const double PageGap = 1.0;
        bool _pageEnding;
        #endregion

        #region 构造方法
        public RptRootInst(RptInfo p_info)
        {
            Info = p_info;
            Pages = new List<RptPage>();
            Rows = new List<PageDefine>() { new PageDefine() };
            Cols = new List<PageDefine>() { new PageDefine() };
        }
        #endregion

        #region 事件
        /// <summary>
        /// 开始输出垂直新页事件
        /// </summary>
        public event EventHandler<RptPage> VerPageBegin;

        /// <summary>
        /// 垂直页面输出结束事件
        /// </summary>
        public event EventHandler<PageDefine> VerPageEnd;

        /// <summary>
        /// 开始输出水平新页事件
        /// </summary>
        public event EventHandler<RptPage> HorPageBegin;

        /// <summary>
        /// 水平页面输出结束事件
        /// </summary>
        public event EventHandler<PageDefine> HorPageEnd;
        #endregion

        #region 属性
        /// <summary>
        /// 获取报表描述信息
        /// </summary>
        public RptInfo Info { get; }

        /// <summary>
        /// 获取报表页眉
        /// </summary>
        public RptHeaderInst Header { get; set; }

        /// <summary>
        /// 获取报表内容
        /// </summary>
        public RptBodyInst Body { get; set; }

        /// <summary>
        /// 获取报表页脚
        /// </summary>
        public RptFooterInst Footer { get; set; }

        /// <summary>
        /// 获取页眉高度
        /// </summary>
        public double HeaderHeight { get; set; }

        /// <summary>
        /// 获取页脚高度
        /// </summary>
        public double FooterHeight { get; set; }

        /// <summary>
        /// 获取内容高度
        /// </summary>
        public double BodyHeight { get; set; }

        /// <summary>
        /// 获取内容宽度
        /// </summary>
        public double BodyWidth { get; set; }

        /// <summary>
        /// 获取所有输出页面
        /// </summary>
        public List<RptPage> Pages { get; }

        /// <summary>
        /// 获取按页组织的行定义
        /// </summary>
        public List<PageDefine> Rows { get; }

        /// <summary>
        /// 获取按页组织的列定义
        /// </summary>
        public List<PageDefine> Cols { get; }

        /// <summary>
        /// 获取当前正在构造的容器对象，如表格表头
        /// </summary>
        public RptItemPartInst CurrentParent { get; set; }

        /// <summary>
        /// 获取当前正在构造的表格
        /// </summary>
        public RptTableInst CurrentTable { get; set; }

        /// <summary>
        /// 获取当前表尾的高度
        /// </summary>
        public double TblFooterHeight { get; set; }

        /// <summary>
        /// 获取分页的列数
        /// </summary>
        public int PageCols { get; set; }
        #endregion

        #region 外部方法
        /// <summary>
        /// 绘制报表
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Draw()
        {
            // 构造报表项实例
            RptRoot root = Info.Root;
            await root.Build(this);

            // 页面固定尺寸
            HeaderHeight = root.Header.ActualHeight;
            FooterHeight = root.Footer.ActualHeight;
            BodyHeight = root.PageSetting.ValidHeight - HeaderHeight - FooterHeight - PageGap;
            BodyWidth = root.PageSetting.ValidWidth - PageGap;

            // 输出成页，页眉页脚在创建页时输出
            CreatePage(0, 0);
            if (Body != null)
                Body.Output();

            // 渲染
            Info.Inst = this;
            RptRender render = new RptRender(Info);
            render.Render();
            return true;
        }

        /// <summary>
        /// 输出报表项
        /// </summary>
        /// <param name="p_item">报表项</param>
        public void OutputItem(RptOutputInst p_item)
        {
            PrepareItem(p_item);

            // 报表项所在页位置
            int pageX = GetPageIndex(Cols, p_item.Region.Col);
            int pageY = GetPageIndex(Rows, p_item.Region.Row);
            RptPage page = GetPage(pageX, pageY);
            if (page == null)
                page = CreatePage(pageX, pageY);

            if (p_item.Parent is RptTblRowInst && p_item.Region.Row != p_item.Parent.Region.Row)
                p_item.Region.Row = p_item.Parent.Region.Row;

            page.AddItem(p_item);
        }

        /// <summary>
        /// 自动行高时重置行高，前提：
        /// 1. AutoHeight为true
        /// 2. 只占一行
        /// 3. 当前输出位置在页的末尾
        /// 4. 测量高度大于原高度
        /// </summary>
        /// <param name="p_item"></param>
        /// <param name="p_height"></param>
        public void SyncRowHeight(RptTextInst p_item, double p_height)
        {
            PageDefine rows = p_item.Page.Rows;
            RptRegion region = p_item.Region;

            // 只支持当前输出位置在页的末尾，非末尾情况效率太低！
            if (region.Row != rows.Start + rows.Count - 1)
                return;

            // 无需调整
            int index = rows.Count - 1;
            if (p_height <= rows.Size[index] || _pageEnding)
                return;

            // 上部高度
            double topHeight = 0;
            for (int i = 0; i < index; i++)
            {
                topHeight += rows.Size[i];
            }

            // 当前页能放下
            if (topHeight + p_height <= BodyHeight - TblFooterHeight)
            {
                rows.Size[index] = p_height;
                return;
            }

            // 需要垂直分页

            // 记录同行项
            RptPage prePage = p_item.Page;
            var sameRowItems = (from item in prePage.Items
                                where item.Region.Row == region.Row
                                select item).ToList();
            rows.Size.RemoveAt(index);

            if (VerPageEnd != null)
            {
                _pageEnding = true;
                VerPageEnd(this, rows);
                _pageEnding = false;
            }

            PageDefine next = new PageDefine();
            next.Start = rows.Start + rows.Count;
            Rows.Add(next);

            // 重新计算行位置
            int newIndex = next.Start + next.Count;
            double validHeight = BodyHeight - TblFooterHeight - next.Size.Sum();
            if (p_height > validHeight)
                p_height = validHeight;
            next.Size.Add(p_height);

            // 处理同行项
            RptPage page = CreatePage(prePage.X, prePage.Y + 1);
            foreach (var item in sameRowItems)
            {
                item.Region.Row = newIndex;
                prePage.Items.Remove(item);
                page.AddItem(item);
            }
        }

        /// <summary>
        /// 查询指定位置的页面
        /// </summary>
        /// <param name="p_x"></param>
        /// <param name="p_y"></param>
        /// <returns></returns>
        public RptPage GetPage(int p_x, int p_y)
        {
            return (from page in Pages
                    where page.X == p_x && page.Y == p_y
                    select page).FirstOrDefault();
        }

        /// <summary>
        /// 获取指定区域所属的页面
        /// </summary>
        /// <param name="p_region"></param>
        /// <returns></returns>
        public RptPage GetPage(RptRegion p_region)
        {
            int pageX = GetPageIndex(Cols, p_region.Col);
            int pageY = GetPageIndex(Rows, p_region.Row);
            return GetPage(pageX, pageY);
        }

        /// <summary>
        /// 是否需要垂直分页
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        public bool TestPageBreak(RptItemInst p_item)
        {
            RptRegion region = p_item.Region;
            PageDefine rowPage = Rows[Rows.Count - 1];
            int lastRow = rowPage.Start + rowPage.Count;
            int deltaRow = region.Row + region.RowSpan - lastRow;
            RptItemBase item = p_item.Item;
            double topHeight = 0;
            if (deltaRow > 0)
            {
                topHeight = rowPage.Size.Sum();
            }
            else
            {
                for (int i = 0; i < region.Row - rowPage.Start; i++)
                {
                    topHeight += rowPage.Size[i];
                }
            }
            return topHeight + item.Height > BodyHeight - TblFooterHeight;
        }

        #endregion

        #region 内部方法
        /// <summary>
        /// 补充报表项及左上的行列定义，处理分页
        /// </summary>
        /// <param name="p_item"></param>
        void PrepareItem(RptItemInst p_item)
        {
            RptRegion region = p_item.Region;
            RptItemBase item = p_item.Item;
            PageDefine rowPage = Rows[Rows.Count - 1];
            PageDefine colPage = Cols[Cols.Count - 1];
            int lastRow = rowPage.Start + rowPage.Count;
            int lastCol = colPage.Start + colPage.Count;
            int deltaRow = region.Row + region.RowSpan - lastRow;
            int deltaCol = region.Col + region.ColSpan - lastCol;

            //已经垂直分页
            if (region.Row < rowPage.Start && region.Row + region.RowSpan > rowPage.Start)
            {
                deltaRow = region.RowSpan - rowPage.Count;
                region.Row = rowPage.Start;
            }
            //已经水平分页
            if (region.Col < colPage.Start && region.Col + region.ColSpan > colPage.Start)
            {
                deltaCol = region.ColSpan - colPage.Count;
                region.Col = colPage.Start;
            }

            // 无需补充
            if (deltaRow <= 0 && deltaCol <= 0)
                return;

            // 补充行定义
            if (deltaRow > 0)
            {
                // 页面已有的总高
                double topHeight = rowPage.Size.Sum();
                int blankRow = deltaRow - region.RowSpan;
                double heights = 0;
                if (blankRow < 0)
                {
                    for (int i = 0; i < deltaRow; i++)
                    {
                        heights += item.Part.GetRowHeight(item.Row + item.RowSpan - deltaRow + i);
                    }

                    //需要分页
                    if (!_pageEnding && topHeight + heights > (BodyHeight - TblFooterHeight) && region.Row != rowPage.Start)
                    {
                        VerPageBreak(ref rowPage, ref topHeight);

                        //从元素的起始位置开始分页
                        for (int j = 0; j < region.RowSpan; j++)
                        {
                            double height = item.Part.GetRowHeight(item.Row + j);
                            rowPage.Size.Add(height);
                            topHeight += height;
                        }
                    }
                    else
                    {
                        //不需要分页
                        for (int i = 0; i < deltaRow; i++)
                        {
                            double height = item.Part.GetRowHeight(item.Row + item.RowSpan - deltaRow + i);
                            if (topHeight + height > (BodyHeight - TblFooterHeight))//元素高度大于页面高度，截断
                                height = (BodyHeight - TblFooterHeight - topHeight) < 0 ? 0 : BodyHeight - TblFooterHeight - topHeight;
                            rowPage.Size.Add(height);
                            topHeight += height;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < deltaRow; i++)
                    {
                        double height = item.Part.GetRowHeight(item.Row + item.RowSpan - deltaRow + i);
                        if (i == blankRow)
                        {
                            for (int j = i; j < deltaRow; j++)
                            {
                                heights += item.Part.GetRowHeight(item.Row + item.RowSpan - deltaRow + j);
                            }
                        }

                        // 在报表项上
                        bool inItem = lastRow + i >= region.Row;
                        if (height > BodyHeight - TblFooterHeight)
                            height = BodyHeight - TblFooterHeight;
                        if (i == blankRow && heights > BodyHeight - TblFooterHeight)
                            heights = BodyHeight - TblFooterHeight;

                        // 需要垂直分页
                        bool verBreak = (i == blankRow) ? (topHeight + heights > BodyHeight - TblFooterHeight)
                            : (topHeight + height > BodyHeight - TblFooterHeight);
                        if (!_pageEnding && verBreak)
                        {
                            VerPageBreak(ref rowPage, ref topHeight, inItem);
                        }
                        rowPage.Size.Add(height);
                        topHeight += height;
                    }
                }

                // 重新计算行位置
                region.Row = rowPage.Start + rowPage.Count - region.RowSpan;
            }

            //补充列定义
            if (deltaCol > 0)
            {
                // 页面已有的宽度
                double leftWidth = colPage.Size.Sum();
                int blankCol = deltaCol - region.ColSpan;
                double widths = 0;

                //不需要补充空行
                if (blankCol < 0)
                {
                    for (int i = 0; i < deltaCol; i++)
                    {
                        widths += Info.Root.Cols[item.Col + item.ColSpan - deltaCol + i];
                    }

                    //需要分页
                    if (!_pageEnding && leftWidth + widths > BodyWidth && region.Col != colPage.Start)
                    {
                        HorPageBreak(ref colPage, ref leftWidth);

                        //从元素的起始位置开始分页
                        for (int j = 0; j < region.ColSpan; j++)
                        {
                            double width = Info.Root.Cols[item.Col + j];
                            colPage.Size.Add(width);
                            leftWidth += width;
                        }
                    }
                    else
                    {
                        //不需要分页
                        for (int i = 0; i < deltaCol; i++)
                        {
                            double width = Info.Root.Cols[item.Col + item.ColSpan - deltaCol + i];
                            if (leftWidth + width > BodyWidth)//元素宽度大于页面宽度，截断
                                width = (BodyWidth - leftWidth) < 0 ? 0 : BodyWidth - leftWidth;
                            colPage.Size.Add(width);
                            leftWidth += width;
                        }
                    }
                }
                else
                {
                    //需要空行补充
                    for (int i = 0; i < deltaCol; i++)
                    {
                        double width = Info.Root.Cols[item.Col + item.ColSpan - deltaCol + i];
                        if (i == blankCol)
                        {
                            for (int j = i; j < deltaCol; j++)
                            {
                                widths += Info.Root.Cols[item.Col + item.ColSpan - deltaCol + j];
                            }
                        }

                        // 在报表项上
                        bool inItem = lastCol + i >= region.Col;
                        // 需要水平分页
                        bool horBreak = (i == blankCol) ? (leftWidth + widths > BodyWidth) : (leftWidth + width > BodyWidth);
                        if (!_pageEnding && horBreak)
                        {
                            HorPageBreak(ref colPage, ref leftWidth, inItem);
                        }
                        colPage.Size.Add(width);
                        leftWidth += width;
                    }
                }

                // 重新计算列位置
                region.Col = colPage.Start + colPage.Count - region.ColSpan;
            }
        }

        /// <summary>
        /// 水平分页
        /// </summary>
        /// <param name="p_colPage"></param>
        /// <param name="p_leftWidth"></param>
        /// <param name="p_inItem"></param>
        void HorPageBreak(ref PageDefine p_colPage, ref double p_leftWidth, bool p_inItem = true)
        {
            //上页结束
            if (p_inItem && HorPageEnd != null)
            {
                _pageEnding = true;
                HorPageEnd(this, p_colPage);
                _pageEnding = false;
            }

            PageDefine next = new PageDefine();
            next.Start = p_colPage.Start + p_colPage.Count;
            Cols.Add(next);
            p_leftWidth = 0;
            p_colPage = next;
        }

        /// <summary>
        /// 垂直分页
        /// </summary>
        /// <param name="p_rowPage"></param>
        /// <param name="p_topHeight"></param>
        /// <param name="p_inItem"></param>
        void VerPageBreak(ref PageDefine p_rowPage, ref double p_topHeight, bool p_inItem = true)
        {
            // 上页结束
            if (p_inItem && VerPageEnd != null)
            {
                _pageEnding = true;
                VerPageEnd(this, p_rowPage);
                _pageEnding = false;
            }

            PageDefine next = new PageDefine();
            next.Start = p_rowPage.Start + p_rowPage.Count;
            Rows.Add(next);
            p_topHeight = 0;
            p_rowPage = next;
        }

        /// <summary>
        /// 创建输出页面
        /// </summary>
        /// <param name="p_x"></param>
        /// <param name="p_y"></param>
        /// <returns></returns>
        RptPage CreatePage(int p_x, int p_y)
        {
            if (p_x >= PageCols)
                PageCols = p_x + 1;
            RptPage page = new RptPage(p_x, p_y, this);
            Pages.Add(page);

            if (p_x > 0)
                HorPageBegin?.Invoke(this, page);

            if (p_y > 0)
                VerPageBegin?.Invoke(this, page);

            // 输出新页面时重复页眉页脚
            if (Header != null)
                Header.Clone().Output(page);

            if (Footer != null)
                Footer.Clone().Output(page);
            return page;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_def"></param>
        /// <param name="p_index"></param>
        /// <returns></returns>
        int GetPageIndex(List<PageDefine> p_def, int p_index)
        {
            for (int i = 0; i < p_def.Count; i++)
            {
                PageDefine def = p_def[i];
                if (p_index >= def.Start && p_index < def.Start + def.Count)
                    return i;
            }
            throw new Exception("给定位置不在页面中！");
        }
        #endregion
    }
}
