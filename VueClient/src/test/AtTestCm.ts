import { Dict, MsgInfo, Table, Row, SaveItem } from "@/core";
/**
 * AtTestCm 测试序列化RPC调用封装
 */
export class AtTestCm {

    /**
     * 返回字符串
     * @returns
     */
    public static getString(): Promise<string> {
        return kit.rpc<string>("cm", "TestSerialize.GetString");
    }

    public static getStringAsync(): Promise<string> {
        return kit.rpc<string>("cm", "TestSerialize.GetStringAsync");
    }

    /**
     * 字符串参数
     * @param p_str
     */
    public static setString(p_str: string): Promise<boolean> {
        return kit.rpc<boolean>("cm", "TestSerialize.SetString", p_str);
    }

    /**
     * 返回bool值
     * @returns
     */
    public static getBool(): Promise<boolean> {
        return kit.rpc<boolean>("cm", "TestSerialize.GetBool");
    }

    public static getBoolAsync(): Promise<boolean> {
        return kit.rpc<boolean>("cm", "TestSerialize.GetBoolAsync");
    }

    /**
     * bool参数
     * @param p_val
     */
    public static setBool(p_val: boolean): Promise<boolean> {
        return kit.rpc<boolean>("cm", "TestSerialize.SetBool", p_val);
    }

    /**
     * 返回int值
     * @returns
     */
    public static getInt(): Promise<number> {
        return kit.rpc<number>("cm", "TestSerialize.GetInt");
    }

    public static getIntAsync(): Promise<number> {
        return kit.rpc<number>("cm", "TestSerialize.GetIntAsync");
    }

    /**
     * int参数
     * @param p_val
     */
    public static setInt(p_val: number): Promise<number> {
        return kit.rpc<number>("cm", "TestSerialize.SetInt", p_val);
    }

    /**
     * 返回long值
     * @returns
     */
    public static getLong(): Promise<number> {
        return kit.rpc<number>("cm", "TestSerialize.GetLong");
    }

    public static getLongAsync(): Promise<number> {
        return kit.rpc<number>("cm", "TestSerialize.GetLongAsync");
    }

    /**
     * long参数
     * @param p_val
     */
    public static setLong(p_val: number): Promise<number> {
        return kit.rpc<number>("cm", "TestSerialize.SetLong", p_val);
    }

    /**
     * 返回double值
     * @returns
     */
    public static getDouble(): Promise<number> {
        return kit.rpc<number>("cm", "TestSerialize.GetDouble");
    }

    public static getDoubleAsync(): Promise<number> {
        return kit.rpc<number>("cm", "TestSerialize.GetDoubleAsync");
    }

    /**
     * double参数
     * @param p_val
     */
    public static setDouble(p_val: number): Promise<number> {
        return kit.rpc<number>("cm", "TestSerialize.SetDouble", p_val);
    }

    /**
     * 返回DateTime值
     * @returns
     */
    public static getDateTime(): Promise<Date> {
        return kit.rpc<Date>("cm", "TestSerialize.GetDateTime");
    }

    public static getDateTimeAsync(): Promise<Date> {
        return kit.rpc<Date>("cm", "TestSerialize.GetDateTimeAsync");
    }

    /**
     * DateTime参数
     * @param p_val
     */
    public static setDateTime(p_val: Date): Promise<Date> {
        return kit.rpc<Date>("cm", "TestSerialize.SetDateTime", p_val);
    }

    /**
     * 返回byte[]值
     * @returns
     */
    public static getByteArray(): Promise<Uint8Array> {
        return kit.rpc<Uint8Array>("cm", "TestSerialize.GetByteArray");
    }

    public static getByteArrayAsync(): Promise<Uint8Array> {
        return kit.rpc<Uint8Array>("cm", "TestSerialize.GetByteArrayAsync");
    }

    /**
     * byte[]参数
     * @param p_val
     */
    public static setByteArray(p_val: Uint8Array): Promise<Uint8Array> {
        return kit.rpc<Uint8Array>("cm", "TestSerialize.SetByteArray", p_val);
    }

    /**
     * 返回MsgInfo
     * @returns
     */
    public static getMsgInfo(): Promise<MsgInfo> {
        return kit.rpc<MsgInfo>("cm", "TestSerialize.GetMsgInfo");
    }

    public static getMsgInfoAsync(): Promise<MsgInfo> {
        return kit.rpc<MsgInfo>("cm", "TestSerialize.GetMsgInfoAsync");
    }

