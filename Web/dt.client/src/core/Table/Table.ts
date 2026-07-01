import Column from "./Column.ts";
import type Row from "./Row.ts";

export default class Table extends Array<Row> {
    columns: Column[] = [];

    serializeChanged: boolean = false;

    existDeleted: boolean = false;

    // 自定义 toString 标签，Object.prototype.toString 调用
    get [Symbol.toStringTag](): string {
        return 'Table';
    }
}