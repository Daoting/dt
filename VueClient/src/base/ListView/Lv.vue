<template>
  <div class="lv-shell">
    <div
      v-if="showFilterBar"
      class="lv-table-toolbar row items-center q-col-gutter-sm q-mb-sm"
    >
      <q-input
        v-if="filterable"
        v-model="filterText"
        dense
        outlined
        clearable
        debounce="200"
        class="col"
        label="筛选"
        @update:model-value="handleTableFilterUpdate"
      />

      <q-btn-dropdown
        v-if="sortable"
        ref="sortMenuRef"
        flat
        color="primary"
        icon="sort"
        :label="sortLabel"
      >
        <q-list dense>
          <q-item v-for="field in sortFieldsList" :key="field.key" dense>
            <q-item-section>
              <div class="lv-sort-field-label">{{ field.label }}</div>
            </q-item-section>

            <q-item-section side class="lv-sort-actions">
              <q-btn
                flat
                dense
                size="sm"
                label="升序"
                :color="
                  sortBy === field.key && !sortDesc ? 'primary' : 'grey-7'
                "
                @click="applySort(field.key, false)"
              />
              <q-btn
                flat
                dense
                size="sm"
                label="降序"
                :color="sortBy === field.key && sortDesc ? 'primary' : 'grey-7'"
                @click="applySort(field.key, true)"
              />
            </q-item-section>
          </q-item>
        </q-list>
      </q-btn-dropdown>

      <q-btn
        v-if="exportCsv"
        flat
        color="primary"
        icon="download"
        label="导出"
        @click="exportTableCsv"
      />
    </div>

    <template v-if="resolvedViewMode === 'list'">
      <div class="lv-list-shell">
        <q-list
          v-bind="$attrs"
          :bordered="bordered"
          :dense="dense"
          :separator="listSeparator"
          class="lv-list"
        >
          <q-item
            v-for="row in listRows"
            :key="rowIdentity(row)"
            :clickable="clickable"
            :active="isSelected(row)"
            v-ripple="clickable"
            @click="handleRowClick(row)"
          >
            <slot :row="row" :cells="row.cells" :data="toPlainObject(row)" />
          </q-item>

          <q-item v-if="!listRows.length">
            <slot name="empty" :rows="listRows">
              <q-item-section class="text-grey-6">
                {{ emptyText }}
              </q-item-section>
            </slot>
          </q-item>
        </q-list>
      </div>
    </template>

    <template v-else>
      <div class="lv-table-shell">
        <q-table
          v-bind="$attrs"
          :rows="filteredTableRows"
          :columns="resolvedColumns"
          :row-key="resolvedRowKey"
          :filter="filterText"
          :separator="resolvedSeparator"
          :dense="dense"
          :selection="selection"
          :selected="selectedTableRows"
          :pagination="paginationValue"
          :rows-per-page-options="rowsPerPageOptions"
          :hide-pagination="hidePagination"
          :loading="loading"
          class="lv-table"
          @update:filter="handleFilterUpdate"
          @row-click="handleTableRowClick"
          @selection="handleTableSelection"
          @update:selected="handleTableSelectedUpdate"
        >
          <template #no-data>
            <slot name="no-data">
              <slot name="empty">
                <div class="lv-empty-state text-grey-6">
                  {{ emptyText }}
                </div>
              </slot>
            </slot>
          </template>

          <slot v-for="(_, slotName) in $slots" :name="slotName" />
        </q-table>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import type { QTableColumn } from "quasar";
import type { Row, Table } from "@/core";

const props = withDefaults(
  defineProps<{
    table?: Table | null;
    rows?: Row[] | Record<string, any>[];
    columns?: QTableColumn[];
    rowKey?: string;
    viewMode?: "list" | "table";
    dense?: boolean;
    bordered?: boolean;
    separator?: boolean | "horizontal" | "vertical" | "cell" | "none";
    clickable?: boolean;
    selection?: "single" | "multiple" | undefined;
    selectedRowKey?: string | number | null;
    selectedRowKeys?: Array<string | number>;
    selected?:
      Array<string | number | Record<string, any>> | string | number | null;
    filter?: string | null;
    filterable?: boolean;
    sortable?: boolean;
    sortBy?: string | null;
    sortDesc?: boolean;
    sortFields?: Array<{ key: string; label?: string }>;
    showFilterBar?: boolean;
    exportCsv?: boolean;
    exportCsvFileName?: string;
    filterFields?: string[];
    filterCaseSensitive?: boolean;
    rowsPerPageOptions?: number[];
    pagination?: Record<string, unknown>;
    hidePagination?: boolean;
    loading?: boolean;
    emptyText?: string;
  }>(),
  {
    table: null,
    rows: undefined,
    columns: undefined,
    rowKey: "",
    viewMode: "list",
    dense: false,
    bordered: true,
    separator: true,
    clickable: true,
    selection: undefined,
    selectedRowKey: null,
    selectedRowKeys: () => [],
    selected: undefined,
    filter: "",
    filterable: true,
    sortable: true,
    sortBy: "",
    sortDesc: false,
    sortFields: () => [],
    showFilterBar: true,
    exportCsv: false,
    exportCsvFileName: "lv-export.csv",
    filterFields: () => [],
    filterCaseSensitive: false,
    rowsPerPageOptions: () => [10, 20, 50],
    pagination: () => ({ page: 1, rowsPerPage: 10 }),
    hidePagination: false,
    loading: false,
    emptyText: "暂无数据",
  },
);

