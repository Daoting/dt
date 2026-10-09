#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-17 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core;

/// <summary>
/// 缓存Sql字典
/// </summary>
internal static class SqlDict
{
    static readonly Dictionary<string, string> _sqlDict = new(StringComparer.OrdinalIgnoreCase);
    
    public static string Cache(SvcInfo p_svcInfo)
    {
        try
        {
            var da = p_svcInfo.DbInfo.GetDa();
            var tbl = p_svcInfo.SvcName + "_sql";
            var schema = da.GetTableSchema(tbl).Result;
            if (schema == null)
                return $"未缓存sql(无{tbl})";
            return "无表";
        }
        catch (Exception e)
        {
            return "";
        }
    }
}
