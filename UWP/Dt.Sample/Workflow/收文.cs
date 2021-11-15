#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-11-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.Sample
{
    public partial class 收文
    {
        void OnSaving()
        {
            Throw.IfNullOrEmpty(文件标题, "文件标题不可为空");
        }
    }

    public enum 密级
    {
        普通,
        内部,
        绝密
    }

    #region 自动生成
    [Tbl("oa_收文")]
    public partial class 收文 : Entity
    {
        #region 构造方法
        收文() { }

        public 收文(
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
            AddCell<long>("ID", ID);
            AddCell<string>("来文单位", 来文单位);
            AddCell<DateTime>("来文时间", 来文时间);
            AddCell<byte>("密级", (byte)密级);
            AddCell<string>("文件标题", 文件标题);
            AddCell<string>("文件附件", 文件附件);
            AddCell<string>("市场部经理意见", 市场部经理意见);
            AddCell<string>("综合部经理意见", 综合部经理意见);
            AddCell<DateTime>("收文完成时间", 收文完成时间);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
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
            get { return (密级)((byte)this["密级"]); }
            set { this["密级"] = (byte)value; }
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
        #endregion
    }
    #endregion
}