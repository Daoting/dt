#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Sqlite;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 模型管理Api
    /// </summary>
    [Api(GroupName = "系统工具", AgentMode = AgentMode.Generic)]
    public class ModelMgr
    {
        /// <summary>
        /// 获取参数配置，包括模型文件版本号、服务器时间
        /// </summary>
        /// <returns></returns>
        public Dict GetConfig()
        {
            var handler = Kit.GetObj<SqliteModelHandler>();
            Throw.IfNull(handler, SqliteModelHandler.Warning);
            return new Dict { { "ver", handler.GetVersion() }, { "now", Kit.Now } };
        }

        /// <summary>
        /// 刷新模型版本
        /// </summary>
        /// <returns></returns>
        public bool 更新模型()
        {
            var handler = Kit.GetObj<SqliteModelHandler>();
            Throw.IfNull(handler, SqliteModelHandler.Warning);
            return handler.Refresh();
        }
    }
}
