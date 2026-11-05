<template>
  <div class="row flex-center full-height">
    <div style="margin-bottom: 2rem">
      <div class="title text-h6 text-center">
        <q-item-label>搬运工</q-item-label>
      </div>
      <div class="yzm row justify-center">
        <q-input
          placeholder="请输入密码"
          v-model="password"
          style="width: 336px"
          :type="isPwd ? 'password' : 'text'"
          v-on:keyup.enter="submit"
        >
          <template #prepend>
            <q-item-label>密码</q-item-label>
          </template>
          <template #append>
            <q-icon
              :name="isPwd ? 'visibility_off' : 'visibility'"
              class="cursor-pointer"
              @click="isPwd = !isPwd"
            />
          </template>
        </q-input>
      </div>
      <div class="row justify-center">
        <q-btn
          unelevated
          class="login-btn"
          color="primary"
          label="登 录"
          :disable="!password"
          @click="submit"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useUserStore } from "./stores/user";
import router from "./router.ts";

const password = ref("");
const isPwd = ref(true);

function submit() {
  const user = useUserStore();
  user.login();
  router.push("/");
}
</script>

<style scoped>
.login-form {
}
.title {
  font-size: 22px;
  font-family:
    PingFangSC-Semibold,
    PingFang SC;
  font-weight: 600;
  color: rgba(51, 51, 51, 1);
  line-height: 30px;
}

.yzm {
  margin-top: 34px;
  .q-item__label {
    height: 20px;
    font-size: 14px;
    font-family:
      PingFangSC-Medium,
      PingFang SC;
    font-weight: 500;
    color: rgba(51, 51, 51, 1);
    line-height: 20px;
  }
  .yzm-action {
    height: 20px;
    font-size: 14px;
    font-family:
      PingFangSC-Medium,
      PingFang SC;
    font-weight: 500;
    color: rgba(52, 200, 231, 1);
    line-height: 20px;
  }
}
.login-btn {
  margin-top: 38px;
  width: 336px;
  height: 54px;
  background: rgba(52, 200, 231, 1);
  border-radius: 27px;
}
</style>
