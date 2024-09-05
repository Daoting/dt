﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 客户端Cookie字典
    /// </summary>
    [Sqlite("state")]
    public partial class CookieX : EntityX<CookieX>
    {
        #region 构造方法
        CookieX() { }

        public CookieX(
            string Key,
            string Val = default)
        {
            Add("Key", Key);
            Add("Val", Val);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 键
        /// </summary>
        [PrimaryKey]
        public string Key
        {
            get { return (string)this["Key"]; }
            set { this["Key"] = value; }
        }

        /// <summary>
        /// 值
        /// </summary>
        public string Val
        {
            get { return (string)this["Val"]; }
            set { this["Val"] = value; }
        }
    }
}
