import { QTableColumn } from "quasar";

/**
 * 单元格模型，isChanged 引用公共函数
 */
export type Cell =
  | {
      id: string;
      type: "String";
      val: string;
      originalVal: string;
    }
  | {
      id: string;
      type: "Boolean";
      val: boolean;
      originalVal: boolean;
    }
  | {
      id: string;
      type: "Int32";
      val: number;
      originalVal: number;
    }
  | {
      id: string;
      type: "Int64";
      val: number;
      originalVal: number;
    }
  | {
      id: string;
      type: "DateTime";
      val: Date | null;
      originalVal: Date | null;
    }
  | {
      id: string;
      type: "Byte";
      val: number;
      originalVal: number;
    }
  | {
      id: string;
      type: "Byte[]";
      val: Uint8Array | null;
      originalVal: Uint8Array | null;
    }
  | {
      id: string;
      type: "Double";
      val: number;
      originalVal: number;
    }
  | {
      id: string;
      type: "Single";
      val: number;
      originalVal: number;
    };

export interface Row {
  /** key = Cell.id，业务保证 cells[key].id === key */
  cells: Record<string, Cell>;
  isAdded: boolean;
  table: Table;
  get index(): number;
}

export type Column = QTableColumn & {
  colType?: "String" | "Boolean" | "Int32" | "Int64" | "DateTime" | "Byte" | "Byte[]" | "Double" | "Single";
};

export interface Table {
  cols: Column[];
  rows: Row[];
}

/**
 * 保存项，对应C# SaveItem
 */
export interface SaveItem {
  /**
   * 表名
   */
  table: string;

  /**
   * 是否为删除操作，默认false (insert 或 update)
   */
  isDeleted: boolean;

  /**
   * 待保存的数据，类型必须为 Table 或 Row
   */
  data: Table | Row;
}

export class Dict extends Map<string, unknown> {
  // 自定义 toString 标签，Object.prototype.toString 调用
  get [Symbol.toStringTag](): string {
    return "Dict";
  }
}

/**
 * 消息信息
 */
export interface MsgInfo {
  /**
   * 在线时调用客户端的方法名
   */
  MethodName: string;
  /**
   * 在线时调用客户端方法的参数
   */
  Params: unknown[];
  /**
   * 离线推送时的消息标题
   */
  Title: string;
  /**
   * 离线推送时的消息内容
   */
  Content: string;
}

/**
 * 信件信息
 */
export interface LetterInfo {
  /**
   * 信息标识
   */
  id: string;
  /**
   * 发送者标识
   */
  senderId: number;
  /**
   * 发送者名称
   */
  senderName: string;
  /**
   * 内容类型
   */
  letterType: number;
  /**
   * 内容
   */
  content: string;
  /**
   * 发送时间
   */
  sendTime: Date;
}
