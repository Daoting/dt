export function serialize(obj: any): any {
  console.log("serialize");
    const tp = Object.prototype.toString.call(obj);
    if (tp === "[object String]" || tp === "[object Boolean]" || tp === "[object Number]" || tp === "[object Date]") return obj;
  return null;
}

export function deserialize(jsonObj: any): any {
    const tp = Object.prototype.toString.call(jsonObj);
    if (tp === "[object String]" || tp === "[object Boolean]" || tp === "[object Number]" || tp === "[object Date]") return jsonObj;
  return null;
}
// /**
//  * 序列化
//  * @param p_value
//  * @returns
//  */
// export function serialize(p_value: any): any {
//   if (p_value == null) return null;

//   // 类型
//   const tp = Object.prototype.toString.call(p_value);

//   // 简单类型
//   if (
//     tp === "[object String]" ||
//     tp === "[object Boolean]" ||
//     tp === "[object Number]" ||
//     tp === "[object Date]"
//   )
//     return p_value;

//   // 内置类型
//   if (tp === "[object Table]") return serializeTable(p_value);
//   if (tp === "[object Row]") return serializeRow(p_value);
//   if (tp === "[object Dict]") return serializeDict(p_value);

//   // 数组列表
//   if (tp === "[object Array]") return serializeArray(p_value);

//   // 字节数组需要base64编码
//   if (tp === "[object Uint8Array]")
//     return btoa(String.fromCharCode(...p_value));

//   // 非内置对象
//   return serializeObject(p_value);
// }

// /**
//  * 序列化Table
//  * @param tbl
//  * @returns
//  */
// function serializeTable(tbl: Table): any[] {
//   const arr: any[] = [];
//   // 类型
//   arr.push("#tbl");

//   // 列
//   const cols: any[] = [];
//   const columns = tbl.columns;
//   for (let i = 0; i < columns.length; i++) {
//     const col: any[] = [];
//     const column = columns[i];
//     col.push(column.id);
//     // string类型省略
//     if (column.type !== "String") col.push(column.type);

//     cols.push(col);
//   }
//   arr.push(cols);

//   // 行
//   const rows: any[] = [];
//   if (tbl.serializeChanged) {
//     // 只序列化需要增删改的行
//     for (let p = 0; p < tbl.length; p++) {
//       if (tbl[p].isChanged) {
//         const row = Serializer.serializeChildRow(tbl[p]);
//         rows.push(row);
//       }
//     }
//   } else {
//     for (let j = 0; j < tbl.length; j++) {
//       const row = Serializer.serializeChildRow(tbl[j]);
//       rows.push(row);
//     }
//   }

//   arr.push(rows);
//   return arr;
// }

// /**
//  * 序列化Table内的Row
//  * @param row
//  * @returns
//  */
// function serializeChildRow(row: Row): any[] {
//   const arr: any[] = [];
//   const cells = row.cells;
//   for (let i = 0; i < cells.length; i++) {
//     const cell = cells[i];
//     if (cell.isChanged) {
//       // 值变化时传递两值数组 [原始值,当前值]
//       const val: any[] = [];
//       val.push(Serializer.serialize(cell.originalVal));
//       val.push(Serializer.serialize(cell.val));
//       arr.push(val);
//     } else {
//       arr.push(Serializer.serialize(cell.val));
//     }
//   }
//   return arr;
// }

// /**
//  * 序列化独立Row
//  * @param row
//  * @returns
//  */
// function serializeRow(row: Row): any[] {
//   const arr: any[] = [];
//   arr.push("#row");

//   // 行状态
//   if (row.isAdded) {
//     arr.push("Added");
//   } else if (row.isChanged) {
//     arr.push("Modified");
//   }

