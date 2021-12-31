#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 系统内核Api
    /// </summary>
    [Api]
    public class SysKernel
    {
        /// <summary>
        /// 获取参数配置，包括服务器时间、所有服务地址、模型文件版本号
        /// </summary>
        /// <returns></returns>
        public List<object> GetConfig()
        {
            var ls = new List<object> { Kit.Now };
            if (Kit.IsSingletonSvc)
            {
                ls.Add(true);
            }
            else
            {
                Dict dt = new Dict { { "msg", "*/dt-msg" }, { "fsm", "*/dt-fsm" } };
                ls.Add(dt);
            }
            ls.Add(Kit.GetObj<SqliteModelHandler>().GetVersion());
            return ls;
        }

        /// <summary>
        /// 更新模型库文件
        /// </summary>
        /// <returns></returns>
        public bool UpdateModelDbFile()
        {
            var handler = Kit.GetObj<SqliteModelHandler>();
            Throw.IfNull(handler, SqliteModelHandler.Warning);
            return handler.Refresh("cm");
        }
    }
}
