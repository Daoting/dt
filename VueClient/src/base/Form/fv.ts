import { ref, type Ref, type InjectionKey } from "vue";

export interface FormCellContext<T extends Record<string, any>> {
  setCellValue: (field: keyof T, value: T[keyof T]) => void;
  getCell: (field: keyof T) => {
    originalValue: unknown;
    currentValue: unknown;
    changed: boolean;
  };
}

export const FORM_CELL_CONTEXT_KEY: InjectionKey<FormCellContext<any>> = Symbol(
  "fv-form-cell-context",
);

export interface CellSnapshot<TValue> {
  originalValue: TValue | undefined;
  currentValue: TValue | undefined;
  changed: boolean;
}

export interface FormCellController<T extends Record<string, any>> {
  originalValues: Ref<Partial<T>>;
  currentValues: Ref<Partial<T>>;
  getCell: (field: keyof T) => CellSnapshot<T[keyof T]>;
  getOriginalValue: (field: keyof T) => T[keyof T] | undefined;
  getCurrentValue: (field: keyof T) => T[keyof T] | undefined;
  setCellValue: (field: keyof T, value: T[keyof T]) => void;
  setCurrentValues: (values: Partial<T>) => void;
  syncInitialValues: (values: Partial<T>) => void;
  commitCells: () => void;
  rollbackCell: (field: keyof T) => void;
  rollbackAll: () => void;
  getChangedCells: () => Partial<T>;
}

function cloneValue<T>(value: T): T {
  if (value === null || value === undefined) return value;

  if (Array.isArray(value) || typeof value === "object") {
    try {
      // 不丢失日期类型
      const json = JSON.stringify(value, (key, val) => {
        // 关键点：this[key] 才是原始值，val 已被转换为字符串
        const original = value[key];
        if (original instanceof Date) {
          return { $type: "Date", value: original.toISOString() };
        }
        return val;
      });

      return JSON.parse(json, (key, value) => {
        if (value?.$type === "Date") {
          return new Date(value.value);
        }
        return value;
      });
    } catch {
      return value;
    }
  }

  return value;
}

function asPartial<T extends Record<string, any>>(
  value: Partial<T>,
): Partial<T> {
  return value as Partial<T>;
}

export function createFormCellController<T extends Record<string, any>>(
  initialValues: Partial<T>,
): FormCellController<T> {
  const originalValues = ref<Partial<T>>(
    asPartial<T>(cloneValue(initialValues)),
  ) as Ref<Partial<T>>;
  const currentValues = ref<Partial<T>>(
    asPartial<T>(cloneValue(initialValues)),
  ) as Ref<Partial<T>>;

  const getCell = (field: keyof T): CellSnapshot<T[keyof T]> => {
    const original = originalValues.value[field];
    const current = currentValues.value[field];
    return {
      originalValue: original as T[keyof T] | undefined,
      currentValue: current as T[keyof T] | undefined,
      changed: current !== original,
    };
  };

  const getOriginalValue = (field: keyof T) => getCell(field).originalValue;
  const getCurrentValue = (field: keyof T) => getCell(field).currentValue;

  const setCellValue = (field: keyof T, value: T[keyof T]) => {
    currentValues.value = {
      ...currentValues.value,
      [field]: value,
    };
  };

  const setCurrentValues = (values: Partial<T>) => {
    currentValues.value = {
      ...currentValues.value,
      ...cloneValue(values),
    };
  };

  const syncInitialValues = (values: Partial<T>) => {
    const next = cloneValue(values);
    originalValues.value = next;
    currentValues.value = next;
  };

  const commitCells = () => {
    originalValues.value = cloneValue(currentValues.value);
  };

  const rollbackCell = (field: keyof T) => {
    const original = originalValues.value[field];
    currentValues.value = {
      ...currentValues.value,
      [field]: original as T[keyof T],
    };
  };

  const rollbackAll = () => {
    currentValues.value = cloneValue(originalValues.value);
  };

  const isDate = (val: unknown): val is Date => {
    if (Object.prototype.toString.call(val) !== "[object Date]") {
      return false;
    }
    return !Number.isNaN((val as Date).getTime());
  }

  const getChangedCells = (): Partial<T> => {
    const changed: Partial<T> = {};
    for (const key of Object.keys(currentValues.value) as Array<keyof T>) {
      const cur = currentValues.value[key];
      const orig = originalValues.value[key];
      // 处理日期类型的比较
      if (isDate(cur) && isDate(orig)) {
        if (cur.getTime() !== (orig as Date).getTime()) {
          changed[key] = cur as T[keyof T];
        }
      } else if (cur !== orig) {
        changed[key] = cur as T[keyof T];
      }
    }
    return changed;
  };

  const controller: FormCellController<T> = {
    originalValues,
    currentValues,
    getCell,
    getOriginalValue,
    getCurrentValue,
    setCellValue,
    setCurrentValues,
    syncInitialValues,
    commitCells,
    rollbackCell,
    rollbackAll,
    getChangedCells,
  };

  return controller;
}
