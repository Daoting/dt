<template>
  <q-input
    v-model="fieldValue"
    :label="label"
    :type="type"
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
import type { QInputProps } from "quasar";
import { useField } from "vee-validate";
import { FORM_CELL_CONTEXT_KEY, type FormCellContext } from "./fv";

const props = withDefaults(
  defineProps<{
    id: string;
    modelValue?: string | number | FileList | null | undefined;
    label?: string;
    type?: QInputProps["type"];
    placeholder?: string;
    dense?: boolean;
    outlined?: boolean;
    clearable?: boolean;
    initialValue?: unknown;
    rules?: unknown;
  }>(),
  {
    label: "",
    type: "text" as QInputProps["type"],
    placeholder: "",
    dense: false,
    outlined: true,
    clearable: false,
    initialValue: undefined,
    rules: undefined,
  },
);

const emit = defineEmits<{
  (e: "update:modelValue", value: string | number | FileList | null | undefined): void;
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
    const normalized = props.type === "number" ? Number(candidate) : candidate;
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

const fieldValue = computed<string | number | FileList | null | undefined>({
  get: () =>
    (value.value as string | number | FileList | null | undefined) ?? "",
  set: (raw: string | number | FileList | null | undefined) => {
    const normalized = props.type === "number" ? Number(raw) : raw;
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
  (newValue: unknown) => {
    const normalized =
      props.type === "number"
        ? Number(newValue)
        : (newValue as string | number | FileList | null | undefined);
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
