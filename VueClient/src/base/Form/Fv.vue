<template>
  <q-form
    ref="formRef"
    v-bind="$attrs"
    class="col scroll-y"
    @submit.prevent="handleSubmit(onFormSubmit)"
  >
    <slot />
  </q-form>
</template>

<script setup lang="ts" generic="T extends Record<string, unknown>">
import { useForm } from "vee-validate";
import { toTypedSchema } from "@vee-validate/zod";
import type { ZodType } from "zod";
import type { QForm } from "quasar";
import { createFormCellController, FORM_CELL_CONTEXT_KEY } from "./fv";
import type { QDialogOptions } from "quasar";

const props = defineProps<{
  modelValue: T;
  schema: ZodType<T>;
  submit: (data: unknown) => Promise<void> | void;
  confirm?: boolean;
  confirmText?: string;
  resetAfterSubmit?: boolean;
}>();

const typedSchema = computed(() => toTypedSchema(props.schema));

const { handleSubmit } = useForm({
  initialValues: props.modelValue,
  validationSchema: typedSchema,
});

const emit = defineEmits<{
  "submit-success": [];
  "submit-fail": [error: unknown];
  "before-submit": [];
  "update:modelValue": [value: T];
}>();

const formRef = ref<QForm | null>(null);
const $q = useQuasar();
const cellController = createFormCellController<T>(props.modelValue as T);

const syncParentData = () => {
  emit("update:modelValue", {
    ...(props.modelValue as T),
    ...cellController.currentValues.value,
  } as T);
};

watch(
  () => props.modelValue,
  (nextData) => {
    if (!nextData || typeof nextData !== "object") return;
    Object.entries(nextData).forEach(([key, value]) => {
      cellController.setCellValue(key as keyof T, value as T[keyof T]);
    });
  },
  { immediate: true, deep: true },
);

provide(FORM_CELL_CONTEXT_KEY, {
  setCellValue: (field, value) => {
    const key = field as keyof T;
    cellController.setCellValue(key, value as T[keyof T]);
    syncParentData();
  },
  getCell: (field) => cellController.getCell(field as keyof T),
});

const showConfirm = (opts: QDialogOptions): Promise<boolean> => {
  return new Promise((resolve) => {
    $q.dialog(opts)
      .onOk(() => resolve(true))
      .onCancel(() => resolve(false));
  });
};

const onFormSubmit = async (payload: T) => {
  emit("before-submit");

  if (props.confirm) {
    const ok = await showConfirm({
      title: "确认",
      message: props.confirmText ?? "确定提交吗？",
      ok: { label: "确定" },
      cancel: { label: "取消" },
    });
    if (!ok) {
      return;
    }
  }

  try {
    cellController.commitCells();
    await props.submit(props.modelValue);
    emit("submit-success");
  } catch (err) {
    emit("submit-fail", err);
  }
};

defineExpose({
  formRef,
  setFieldValue: (field: string, fieldValue: unknown) => {
    cellController.setCellValue(field as keyof T, fieldValue as T[keyof T]);
    syncParentData();
  },
  setValues: (nextValues: T) => {
    cellController.setCurrentValues(nextValues);
    syncParentData();
  },
  cells: cellController,
  getCell: cellController.getCell,
  getOriginalValue: cellController.getOriginalValue,
  getCurrentValue: cellController.getCurrentValue,
  setCellValue: cellController.setCellValue,
  setCurrentValues: cellController.setCurrentValues,
  syncInitialValues: cellController.syncInitialValues,
  commitCells: cellController.commitCells,
  rollbackCell: cellController.rollbackCell,
  rollbackAll: cellController.rollbackAll,
  getChangedCells: cellController.getChangedCells,
  submit: handleSubmit(onFormSubmit),
});
</script>
