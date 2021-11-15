#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-06-07 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 登录后的返回结果
    /// </summary>
    public class LoginResult : Entity
    {
        /// <summary>
        /// 登录是否成功
        /// </summary>
        public bool IsSuc
        {
            get { return (bool)this["IsSuc"]; }
            set { this["IsSuc"] = value; }
        }

        /// <summary>
        /// 登录失败后的错误提示
        /// </summary>
        public string Error
        {
            get { return (string)this["Error"]; }
            set { this["Error"] = value; }
        }

        /// <summary>
        /// 用户标识
        /// </summary>
        public long UserID
        {
            get { return (long)this["UserID"]; }
            set { this["UserID"] = value; }
        }

        /// <summary>
        /// 手机号，唯一
        /// </summary>
        public string Phone
        {
            get { return (string)this["Phone"]; }
            set { this["Phone"] = value; }
        }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 密码的md5
        /// </summary>
        public string Pwd
        {
            get { return (string)this["Pwd"]; }
            set { this["Pwd"] = value; }
        }

        /// <summary>
        /// 头像
        /// </summary>
        public string Photo
        {
            get { return (string)this["Photo"]; }
            set { this["Photo"] = value; }
        }

        /// <summary>
        /// 缓存数据的版本号列表
        /// </summary>
        public string Version
        {
            get { return (string)this["Version"]; }
            set { this["Version"] = value; }
        }
    }
}
