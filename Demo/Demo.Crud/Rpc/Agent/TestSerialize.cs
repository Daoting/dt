namespace Demo.Crud;

/// <summary>
/// 功能测试Api
/// </summary>
public partial class AtTestCm
{
    /// <summary>
    /// 返回字符串
    /// </summary>
    /// <returns></returns>
    public static Task<string> GetString()
    {
        return Kit.Rpc<string>(
            "cm",
            "TestSerialize.GetString"
        );
    }

    public static Task<string> GetStringAsync()
    {
        return Kit.Rpc<string>(
            "cm",
            "TestSerialize.GetStringAsync"
        );
    }

    /// <summary>
    /// 字符串参数
    /// </summary>
    /// <param name="p_str"></param>
    public static Task<bool> SetString(string p_str)
    {
        return Kit.Rpc<bool>(
            "cm",
            "TestSerialize.SetString",
            p_str
        );
    }

    /// <summary>
    /// 返回bool值
    /// </summary>
    /// <returns></returns>
    public static Task<bool> GetBool()
    {
        return Kit.Rpc<bool>(
            "cm",
            "TestSerialize.GetBool"
        );
    }

    public static Task<bool> GetBoolAsync()
    {
        return Kit.Rpc<bool>(
            "cm",
            "TestSerialize.GetBoolAsync"
        );
    }

    /// <summary>
    /// bool参数
    /// </summary>
    /// <param name="p_val"></param>
    public static Task<bool> SetBool(bool p_val)
    {
        return Kit.Rpc<bool>(
            "cm",
            "TestSerialize.SetBool",
            p_val
        );
    }

    /// <summary>
    /// 返回int值
    /// </summary>
    /// <returns></returns>
    public static Task<int> GetInt()
    {
        return Kit.Rpc<int>(
            "cm",
            "TestSerialize.GetInt"
        );
    }

    public static Task<int> GetIntAsync()
    {
        return Kit.Rpc<int>(
            "cm",
            "TestSerialize.GetIntAsync"
        );
    }

    /// <summary>
    /// int参数
    /// </summary>
    /// <param name="p_val"></param>
    public static Task<int> SetInt(int p_val)
    {
        return Kit.Rpc<int>(
            "cm",
            "TestSerialize.SetInt",
            p_val
        );
    }

    /// <summary>
    /// 返回long值
    /// </summary>
    /// <returns></returns>
    public static Task<long> GetLong()
    {
        return Kit.Rpc<long>(
            "cm",
            "TestSerialize.GetLong"
        );
    }

    public static Task<long> GetLongAsync()
    {
        return Kit.Rpc<long>(
            "cm",
            "TestSerialize.GetLongAsync"
        );
    }

    /// <summary>
    /// long参数
    /// </summary>
    /// <param name="p_val"></param>
    public static Task<long> SetLong(long p_val)
    {
        return Kit.Rpc<long>(
            "cm",
            "TestSerialize.SetLong",
            p_val
        );
    }

    /// <summary>
    /// 返回double值
    /// </summary>
    /// <returns></returns>
    public static Task<double> GetDouble()
    {
        return Kit.Rpc<double>(
            "cm",
            "TestSerialize.GetDouble"
        );
    }

    public static Task<double> GetDoubleAsync()
    {
        return Kit.Rpc<double>(
            "cm",
            "TestSerialize.GetDoubleAsync"
        );
    }

    /// <summary>
    /// double参数
    /// </summary>
    /// <param name="p_val"></param>
    public static Task<double> SetDouble(double p_val)
    {
        return Kit.Rpc<double>(
            "cm",
            "TestSerialize.SetDouble",
            p_val
        );
    }

    /// <summary>
    /// 返回DateTime值
    /// </summary>
    /// <returns></returns>
    public static Task<DateTime> GetDateTime()
    {
        return Kit.Rpc<DateTime>(
            "cm",
            "TestSerialize.GetDateTime"
        );
    }

    public static Task<DateTime> GetDateTimeAsync()
    {
        return Kit.Rpc<DateTime>(
            "cm",
            "TestSerialize.GetDateTimeAsync"
        );
    }

    /// <summary>
    /// DateTime参数
    /// </summary>
    /// <param name="p_val"></param>
    public static Task<DateTime> SetDateTime(DateTime p_val)
    {
        return Kit.Rpc<DateTime>(
            "cm",
            "TestSerialize.SetDateTime",
            p_val
        );
    }

