<template>
  <q-breadcrumbs>
    <q-breadcrumbs-el
      v-for="(item, idx) in breadcrumbs"
      :key="idx"
      :to="item.to"
      :disable="idx === breadcrumbs.length - 1"
    >
      <!-- 如果有 icon 就显示 -->
      <q-icon v-if="item.icon" :name="item.icon as string" class="q-mr-xs" />

      {{ item.label }}
    </q-breadcrumbs-el>
  </q-breadcrumbs>
</template>

<script setup lang="ts">
import { useRoute } from "vue-router";
import { computed } from "vue";

const route = useRoute();

const breadcrumbs = computed(() => {
  return route.matched
    //.filter((route) => route.path !== "/")
    .map((r) => ({
      to: { path: r.path },
      label: (r.meta?.title as string) || r.name || "未命名",
      icon: r.meta?.icon,
    }));
});
</script>