    /**
     * MsgInfo参数
     * @param p_msg
     */
    public static setMsgInfo(p_msg: MsgInfo): Promise<MsgInfo> {
        return kit.rpc<MsgInfo>("cm", "TestSerialize.SetMsgInfo", p_msg);
    }

    /**
     * 返回字符串数组
     * @returns
     */
    public static getStringList(): Promise<string[]> {
        return kit.rpc<string[]>("cm", "TestSerialize.GetStringList");
    }

    public static getStringListAsync(): Promise<string[]> {
        return kit.rpc<string[]>("cm", "TestSerialize.GetStringListAsync");
    }

    /**
     * 字符串列表
     * @param p_ls
     */
    public static setStringList(p_ls: string[]): Promise<boolean> {
        return kit.rpc<boolean>("cm", "TestSerialize.SetStringList", p_ls);
    }

    /**
     * 返回bool值列表
     * @returns
     */
    public static getBoolList(): Promise<boolean[]> {
        return kit.rpc<boolean[]>("cm", "TestSerialize.GetBoolList");
    }

    public static getBoolListAsync(): Promise<boolean[]> {
        return kit.rpc<boolean[]>("cm", "TestSerialize.GetBoolListAsync");
    }

    /**
     * bool值列表
     * @param p_val
     */
    public static setBoolList(p_val: boolean[]): Promise<boolean[]> {
        return kit.rpc<boolean[]>("cm", "TestSerialize.SetBoolList", p_val);
    }

    /**
     * 返回int值列表
     * @returns
     */
    public static getIntList(): Promise<number[]> {
        return kit.rpc<number[]>("cm", "TestSerialize.GetIntList");
    }

    public static getIntListAsync(): Promise<number[]> {
        return kit.rpc<number[]>("cm", "TestSerialize.GetIntListAsync");
    }

    /**
     * int列表
     * @param p_val
     */
    public static setIntList(p_val: number[]): Promise<number[]> {
        return kit.rpc<number[]>("cm", "TestSerialize.SetIntList", p_val);
    }

    /**
     * 返回long值列表
     * @returns
     */
    public static getLongList(): Promise<number[]> {
        return kit.rpc<number[]>("cm", "TestSerialize.GetLongList");
    }

    public static getLongListAsync(): Promise<number[]> {
        return kit.rpc<number[]>("cm", "TestSerialize.GetLongListAsync");
    }

    /**
     * long列表
     * @param p_val
     */
    public static setLongList(p_val: number[]): Promise<number[]> {
        return kit.rpc<number[]>("cm", "TestSerialize.SetLongList", p_val);
    }

    /**
     * 返回double值列表
     * @returns
     */
    public static getDoubleList(): Promise<number[]> {
        return kit.rpc<number[]>("cm", "TestSerialize.GetDoubleList");
    }

    public static getDoubleListAsync(): Promise<number[]> {
        return kit.rpc<number[]>("cm", "TestSerialize.GetDoubleListAsync");
    }

    /**
     * double列表
     * @param p_val
     */
    public static setDoubleList(p_val: number[]): Promise<number[]> {
        return kit.rpc<number[]>("cm", "TestSerialize.SetDoubleList", p_val);
    }

    /**
     * DateTime列表
     * @returns
     */
    public static getDateTimeList(): Promise<Date[]> {
        return kit.rpc<Date[]>("cm", "TestSerialize.GetDateTimeList");
    }

    public static getDateTimeListAsync(): Promise<Date[]> {
        return kit.rpc<Date[]>("cm", "TestSerialize.GetDateTimeListAsync");
    }

    /**
     *
     * @param p_times
     * @returns
     */
    public static setDateTimeList(p_times: Date[]): Promise<boolean> {
        return kit.rpc<boolean>("cm", "TestSerialize.SetDateTimeList", p_times);
    }

    // /**
    //  *
    //  * @returns
    //  */
    // public static getObjectList(): Promise<object[]> {
    //     return kit.rpc<object[]>("cm", "TestSerialize.GetObjectList");
    // }

    // public static getObjectListAsync(): Promise<object[]> {
    //     return kit.rpc<object[]>("cm", "TestSerialize.GetObjectListAsync");
    // }

    // /**
    //  *
    //  * @param p_ls
    //  * @returns
    //  */
    // public static setObjectList(...p_ls: object[]): Promise<boolean> {
    //     const arg = (p_ls == null || p_ls.length === 0) ? null : p_ls;
    //     return kit.rpc<boolean>("cm", "TestSerialize.SetObjectList", arg);
    // }

