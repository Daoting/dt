#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-07-08 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.App.Workflow;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#endregion

namespace Dt.App
{
    /// <summary>
    /// 后续活动接收者信息
    /// </summary>
    public class AtvRecv
    {
        /// <summary>
        /// 获取设置活动定义
        /// </summary>
        public WfdAtvObj Def { get; set; }

        /// <summary>
        /// 获取设置是否按角色接收
        /// </summary>
        public bool IsRole { get; set; }

        /// <summary>
        /// 获取设置接收者(角色或用户)列表
        /// </summary>
        public Table Recvs { get; set; }

        /// <summary>
        /// 已选择的接收者id(角色或用户)
        /// </summary>
        public List<long> SelectedRecvs { get; set; }

        /// <summary>
        /// 获取设置发送时的备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 获取是否可多选
        /// </summary>
        public bool MultiSelection
        {
            get { return Def.ExecScope == WfdAtvExecScope.一组用户 || Def.ExecScope == WfdAtvExecScope.所有用户; }
        }

        /// <summary>
        /// 获取是否为只读(不可手动选择)
        /// </summary>
        public bool IsReadOnly
        {
            get { return IsRole || Def.ExecScope == WfdAtvExecScope.所有用户; }
        }
    }

    /// <summary>
    /// 同步活动的后续活动
    /// </summary>
    public class AtvSyncRecv : AtvRecv
    {
        /// <summary>
        /// 同步活动定义
        /// </summary>
        public WfdAtvObj SyncDef { get; set; }
    }

    /// <summary>
    /// 结束活动
    /// </summary>
    public class AtvFinishedRecv
    {
        /// <summary>
        /// 结束活动
        /// </summary>
        public WfdAtvObj Def { get; set; }

        /// <summary>
        /// 是否选择结束活动
        /// </summary>
        public bool IsSelected { get; set; }
    }
}