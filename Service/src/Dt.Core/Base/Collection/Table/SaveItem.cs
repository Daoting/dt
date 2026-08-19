using System.Reflection;

namespace Dt.Core;

public class SaveItem
{
    string _tblName;
    object _data;
    
    /// <summary>
    /// 表名，实体类型的Table{} Entity不需要设置表名
    /// </summary>
    public string TableName
    {
        get
        {
            if (!string.IsNullOrEmpty(TableName))
                return _tblName;

            // 实体类型不需要设置表名
            if (_data is Table tbl && tbl.GetType().IsGenericType)
            {
                var tag = tbl.GetType().GenericTypeArguments[0].GetCustomAttribute<TblAttribute>(false);
                return tag?.Name;
            }

            if (_data is Entity en)
            {
                var tag = en.GetType().GetCustomAttribute<TblAttribute>(false);
                return tag?.Name;
            }
            return null;
        }
        set { _tblName = value; }
    }

    /// <summary>
    /// 是否为删除操作，默认false (insert 或 update)
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public object Data
    {
        get => _data;
        set
        {
            if (value == null
                || (value is not Table && value is not Row))
                Throw.Msg("待保存的数据类型必须为 Table 或 Row！");
            _data = value;
        }
    }
}