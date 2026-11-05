import type { ComponentPublicInstance } from "vue";

interface UserForm {
  username: string;
  age: number;
}

export interface FvExpose extends ComponentPublicInstance {
  getCell?: (field: keyof UserForm) => {
    originalValue: unknown;
    currentValue: unknown;
    changed: boolean;
  };
  setCellValue?: (field: keyof UserForm, value: unknown) => void;
  getChangedCells?: () => Partial<UserForm>;
  commitCells?: () => void;
  rollbackAll?: () => void;
}
