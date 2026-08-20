using System.Reflection;
using System.Text.Json;

namespace Dt.Core;

public class SaveItem : IRpcJson
{
    string _tbl;
    object _data;

    /// <summary>
    /// 表名，实体类型的Table{} Entity不需要设置表名
    /// </summary>
    public string Table
    {
        get
        {
            if (!string.IsNullOrEmpty(_tbl))
                return _tbl;

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
        set { _tbl = value; }
    }

    /// <summary>
    /// 是否为删除操作，默认false (insert 或 update)
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 待保存的数据，类型必须为 Table 或 Row
    /// </summary>
    public object Data
    {
        get => _data;
        set
        {
            if (value == null
                || (value is not Core.Table && value is not Row))
                Throw.Msg("待保存的数据类型必须为 Table 或 Row！");
            _data = value;
        }
    }

    #region IRpcJson
    void IRpcJson.ReadRpcJson(ref Utf8JsonReader p_reader)
    {
        p_reader.Read();
        Table = p_reader.GetString();
        IsDeleted = p_reader.ReadAsBool();

        // #tbl或#row外层 [
        p_reader.Read();
        var tag = p_reader.ReadAsString();
        if (tag == "#tbl")
            Data = DeserializeTable(ref p_reader);
        else if (tag == "#row")
            Data = DeserializeRow(ref p_reader);

        // 最外层 ]
        p_reader.Read();
    }

    object DeserializeTable(ref Utf8JsonReader p_reader)
    {
#if SERVER
        Table tbl;
        var tp = Silo.GetEntityType(Table);
        if (tp != null)
        {
            tbl = Activator.CreateInstance(typeof(Table<>).MakeGenericType(tp)) as Table;
        }
        else
        {
            tbl = new Table();
        }
        ((IRpcJson)tbl).ReadRpcJson(ref p_reader);
        return tbl;
#else
        var tbl = new Table();
        ((IRpcJson)tbl).ReadRpcJson(ref p_reader);
        return tbl;
#endif
    }

    object DeserializeRow(ref Utf8JsonReader p_reader)
    {
#if SERVER
        Row row;
        var tp = Silo.GetEntityType(Table);
        if (tp != null)
        {
            row = Activator.CreateInstance(tp) as Row;
        }
        else
        {
            row = new Row();
        }
        ((IRpcJson)row).ReadRpcJson(ref p_reader);
        return row;
#else
        var row = new Row();
        ((IRpcJson)row).ReadRpcJson(ref p_reader);
        return row;
#endif
    }

    void IRpcJson.WriteRpcJson(Utf8JsonWriter p_writer)
    {
        if (!IsValid())
            return;

        p_writer.WriteStartArray();
        p_writer.WriteStringValue("#si");
        p_writer.WriteStringValue(Table);
        p_writer.WriteBooleanValue(IsDeleted);

        if (_data is Table tbl)
        {
            tbl.WriteRpcJsonInternal(p_writer, this);
        }
        else if (_data is Row row)
        {
            row.WriteSaveItemJson(p_writer, this);
        }
        p_writer.WriteEndArray();
    }

    bool IsValid()
    {
        if (_data == null)
            return false;

        if (_data is Row r)
        {
            if (IsDeleted)
                return !r.IsAdded;
            return r.IsAdded || r.IsChanged;
        }

        if (_data is Core.Table tbl)
        {
            if (IsDeleted)
            {
                return (from row in tbl
                        where !row.IsAdded
                        select row).Any();
            }

            return (from row in tbl
                    where row.IsAdded || row.IsChanged
                    select row).Any();
        }
        return false;
    }
    #endregion
}