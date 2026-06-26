<template>
  <div>
    <h2>测试我的组件</h2>
    <TestSerialize></TestSerialize>
  </div>
</template>

<script setup>
import MyButton from '../components/MyButton.vue'
import Rpc from '../core/Rpc.ts'
import Dict from "../core/Dict.ts";
import Serializer from "../core/Serializer.ts";
import TestSerialize from "../test/TestSerialize.vue";

const handleClick = async () => {
  console.log('点击了')
  // const a = new Rpc("cm", "SysKernel.GetConfig");
  // const result = await a.call();
  // console.log(result);
  const a = new Dict();
  a.add("key1", "value1");
  a.add("key2", 12);
  a.add("key3", true);
  a.add("key4", new Date(2026, 5, 25, 10, 0, 0));
  a.add("key7", new Dict([["subkey1", "subvalue1"], ["subkey2", 22]]));
  a.add("key5", [1, "abc", false]);
  a.add("key6", new Uint8Array([1, 2, 3, 4, 5]));
  const b = Serializer.Serialize(a);
  console.log(JSON.stringify(b));
}
</script>