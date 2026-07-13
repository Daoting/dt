import {Table } from "./types";

export function newTable(): Table {

    return {
        type: "table",
        cols: [],
        rows: []
    };
  }