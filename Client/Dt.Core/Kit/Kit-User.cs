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
    public partial class Kit
    {
        #region 用户信息
        /// <summary>
        /// 用户ID
        /// </summary>
        public static long UserID { get; internal set; } = -1;

        /// <summary>
        /// 姓名
        /// </summary>
        public static string UserName { get; set; } = "无";

        /// <summary>
        /// 手机号码
        /// </summary>
        public static string UserPhone { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public static string UserPhoto { get; set; }

        /// <summary>
        /// 是否已登录
        /// </summary>
        public static bool IsLogon => UserID > 0;
        #endregion

        #region 头像路径
        /// <summary>
        /// 缺省头像文件的路径
        /// </summary>
        public const string DefaultUserPhoto = "photo/profilephoto.jpg";
        #endregion
    }
}
