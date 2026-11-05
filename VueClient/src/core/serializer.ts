import {
  Table,
  Dict,
  LetterInfo,
  MsgInfo,
  Row,
  Cell,
  SaveItem,
  Column,
} from "./types";
import { kit } from "./index";

/**
 * 序列化
 * @param p_value
 * @returns
 */
export function serialize(p_value: any): any {
  if (p_value == null) return null;

  // 类型
  const tp = Object.prototype.toString.call(p_value);

  // 简单类型
  if (
    tp === "[object String]"
    || tp === "[object Boolean]"
    || tp === "[object Number]"
  )
    return p_value;

  // int64
  if (tp === "[object BigInt]") return p_value.toString();

  // 日期加前缀
  if (tp === "[object Date]") return "*0*" + p_value.toISOString();

  // 字节数组需要base64编码
  if (tp === "[object Uint8Array]")
    return "*1*" + btoa(String.fromCharCode(...p_value));

  // 内置类型
  if (tp === "[object Dict]") return serializeDict(p_value);
  if (kit.isTable(p_value)) return serializeTable(p_value);
  if (kit.isRow(p_value)) return serializeRow(p_value);
  if (kit.isSaveItem(p_value)) return serializeSaveItem(p_value);
  if (kit.isMsgInfo(p_value)) return serializeMsgInfo(p_value);
  if (kit.isLetterInfo(p_value)) return serializeLetterInfo(p_value);

  // 数组列表
  if (tp === "[object Array]") return serializeArray(p_value);

  // 非内置对象
  return serializeObject(p_value);
}

/**
 * 序列化Table
 * @param tbl
 * @returns
 */
function serializeTable(tbl: Table): any {
  const arr: any[] = [];
  // 类型
  arr.push("#tbl");

  // 列
  const cols: any[] = tbl.cols.map((col) =>
    col.colType === "String" ? [col.name] : [col.name, col.colType],
  );
  arr.push(cols);

  // 行
  const rows: any[] = [];
  for (let j = 0; j < tbl.rows.length; j++) {
    const row: any[] = [];
    const cells = tbl.rows[j].cells;
    for (const colKey of Object.keys(cells)) {
      row.push(serialize(cells[colKey].val));
    }
    rows.push(row);
  }

  arr.push(rows);
  return arr;
}

/**
 * 序列化独立Row
 * @param row
 * @returns
 */
function serializeRow(row: Row): any {
  const arr: any[] = [];
  arr.push("#row");

  const r = {};
  const cells = row.cells;
  for (const colKey of Object.keys(cells)) {
    const cell = cells[colKey];
    if (cell.type === "String") {
      r[colKey] = serialize(cell.val);
    } else {
      r[colKey] = [cell.type, serialize(cell.val)];
    }
  }
  arr.push(r);
  return arr;
}

function serializeSaveItem(si: SaveItem): any {
  const arr: any[] = [];
  arr.push("#si");
  arr.push(si.table);
  arr.push(si.isDeleted);
  if (kit.isTable(si.data)) arr.push(serializeSaveItemTable(si));
  else if (kit.isRow(si.data)) arr.push(serializeSaveItemRow(si));
  return arr;
}

