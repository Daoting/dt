#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-09-07 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo.多对多
{
    /// <summary>
    /// 系统用户
    /// </summary>
    [Tbl("DEMO_用户")]
    public partial class 用户X : EntityX<用户X>
    {
        #region 构造方法
        用户X() { }

        public 用户X(CellList p_cells) : base(p_cells) { }

        public 用户X(
            long ID,
            string 手机号 = default,
            string 姓名 = default,
            string 密码 = default)
        {
            Add("ID", ID);
            Add("手机号", 手机号);
            Add("姓名", 姓名);
            Add("密码", 密码);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 手机号，唯一
        /// </summary>
        public string 手机号
        {
            get { return (string)this["手机号"]; }
            set { this["手机号"] = value; }
        }

        public Cell c手机号 => _cells["手机号"];

        /// <summary>
        /// 姓名
        /// </summary>
        public string 姓名
        {
            get { return (string)this["姓名"]; }
            set { this["姓名"] = value; }
        }

        public Cell c姓名 => _cells["姓名"];

        /// <summary>
        /// 密码的md5
        /// </summary>
        public string 密码
        {
            get { return (string)this["密码"]; }
            set { this["密码"] = value; }
        }

        public Cell c密码 => _cells["密码"];
    }
}