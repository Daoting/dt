<template>
  <div>
    <h2>测试我的组件</h2>
    <MyButton @click="handleClick">点我</MyButton>
  </div>
</template>

<script setup>
import MyButton from '../components/MyButton.vue'
import Rpc from '../core/Rpc.ts'


const handleClick = async () => {
  console.log('点击了')
  const a = new Rpc("cm", "SysKernel.GetConfig");
  const result = await a.call();
  console.log(result);
}
</script>