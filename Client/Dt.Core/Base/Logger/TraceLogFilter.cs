#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Core
{
    public class TraceLogFilter : ViewModel
    {
        #region 成员变量
        bool _showRpcLog = true;
        bool _showSqliteLog = true;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public bool ShowRpcLog
        {
            get { return _showRpcLog; }
            set { SetProperty(ref _showRpcLog, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowSqliteLog
        {
            get { return _showSqliteLog; }
            set { SetProperty(ref _showSqliteLog, value); }
        }
    }
}
