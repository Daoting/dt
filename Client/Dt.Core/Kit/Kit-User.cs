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
        public static long UserID { get; set; } = -1;

        /// <summary>
        /// 姓名，始终有值
        /// <para>无姓名时：账号非空返回账号，否则返回手机号</para>
        /// </summary>
        public static string UserName { get; set; } = "无";

        /// <summary>
        /// 登录账号
        /// </summary>
        public static string UserAccount { get; set; }

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

        #region 登录
        /// <summary>
        /// cookie自动登录
        /// </summary>
        /// <param name="p_showWarning">是否显示警告信息</param>
        /// <returns></returns>
        public static Task<bool> LoginByCookie(bool p_showWarning = false)
        {
            return Stub.Inst.LoginByCookie(p_showWarning);
        }
        #endregion

        #region 权限
        /// <summary>
        /// 判断当前登录用户是否具有指定权限
        /// </summary>
        /// <param name="p_perID">权限ID</param>
        /// <returns>true 表示有权限</returns>
        public static Task<bool> HasPermission(long p_perID)
        {
            return Stub.Inst.HasPermission(p_perID);
        }
        #endregion

        #region 用户参数
        /// <summary>
        /// 根据参数id获取用户参数值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_paramID"></param>
        /// <returns></returns>
        public static Task<T> GetParamByID<T>(long p_paramID)
        {
            return Stub.Inst.GetParamByID<T>(p_paramID);
        }

        /// <summary>
        /// 根据参数名称获取用户参数值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_paramName"></param>
        /// <returns></returns>
        public static Task<T> GetParamByName<T>(string p_paramName)
        {
            return Stub.Inst.GetParamByName<T>(p_paramName);
        }

        /// <summary>
        /// 保存用户参数值
        /// </summary>
        /// <param name="p_paramID"></param>
        /// <param name="p_value"></param>
        /// <returns></returns>
        public static Task<bool> SaveParams(string p_paramID, string p_value)
        {
            return Stub.Inst.SaveParams(p_paramID, p_value);
        }
        #endregion
    }
}
