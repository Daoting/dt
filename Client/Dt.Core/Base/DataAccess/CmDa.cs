#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-28 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// cm服务的数据访问类
    /// </summary>
    public class CmDa : DataAccess
    {
        public CmDa(string p_tblName)
            :base("cm", p_tblName)
        { }
    }
}