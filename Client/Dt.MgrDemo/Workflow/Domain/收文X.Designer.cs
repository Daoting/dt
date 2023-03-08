#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-30 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.MgrDemo
{
    [Tbl("demo_收文")]
    public partial class 收文X : EntityX<收文X>
    {
        #region 构造方法
        收文X() { }

        public 收文X(CellList p_cells) : base(p_cells) { }

        public 收文X(
            long ID,
            string 来文单位 = default,
            DateTime 来文时间 = default,
            密级 密级 = default,
            string 文件标题 = default,
            string 文件附件 = default,
            string 市场部经理意见 = default,
            string 综合部经理意见 = default,
            DateTime 收文完成时间 = default)
        {
            AddCell("ID", ID);
            AddCell("来文单位", 来文单位);
            AddCell("来文时间", 来文时间);
            AddCell("密级", 密级);
            AddCell("文件标题", 文件标题);
            AddCell("文件附件", 文件附件);
            AddCell("市场部经理意见", 市场部经理意见);
            AddCell("综合部经理意见", 综合部经理意见);
            AddCell("收文完成时间", 收文完成时间);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string 来文单位
        {
            get { return (string)this["来文单位"]; }
            set { this["来文单位"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime 来文时间
        {
            get { return (DateTime)this["来文时间"]; }
            set { this["来文时间"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public 密级 密级
        {
            get { return (密级)this["密级"]; }
            set { this["密级"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string 文件标题
        {
            get { return (string)this["文件标题"]; }
            set { this["文件标题"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string 文件附件
        {
            get { return (string)this["文件附件"]; }
            set { this["文件附件"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string 市场部经理意见
        {
            get { return (string)this["市场部经理意见"]; }
            set { this["市场部经理意见"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string 综合部经理意见
        {
            get { return (string)this["综合部经理意见"]; }
            set { this["综合部经理意见"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime 收文完成时间
        {
            get { return (DateTime)this["收文完成时间"]; }
            set { this["收文完成时间"] = value; }
        }
    }
}