const emit = defineEmits<{
  "row-click": [row: Row];
  selection: [selection: { rows: Record<string, any>[] }];
  "update:viewMode": [value: "list" | "table"];
  "update:sortBy": [value: string | null];
  "update:sortDesc": [value: boolean];
  "update:selectedRowKey": [value: string | number | null];
  "update:selectedRowKeys": [value: Array<string | number>];
  "update:selected": [
    value:
      Array<string | number | Record<string, any>> | string | number | null,
  ];
  "update:filter": [value: string | null];
}>();

const normalizeRow = (row: any): Row => {
  const rawCells = row?.cells && typeof row.cells === "object" ? row.cells : {};
  const normalized = { ...row, cells: { ...rawCells } } as Row;

  if (!normalized.cells || typeof normalized.cells !== "object") {
    normalized.cells = {};
  }

  for (const [key, value] of Object.entries(row ?? {})) {
    if (
      key === "cells"
      || key === "table"
      || key === "isAdded"
      || key === "index"
    )
      continue;
    if (!normalized.cells[key]) {
      normalized.cells[key] = {
        id: key,
        type: typeof value === "number" ? "Int32" : "String",
        val: value,
        originalVal: value,
      } as any;
    }
  }

  return normalized;
};

const sourceRows = computed<Row[]>(() => {
  const rows =
    props.rows && Array.isArray(props.rows) && props.rows.length > 0
      ? props.rows
      : (props.table?.rows ?? []);
  if (!Array.isArray(rows)) return [];
  return rows.map((row) => normalizeRow(row));
});

const resolvedRowKey = computed(() => props.rowKey || "id");

const toPlainObject = (row: Row): Record<string, unknown> => {
  const cells = row?.cells && typeof row.cells === "object" ? row.cells : {};
  return Object.fromEntries(
    Object.entries(cells).map(([key, cell]) => [key, cell?.val ?? cell]),
  );
};

const getRowCells = (row: Row): Record<string, any> => {
  if (row && row.cells && typeof row.cells === "object") return row.cells;
  return {};
};

const getCellValue = (row: Row, field?: string): unknown => {
  if (!field) return undefined;
  const cells = getRowCells(row);
  const cell =
    cells[field] ?? Object.values(cells).find((item) => item?.id === field);
  return cell?.val ?? cell?.originalVal ?? undefined;
};

const getRowIdentity = (row: any): string | number => {
  const source = row?.__row ?? row;

  if (props.rowKey) {
    const value =
      source?.cells?.[props.rowKey]?.val
      ?? source?.cells?.[props.rowKey]?.originalVal
      ?? source?.[props.rowKey]
      ?? row?.[props.rowKey];
    if (value !== undefined && value !== null) return String(value);
  }

  const rowIndex = source?.index ?? row?.index;
  if (typeof rowIndex === "number") return rowIndex;

  return String(Math.random());
};

const rowIdentity = (row: Row): string | number => getRowIdentity(row);

const isSelected = (row: Row): boolean => {
  const current = getRowIdentity(row);
  const selectedKeys = selectedRowKeyList.value;

  if (props.selection === "multiple") {
    return selectedKeys.some((key) => String(key) === String(current));
  }

  const primary = selectedSingleKey.value;
  return primary !== null && String(current) === String(primary);
};

const selectedSingleKey = computed<string | number | null>(() => {
  if (props.selectedRowKey !== null && props.selectedRowKey !== undefined) {
    return String(props.selectedRowKey) as string | number;
  }

  if (Array.isArray(props.selected) && props.selected.length > 0) {
    const [first] = props.selected;
    if (first === null || first === undefined) return null;
    if (typeof first === "object") return getRowIdentity(first);
    return String(first) as string | number;
  }

  if (
    props.selected !== undefined
    && props.selected !== null
    && !Array.isArray(props.selected)
    && typeof props.selected !== "object"
  ) {
    return String(props.selected) as string | number;
  }

  return null;
});

