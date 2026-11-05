<template>
  <Fv
    ref="form"
    v-model="data"
    :schema="schema"
    :submit="handleSubmit"
  >
    <CText id="username" label="用户名" />
    <CText id="age" label="年龄" type="number" />
    <CList
      id="favoriteColor"
      label="喜欢的颜色"
      :options="colorOptions"
      option-label="label"
      option-value="value"
      placeholder="请选择颜色"
      
    />
    <CTip prefix="选择颜色：" icon="info" v-model="tipText" />
    
    <CDate id="birthday" label="出生日期" mode="date" />
    <CDate id="meetingTime" label="会议时间" mode="datetime" />


    <!-- <q-btn
        type="submit"
        label="提交"
        color="primary"
      />
      <q-btn label="重置" class="q-ml-sm" @click="form?.resetForm()" /> -->
  </Fv>
</template>

<script setup lang="ts">
interface UserForm {
  username: string;
  age: number;
  favoriteColor: string;
  birthday: string;
  meetingTime: string;
}

const data = ref<UserForm>({ username: "abc", age: 20, favoriteColor: "red", birthday: "", meetingTime: "" });
const schema = z.object({
  username: z.string().min(2),
  age: z.number().min(1),
  favoriteColor: z.string().min(1),
  birthday: z.string().min(1, "请选择日期"),
  meetingTime: z.string().min(1, "请选择日期时间"),
});

const colorOptions = [
  { label: "红色", value: "red" },
  { label: "绿色", value: "green" },
  { label: "蓝色", value: "blue" },
];

const tipText = computed(() => {
  return data.value.favoriteColor;
});

const form = ref<any>(null);

const handleSubmit = (data: UserForm) => {
  console.log("提交", data);
  console.log("变更字段", form.value?.getChangedCells?.());
};
</script>