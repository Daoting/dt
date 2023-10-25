﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-28 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 数据库操作接口
    /// </summary>
    interface IDbTools
    {
        Task<bool> ExistsDb();

        Task<bool> ExistsUser();

        Task<bool> IsPwdCorrect();

        Task<bool> InitDb(int p_initType);

        Task<bool> ImportToDb(int p_initType);
    }
}
