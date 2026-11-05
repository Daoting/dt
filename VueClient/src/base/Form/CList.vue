<template>
  <q-select
    v-model="fieldValue"
    :label="label"
    :options="options"
    :option-label="optionLabel"
    :option-value="optionValue"
    :multiple="multiple"
    :emit-value="true"
    :map-options="true"
    :placeholder="placeholder"
    :dense="dense"
    :outlined="outlined"
    :clearable="clearable"
    :error="!!errorMessage"
    :error-message="errorMessage"
    v-bind="$attrs"
  />
</template>

<script setup lang="ts">
import type { QSelectProps } from "quasar";
import { useField } from "vee-validate";
import { FORM_CELL_CONTEXT_KEY, type FormCellContext } from "./fv";

const props = withDefaults(
  defineProps<{
    id: string;
    modelValue?: string | number | string[] | number[] | null | undefined;
    label?: string;
    options?: QSelectProps["options"];
    optionLabel?: string;
    optionValue?: string;
    multiple?: boolean;
    placeholder?: string;
    dense?: boolean;
    outlined?: boolean;
    clearable?: boolean;
    initialValue?: unknown;
    rules?: unknown;
  }>(),
  {
    label: "",
    options: undefined,
    optionLabel: "label",
    optionValue: "value",
    multiple: false,
    placeholder: "",
    dense: false,
    outlined: true,
    clearable: false,
    initialValue: undefined,
    rules: undefined,
  },
);

const emit = defineEmits<{
  (e: "update:modelValue", value: string | number | string[] | number[] | null | undefined): void;
}>();

const formContext = inject(
  FORM_CELL_CONTEXT_KEY,
  null as FormCellContext<any> | null,
);

const { value, errorMessage, handleChange } = useField(
  props.id,
  props.rules as any,
  {
    initialValue: props.initialValue,
  },
);

const syncFromContext = () => {
  try {
    const cell = formContext?.getCell?.(props.id as any);
    const candidate = cell?.currentValue ?? cell?.originalValue;
    const normalized = normalizeValue(candidate);
    if (normalized !== value.value) {
      handleChange(normalized as never);
    }
  } catch (e) {
    // noop
  }
};

onMounted(() => {
  syncFromContext();
});

const normalizeValue = (raw: unknown) => {
  if (Array.isArray(raw)) {
    return raw.map((item) => normalizeValue(item));
  }

  if (raw && typeof raw === "object" && props.optionValue) {
    return (raw as Record<string, unknown>)[props.optionValue];
  }

  return raw;
};

const fieldValue = computed<
  string | number | string[] | number[] | null | undefined
>({
  get: () =>
    value.value as string | number | string[] | number[] | null | undefined,
  set: (raw) => {
    const normalized = normalizeValue(raw);
    handleChange(normalized as never);
    formContext?.setCellValue?.(props.id, normalized as never);
    emit("update:modelValue", normalized as never);
  },
});

watch(
  () => props.modelValue,
  (newValue) => {
    if (newValue !== value.value) {
      syncFromContext();
    }
  },
  { immediate: true },
);

watch(
  () => value.value,
  (newValue) => {
    const normalized = normalizeValue(newValue);
    formContext?.setCellValue?.(props.id, normalized as never);
  },
  { immediate: true },
);

defineExpose({
  fieldValue,
  value,
  errorMessage,
});
</script>
