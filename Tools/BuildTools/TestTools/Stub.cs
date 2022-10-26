namespace Dt.Core
{
    /// <summary>
    /// 系统存根
    /// </summary>
    public abstract partial class Stub
    {
        protected virtual void MergeTypeAlias(Dictionary<string, Type> p_dict) { }

        protected virtual void MergeSqliteDbs(Dictionary<string, SqliteTblsInfo> p_dict) { }
    }

}