//   const cells = row.cells;
//   const obj = { __proto__: null };
//   for (let i = 0; i < cells.length; i++) {
//     const cell = cells[i];
//     if (cell.isChanged) {
//       // 值变化时传递完整信息 ["类型", "当前值", "原始值"]
//       const val: any[] = [];
//       val.push(cell.type);
//       val.push(Serializer.serialize(cell.val));
//       val.push(Serializer.serialize(cell.originalVal));
//       obj[cell.id] = val;
//     } else if (cell.type === "String") {
//       // string类型，值无变化
//       obj[cell.id] = Serializer.serialize(cell.val);
//     } else {
//       // 非string类型，值无变化 ["类型", "当前值"]
//       const val: any[] = [];
//       val.push(cell.type);
//       val.push(Serializer.serialize(cell.val));
//       obj[cell.id] = val;
//     }
//   }
//   arr.push(obj);
//   return arr;
// }

// /**
//  * 序列化Dict
//  * @param p_value
//  * @returns
//  */
// function serializeDict(p_value: Dict): any[] {
//   //[
//   //	"#dict",
//   //	["key1", "类型", "val1"], // 简单类型System.XXX
//   //	["key2", "Int64", 11],
//   //	["key3", "Byte[]", "CgwOEA=="],
//   //	["key4", "", ["#row",...]] // 复杂类型空即可
//   //]
//   const dict: any[] = [];
//   dict.push("#dict");

//   p_value.forEach((v, k) => {
//     let item: any[] = [];
//     item.push(k);
//     if (v == null) {
//       item.push("Object");
//       item.push(null);
//     } else {
//       let tp = v.constructor.name;
//       if (tp === "String" || tp === "Boolean") {
//         // 简单类型，Number特殊，服务器端按空处理
//       } else if (tp === "Date") {
//         // 和c#同名
//         tp = "DateTime";
//       } else {
//         // Number和复杂类型 空即可
//         tp = "";
//       }

//       item.push(tp);
//       item.push(Serializer.serialize(v));
//     }
//     dict.push(item);
//   });

//   return dict;
// }

// /**
//  * 序列化数组
//  * @param p_value
//  * @returns
//  */
// function serializeArray(p_value: any[]): any[] {
//   if (p_value == null || p_value.length === 0) return ["&objs"];

//   const obj: any[] = [];
//   const len = p_value.length;

//   // 确定数组元素类型
//   let tp = Object.prototype.toString.call(p_value[0]);
//   if (tp === "[object Number]") {
//     // 判断是否都是整数
//     let isInt = Number.isInteger(p_value[0]);
//     for (let i = 1; i < len; i++) {
//       if (typeof p_value[i] === "number") {
//         if (isInt != Number.isInteger(p_value[i])) {
//           isInt = false;
//           break;
//         }
//       } else {
//         tp = "";
//         break;
//       }
//     }
//     if (tp !== "") {
//       // List<int> List<double>
//       obj.push(isInt ? "&is" : "&ds");
//       for (let i = 0; i < len; i++) {
//         obj.push(p_value[i]);
//       }
//       return obj;
//     }
//   } else {
//     for (let i = 1; i < len; i++) {
//       const item = p_value[i];
//       if (tp !== Object.prototype.toString.call(item)) {
//         tp = "";
//         break;
//       }
//     }
//   }

//   if (tp === "") {
//     // 多类型
//     obj.push("&objs");
//     for (let i = 0; i < len; i++) {
//       const item = p_value[i];
//       let type = item.constructor.name;
//       if (type === "String" || type === "Boolean") {
//         // 简单类型，Number特殊，服务器端按空处理
//       } else if (type === "Date") {
//         // 和c#同名
//         type = "DateTime";
//       } else {
//         // Number和复杂类型 空即可
//         type = "";
//       }
//       const val: any[] = [];
//       val.push(type);
//       val.push(Serializer.serialize(item));
//       obj.push(val);
//     }
//   } else {
//     if (tp === "[object String]") {
//       obj.push("&ss");
//     } else if (tp === "[object Boolean]") {
//       obj.push("&bs");
//     } else if (tp === "[object Date]") {
//       obj.push("&dates");
//     } else if (tp === "[object Table]") {
//       obj.push("&tbls");
//     } else if (tp === "[object Dict]") {
//       obj.push("&dicts");
//     } else {
//       // 非内置对象列表
//       obj.push("&object");
//     }
//     for (let i = 0; i < len; i++) {
//       obj.push(Serializer.serialize(p_value[i]));
//     }
//   }
//   return obj;
// }