const selectedRowKeyList = computed<Array<string | number>>(() => {
  if (
    Array.isArray(props.selectedRowKeys)
    && props.selectedRowKeys.length > 0
  ) {
    return props.selectedRowKeys.map((key) => String(key) as string | number);
  }

  if (Array.isArray(props.selected) && props.selected.length > 0) {
    return props.selected
      .map((item) => {
        if (item === null || item === undefined) return null;
        if (typeof item === "object") return getRowIdentity(item);
        return String(item) as string | number;
      })
      .filter(
        (item): item is string | number => item !== null && item !== undefined,
      );
  }

  if (selectedSingleKey.value !== null) {
    return [selectedSingleKey.value];
  }

  return [];
});

const tableRows = computed<Record<string, any>[]>(() =>
  sourceRows.value.map((row) => ({
    ...toPlainObject(row),
    __row: row,
  })),
);

const resolvedColumns = computed<QTableColumn[]>(() => {
  const sourceCols =
    props.columns && props.columns.length > 0
      ? props.columns
      : (props.table?.cols ?? []);

  if (!sourceCols.length) {
    const sampleKeys = Object.keys(tableRows.value[0] ?? {});
    return sampleKeys.map((key) => ({
      name: key,
      label: key,
      field: key,
      sortable: true,
      align: "left",
    }));
  }

  return sourceCols.map((column: any, index: number) => {
    const fieldName =
      column?.field ?? column?.name ?? column?.label ?? `field_${index}`;
    return {
      ...column,
      name: column?.name ?? fieldName,
      field: fieldName,
      label: column?.label ?? column?.name ?? fieldName,
      sortable: column?.sortable ?? true,
      align: column?.align ?? "left",
    } as QTableColumn;
  });
});

const selectedTableRows = computed(() =>
  tableRows.value.filter((row) => {
    const current = getRowIdentity(row.__row ?? row);
    const keys =
      props.selection === "multiple"
        ? selectedRowKeyList.value
        : [selectedSingleKey.value].filter(
            (item): item is string | number =>
              item !== null && item !== undefined,
          );
    return keys.some((key) => String(key) === String(current));
  }),
);

const filterText = ref<string>(String(props.filter ?? ""));

watch(
  () => props.filter,
  (value) => {
    const next = value ?? "";
    if (next !== filterText.value) {
      filterText.value = String(next);
    }
  },
  { immediate: true },
);

const paginationValue = computed<Record<string, unknown>>(() => ({
  ...(props.pagination ?? { page: 1, rowsPerPage: 10 }),
}));

const showFilterBar = computed(
  () =>
    props.showFilterBar
    && (props.filterable || props.exportCsv || props.sortable),
);

const sortFieldsList = computed<Array<{ key: string; label: string }>>(() => {
  const provided = Array.isArray(props.sortFields) ? props.sortFields : [];
  if (provided.length > 0) {
    return provided
      .filter((field) => field && typeof field.key === "string" && field.key)
      .map((field) => ({
        key: field.key,
        label: field.label || field.key,
      }));
  }

  return resolvedColumns.value
    .filter((column) => typeof column.field === "string" && column.field)
    .map((column) => ({
      key: String(column.field),
      label: String(column.label ?? column.name ?? column.field),
    }));
});

const listSortField = computed<string>(() => {
  const direct =
    typeof props.sortBy === "string" && props.sortBy ? props.sortBy : "";
  if (direct) return direct;

  return sortFieldsList.value[0]?.key ?? "";
});

const sortLabel = computed(() => {
  if (!props.sortable) return "排序";
  if (
    props.sortBy
    && sortFieldsList.value.some((field) => field.key === props.sortBy)
  ) {
    const current = sortFieldsList.value.find(
      (field) => field.key === props.sortBy,
    );
    return `${current?.label ?? props.sortBy}${props.sortDesc ? " ↓" : " ↑"}`;
  }
  return "排序";
});

const tableFilterFields = computed<string[]>(() => {
  if (Array.isArray(props.filterFields) && props.filterFields.length > 0) {
    return props.filterFields;
  }

  return resolvedColumns.value
    .map((column) => (typeof column.field === "string" ? column.field : ""))
    .filter(Boolean) as string[];
});

const getFieldRawValue = (row: Record<string, any>, field: string): unknown => {
  const path = String(field).split(".");
  let value: any = row;
  for (const key of path) {
    if (value == null) return "";
    value = value[key];
  }
  return value;
};

