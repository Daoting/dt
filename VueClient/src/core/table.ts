import { Table, Row, TableInit } from "./types";
import { QTableColumn } from "quasar";

export function newTable(init: TableInit): Table {
  // 列结构
  const cols: QTableColumn[] = new Array(init.cols.length);
  for (let i = 0; i < init.cols.length; i++) {
    const colName = String(init.cols[i][0]);
    cols[i] = {
      name: colName,
      label: colName,
      field: colName,
    };
  }

  // 数据行
  const rows: Row[] = new Array(init.rows.length);
  for (let i = 0; i < init.rows.length; i++) {
    const rowData = init.rows[i];
    rows[i] = {};
    for (let j = 0; j < init.cols.length; j++) {
      rows[i][String(init.cols[j][0])] = rowData[j];
    }
  }

  return {
    cols: cols,
    rows: rows,
  };
}