// /**
//  * 序列化对象
//  * @param p_value
//  * @returns
//  */
// function serializeObject(p_value: any[]): any[] {
//   const obj: any[] = [];
//   obj.push("#object");
//   // 序列化对象属性值
//   Object.keys(p_value).forEach((key) => {
//     p_value[key] = Serializer.serialize(p_value[key]);
//   });
//   obj.push(p_value);
//   return obj;
// }

// /**
//  * 反序列化
//  * @param p_jsonObj 用JSON.parse解析的JSON对象
//  * @returns 反序列化结果
//  */
// export function deserialize(p_jsonObj: any): any {
//   if (p_jsonObj == null) return null;

//   // 类型
//   const tp = Object.prototype.toString.call(p_jsonObj);

//   // 自定义类型：数组且第一个元素为字符串，且以#或&开头
//   let first: string;
//   if (
//     tp === "[object Array]" &&
//     p_jsonObj.length > 1 &&
//     (first = p_jsonObj[0]) != null &&
//     typeof first === "string" &&
//     (first.startsWith("#") || first.startsWith("&"))
//   ) {
//     const alias = first.substring(1);
//     if (first.startsWith("#")) {
//       if (alias === "tbl") return deserializeTable(p_jsonObj);
//       if (alias === "dict") return deserializeDict(p_jsonObj);
//       if (alias === "row") return deserializeRow(p_jsonObj);
//       if (alias === "msg") return deserializeTable(p_jsonObj);
//       if (alias === "letter") return deserializeLetter(p_jsonObj);
//       if (alias === "object") return deserializeObject(p_jsonObj[1]);
//     } else if (first.startsWith("&")) {
//       // 简单类型的列表，删除第一个标志元素
//       if (
//         alias === "ss" ||
//         alias === "bs" ||
//         alias === "is" ||
//         alias === "ds" ||
//         alias === "dates" ||
//         alias === "ds"
//       ) {
//         p_jsonObj.shift();
//         return p_jsonObj;
//       }

//       const ls: any[] = [];
//       // List<object>
//       if (alias === "objs") {
//         for (let i = 1; i < p_jsonObj.length; i++) {
//           const arr = p_jsonObj[i];
//           if (Array.isArray(arr) && arr.length == 2) {
//             ls.push(deserialize(arr[1]));
//           } else {
//             throw new Error("反序列化List<object>时，元素结构错误！");
//           }
//         }
//       } else {
//         for (let i = 1; i < p_jsonObj.length; i++) {
//           const arr = p_jsonObj[i];
//           if (Array.isArray(arr)) {
//             ls.push(deserialize(arr));
//           } else {
//             throw new Error("反序列化列表时，元素不是数组！");
//           }
//         }
//       }
//       return ls;
//     }
//     throw new Error("未知的序列化类型别名：" + alias);
//   }
//   return p_jsonObj;
// }

// /**
//  * 反序列化Table
//  * @param p_json
//  * @returns
//  */
// function deserializeTable(p_json: any[]): Table {
//   const tbl = new Table();
//   if (p_json.length !== 3) throw new Error("反序列化Table时，JSON格式不正确！");

//   // 列结构
//   const cols = p_json[1];
//   for (let i = 0; i < cols.length; i++) {
//     const col = cols[i];
//     const colName = col[0];
//     const type = col.length === 2 ? col[1] : "String";
//     tbl.addColumn(colName, type);
//   }

//   // 数据行
//   const rows = p_json[2];
//   for (let i = 0; i < rows.length; i++) {
//     const row = tbl.addRow();
//     row.isAdded = false;

