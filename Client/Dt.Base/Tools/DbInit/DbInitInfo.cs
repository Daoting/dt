#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-01-17 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base.Tools
{
    class DbInitInfo
    {
        public DatabaseType DbType { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string DefDb { get; set; }
        public string DefUser { get; set; }
        public string Pwd { get; set; }
        
        public string NewDb { get; set; }
        public string NewUser { get; set; }
        public string NewPwd { get; set; }
        
        public Action<string> Log { get; set; }

        public IDbTools Tools { get; set; }
    }
}