    /**
     * 返回Table到客户端
     * @returns
     */
    public static getTable(): Promise<Table> {
        return kit.rpc<Table>("cm", "TestSerialize.GetTable");
    }

    public static getTableAsync(): Promise<Table> {
        return kit.rpc<Table>("cm", "TestSerialize.GetTableAsync");
    }

    /**
     * 由外部传递Table
     * @param p_tbl
     */
    public static setTable(p_tbl: Table): Promise<Table> {
        return kit.rpc<Table>("cm", "TestSerialize.SetTable", p_tbl);
    }

    /**
     * 返回Row到客户端
     * @returns
     */
    public static getRow<T extends Row>(): Promise<T> {
        return kit.rpc<T>("cm", "TestSerialize.GetRow");
    }

    public static getRowAsync<T extends Row>(): Promise<T> {
        return kit.rpc<T>("cm", "TestSerialize.GetRowAsync");
    }

    /**
     * 由外部传递Row
     * @param p_row
     */
    public static setRow<T extends Row>(p_row: Row): Promise<T> {
        return kit.rpc<T>("cm", "TestSerialize.SetRow", p_row);
    }

    // public static getEntityTable<T extends Entity>(): Promise<Table<T>> {
    //     return kit.rpc<Table<T>>("cm", "TestSerialize.GetEntityTable");
    // }

    // public static setEntityTable(p_tbl: Table): Promise<boolean> {
    //     return kit.rpc<boolean>("cm", "TestSerialize.SetEntityTable", p_tbl);
    // }

    // public static getEntity<T extends Row>(): Promise<T> {
    //     return kit.rpc<T>("cm", "TestSerialize.GetEntity");
    // }

    // public static setEntity(p_entity: Row): Promise<boolean> {
    //     return kit.rpc<boolean>("cm", "TestSerialize.SetEntity", p_entity);
    // }

    /**
     * 返回多个Table到客户端
     * @returns
     */
    public static getTableDict(): Promise<Dict> {
        return kit.rpc<Dict>("cm", "TestSerialize.GetTableDict");
    }

    public static getTableDictAsync(): Promise<Dict> {
        return kit.rpc<Dict>("cm", "TestSerialize.GetTableDictAsync");
    }

    /**
     * 由外部传递多个Table
     * @param p_dict
     */
    public static setTableDict(p_dict: Dict): Promise<Dict> {
        return kit.rpc<Dict>("cm", "TestSerialize.SetTableDict", p_dict);
    }

    /**
     * 返回多个Table到客户端
     * @returns
     */
    public static getTableList(): Promise<Table[]> {
        return kit.rpc<Table[]>("cm", "TestSerialize.GetTableList");
    }

    public static getTableListAsync(): Promise<Table[]> {
        return kit.rpc<Table[]>("cm", "TestSerialize.GetTableListAsync");
    }

    /**
     * 由外部传递多个Table
     * @param p_ls
     */
    public static setTableList(p_ls: Table[]): Promise<Table[]> {
        return kit.rpc<Table[]>("cm", "TestSerialize.SetTableList", p_ls);
    }

    public static getTableSaveItem(): Promise<SaveItem> {
        return kit.rpc<SaveItem>("cm", "TestSerialize.GetTableSaveItem");
    }

    public static setTableSaveItem(p_si: SaveItem): Promise<SaveItem> {
        return kit.rpc<SaveItem>("cm", "TestSerialize.SetTableSaveItem", p_si);
    }

    public static getRowSaveItem(): Promise<SaveItem> {
        return kit.rpc<SaveItem>("cm", "TestSerialize.GetRowSaveItem");
    }

    public static setRowSaveItem(p_si: SaveItem): Promise<SaveItem> {
        return kit.rpc<SaveItem>("cm", "TestSerialize.SetRowSaveItem", p_si);
    }

    public static getSaveItems(): Promise<SaveItem[]> {
        return kit.rpc<SaveItem[]>("cm", "TestSerialize.GetSaveItems");
    }

    public static setSaveItems(p_sis: SaveItem[]): Promise<SaveItem[]> {
        return kit.rpc<SaveItem[]>("cm", "TestSerialize.SetSaveItems", p_sis);
    }

    /**
     * 返回基本数据类型的Dict
     * @returns
     */
    public static getBaseDict(): Promise<Dict> {
        return kit.rpc<Dict>("cm", "TestSerialize.GetBaseDict");
    }

    public static getBaseDictAsync(): Promise<Dict> {
        return kit.rpc<Dict>("cm", "TestSerialize.GetBaseDictAsync");
    }