function serializeSaveItemTable(si: SaveItem): any {
  const arr: any[] = [];
  // 类型
  arr.push("#tbl");

  const tbl = si.data as Table;
  // 列
  const cols: any[] = tbl.cols.map((col) =>
    col.colType === "String" ? [col.name] : [col.name, col.colType],
  );
  arr.push(cols);

  // 行
  const rows: any[] = [];
  if (si.isDeleted) {
    // 删除时只序列化非新增的行
    for (let j = 0; j < tbl.rows.length; j++) {
      const row: any[] = [];
      if (!tbl.rows[j].isAdded) {
        const cells = tbl.rows[j].cells;
        for (const colKey of Object.keys(cells)) {
          row.push(serialize(cells[colKey].val));
        }
        rows.push(row);
      }
    }
  } else {
    // 只序列化需要增加、修改的行
    for (let j = 0; j < tbl.rows.length; j++) {
      const row: any[] = [];
      const r = tbl.rows[j];

      if (r.isAdded) {
        const cells = r.cells;
        for (const colKey of Object.keys(cells)) {
          row.push(serialize(cells[colKey].val));
        }
        row.push(1); // 新增行标记
        rows.push(row);
      } else if (kit.rowIsChanged(r)) {
        const cells = r.cells;
        for (const colKey of Object.keys(cells)) {
          const c = cells[colKey];
          if (kit.cellIsChanged(c)) {
            const cVal: any[] = [];
            // 值变化时传递两值数组 [原始值,当前值]
            cVal.push(serialize(c.originalVal));
            cVal.push(serialize(c.val));
            row.push(cVal);
          }
        }
        row.push(2); // 修改行标记
        rows.push(row);
      }
    }
  }
  arr.push(rows);
  return arr;
}

function serializeSaveItemRow(si: SaveItem): any {
  const arr: any[] = [];
  arr.push("#row");

  const row = si.data as Row;
  arr.push(row.isAdded ? 1 : 2);

  const r = {};
  for (const colKey of Object.keys(row.cells)) {
    const cell = row.cells[colKey];
    if (row.isAdded || !kit.cellIsChanged(cell)) {
      if (cell.type === "String") {
        r[colKey] = serialize(cell.val);
      } else {
        r[colKey] = [cell.type, serialize(cell.val)];
      }
    } else {
      r[colKey] = [cell.type, serialize(cell.val), serialize(cell.originalVal)];
    }
  }

  arr.push(r);
  return arr;
}

/**
 * 序列化Dict
 * @param p_value
 * @returns
 */
function serializeDict(p_value: Dict): any[] {
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
      if (tp === "String" || tp === "Boolean") {
        // 简单类型，Number特殊，服务器端按空处理
      } else if (tp === "Date") {
        // 和c#同名
        tp = "DateTime";
      } else {
        // Number和复杂类型 空即可
        tp = "";
      }

      item.push(tp);
      item.push(serialize(v));
    }
    dict.push(item);
  });

  return dict;
}

/**
 * 序列化数组
 * @param p_value
 * @returns
 */
function serializeArray(p_value: any[]): any[] {
  if (p_value == null || p_value.length === 0) return ["&objs"];

  const obj: any[] = [];
  const len = p_value.length;

  // 确定数组元素类型
  let tp = Object.prototype.toString.call(p_value[0]);
  if (tp === "[object Number]") {
    // 判断是否都是整数
    let isInt = Number.isInteger(p_value[0]);
    for (let i = 1; i < len; i++) {
      if (typeof p_value[i] === "number") {
        if (isInt != Number.isInteger(p_value[i])) {
          isInt = false;
          break;
        }
      } else {
        tp = "";
        break;
      }
    }
    if (tp !== "") {
      // List<int> List<double>
      obj.push(isInt ? "&is" : "&ds");
      for (let i = 0; i < len; i++) {
        obj.push(p_value[i]);
      }
      return obj;
    }
  } else {
    for (let i = 1; i < len; i++) {
      const item = p_value[i];
      if (tp !== Object.prototype.toString.call(item)) {
        tp = "";
        break;
      }
    }
  }

  if (tp === "") {
    // 多类型
    obj.push("&objs");
    for (let i = 0; i < len; i++) {
      const item = p_value[i];
      let type = item.constructor.name;
      if (type === "String" || type === "Boolean") {
        // 简单类型，Number特殊，服务器端按空处理
      } else if (type === "Date") {
        // 和c#同名
        type = "DateTime";
      } else {
        // Number和复杂类型 空即可
        type = "";
      }
      const val: any[] = [];
      val.push(type);
      val.push(serialize(item));
      obj.push(val);
    }
  } else {
    if (tp === "[object String]") {
      obj.push("&ss");
    } else if (tp === "[object Boolean]") {
      obj.push("&bs");
    } else if (tp === "[object Date]") {
      obj.push("&dates");
    } else if (tp === "[object Table]") {
      obj.push("&tbls");
    } else if (tp === "[object Dict]") {
      obj.push("&dicts");
    } else {
      // 非内置对象列表
      obj.push("&object");
    }
    for (let i = 0; i < len; i++) {
      obj.push(serialize(p_value[i]));
    }
  }
  return obj;
}