const normalizeComparableValue = (value: unknown): number | string => {
  if (value instanceof Date && !Number.isNaN(value.getTime())) {
    return value.getTime();
  }

  if (typeof value === "number" && Number.isFinite(value)) {
    return value;
  }

  if (typeof value === "boolean") {
    return value ? 1 : 0;
  }

  if (typeof value === "string") {
    const text = value.trim();
    if (!text) return "";

    const normalizedDate = new Date(text);
    if (!Number.isNaN(normalizedDate.getTime())) {
      return normalizedDate.getTime();
    }

    const normalizedNumber = Number(text.replace(/,/g, ""));
    if (Number.isFinite(normalizedNumber)) {
      return normalizedNumber;
    }

    return text.toLowerCase();
  }

  if (value == null) return "";
  return String(value).toLowerCase();
};

const listRows = computed<Row[]>(() => {
  const rawFilter = (filterText.value ?? "").trim();
  const rows = [...sourceRows.value];

  const filtered = rawFilter
    ? rows.filter((row) => {
        const record = toPlainObject(row);
        return tableFilterFields.value.some((field) => {
          const value = getFieldRawValue(record, field);
          const text = value == null ? "" : String(value);
          const normalized = props.filterCaseSensitive
            ? text
            : text.toLowerCase();
          return normalized.includes(
            props.filterCaseSensitive ? rawFilter : rawFilter.toLowerCase(),
          );
        });
      })
    : rows;

  const sortField = listSortField.value;
  if (!props.sortable || !sortField) return filtered;

  return filtered.sort((left, right) => {
    const leftValue = getCellValue(left, sortField) ?? "";
    const rightValue = getCellValue(right, sortField) ?? "";

    const leftComparable = normalizeComparableValue(leftValue);
    const rightComparable = normalizeComparableValue(rightValue);

    if (
      typeof leftComparable === "number"
      && typeof rightComparable === "number"
    ) {
      return props.sortDesc
        ? rightComparable - leftComparable
        : leftComparable - rightComparable;
    }

    const leftText = String(leftComparable);
    const rightText = String(rightComparable);
    const result = leftText.localeCompare(rightText);
    return props.sortDesc ? -result : result;
  });
});

const filteredTableRows = computed<Record<string, any>[]>(() => {
  const rawFilter = (filterText.value ?? "").trim();
  if (!rawFilter) return tableRows.value;

  const query = props.filterCaseSensitive ? rawFilter : rawFilter.toLowerCase();

  return tableRows.value.filter((row) => {
    const record = row.__row ? toPlainObject(row.__row) : row;
    return tableFilterFields.value.some((field) => {
      const value = getFieldRawValue(record, field);
      const text = value == null ? "" : String(value);
      const normalized = props.filterCaseSensitive ? text : text.toLowerCase();
      return normalized.includes(query);
    });
  });
});

