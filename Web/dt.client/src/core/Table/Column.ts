import type { CsType } from "../Kit.ts";

/**
 * 数据列
 */
export default class Column {
  /**
   * 列字段名
   */
  readonly id: string;

  /**
   * 列类型
   */
  type: CsType = 'String';

  /**
   * 字符串类型时的最大长度
   */
  maxLength: number = 0;

  /**
   * 列名不可为空，列类型默认为 string
   * @param colName - 列名
   * @param colType - 列数据类型
   */
  constructor(colName: string, colType?: CsType) {
    if (!colName) {
      throw new Error("未指定列名！");
    }

    this.id = colName;
    this.type = colType ?? 'String';
  }
}