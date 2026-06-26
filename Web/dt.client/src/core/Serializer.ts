import Dict from "./Dict.ts";

export default class Serializer {
    /// <summary>序列化</summary>
    static Serialize(p_value: any): any {
        if (p_value == null)
            return null;

        // 类型
        const tp = Object.prototype.toString.call(p_value);

        // 简单类型
        if (tp === "[object String]" || tp === "[object Boolean]" || tp === "[object Number]" || tp === "[object Date]")
            return p_value;

        if (tp === "[object Table]")
            return Serializer.SerializeTable(p_value);
        if (tp === "[object Dict]")
            return Serializer.SerializeDict(p_value);

        if (tp === "[object Array]")
            return Serializer.SerializeObject(p_value);
        return p_value;
    }

    /// <summary>序列化数组</summary>
    static SerializeObject(p_value: any[]): any[] {
        const obj: any[] = [];
        obj.push("&objs");
        const len = p_value.length;

        for (let i = 0; i < len; i++) {
            obj.push(Serializer.Serialize(p_value[i]));
        }
        return obj;
    }

    /// <summary>序列化Table</summary>
    static SerializeTable(p_value: any): any[] {
        const tbl: any[] = [];
        // 类型
        tbl.push("#tbl");

        // 表名
        if (p_value.Act)
            tbl.push(p_value.Act);

        // 列
        const cols: any[] = [];
        const columns = p_value.Columns;
        const len = columns.length;

        for (let i = 0; i < len; i++) {
            const col: any[] = [];
            const column = columns[i];
            col.push(column.ID());

            if (column.Type() !== "string")
                col.push(column.Type());

            cols.push(col);
        }

        tbl.push(cols);

        // 行
        const rows: any[] = [];
        const dataRows = p_value;
        const dlen = dataRows.length;

        if (p_value.SerializeChanged) {
            // 只序列化需要增删改的行
            for (let p = 0; p < dlen; p++) {
                if (dataRows[p].GetIsChanged()) {
                    // 修复原代码笔误：dr → dataRows[p]
                    const dr = Serializer.SerializeRow(dataRows[p]);
                    rows.push(dr);
                }
            }

            if (p_value.ExistDeleted()) {
                const delRows = p_value.DeletedRows();
                const delLen = delRows.length;

                for (let d = 0; d < delLen; d++) {
                    const delRow = Serializer.SerializeRow(delRows[d]);
                    rows.push(delRow);
                }
            }
        } else {
            for (let j = 0; j < dlen; j++) {
                const dataRow = Serializer.SerializeRow(dataRows[j]);
                rows.push(dataRow);
            }
        }

        tbl.push(rows);
        return tbl;
    }

    /// <summary>序列化DataRow</summary>
    static SerializeRow(p_dataRow: any): any[] {
        const data: any[] = [];
        const cells = p_dataRow.Cells;
        const len = cells.length;

        for (let i = 0; i < len; i++) {
            const cell = cells[i];
            data.push(cell.Val());
        }

        return data;
    }

    /// <summary>序列化Dict</summary>
    static SerializeDict(p_value: Dict): any[] {
        //[
        //	"#dict",
        //	["key1", "类型", "val1"], // 简单类型System.XXX
        //	["key2", "Int64", 11],
        //	["key3", "Byte[]", "CgwOEA=="],
        //	["key4", "", ["#row",...]] // 复杂类型空即可
        //]
        const dict: any[] = [];
        dict.push("#dict");

        p_value.forEach((v, k) => {
            let item: any[] = [];
            item.push(k);
            if (v == null) {
                item.push("Object");
                item.push(null);
            } else {
                let tp = v.constructor.name;
                if (tp === "String" || tp === "Boolean" || tp === "Number") {
                    // 简单类型，Number需要服务器端特殊处理
                }
                else if (tp === "Date") {
                    // 和c#同名
                    tp = "DateTime";
                }
                else {
                    // 复杂类型空即可
                    tp = "";
                }

                item.push(tp);
                item.push(Serializer.Serialize(v));
            }
            dict.push(item);
        });

        return dict;
    }

    /**
     * 反序列化
     * @param p_jsonObj 用JSON.parse解析的JSON对象
     * @returns 反序列化结果
     */
    static Deserialize(p_jsonObj: any): any {
        if (p_jsonObj == null)
            return null;

        // 类型
        const tp = Object.prototype.toString.call(p_jsonObj);

        // 自定义类型：数组且第一个元素为字符串，且以#或&开头
        if (tp === "[object Array]" && p_jsonObj.length > 1) {
            const first = p_jsonObj[0];
            if (typeof first === "string") {
                if (first.startsWith("#")) {
                    const alias = first.substring(1);
                    if (alias === "tbl")
                        return Serializer.DeserializeTable(p_jsonObj);
                    if (alias === "dict")
                        return Serializer.DeserializeDict(p_jsonObj);
                    if (alias === "row")
                        return Serializer.DeserializeTable(p_jsonObj);
                    if (alias === "msg")
                        return Serializer.DeserializeTable(p_jsonObj);
                    if (alias === "letter")
                        return Serializer.DeserializeTable(p_jsonObj);
                    if (alias === "object")
                        return Serializer.DeserializeTable(p_jsonObj);
                    throw new Error("未知的序列化类型别名：" + alias);
                }
                else if (first.startsWith("&")) {

                }
            }
        }
        return p_jsonObj;
    }

    private static DeserializeDict(p_jsonObj: any[]): Dict {
        const dict = new Dict();
        for (let i = 1; i < p_jsonObj.length; i++) {
            const arr = p_jsonObj[i];
            if (Array.isArray(arr)) {
                const key = arr[0];
                const val = arr[2];
                dict.set(key, Serializer.Deserialize(val));
            }
            else {
                throw new Error("反序列化Dict时，元素不是数组！");
            }
        }
        return dict;
    }

    /// <summary>反序列化DataTable</summary>
    static DeserializeTable(p_json: any[]): DataTable {
        const tbl = new DataTable();
        const len = p_json.length;

        // 表名
        if (len === 4)
            tbl.Act = p_json[1];

        // 列结构
        const cols = p_json[len - 2];
        const clen = cols.length;

        for (let i = 0; i < clen; i++) {
            const col = cols[i];
            const colName = col[0];
            const type = col.length === 2 ? col[1] : null;
            tbl.AddCol(colName, type);
        }

        // 数据行
        const rows = p_json[p_json.length - 1];
        const rlen = rows.length;

        for (let i = 0; i < rlen; i++) {
            const row = tbl.NewRow();
            row.IsAdd = false;
            row.Table = tbl;

            const data = rows[i];
            const dlen = data.length;

            for (let j = 0; j < dlen; j++) {
                row.Cells[j].SetInitVal(data[j]);
            }

            tbl.Add(row);
        }

        return tbl;
    }

    /// <summary>反序列化集合类型</summary>
    static DeserializeArray(p_json: any[]): any[] {
        const list: any[] = [];
        const len = p_json.length;

        for (let i = 1; i < len; i++) {
            list.push(Serializer.Deserialize(p_json[i]));
        }

        return list;
    }
}