const exportTableCsv = () => {
  const columns = resolvedColumns.value;
  const rows =
    resolvedViewMode.value === "list"
      ? listRows.value.map((row) => toPlainObject(row))
      : filteredTableRows.value.map((row) =>
          row.__row ? toPlainObject(row.__row) : row,
        );

  const fields = columns
    .map((column) => String(column.field ?? column.name ?? ""))
    .filter(Boolean);

  const header = fields
    .map((field) => {
      const column = columns.find(
        (item) => String(item.field ?? item.name ?? "") === field,
      );
      return `"${String(column?.label ?? column?.name ?? field ?? "").replace(/"/g, '""')}"`;
    })
    .join(",");

  const body = rows
    .map((record) =>
      fields
        .map((field) => {
          const value = field ? getFieldRawValue(record, field) : "";
          const text = value == null ? "" : String(value).replace(/"/g, '""');
          return `"${text}"`;
        })
        .join(","),
    )
    .join("\n");

  const csv = `${header}\n${body}`.trim();
  const bom = new Uint8Array([0xef, 0xbb, 0xbf]);
  const blob = new Blob([bom, csv], { type: "text/csv;charset=utf-8;" });
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = props.exportCsvFileName || "lv-export.csv";
  document.body.appendChild(link);
  link.click();
  link.remove();
  URL.revokeObjectURL(url);
};

const handleRowClick = (row: Row) => {
  const value = getRowIdentity(row);
  const nextValue =
    props.selection === "multiple"
      ? (() => {
          const list = selectedRowKeyList.value;
          const exists = list.some((item) => String(item) === String(value));
          return exists
            ? list.filter((item) => String(item) !== String(value))
            : [...list, value];
        })()
      : value;

  if (props.selection === "multiple") {
    emit("update:selectedRowKeys", nextValue as Array<string | number>);
    emit("update:selected", nextValue as Array<string | number>);
  } else {
    emit("update:selectedRowKey", nextValue as string | number | null);
    emit("update:selected", nextValue as string | number | null);
  }

  emit("row-click", row);
};

const resolvedSeparator = computed(() => {
  if (props.separator === false || props.separator === "none") return "none";
  if (props.separator === "vertical" || props.separator === "cell")
    return props.separator;
  return "horizontal";
});

const listSeparator = computed(() =>
  props.separator === false || props.separator === "none" ? false : true,
);

const sortMenuRef = ref<any>(null);

const applySort = (field: string, desc: boolean) => {
  emit("update:sortBy", field);
  emit("update:sortDesc", desc);
  sortMenuRef.value?.hide?.();
};

const toggleListSort = () => {
  if (!listSortField.value) return;
  emit("update:sortBy", listSortField.value);
  emit("update:sortDesc", !props.sortDesc);
};

const handleTableFilterUpdate = (value: string | null) => {
  filterText.value = value ?? "";
  emit("update:filter", filterText.value || null);
};

const handleFilterUpdate = (value: string | null) => {
  handleTableFilterUpdate(value);
};

const handleTableRowClick = (_evt: Event, row: Record<string, any>) => {
  const sourceRow = row?.__row ?? row;
  handleRowClick(sourceRow as Row);
};

const handleTableSelection = (details: {
  rows: readonly Record<string, any>[];
  keys: readonly unknown[];
  added: boolean;
  evt: Event;
}) => {
  const rows = Array.isArray(details?.rows) ? [...details.rows] : [];
  const keys = rows.map((row) => getRowIdentity(row.__row ?? row));

  emit("selection", { rows });

  if (props.selection === "multiple") {
    emit("update:selectedRowKeys", keys);
    emit("update:selected", keys);
    return;
  }

  const next = keys[0] ?? null;
  emit("update:selectedRowKey", next as string | number | null);
  emit("update:selected", next as string | number | null);
};

const handleTableSelectedUpdate = (
  newSelected: readonly Record<string, any>[] | null,
) => {
  const rows = Array.isArray(newSelected) ? [...newSelected] : [];
  const keys = rows.map((row) => getRowIdentity(row.__row ?? row));

  if (props.selection === "multiple") {
    emit("update:selectedRowKeys", keys);
    emit("update:selected", keys);
    return;
  }

  const next = keys[0] ?? null;
  emit("update:selectedRowKey", next as string | number | null);
  emit("update:selected", next as string | number | null);
};

const resolvedViewMode = computed(() => props.viewMode ?? "list");

const viewMode = computed({
  get: () => resolvedViewMode.value,
  set: (value) => emit("update:viewMode", value),
});
</script>

<style scoped>
.lv-shell {
  display: flex;
  flex-direction: column;
  width: 100%;
  max-width: 100%;
  flex: 1 1 auto;
  min-height: 0;
  height: 100%;
  min-width: 0;
  overflow: hidden;
}

.lv-list-shell {
  display: flex;
  flex: 1 1 auto;
  min-height: 0;
  width: 100%;
  max-width: 100%;
  height: 100%;
  min-width: 0;
  overflow: hidden;
}

.lv-list {
  width: 100%;
  max-width: 100%;
  flex: 1 1 auto;
  min-height: 0;
  min-width: 0;
  height: 100%;
  overflow-y: auto;
  overflow-x: hidden;
}

.lv-table-shell {
  display: flex;
  flex-direction: column;
  width: 100%;
  max-width: 100%;
  flex: 1 1 auto;
  min-height: 0;
  height: 100%;
  overflow: hidden;
}

.lv-table-toolbar {
  width: 100%;
  gap: 8px;
  flex: 0 0 auto;
}

.lv-table {
  width: 100%;
  max-width: 100%;
  flex: 1 1 auto;
  min-height: 0;
}
.lv-sort-field-label {
  font-size: 0.9rem;
  color: #374151;
}

.lv-sort-actions {
  display: flex;
  align-items: center;
  gap: 4px;
}
:deep(.q-table__container) {
  flex: 1 1 auto;
  max-height: 100%;
  height: 100%;
  overflow-y: auto;
  overflow-x: auto;
}

:deep(.q-list) {
  width: 100%;
  max-width: 100%;
  overflow: hidden;
}

:deep(.q-item) {
  width: 100%;
  max-width: 100%;
  overflow: hidden;
}

:deep(.q-item__section) {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
</style>
