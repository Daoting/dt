<template>
  <q-toolbar class="q-pa-sm bg-grey-2">
    <Routes v-if="showRoutes" />
    <q-space />

    <!-- 右侧按钮：自动全部 flat -->
    <div class="row items-center gap-2">
      <template v-for="(node, idx) in allChildren" :key="idx">
        <!-- 1. QBtn：原生 flat -->
        <component
          v-if="(node.type as { name?: string })?.name === 'QBtn'"
          :is="node"
          v-bind="node.props"
          flat
        />

        <!-- 2. QBtnDropdown：不加 flat，给一个统一样式类 -->
        <component
          v-else-if="(node.type as { name?: string })?.name === 'QBtnDropdown'"
          :is="node"
          v-bind="node.props"
          class="btn-as-flat"
          unelevated
        />

        <!-- 3. 其他元素原样 -->
        <component v-else :is="node" v-bind="node.props" />
      </template>
    </div>
  </q-toolbar>
</template>

<script setup lang="ts">
import { useSlots, computed } from "vue";

const slots = useSlots();

// 控制是否显示面包屑：默认 true 显示
const props = withDefaults(
  defineProps<{
    showRoutes?: boolean;
  }>(),
  {
    showRoutes: true,
  },
);

const allChildren = computed(() => {
  return slots.default?.() ?? [];
});
</script>

<style scoped>
:deep(.btn-as-flat) {
  background: transparent !important;
  border: none !important;
  box-shadow: none !important;
  padding: 0 12px !important;
  min-height: 36px !important;
}
</style>