    /**
     * 返回基本数据类型的Dict
     * @returns
     */
    public static getCombineDict(): Promise<Dict> {
        return kit.rpc<Dict>("cm", "TestSerialize.GetCombineDict");
    }

    /**
     * 本数据类型的Dict
     * @param p_dict
     */
    public static sendDict(p_dict: Dict): Promise<Dict> {
        return kit.rpc<Dict>("cm", "TestSerialize.SendDict", p_dict);
    }

    /**
     * 返回Dict列表
     * @returns
     */
    public static getDictList(): Promise<Dict[]> {
        return kit.rpc<Dict[]>("cm", "TestSerialize.GetDictList");
    }

    public static getDictListAsync(): Promise<Dict[]> {
        return kit.rpc<Dict[]>("cm", "TestSerialize.GetDictListAsync");
    }

    /**
     * 发送Dict列表
     * @param p_dicts
     */
    public static sendDictList(p_dicts: Dict[]): Promise<boolean> {
        return kit.rpc<boolean>("cm", "TestSerialize.SendDictList", p_dicts);
    }

    // /**
    //  * 返回基础自定义类型
    //  * @returns
    //  */
    // public static getCustomBase<T extends object>(): Promise<T> {
    //     return kit.rpc<T>("cm", "TestSerialize.GetCustomBase");
    // }

    // public static getCustomBaseAsync<T extends object>(): Promise<T> {
    //     return kit.rpc<T>("cm", "TestSerialize.GetCustomBaseAsync");
    // }

    // /**
    //  * 由外部传递基础自定义类型
    //  * @param p_product
    //  * @returns
    //  */
    // public static setCustomBase(p_product: object): Promise<boolean> {
    //     return kit.rpc<boolean>("cm", "TestSerialize.SetCustomBase", p_product);
    // }

    // /**
    //  * 返回自定义对象列表
    //  * @returns
    //  */
    // public static getCustomList<T extends object>(): Promise<T[]> {
    //     return kit.rpc<T[]>("cm", "TestSerialize.GetCustomList");
    // }

    // public static getCustomListAsync<T extends object>(): Promise<T[]> {
    //     return kit.rpc<T[]>("cm", "TestSerialize.GetCustomListAsync");
    // }

    // /**
    //  * 由外部传递自定义对象列表
    //  * @param p_products
    //  * @returns
    //  */
    // public static setCustomList(p_products: object): Promise<boolean> {
    //     return kit.rpc<boolean>("cm", "TestSerialize.SetCustomList", p_products);
    // }

    // /**
    //  * 返回复杂自定义类型
    //  * @returns
    //  */
    // public static getCustomCombine<T extends object>(): Promise<T> {
    //     return kit.rpc<T>("cm", "TestSerialize.GetCustomCombine");
    // }

    // public static getCustomCombineAsync<T extends object>(): Promise<T> {
    //     return kit.rpc<T>("cm", "TestSerialize.GetCustomCombineAsync");
    // }

    // /**
    //  * 由外部传递复杂自定义类型
    //  * @param p_person
    //  * @returns
    //  */
    // public static setCustomCombine(p_person: object): Promise<boolean> {
    //     return kit.rpc<boolean>("cm", "TestSerialize.SetCustomCombine", p_person);
    // }

    // /**
    //  * 返回嵌套自定义类型
    //  * @returns
    //  */
    // public static getContainCustom<T extends object>(): Promise<T> {
    //     return kit.rpc<T>("cm", "TestSerialize.GetContainCustom");
    // }

    // public static getContainCustomAsync<T extends object>(): Promise<T> {
    //     return kit.rpc<T>("cm", "TestSerialize.GetContainCustomAsync");
    // }

    // /**
    //  * 由外部传递嵌套自定义类型
    //  * @param p_dept
    //  * @returns
    //  */
    // public static setContainCustom(p_dept: object): Promise<boolean> {
    //     return kit.rpc<boolean>("cm", "TestSerialize.SetContainCustom", p_dept);
    // }

    // public static asyncVoid<T extends object>(p_msg: string): Promise<T> {
    //     return kit.rpc<T>("cm", "TestSerialize.AsyncVoid", p_msg);
    // }

    // public static asyncDb(): Promise<Table> {
    //     return kit.rpc<Table>("cm", "TestSerialize.AsyncDb");
    // }

    // public static asyncWait(): Promise<number> {
    //     return kit.rpc<number>("cm", "TestSerialize.AsyncWait");
    // }
}