/**
 * 序列化对象
 * @param p_value
 * @returns
 */
function serializeObject(p_value: any): any[] {
  // const obj: any[] = [];
  // obj.push("#object");
  // // 序列化对象属性值
  // Object.keys(p_value).forEach((key) => {
  //   p_value[key] = serialize(p_value[key]);
  // });
  // obj.push(p_value);
  // return obj;

  // 按Dict序列化
  const dict: any[] = [];
  dict.push("#dict");

  Object.keys(p_value).forEach((k) => {
    const v = p_value[k];
    let item: any[] = [];
    item.push(k);
    if (v == null) {
      item.push("Object");
      item.push(null);
    } else {
      let tp = v.constructor.name;
      if (tp === "String" || tp === "Boolean") {
        // 简单类型，Number特殊，服务器端按空处理
      } else if (tp === "Date") {
        // 和c#同名
        tp = "DateTime";
      } else if (tp === "BigInt") {
        // Number和复杂类型 空即可
        tp = "Int64";
      } else {
        // Number和复杂类型 空即可
        tp = "";
      }

      item.push(tp);
      item.push(serialize(v));
    }
    dict.push(item);
  });

  return dict;
}

function serializeLetterInfo(p_value: LetterInfo): any[] {
  const obj: any[] = [];
  obj.push("#letter");
  obj.push(p_value.id);
  obj.push(p_value.senderId);
  obj.push(p_value.senderName);
  obj.push(p_value.letterType);
  obj.push(p_value.content);
  obj.push(p_value.sendTime);
  return obj;
}

function serializeMsgInfo(p_value: MsgInfo): any[] {
  const obj: any[] = [];
  obj.push("#msg");
  obj.push(p_value.MethodName);
  const arr: any[] = [];
  if (p_value.Params && p_value.Params.length > 0) {
    for (let i = 0; i < p_value.Params.length; i++) {
      arr.push(serialize(p_value.Params[i]));
    }
  }
  obj.push(arr);
  obj.push(p_value.Title);
  obj.push(p_value.Content);
  return obj;
}

/**
 * 反序列化
 * @param p_jsonObj 用JSON.parse解析的JSON对象
 * @returns 反序列化结果
 */
