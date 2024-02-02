#region 文件描述
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
    class SvcAccessInfo : IAccessInfo
    {
        readonly SvcAccess _da;

        public SvcAccessInfo(string p_name)
        {
            Name = p_name;
            _da = new SvcAccess(this);
        }

        public AccessType Type => AccessType.Service;

        public string Name { get; }

        public IDataAccess GetDa() => _da;
    }
}
