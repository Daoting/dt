import { Table, Row, SaveItem } from "@/core";

/**
 * 以参数值方式执行Sql语句，返回结果集
 * @param p_sqlOrSp Sql语句 或 存储过程名
 * @param p_params 参数值，支持对象/Record，默认undefined
 * @returns 返回Table数据
 */
export function query(
  p_sqlOrSp: string,
  p_params?: object,
  p_svc: string = "da",
): Promise<Table> {
  return kit.rpc<Table>(p_svc, "Da.Query", p_sqlOrSp, p_params);
}

/**
 * 按页查询数据
 * @param p_starRow 起始行号：mysql中第一行为0行
 * @param p_pageSize 每页显示行数
 * @param p_sqlOrSp Sql语句 或 存储过程名
 * @param p_params 参数值，支持对象/Record，默认undefined
 * @returns 返回Table数据
 */
export function page(
  p_starRow: number,
  p_pageSize: number,
  p_sqlOrSp: string,
  p_params?: object,
  p_svc: string = "da",
): Promise<Table> {
  return kit.rpc<Table>(
    p_svc,
    "Da.Page",
    p_starRow,
    p_pageSize,
    p_sqlOrSp,
    p_params,
  );
}

/**
 * 以参数值方式执行Sql语句，只返回第一行数据
 * @param p_sqlOrSp Sql语句 或 存储过程名
 * @param p_params 参数值，支持对象/Record，默认undefined
 * @returns 返回第一行Row或null
 */
export function first<T extends Row>(
  p_sqlOrSp: string,
  p_params?: object,
  p_svc: string = "da",
): Promise<T | null> {
  return kit.rpc<T>(p_svc, "Da.First", p_sqlOrSp, p_params);
}

/**
 * 以参数值方式执行Sql语句，返回符合条件的第一列数据，并转换为指定类型
 * @param p_type 第一列数据类型
 * @param p_sqlOrSp Sql语句 或 存储过程名
 * @param p_params 参数值，支持对象/Record，默认undefined
 * @returns 返回第一列数据的泛型列表
 */
export function firstCol<T extends object>(
  p_type: string,
  p_sqlOrSp: string,
  p_params?: object,
  p_svc: string = "da",
): Promise<T> {
  return kit.rpc<T>(p_svc, "Da.FirstCol", p_type, p_sqlOrSp, p_params);
}

/**
 * 以参数值方式执行Sql语句，只返回第一个单元格数据
 * @param p_sqlOrSp Sql语句 或 存储过程名
 * @param p_params 参数值，支持对象/Record，默认undefined
 * @returns 返回第一个单元格数据
 */
export function getScalar<T>(
  p_sqlOrSp: string,
  p_params?: object,
  p_svc: string = "da",
): Promise<T> {
  return kit.rpc<T>(p_svc, "Da.GetScalar", p_sqlOrSp, p_params);
}

/**
 * 以参数值方式执行Sql语句，返回影响的行数
 * @param p_sqlOrSp Sql语句 或 存储过程名
 * @param p_params 参数值，支持对象/Record
 * @returns 返回执行后影响的行数
 */
export function exec(
  p_sqlOrSp: string,
  p_params: object,
  p_svc: string = "da",
): Promise<number> {
  return kit.rpc<number>(p_svc, "Da.Exec", p_sqlOrSp, p_params);
}

/**
 * 一个事务内执行多个Sql
 * @param p_dts 参数列表，每个Dict中包含两个键：text,params，text为sql语句params类型为Dict或List{Dict}
 * @returns 返回执行后影响的行数
 */
export function batchExec(
  p_dts: Record<string, unknown>[],
  p_svc: string = "da",
): Promise<number> {
  return kit.rpc<number>(p_svc, "Da.BatchExec", p_dts);
}

/**
 * 一个事务内批量保存列表中所有需要新增、修改、删除的数据
 * 1. SaveItem项中的Data只支持：有表名的Table、有表名的Row、Entity、Table{TEntity}
 * 2. Entity、Table{TEntity}支持实体的各种回调和领域事件
 * 3. 反序列化时根据表名确定是否有对应Entity，没有时按Row、Table处理
 * @param p_datas 保存项列表
 * @returns 是否成功
 */
export function save(
  p_datas: SaveItem[],
  p_svc: string = "da",
): Promise<boolean> {
  return kit.rpc<boolean>(p_svc, "Da.Save", p_datas);
}

/**
 * 获取新ID，默认无标志位
 * @returns 新ID
 */
export function newID(p_svc: string = "da"): Promise<number> {
  return kit.rpc<number>(p_svc, "Da.NewID");
}

/**
 * 获取新序列值
 * @param p_seqName 序列名称，不可为空
 * @returns 新序列值
 */
export function newSeq(
  p_seqName: string,
  p_svc: string = "da",
): Promise<number> {
  return kit.rpc<number>(p_svc, "Da.NewSeq", p_seqName);
}

/**
 * 根据表名创建行数据，自动填写ID和列的默认值
 * @param p_tblName 表名
 * @param p_count 批量创建的行数，默认1
 * @returns Promise<Table>
 */
export function newRow(
  p_tblName: string,
  p_count: number = 1,
  p_svc: string = "da",
): Promise<Table> {
  return kit.rpc<Table>(p_svc, "Da.NewRow", p_tblName, p_count);
}

/**
 * 获取当前服务的默认数据源键名
 * @returns 数据源键名
 */
export function getDbKey(p_svc: string = "da"): Promise<string> {
  return kit.rpc<string>(p_svc, "Da.GetDbKey");
}