export function deserialize(p_jsonObj: any): any {
  if (p_jsonObj == null) return null;

  // 类型
  const tp = Object.prototype.toString.call(p_jsonObj);

  // 自定义类型：数组且第一个元素为字符串，且以#或&开头
  let first: string;
  if (
    tp === "[object Array]"
    && p_jsonObj.length > 1
    && (first = p_jsonObj[0]) != null
    && typeof first === "string"
    && (first.startsWith("#") || first.startsWith("&"))
  ) {
    const alias = first.substring(1);
    if (first.startsWith("#")) {
      if (alias === "tbl") return deserializeTable(p_jsonObj);
      if (alias === "dict") return deserializeDict(p_jsonObj);
      if (alias === "row") return deserializeRow(p_jsonObj);
      if (alias === "si") return deserializeSaveItem(p_jsonObj);
      if (alias === "msg") return deserializeMsgInfo(p_jsonObj);
      if (alias === "letter") return deserializeLetter(p_jsonObj);
      if (alias === "object") return deserializeObject(p_jsonObj[1]);
    } else if (first.startsWith("&")) {
      // 简单类型的列表或数组，删除第一个标志元素
      if (
        alias === "ss"
        || alias === "bs"
        || alias === "is"
        || alias === "ls"
        || alias === "ds"
        || alias === "dates"
        || alias === "sarr"
        || alias === "barr"
        || alias === "iarr"
        || alias === "larr"
        || alias === "darr"
        || alias === "datearr"
      ) {
        p_jsonObj.shift();
        return p_jsonObj;
      }

      const ls: any[] = [];
      // List<object>
      if (alias === "objs") {
        for (let i = 1; i < p_jsonObj.length; i++) {
          const arr = p_jsonObj[i];
          if (Array.isArray(arr) && arr.length == 2) {
            ls.push(deserialize(arr[1]));
          } else {
            throw new Error("反序列化List<object>时，元素结构错误！");
          }
        }
      } else {
        for (let i = 1; i < p_jsonObj.length; i++) {
          const arr = p_jsonObj[i];
          if (Array.isArray(arr)) {
            ls.push(deserialize(arr));
          } else {
            throw new Error("反序列化列表时，元素不是数组！");
          }
        }
      }
      return ls;
    }
    throw new Error("未知的序列化类型别名：" + alias);
  }

  if (tp === "[object String]") {
    if (p_jsonObj.startsWith("*0*")) return new Date(p_jsonObj.substring(3));
    if (p_jsonObj.startsWith("*1*"))
      return base64ToUint8Array(p_jsonObj.substring(3));
    return p_jsonObj;
  }

  return p_jsonObj;
}

/**
 * 反序列化Table
 * @param p_json
 * @returns
 */
function deserializeTable(p_json: any): Table {
  if (p_json.length !== 3) throw new Error("反序列化Table时，JSON格式不正确！");

  const cols: Column[] = new Array(p_json[1].length);
  const rows: Row[] = new Array(p_json[2].length);
  const tbl = {
    cols: cols,
    rows: rows,
  };

  // 列结构
  for (let i = 0; i < p_json[1].length; i++) {
    const colName = String(p_json[1][i][0]);
    cols[i] = {
      name: colName,
      label: colName,
      field: colName,
      colType: p_json[1][i].length === 2 ? (p_json[1][i][1] as any) : "String",
    };
  }

  // 数据行
  for (let i = 0; i < p_json[2].length; i++) {
    const rowData = p_json[2][i];
    const row = {
      isAdded: false,
      table: tbl,
      get index() {
        return i;
      },
      cells: {} as Record<string, Cell>,
    };
    rows[i] = row;

    for (let j = 0; j < cols.length; j++) {
      const col = cols[j];
      const cd = rowData[j];
      const tp = Object.prototype.toString.call(cd);

      const cell: Cell = {
        id: col.name,
        type: col.colType as any,
        val: null,
        originalVal: null,
      };

      if (tp === "[object Array]" && cd.length === 2) {
        // ["列1原始值","列1当前值"]
        cell.originalVal = deserialize(cd[0]);
        cell.val = deserialize(cd[1]);
      } else {
        const val = deserialize(cd);
        cell.val = val;
        cell.originalVal = val;
      }
      row.cells[col.name] = cell;
    }

    // 超出的列值为行状态
    if (rowData.length > cols.length) {
      const rowState = rowData[cols.length];
      if (rowState === 1) row.isAdded = true;
      // else if (rowState === 2) row.isChanged = true;
      // else throw new Error("反序列化Table时，行状态不正确！");
    }
  }
  return tbl;
}