    /// <summary>
    /// 返回byte[]值
    /// </summary>
    /// <returns></returns>
    public static Task<byte[]> GetByteArray()
    {
        return Kit.Rpc<byte[]>(
            "cm",
            "TestSerialize.GetByteArray"
        );
    }

    public static Task<byte[]> GetByteArrayAsync()
    {
        return Kit.Rpc<byte[]>(
            "cm",
            "TestSerialize.GetByteArrayAsync"
        );
    }

    /// <summary>
    /// byte[]参数
    /// </summary>
    /// <param name="p_val"></param>
    public static Task<byte[]> SetByteArray(byte[] p_val)
    {
        return Kit.Rpc<byte[]>(
            "cm",
            "TestSerialize.SetByteArray",
            p_val
        );
    }

    /// <summary>
    /// 返回MsgInfo
    /// </summary>
    /// <returns></returns>
    public static Task<MsgInfo> GetMsgInfo()
    {
        return Kit.Rpc<MsgInfo>(
            "cm",
            "TestSerialize.GetMsgInfo"
        );
    }

    public static Task<MsgInfo> GetMsgInfoAsync()
    {
        return Kit.Rpc<MsgInfo>(
            "cm",
            "TestSerialize.GetMsgInfoAsync"
        );
    }

    /// <summary>
    /// MsgInfo参数
    /// </summary>
    /// <param name="p_msg"></param>
    public static Task<MsgInfo> SetMsgInfo(MsgInfo p_msg)
    {
        return Kit.Rpc<MsgInfo>(
            "cm",
            "TestSerialize.SetMsgInfo",
            p_msg
        );
    }

    /// <summary>
    /// 返回字符串数组
    /// </summary>
    /// <returns></returns>
    public static Task<List<string>> GetStringList()
    {
        return Kit.Rpc<List<string>>(
            "cm",
            "TestSerialize.GetStringList"
        );
    }

    public static Task<List<string>> GetStringListAsync()
    {
        return Kit.Rpc<List<string>>(
            "cm",
            "TestSerialize.GetStringListAsync"
        );
    }

    /// <summary>
    /// 字符串列表
    /// </summary>
    /// <param name="p_ls"></param>
    public static Task<bool> SetStringList(List<string> p_ls)
    {
        return Kit.Rpc<bool>(
            "cm",
            "TestSerialize.SetStringList",
            p_ls
        );
    }

    /// <summary>
    /// 返回bool值列表
    /// </summary>
    /// <returns></returns>
    public static Task<List<bool>> GetBoolList()
    {
        return Kit.Rpc<List<bool>>(
            "cm",
            "TestSerialize.GetBoolList"
        );
    }

    public static Task<List<bool>> GetBoolListAsync()
    {
        return Kit.Rpc<List<bool>>(
            "cm",
            "TestSerialize.GetBoolListAsync"
        );
    }

    /// <summary>
    /// bool值列表
    /// </summary>
    /// <param name="p_val"></param>
    public static Task<List<bool>> SetBoolList(List<bool> p_val)
    {
        return Kit.Rpc<List<bool>>(
            "cm",
            "TestSerialize.SetBoolList",
            p_val
        );
    }

    /// <summary>
    /// 返回int值列表
    /// </summary>
    /// <returns></returns>
    public static Task<List<int>> GetIntList()
    {
        return Kit.Rpc<List<int>>(
            "cm",
            "TestSerialize.GetIntList"
        );
    }

    public static Task<List<int>> GetIntListAsync()
    {
        return Kit.Rpc<List<int>>(
            "cm",
            "TestSerialize.GetIntListAsync"
        );
    }

    /// <summary>
    /// int列表
    /// </summary>
    /// <param name="p_val"></param>
    public static Task<List<int>> SetIntList(List<int> p_val)
    {
        return Kit.Rpc<List<int>>(
            "cm",
            "TestSerialize.SetIntList",
            p_val
        );
    }

    /// <summary>
    /// 返回long值列表
    /// </summary>
    /// <returns></returns>
    public static Task<List<long>> GetLongList()
    {
        return Kit.Rpc<List<long>>(
            "cm",
            "TestSerialize.GetLongList"
        );
    }

    public static Task<List<long>> GetLongListAsync()
    {
        return Kit.Rpc<List<long>>(
            "cm",
            "TestSerialize.GetLongListAsync"
        );
    }

