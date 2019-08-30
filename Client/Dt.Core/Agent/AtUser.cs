#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 当前登录用户相关信息
    /// </summary>
    public static class AtUser
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public static string ID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public static string Name { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public static string Phone { get; set; }

        /// <summary>
        /// 是否已登录
        /// </summary>
        public static bool IsLogon => !string.IsNullOrEmpty(ID);
    }
}