function deserializeRow(p_json: any[]): any {
  // [
  //   "#row",
  //   "新建/修改状态", // 可能没有
  //   {
  //       "key1": "当前值", // string类型，值无变化
  //       "key2": ["类型", "当前值"], // 非string类型，值无变化
  //   }
  // ]
  if (p_json.length < 2 || p_json.length > 3)
    throw new Error("反序列化Row时，JSON格式不正确！");

  const row = {
    isAdded: false,
    table: null,
    get index() {
      return 0;
    },
    cells: {} as Record<string, Cell>,
  };
  if (p_json.length === 3) {
    const rowState = p_json[1];
    if (rowState === 1) row.isAdded = true;
    // else if (rowState === 2) row.isChanged = true;
    // else throw new Error("反序列化Table时，行状态不正确！");
  }

  const obj = p_json[p_json.length - 1];
  Object.keys(obj).forEach((key) => {
    const val = obj[key];
    if (Array.isArray(val)) {
      if (val.length === 2) {
        // 非string类型，["类型", "当前值"]
        row.cells[key] = {
          id: key,
          type: val[0] as any,
          val: deserialize(val[1]) as any,
          originalVal: deserialize(val[1]) as any,
        };
      } else if (val.length === 3) {
        // ["类型", "当前值", "原始值"]
        row.cells[key] = {
          id: key,
          type: val[0] as any,
          val: deserialize(val[1]) as any,
          originalVal: deserialize(val[2]) as any,
        };
      } else {
        throw new Error("反序列化Row时，Cell值数组长度不正确！");
      }
    } else {
      // string类型，值无变化
      row.cells[key] = {
        id: key,
        type: "String",
        val: deserialize(val) as any,
        originalVal: deserialize(val) as any,
      };
    }
  });
  return row;
}

function deserializeSaveItem(p_json: any): SaveItem {
  if (p_json.length !== 4)
    throw new Error("反序列化SaveItem时，JSON格式不正确！");
  const si: SaveItem = {
    table: p_json[1],
    isDeleted: p_json[2],
    data: deserialize(p_json[3]),
  };
  return si;
}

function deserializeDict(p_jsonObj: any[]): Dict {
  const dict = new Dict();
  for (let i = 1; i < p_jsonObj.length; i++) {
    const arr = p_jsonObj[i];
    if (Array.isArray(arr)) {
      const key = arr[0];
      const val = arr[2];
      dict.set(key, deserialize(val));
    } else {
      throw new Error("反序列化Dict时，元素不是数组！");
    }
  }
  return dict;
}

function deserializeObject(p_jsonObj: any): any {
  Object.keys(p_jsonObj).forEach((key) => {
    p_jsonObj[key] = deserialize(p_jsonObj[key]);
  });
  return p_jsonObj;
}

function deserializeMsgInfo(p_jsonObj: any): any {
  // [
  //   "#msg",
  //   "abc.fun",
  //   [
  //       1,
  //       "a",
  //       true
  //   ],
  //   null,
  //   null
  // ]
  if (p_jsonObj.length !== 5) {
    throw new Error("反序列化MsgInfo时，JSON格式不正确！");
  }
  return {
    MethodName: p_jsonObj[1],
    Params: p_jsonObj[2],
    Title: p_jsonObj[3],
    Content: p_jsonObj[4],
  };
}

function deserializeLetter(p_jsonObj: any): LetterInfo {
  // [
  //   "#letter",
  //   "ID",
  //   SenderID, // long
  //   "SenderName",
  //   LetterType, // int
  //   "Content",
  //   "SendTime"
  // ]
  if (p_jsonObj.length !== 7) {
    throw new Error("反序列化LetterInfo时，JSON格式不正确！");
  }
  return {
    id: p_jsonObj[1],
    senderId: p_jsonObj[2],
    senderName: p_jsonObj[3],
    letterType: p_jsonObj[4],
    content: p_jsonObj[5],
    sendTime: p_jsonObj[6].startsWith("*0*")
      ? new Date(p_jsonObj[6].substring(3))
      : new Date(p_jsonObj[6]),
  };
}

/**
 * base64字符串 转 Uint8Array
 * @param base64 base64编码字符串
 */
function base64ToUint8Array(base64: string): Uint8Array {
  const binary = atob(base64);
  const len = binary.length;
  const arr = new Uint8Array(len);
  for (let i = 0; i < len; i++) {
    arr[i] = binary.charCodeAt(i);
  }
  return arr;
}