    /// <summary>
    /// long列表
    /// </summary>
    /// <param name="p_val"></param>
    public static Task<List<long>> SetLongList(List<long> p_val)
    {
        return Kit.Rpc<List<long>>(
            "cm",
            "TestSerialize.SetLongList",
            p_val
        );
    }

    /// <summary>
    /// 返回double值列表
    /// </summary>
    /// <returns></returns>
    public static Task<List<double>> GetDoubleList()
    {
        return Kit.Rpc<List<double>>(
            "cm",
            "TestSerialize.GetDoubleList"
        );
    }

    public static Task<List<double>> GetDoubleListAsync()
    {
        return Kit.Rpc<List<double>>(
            "cm",
            "TestSerialize.GetDoubleListAsync"
        );
    }

    /// <summary>
    /// double列表
    /// </summary>
    /// <param name="p_val"></param>
    public static Task<List<double>> SetDoubleList(List<double> p_val)
    {
        return Kit.Rpc<List<double>>(
            "cm",
            "TestSerialize.SetDoubleList",
            p_val
        );
    }

    /// <summary>
    /// DateTime列表
    /// </summary>
    /// <returns></returns>
    public static Task<List<DateTime>> GetDateTimeList()
    {
        return Kit.Rpc<List<DateTime>>(
            "cm",
            "TestSerialize.GetDateTimeList"
        );
    }

