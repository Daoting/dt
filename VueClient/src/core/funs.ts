import { Table, Row, Cell, SaveItem, LetterInfo, MsgInfo } from "./types";

//#region 表格相关
export function isTable(value: unknown): value is Table {
  if (typeof value !== "object" || value === null) return false;
  if (Array.isArray(value)) return false;
  // 限定普通字面量对象，排除 Date/RegExp 等实例
  if (Object.getPrototypeOf(value) !== Object.prototype) return false;

  const obj = value as Partial<Table>;

  if (!Array.isArray(obj.cols)) return false;

  if (!Array.isArray(obj.rows)) return false;
  return true;
}

//#endregion

//#region Row相关
export function isRow(value: unknown): value is Row {
  // 基础过滤：非对象、null、数组直接返回 false
  if (typeof value !== "object" || value === null) return false;
  if (Array.isArray(value)) return false;
  // 仅普通字面量对象，排除 Date、RegExp 等内置实例
  if (Object.getPrototypeOf(value) !== Object.prototype) return false;

  const obj = value as Partial<Row>;

  // 1. 校验布尔字段
  if (typeof obj.isAdded !== "boolean") return false;

  // 2. 校验 table
  //if (!isTable(obj.table)) return false;

  if (!obj.cells || typeof obj.cells !== "object") return false;

  // 3. 校验 index getter：判断是否存在 index 访问器，读取返回 number
  // 不能直接 typeof obj.index，要检测属性描述符确认是 getter
  const desc = Object.getOwnPropertyDescriptor(value, "index");
  if (!desc || typeof desc.get !== "function") {
    return false;
  }

  try {
    const idx = obj.index;
    if (typeof idx !== "number") return false;
  } catch {
    // getter 执行抛出异常，判定不是 Row
    return false;
  }

  return true;
}

/**
 * row的单元格值生成普通扁平对象
 */
export function rowToObj(row: Row): Record<string, unknown> {
  const plain: Record<string, unknown> = {};
  for (const cell of Object.values(row.cells)) {
    plain[cell.id] = cell.val;
  }
  return plain;
}

/**
 * 初始化已有Row的cells：仅对row.cells中已存在的Cell赋值
 * @param row 已经存在的Row实例，直接原地修改它的cells
 * @param plainObj QForm扁平普通对象
 */
export function fillRow(row: Row, plainObj: Record<string, unknown>): void {
  // 遍历row已有的cell key（只处理Row里面已经有的Cell）
  for (const id of Object.keys(row.cells)) {
    const inputVal = plainObj[id];
    if (inputVal === undefined) continue;
    row.cells[id].val = inputVal as any;
  }
}

export function acceptRowChanges(row: Row): void {
  for (const cell of Object.values(row.cells)) {
    if (cell.originalVal !== cell.val) cell.originalVal = cell.val;
  }
  row.isAdded = false;
}
//#endregion

export function isCell(v: unknown): v is Cell {
  if (!v || typeof v !== "object") return false;
  if (Array.isArray(v)) return false;
  // 限定普通字面量对象，排除 Date/RegExp 等实例
  if (Object.getPrototypeOf(v) !== Object.prototype) return false;

  const o = v as Record<string, unknown>;
  return (
    typeof o.id === "string"
    && typeof o.type === "string"
    && "val" in o
    && "OriginalVal" in o
  );
}

export function isSaveItem(v: unknown): v is SaveItem {
  if (!v || typeof v !== "object") return false;
  const obj = v as Record<string, unknown>;

  // 校验字段存在且类型匹配
  if (typeof obj.table !== "string") return false;
  if (typeof obj.isDeleted !== "boolean") return false;
  if (!isTable(obj.data) && !isRow(obj.data)) return false;

  return true;
}

/** Row 的 isChanged 默认公共实现 */
export function rowIsChanged(row: Row): boolean {
  // 任意一个cell变更，整行即为变更
  return Object.values(row.cells).some((cell) => cellIsChanged(cell));
}

/**
 * 单元格模型（对齐C#命名）
 * id: 列标识，业务保证 data[key].id === key
 * type: C#类型名称 String Boolean Int32 Int64 DateTime Byte Byte[] Double Single
 * val: 当前编辑值
 * originalVal: 加载原始值，用于变更对比、撤销
 */
/** Cell公共isChanged默认实现 */
export function cellIsChanged(cell: Cell): boolean {
  switch (cell.type) {
    case "DateTime": {
      const v1 = cell.val?.getTime();
      const v2 = cell.originalVal?.getTime();
      return v1 !== v2;
    }
    case "Byte[]": {
      const a = cell.val;
      const b = cell.originalVal;
      if (a === null && b === null) return false;
      if (a === null || b === null) return true;
      if (a.length !== b.length) return true;
      for (let i = 0; i < a.length; i++) {
        if (a[i] !== b[i]) return true;
      }
      return false;
    }
    default:
      // String / Boolean / Int32 / Int64 / Byte / Double / Single
      return cell.val !== cell.originalVal;
  }
}

export function isMsgInfo(value: unknown): value is MsgInfo {
  if (typeof value !== "object" || value === null) return false;
  if (Array.isArray(value)) return false;

  const o = value as Partial<MsgInfo>;

  return (
    typeof o.MethodName === "string"
    && Array.isArray(o.Params)
    && typeof o.Title === "string"
    && typeof o.Content === "string"
  );
}

export function isLetterInfo(val: unknown): val is LetterInfo {
  if (typeof val !== "object" || val === null) return false;
  if (Array.isArray(val)) return false;
  const o = val as Partial<LetterInfo>;
  return (
    typeof o.id === "string"
    && typeof o.senderId === "number"
    && typeof o.senderName === "string"
    && typeof o.letterType === "number"
    && typeof o.content === "string"
    && o.sendTime instanceof Date
  );
}
