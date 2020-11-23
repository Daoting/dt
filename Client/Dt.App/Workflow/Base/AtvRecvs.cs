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
        /// 获取设置活动标识
        /// </summary>
        public WfdAtv Def { get; set; }

        /// <summary>
        /// 获取设置是否按角色接收
        /// </summary>
        public bool IsRole { get; set; }

        /// <summary>
        /// 获取设置接收者(角色或用户)列表
        /// </summary>
        public Table Recvs { get; set; }

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
    /// 后续活动列表
    /// </summary>
    public class AtvRecvs : KeyedCollection<string, AtvRecv>
    {
        public AtvRecvs()
            : base(StringComparer.OrdinalIgnoreCase)
        { }

        protected override string GetKeyForItem(AtvRecv p_item)
        {
            if (p_item != null && p_item.Def != null)
                return p_item.Def.ID.ToString();
            throw new Exception("后续活动接收者信息不可空！");
        }
    }
}
