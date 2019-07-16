#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-21 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dts.Auth
{
    /// <summary>
    /// 账号
    /// </summary>
    public class User
    {
        /// <summary>
        /// 用户标识
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 手机号，唯一
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 密码的MD5
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CTime { get; set; }
    }
}
