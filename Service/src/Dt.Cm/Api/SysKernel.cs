#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-27 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 系统内核Api
    /// </summary>
    [Api]
    public class SysKernel : DomainSvc
    {
        static readonly SqliteFileHandler _modelHandler = Kit.GetService<SqliteFileHandler>();
        static readonly Dict _config = new Dict();

        /// <summary>
        /// 获取参数配置，包括服务器时间、所有服务地址、sqlite文件版本号
        /// </summary>
        /// <returns></returns>
        public Dict GetConfig()
        {
            _config["Now"] = Kit.Now;
            return _config;
        }

        /// <summary>
        /// 更新服务端所有sqlite文件，包括sqlite.json中定义的所有sqlite文件，异步处理
        /// </summary>
        /// <returns></returns>
        public string UpdateAllSqliteFile()
        {
            return _modelHandler.RefreshAll();
        }

        /// <summary>
        /// 更新服务端单个sqlite文件，确保文件名已在sqlite.json中定义
        /// </summary>
        /// <param name="p_fileName"></param>
        /// <returns></returns>
        public string UpdateSqliteFile(string p_fileName)
        {
            return _modelHandler.Refresh(p_fileName);
        }

        /// <summary>
        /// 获取所有sqlite文件名
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllSqliteFile()
        {
            return _modelHandler.GetAllFile();
        }

        /// <summary>
        /// 参数配置
        /// </summary>
        internal static Dict Config => _config;
    }
}
