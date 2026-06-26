<template>
  <div>
    <button @click="GetString">返回字符串</button>
    <button @click="SetString">发送字符串</button>
    <button @click="GetBool">返回bool值</button>
    <button @click="SetBool">发送bool值</button>
    <button @click="GetNumber">返回Number值</button>
    <button @click="SetNumber">发送Number值</button>
    <button @click="GetDateTime">返回DateTime值</button>
    <button @click="SetDateTime">发送DateTime值</button>

    <button @click="GetBaseDict">返回基本类型Dict</button>
    <button @click="SetDict">发送基本类型Dict</button>
  </div>
</template>

<script setup lang="ts">
import Rpc from '../core/Rpc.ts'
import Dict from "../core/Dict.ts";
import Serializer from "../core/Serializer.ts";
import Kit from "../core/Kit.ts";

const GetString = async () => {
  const str = await Kit.rpc<string>(
    "cm",
    "TestSerialize.GetString"
  );
  console.log(str);
};

const SetString = async () => {
  const success = await Kit.rpc<boolean>(
    "cm",
    "TestSerialize.SetString",
    "这是一个测试字符串"
  );
  console.log(success ? "调用成功！" : "调用不成功！");
};

const GetBool = async () => {
  const str = await Kit.rpc<boolean>(
    "cm",
    "TestSerialize.GetBool"
  );
  console.log(str);
};

const SetBool = async () => {
  const success = await Kit.rpc<boolean>(
    "cm",
    "TestSerialize.SetBool",
    true
  );
  console.log(success ? "调用成功！" : "调用不成功！");
};

const GetNumber = async () => {
  const str = await Kit.rpc<number>(
    "cm",
    "TestSerialize.GetInt"
  );
  console.log(str);
};

const SetNumber = async () => {
  const success = await Kit.rpc<boolean>(
    "cm",
    "TestSerialize.SetInt",
    42
  );
  console.log(success ? "调用成功！" : "调用不成功！");
};

const GetDateTime = async () => {
  const str = await Kit.rpc<Date>(
    "cm",
    "TestSerialize.GetDateTime"
  );
  console.log(str);
};

const SetDateTime = async () => {
  const success = await Kit.rpc<boolean>(
    "cm",
    "TestSerialize.SetDateTime",
    new Date()
  );
  console.log(success ? "调用成功！" : "调用不成功！");
};

const GetBaseDict = async () => {
  const dict = await Kit.rpc<Dict>(
    "cm",
    "TestSerialize.GetBaseDict"
  );
  console.log(dict);
};

const SetDict = async () => {
  const success = await Kit.rpc<boolean>(
    "cm",
    "TestSerialize.SendDict",
    new Dict([["string", "string value"], ["bool", true], ["number", 123], ["date", new Date()]])
  );
  console.log(success ? "调用成功！" : "调用不成功！");
};
</script>