//     // [
//     //  ["列1原始值","列1当前值"], // 值变化时传递两值数组
//     //  12,     // 无变化时只传单值
//     //  "Added/Modified" // 多出的列为行状态
//     // ],
//     const data = rows[i];
//     for (let j = 0; j < tbl.columns.length; j++) {
//       const cell = data[j];
//       if (Array.isArray(cell) && cell.length === 2) {
//         // 值变化时传递两值数组 [原始值,当前值]
//         row.cells[j].originalVal = Serializer.deserialize(cell[0]);
//         row.cells[j].val = Serializer.deserialize(cell[1]);
//       } else {
//         row.cells[j].initVal(Serializer.deserialize(cell));
//       }
//     }

//     if (data.length > tbl.columns.length) {
//       const rowState = data[tbl.columns.length];
//       if (rowState === "Added") row.isAdded = true;
//       else if (rowState === "Modified") row.isChanged = true;
//       else throw new Error("反序列化Table时，行状态不正确！");
//     }
//   }
//   return tbl;
// }

// function deserializeRow(p_json: any[]): Row {
//   // [
//   //   "#row",
//   //   "新建/修改状态", // 可能没有
//   //   {
//   //       "key1": "当前值", // string类型，值无变化
//   //       "key2": ["类型", "当前值"], // 非string类型，值无变化
//   //       "key3": ["类型", "当前值", "原始值"], // 值变化时传递完整信息
//   //   }
//   // ]
//   const row = new Row();
//   if (p_json.length < 2 || p_json.length > 3)
//     throw new Error("反序列化Row时，JSON格式不正确！");

//   if (p_json.length === 3) {
//     const rowState = p_json[1];
//     if (rowState === "Added") row.isAdded = true;
//     else if (rowState === "Modified") row.isChanged = true;
//     else throw new Error("反序列化Table时，行状态不正确！");
//   }

//   const obj = p_json[p_json.length - 1];
//   Object.keys(obj).forEach((key) => {
//     const cell = new Cell(key);
//     const val = obj[key];
//     if (Array.isArray(val)) {
//       if (val.length === 2) {
//         // 非string类型，值无变化 ["类型", "当前值"]
//         cell.type = val[0];
//         cell.val = Serializer.deserialize(val[1]);
//       } else if (val.length === 3) {
//         // 值变化时传递完整信息 ["类型", "当前值", "原始值"]
//         cell.type = val[0];
//         cell.originalVal = Serializer.deserialize(val[2]);
//         cell.val = Serializer.deserialize(val[1]);
//       } else {
//         throw new Error("反序列化Row时，Cell值数组长度不正确！");
//       }
//     } else {
//       // string类型，值无变化
//       cell.val = Serializer.deserialize(val);
//     }
//   });
//   return row;
// }

// function deserializeDict(p_jsonObj: any[]): Dict {
//   const dict = new Dict();
//   for (let i = 1; i < p_jsonObj.length; i++) {
//     const arr = p_jsonObj[i];
//     if (Array.isArray(arr)) {
//       const key = arr[0];
//       const val = arr[2];
//       dict.set(key, Serializer.deserialize(val));
//     } else {
//       throw new Error("反序列化Dict时，元素不是数组！");
//     }
//   }
//   return dict;
// }

// function deserializeObject(p_jsonObj: any): any {
//   Object.keys(p_jsonObj).forEach((key) => {
//     p_jsonObj[key] = deserialize(p_jsonObj[key]);
//   });
//   return p_jsonObj;
// }

// function deserializeLetter(p_jsonObj: any): LetterInfo {
//   // [
//   //   "#letter",
//   //   "ID",
//   //   SenderID, // long
//   //   "SenderName",
//   //   LetterType, // int
//   //   "Content",
//   //   "SendTime"
//   // ]
//   if (p_jsonObj.length !== 7) {
//     throw new Error("反序列化LetterInfo时，JSON格式不正确！");
//   }
//   return new LetterInfo(
//     p_jsonObj[1],
//     p_jsonObj[2],
//     p_jsonObj[3],
//     p_jsonObj[4],
//     p_jsonObj[5],
//     p_jsonObj[6],
//   );
// }
