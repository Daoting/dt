<template>
  <q-page padding>
    <div class="lv-demo q-gutter-md column">
      <q-card flat bordered>
        <q-card-section class="row items-center justify-between">
          <div class="text-h6">Lv 示例：{{ viewMode === 'list' ? 'list' : 'table' }}</div>

          <div class="row items-center q-gutter-sm">
            <q-btn-toggle
              v-model="viewMode"
              :options="[
                { label: '列表', value: 'list' },
                { label: '表格', value: 'table' },
              ]"
              color="primary"
              dense
              no-caps
            />

            <q-chip color="primary" text-color="white" dense>
              {{ viewMode === 'table' ? `已选 ${selectedIds.length} 项` : `当前选中：${selectedId ?? '无'}` }}
            </q-chip>
          </div>
        </q-card-section>

        <q-separator />

        <q-card-section class="q-gutter-sm">
          <div class="row q-col-gutter-sm items-center">
            <q-input
              v-model="filterText"
              dense
              outlined
              clearable
              class="col"
              label="关键字筛选"
            />

            <q-select
              v-model="statusFilter"
              :options="statusOptions"
              dense
              outlined
              label="状态筛选"
              class="col-md-3 col-sm-5 col-12"
            />
          </div>
        </q-card-section>

        <q-card-section>
          <Lv
            v-model:view-mode="viewMode"
            :rows="filteredRows"
            row-key="id"
            :selection="viewMode === 'table' ? 'multiple' : undefined"
            :selected-row-key="selectedId"
            :selected-row-keys="selectedIds"
            :filter="filterText"
            :filter-fields="['name', 'city', 'status']"
            :sortable="true"
            :sort-fields="sortFields"
            :sort-by="sortBy"
            :sort-desc="sortDesc"
            :export-csv="true"
            export-csv-file-name="lv-demo-export.csv"
            empty-text="暂无数据"
            @update:sort-by="sortBy = $event ?? 'name'"
            @update:sort-desc="sortDesc = !!$event"
            @update:selected-row-key="selectedId = $event"
            @update:selected-row-keys="selectedIds = $event"
            @row-click="onRowClick"
          >
            <template #default="{ row }">
              <q-item-section avatar>
                <q-avatar color="primary" text-color="white" size="32">
                  {{ String(getCellValue(row, 'name') ?? '·').charAt(0).toUpperCase() }}
                </q-avatar>
              </q-item-section>

              <q-item-section>
                <q-item-label class="text-subtitle2">
                  {{ getCellValue(row, 'name') || '—' }}
                </q-item-label>
                <q-item-label caption>
                  <span>{{ getCellValue(row, 'city') }}</span>
                  <span>·</span>
                  <span>{{ getCellValue(row, 'status') }}</span>
                </q-item-label>
              </q-item-section>

              <q-item-section side>
                <div class="side-box">
                  <div class="score">{{ getCellValue(row, 'score') }}</div>
                  <div class="small">{{ getCellValue(row, 'age') }}岁</div>
                </div>
              </q-item-section>
            </template>

            <template #empty>
              <q-item-section class="empty-state">
                <div class="empty-state__icon">📭</div>
                <div>暂无数据，换个筛选条件试试</div>
              </q-item-section>
            </template>
          </Lv>
        </q-card-section>
      </q-card>
    </div>
  </q-page>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import Lv from "@/base/ListView/Lv.vue";

const rows = ref([
  { id: 1, name: "Alice", city: "北京", status: "在线", age: 24, score: 98 },
  { id: 2, name: "Bob", city: "上海", status: "离线", age: 31, score: 85 },
  { id: 3, name: "Carol", city: "深圳", status: "在线", age: 27, score: 92 },
  { id: 4, name: "David", city: "广州", status: "休息", age: 29, score: 88 },
  { id: 5, name: "Ella", city: "杭州", status: "在线", age: 26, score: 95 },
]);

const viewMode = ref<"list" | "table">("list");
const selectedId = ref<number | string | null>(1);
const selectedIds = ref<Array<number | string>>([1, 3]);
const filterText = ref("");
const statusFilter = ref("全部");
const statusOptions = ["全部", "在线", "离线", "休息"];
const sortBy = ref("name");
const sortDesc = ref(false);
const sortFields = [
  { key: "id", label: "ID" },
  { key: "name", label: "姓名" },
  { key: "city", label: "城市" },
  { key: "status", label: "状态" },
  { key: "age", label: "年龄" },
  { key: "score", label: "分数" },
];

const filteredRows = computed(() => {
  const keyword = filterText.value.trim().toLowerCase();
  const byStatus = statusFilter.value === "全部"
    ? rows.value
    : rows.value.filter((row) => row.status === statusFilter.value);

  if (!keyword) return byStatus;

  return byStatus.filter((row) =>
    ["name", "city", "status"].some((field) =>
      String(row[field] ?? "").toLowerCase().includes(keyword),
    ),
  );
});

const getCellValue = (row: any, field: string) => {
  const value = row?.cells?.[field]?.val ?? row?.[field];
  return value ?? "—";
};

const onRowClick = (row: any) => {
  selectedId.value = getCellValue(row, "id");
  console.log("row click", row);
};
</script>

<style scoped>
.lv-demo {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.sub-line {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #666;
  font-size: 12px;
}

.side-box {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 4px;
  min-width: 72px;
}

.score {
  font-size: 18px;
  font-weight: 600;
  color: #1976d2;
}

.small {
  font-size: 12px;
  color: #888;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 20px 12px;
  color: #666;
  text-align: center;
}

.empty-state__icon {
  font-size: 28px;
  opacity: 0.8;
}
</style>
