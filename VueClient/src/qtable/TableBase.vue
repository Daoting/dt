<template>
  <div class="q-pa-md">
          <q-table title="Treats" :rows="rows" :columns="columns" row-key="name" />
        </div>
</template>

<script setup lang="ts">
import { Kit, Table, Dict } from "../core/index";

// 你这里有 await，所以必须用 Suspense
const table = await Kit.rpc<Table>("cm", "TestSerialize.GetTable");
const columns = table.cols;
const rows = table.rows;
</script>