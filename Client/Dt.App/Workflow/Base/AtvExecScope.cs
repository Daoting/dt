﻿#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-24 创建
**************************************************************************/
#endregion

#region 命名空间

#endregion

namespace Dt.App.Workflow
{
    /// <summary>
    /// 执行者范围
    /// </summary>
    public enum AtvExecScope
    {
        /// <summary>
        /// 一个角色的所有用户中有一个或多个用户处理活动
        /// </summary>
        一组用户,

        /// <summary>
        /// 一个角色的所有用户都必须处理活动
        /// </summary>
        所有用户,

        /// <summary>
        /// 一个角色的所有用户中需要指定一个用户处理活动
        /// </summary>
        单个用户,

        /// <summary>
        /// 角色的所有用户都可以接收活动，但只要有一个用户签收了活动，其他用户就不能处理活动
        /// </summary>
        任一用户
    }
}