<template>
  <div ref="wrapperRef">
    <q-input
      v-model="displayValue"
      :label="label"
      :placeholder="placeholder"
      :dense="dense"
      :outlined="outlined"
      :clearable="clearable"
      :readonly="true"
      :error="!!errorMessage"
      :error-message="errorMessage"
      v-bind="$attrs"
      @click="togglePopup"
    >
      <template #append>
        <q-icon
          :id="iconName"
          class="cursor-pointer"
          @click.stop="togglePopup"
        />
      </template>
    </q-input>

    <q-menu
      v-model="showPopup"
      :target="wrapperRef"
      :disable-parent-pointer-events="false"
      class="cdate-popup"
    >
      <q-card class="q-pa-sm" style="min-width: 300px">
        <q-date
          v-if="mode !== 'time'"
          v-model="dateValue"
          :mask="dateMask"
          :first-day-of-week="firstDayOfWeek"
          :minimal="false"
          @update:model-value="onPartChange"
        />
        <q-separator v-if="mode === 'datetime'" />
        <q-time
          v-if="mode !== 'date'"
          v-model="timeValue"
          :mask="timeMask"
          @update:model-value="onPartChange"
        />
        <div class="row justify-end q-mt-sm">
          <q-btn
            label="确定"
            color="primary"
            size="sm"
            @click="confirm"
          />
          <q-btn
            label="取消"
            size="sm"
            class="q-ml-xs"
            @click="showPopup = false"
          />
        </div>
      </q-card>
    </q-menu>
  </div>
</template>

<script setup lang="ts">
import { useField } from "vee-validate";
import { FORM_CELL_CONTEXT_KEY, type FormCellContext } from "./fv";

const props = withDefaults(
  defineProps<{
    id: string;
    modelValue?: string | Date;
    label?: string;
    mode?: "date" | "time" | "datetime";
    dateMask?: string;
    timeMask?: string;
    firstDayOfWeek?: string | number;
    placeholder?: string;
    dense?: boolean;
    outlined?: boolean;
    clearable?: boolean;
    initialValue?: unknown;
    rules?: unknown;
  }>(),
  {
    label: "",
    mode: "date",
    dateMask: "YYYY-MM-DD",
    timeMask: "HH:mm",
    firstDayOfWeek: 1,
    placeholder: "",
    dense: false,
    outlined: true,
    clearable: false,
    initialValue: undefined,
    rules: undefined,
  },
);

const emit = defineEmits<{
  (e: "update:modelValue", value: string | Date): void;
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
    if (candidate !== undefined && candidate !== null) {
      parseValue(candidate);
      handleChange(candidate as never);
    }
  } catch (e) {
    // noop
  }
};

onMounted(() => {
  syncFromContext();
});

const wrapperRef = ref();
const showPopup = ref(false);

const dateValue = ref<string>("");
const timeValue = ref<string>("");

const iconName = computed(() => {
  if (props.mode === "time") return "access_time";
  return "event";
});

const parseValue = (raw: unknown) => {
  if (raw === null || raw === undefined || raw === "") {
    dateValue.value = "";
    timeValue.value = "";
    return;
  }

  let str: string;
  if (raw instanceof Date) {
    const dateStr = raw.toISOString().substring(0, 10);
    const timeStr = raw.toTimeString().substring(0, 5);
    if (props.mode === "date") {
      str = dateStr;
    } else if (props.mode === "time") {
      str = timeStr;
    } else {
      str = `${dateStr} ${timeStr}`;
    }
  } else {
    str = String(raw);
  }

  if (props.mode === "date") {
    dateValue.value = str;
    timeValue.value = "";
  } else if (props.mode === "time") {
    dateValue.value = "";
    timeValue.value = str;
  } else {
    const parts = str.split(" ");
    dateValue.value = parts[0] ?? "";
    timeValue.value = parts[1] ?? "";
  }
};

const combineValue = (): string => {
  if (props.mode === "date") return dateValue.value;
  if (props.mode === "time") return timeValue.value;
  return [dateValue.value, timeValue.value].filter(Boolean).join(" ");
};

const displayValue = computed<string>({
  get: () => {
    if (!dateValue.value && !timeValue.value) return "";
    if (props.mode === "date") return dateValue.value;
    if (props.mode === "time") return timeValue.value;
    return [dateValue.value, timeValue.value].filter(Boolean).join(" ");
  },
  set: (val: string) => {
    if (!val) {
      dateValue.value = "";
      timeValue.value = "";
      commit();
    }
  },
});

const commit = () => {
  const combined = combineValue();
  let output: string | Date = combined;

  if (props.mode === "date") {
    output = dateValue.value ? new Date(dateValue.value) : combined;
  } else if (props.mode === "datetime") {
    output =
      dateValue.value && timeValue.value
        ? new Date(`${dateValue.value}T${timeValue.value}`)
        : combined;
  }

  handleChange(output as never);
  formContext?.setCellValue?.(props.id, output as never);
  emit("update:modelValue", output as never);
};

const onPartChange = () => {
  if (props.mode !== "datetime") {
    commit();
    showPopup.value = false;
  }
};

const confirm = () => {
  commit();
  showPopup.value = false;
};

const togglePopup = () => {
  parseValue(value.value);
  showPopup.value = !showPopup.value;
};

parseValue(value.value);

watch(
  () => value.value,
  (newValue) => {
    parseValue(newValue);
    formContext?.setCellValue?.(props.id, newValue as never);
  },
  { immediate: true },
);

watch(showPopup, (open) => {
  if (open) {
    nextTick(() => parseValue(value.value));
  }
});

defineExpose({
  value,
  errorMessage,
});
</script>