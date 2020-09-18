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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml;
using Windows.UI;
using Windows.UI.Xaml.Media;

#endregion

namespace Dt.Base.Report
{
    internal abstract class RptOutputInst : RptItemInst
    {
        protected RptPage _page;

        public RptOutputInst(RptItemBase p_item)
            : base(p_item)
        {
        }

        /// <summary>
        /// 获取设置报表项所属的输出页面
        /// </summary>
        public RptPage Page
        {
            get { return _page; }
            set { _page = value; }
        }

        /// <summary>
        /// 获取宽度
        /// </summary>
        public double Width
        {
            get
            {
                if (_page == null)
                    return 0;

                double width = 0;
                PageDefine cols = _page.Cols;
                for (int i = 0; i < _region.ColSpan; i++)
                {
                    width += cols.Size[_region.Col - cols.Start + i];
                }
                return width;
            }
        }

        /// <summary>
        /// 获取高度
        /// </summary>
        public double Height
        {
            get
            {
                if (_page == null)
                    return 0;

                double height = 0;
                PageDefine rows = _page.Rows;
                for (int i = 0; i < _region.RowSpan; i++)
                {
                    height += rows.Size[_region.Row - rows.Start + i];
                }
                return height;
            }
        }

        /// <summary>
        /// 输出报表项内容
        /// </summary>
        protected override void DoOutput()
        {
            Inst.OutputItem(this);
        }
    }
}