    public static Task<List<DateTime>> GetDateTimeListAsync()
    {
        return Kit.Rpc<List<DateTime>>(
            "cm",
            "TestSerialize.GetDateTimeListAsync"
        );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p_times"></param>
    /// <returns></returns>
    public static Task<bool> SetDateTimeList(List<DateTime> p_times)
    {
        return Kit.Rpc<bool>(
            "cm",
            "TestSerialize.SetDateTimeList",
            p_times
        );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static Task<List<object>> GetObjectList()
    {
        return Kit.Rpc<List<object>>(
            "cm",
            "TestSerialize.GetObjectList"
        );
    }

    public static Task<List<object>> GetObjectListAsync()
    {
        return Kit.Rpc<List<object>>(
            "cm",
            "TestSerialize.GetObjectListAsync"
        );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p_ls"></param>
    /// <returns></returns>
    public static Task<bool> SetObjectList(params object[] p_ls)
    {
        return Kit.Rpc<bool>(
            "cm",
            "TestSerialize.SetObjectList",
            (p_ls == null || p_ls.Length == 0) ? null : p_ls.ToList()
        );
    }

    /// <summary>
    /// 返回Table到客户端
    /// </summary>
    /// <returns></returns>
    public static Task<Table> GetTable()
    {
        return Kit.Rpc<Table>(
            "cm",
            "TestSerialize.GetTable"
        );
    }

    public static Task<Table> GetTableAsync()
    {
        return Kit.Rpc<Table>(
            "cm",
            "TestSerialize.GetTableAsync"
        );
    }

    /// <summary>
    /// 由外部传递Table
    /// </summary>
    /// <param name="p_tbl"></param>
    public static Task<Table> SetTable(Table p_tbl)
    {
        return Kit.Rpc<Table>(
            "cm",
            "TestSerialize.SetTable",
            p_tbl
        );
    }

    /// <summary>
    /// 返回Row到客户端
    /// </summary>
    /// <returns></returns>
    public static Task<T> GetRow<T>()
        where T : Row
    {
        return Kit.Rpc<T>(
            "cm",
            "TestSerialize.GetRow"
        );
    }

    public static Task<T> GetRowAsync<T>()
        where T : Row
    {
        return Kit.Rpc<T>(
            "cm",
            "TestSerialize.GetRowAsync"
        );
    }

    /// <summary>
    /// 由外部传递Row
    /// </summary>
    /// <param name="p_row"></param>
    public static Task<T> SetRow<T>(Row p_row)
        where T : Row
    {
        return Kit.Rpc<T>(
            "cm",
            "TestSerialize.SetRow",
            p_row
        );
    }

    public static Task<Table<T>> GetEntityTable<T>()
        where T : Entity
    {
        return Kit.Rpc<Table<T>>(
            "cm",
            "TestSerialize.GetEntityTable"
        );
    }

    public static Task<bool> SetEntityTable(Table p_tbl)
    {
        return Kit.Rpc<bool>(
            "cm",
            "TestSerialize.SetEntityTable",
            p_tbl
        );
    }

    public static Task<T> GetEntity<T>()
        where T : Row
    {
        return Kit.Rpc<T>(
            "cm",
            "TestSerialize.GetEntity"
        );
    }

    public static Task<bool> SetEntity(Row p_entity)
    {
        return Kit.Rpc<bool>(
            "cm",
            "TestSerialize.SetEntity",
            p_entity
        );
    }

    /// <summary>
    /// 返回多个Table到客户端
    /// </summary>
    /// <returns></returns>
    public static Task<Dict> GetTableDict()
    {
        return Kit.Rpc<Dict>(
            "cm",
            "TestSerialize.GetTableDict"
        );
    }

    public static Task<Dict> GetTableDictAsync()
    {
        return Kit.Rpc<Dict>(
            "cm",
            "TestSerialize.GetTableDictAsync"
        );
    }

    /// <summary>
    /// 由外部传递多个Table
    /// </summary>
    /// <param name="p_dict"></param>
    public static Task<Dict> SetTableDict(Dict p_dict)
    {
        return Kit.Rpc<Dict>(
            "cm",
            "TestSerialize.SetTableDict",
            p_dict
        );
    }

    /// <summary>
    /// 返回多个Table到客户端
    /// </summary>
    /// <returns></returns>
    public static Task<List<Table>> GetTableList()
    {
        return Kit.Rpc<List<Table>>(
            "cm",
            "TestSerialize.GetTableList"
        );
    }

    public static Task<List<Table>> GetTableListAsync()
    {
        return Kit.Rpc<List<Table>>(
            "cm",
            "TestSerialize.GetTableListAsync"
        );
    }

    /// <summary>
    /// 由外部传递多个Table
    /// </summary>
    /// <param name="p_ls"></param>
    public static Task<List<Table>> SetTableList(List<Table> p_ls)
    {
        return Kit.Rpc<List<Table>>(
            "cm",
            "TestSerialize.SetTableList",
            p_ls
        );
    }

    public static Task<SaveItem> GetTableSaveItem()
    {
        return Kit.Rpc<SaveItem>(
            "cm",
            "TestSerialize.GetTableSaveItem"
        );
    }

    public static Task<SaveItem> SetTableSaveItem(SaveItem p_si)
    {
        return Kit.Rpc<SaveItem>(
            "cm",
            "TestSerialize.SetTableSaveItem",
            p_si
        );
    }

    public static Task<SaveItem> GetRowSaveItem()
    {
        return Kit.Rpc<SaveItem>(
            "cm",
            "TestSerialize.GetRowSaveItem"
        );
    }

    public static Task<SaveItem> SetRowSaveItem(SaveItem p_si)
    {
        return Kit.Rpc<SaveItem>(
            "cm",
            "TestSerialize.SetRowSaveItem",
            p_si
        );
    }

    public static Task<List<SaveItem>> GetSaveItems()
    {
        return Kit.Rpc<List<SaveItem>>(
            "cm",
            "TestSerialize.GetSaveItems"
        );
    }

    public static Task<List<SaveItem>> SetSaveItems(List<SaveItem> p_sis)
    {
        return Kit.Rpc<List<SaveItem>>(
            "cm",
            "TestSerialize.SetSaveItems",
            p_sis
        );
    }

    /// <summary>
    /// 返回基本数据类型的Dict
    /// </summary>
    /// <returns></returns>
    public static Task<Dict> GetBaseDict()
    {
        return Kit.Rpc<Dict>(
            "cm",
            "TestSerialize.GetBaseDict"
        );
    }

    public static Task<Dict> GetBaseDictAsync()
    {
        return Kit.Rpc<Dict>(
            "cm",
            "TestSerialize.GetBaseDictAsync"
        );
    }

    /// <summary>
    /// 返回基本数据类型的Dict
    /// </summary>
    /// <returns></returns>
    public static Task<Dict> GetCombineDict()
    {
        return Kit.Rpc<Dict>(
            "cm",
            "TestSerialize.GetCombineDict"
        );
    }

    /// <summary>
    /// 本数据类型的Dict
    /// </summary>
    /// <param name="p_dict"></param>
    public static Task<Dict> SendDict(Dict p_dict)
    {
        return Kit.Rpc<Dict>(
            "cm",
            "TestSerialize.SendDict",
            p_dict
        );
    }

    /// <summary>
    /// 返回Dict列表
    /// </summary>
    /// <returns></returns>
    public static Task<List<Dict>> GetDictList()
    {
        return Kit.Rpc<List<Dict>>(
            "cm",
            "TestSerialize.GetDictList"
        );
    }

    public static Task<List<Dict>> GetDictListAsync()
    {
        return Kit.Rpc<List<Dict>>(
            "cm",
            "TestSerialize.GetDictListAsync"
        );
    }

    /// <summary>
    /// 发送Dict列表
    /// </summary>
    /// <param name="p_dicts"></param>
    public static Task<bool> SendDictList(List<Dict> p_dicts)
    {
        return Kit.Rpc<bool>(
            "cm",
            "TestSerialize.SendDictList",
            p_dicts
        );
    }

    /// <summary>
    /// 返回基础自定义类型
    /// </summary>
    /// <returns></returns>
    public static Task<T> GetCustomBase<T>()
        where T : class
    {
        return Kit.Rpc<T>(
            "cm",
            "TestSerialize.GetCustomBase"
        );
    }

    public static Task<T> GetCustomBaseAsync<T>()
        where T : class
    {
        return Kit.Rpc<T>(
            "cm",
            "TestSerialize.GetCustomBaseAsync"
        );
    }

    /// <summary>
    /// 由外部传递基础自定义类型
    /// </summary>
    /// <param name="p_product"></param>
    /// <returns></returns>
    public static Task<bool> SetCustomBase(object p_product)
    {
        return Kit.Rpc<bool>(
            "cm",
            "TestSerialize.SetCustomBase",
            p_product
        );
    }

    /// <summary>
    /// 返回自定义对象列表
    /// </summary>
    /// <returns></returns>
    public static Task<List<T>> GetCustomList<T>()
        where T : class
    {
        return Kit.Rpc<List<T>>(
            "cm",
            "TestSerialize.GetCustomList"
        );
    }

    public static Task<List<T>> GetCustomListAsync<T>()
        where T : class
    {
        return Kit.Rpc<List<T>>(
            "cm",
            "TestSerialize.GetCustomListAsync"
        );
    }

    /// <summary>
    /// 由外部传递自定义对象列表
    /// </summary>
    /// <param name="p_products"></param>
    /// <returns></returns>
    public static Task<bool> SetCustomList(object p_products)
    {
        return Kit.Rpc<bool>(
            "cm",
            "TestSerialize.SetCustomList",
            p_products
        );
    }

    /// <summary>
    /// 返回复杂自定义类型
    /// </summary>
    /// <returns></returns>
    public static Task<T> GetCustomCombine<T>()
        where T : class
    {
        return Kit.Rpc<T>(
            "cm",
            "TestSerialize.GetCustomCombine"
        );
    }

    public static Task<T> GetCustomCombineAsync<T>()
        where T : class
    {
        return Kit.Rpc<T>(
            "cm",
            "TestSerialize.GetCustomCombineAsync"
        );
    }

    /// <summary>
    /// 由外部传递复杂自定义类型
    /// </summary>
    /// <param name="p_person"></param>
    /// <returns></returns>
    public static Task<bool> SetCustomCombine(object p_person)
    {
        return Kit.Rpc<bool>(
            "cm",
            "TestSerialize.SetCustomCombine",
            p_person
        );
    }

    /// <summary>
    /// 返回嵌套自定义类型
    /// </summary>
    /// <returns></returns>
    public static Task<T> GetContainCustom<T>()
        where T : class
    {
        return Kit.Rpc<T>(
            "cm",
            "TestSerialize.GetContainCustom"
        );
    }

    public static Task<T> GetContainCustomAsync<T>()
        where T : class
    {
        return Kit.Rpc<T>(
            "cm",
            "TestSerialize.GetContainCustomAsync"
        );
    }

    /// <summary>
    /// 由外部传递嵌套自定义类型
    /// </summary>
    /// <param name="p_dept"></param>
    /// <returns></returns>
    public static Task<bool> SetContainCustom(object p_dept)
    {
        return Kit.Rpc<bool>(
            "cm",
            "TestSerialize.SetContainCustom",
            p_dept
        );
    }

    public static Task<T> AsyncVoid<T>(string p_msg)
        where T : class
    {
        return Kit.Rpc<T>(
            "cm",
            "TestSerialize.AsyncVoid",
            p_msg
        );
    }

    public static Task<Table> AsyncDb()
    {
        return Kit.Rpc<Table>(
            "cm",
            "TestSerialize.AsyncDb"
        );
    }

    public static Task<int> AsyncWait()
    {
        return Kit.Rpc<int>(
            "cm",
            "TestSerialize.AsyncWait"
        );
    }
}
