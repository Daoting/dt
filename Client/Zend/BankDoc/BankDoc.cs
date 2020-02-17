#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-06-08 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Sqlite;
using System;
#endregion

namespace Dt.Zend
{
    /// <summary>
    /// 存单
    /// </summary>
    [StateTable]
    public class BankDoc
    {
        /// <summary>
        /// 主键
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        /// <summary>
        /// 银行
        /// </summary>
        public string Bank { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数额
        /// </summary>
        public double Num { get; set; }

        /// <summary>
        /// 起始日期
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// 期限，年
        /// </summary>
        public int TimeLimit { get; set; }

        /// <summary>
        /// 利率，%
        /// </summary>
        public double Rate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// 利息
        /// </summary>
        public double Interest { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 是否已注销
        /// </summary>
        public bool Obsolete { get; set; }
    }
}
