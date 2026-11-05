<template>
  <q-field v-bind="$attrs">
    <template #prepend v-if="icon">
      <q-icon :name="icon" />
    </template>

    <template #control>
      <div
        class="self-center full-width no-outline"
        :style="color ? { color } : {}"
        tabindex="0"
      >
        {{ Info }}
      </div>
    </template>
  </q-field>
</template>

<script setup lang="ts">
const props = withDefaults(
  defineProps<{
    icon?: string;
    modelValue?: string;
    /** 复用 Quasar 风格 color 属性，控制文字颜色 */
    color?: string;
  }>(),
  {
    icon: "",
    modelValue: "",
  },
);

const attrs = useAttrs();

// 剔除 color，剩下的属性透传给 q-field，防止 q-field 也拿到 color
const restAttrs = { ...attrs };
delete restAttrs.color;

const Info = computed(() => props.modelValue ?? "");
</script>
