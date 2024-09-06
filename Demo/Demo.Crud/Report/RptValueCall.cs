#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-04-25 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Crud
{
    [ValueCall]
    public static class RptValueCall
    {
        public static async Task<long> GetMaxID(string p_tblName)
        {
            return await At.GetScalar<long>($"select id from {p_tblName} order by id desc");
        }
        
        public static int GetRandom()
        {
            return new Random().Next();
        }
    }
}