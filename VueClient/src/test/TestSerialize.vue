<template>
  
  <div>
    <p>基本类型</p>
    <button @click="GetString">返回字符串</button>
    <button @click="SetString">发送字符串</button>
    <button @click="GetBool">返回bool值</button>
    <button @click="SetBool">发送bool值</button>
    <button @click="GetNumber">返回Number值</button>
    <button @click="SetNumber">发送Number值</button>
    <button @click="GetDateTime">返回DateTime值</button>
    <button @click="SetDateTime">发送DateTime值</button>
    <button @click="GetByteArray">返回byte[]值</button>
    <button @click="SetByteArray">发送byte[]值</button>

    <p>集合类型</p>
    <button @click="GetStringList">返回字符串列表</button>
    <button @click="SetStringList">发送字符串列表</button>
    <button @click="GetBoolList">返回bool列表</button>
    <button @click="SetBoolList">发送bool列表</button>
    <button @click="GetNumberList">返回int列表</button>
    <button @click="SetNumberList">发送int列表</button>
    <button @click="GetDoubleList">返回double列表</button>
    <button @click="SetDoubleList">发送double列表</button>
    <button @click="GetDateTimeList">返回DateTime列表</button>
    <button @click="SetDateTimeList">发送DateTime列表</button>
    <button @click="GetObjectList">返回object列表</button>
    <button @click="SetObjectList">发送object列表</button>

    <p>Table类型</p>
    <button @click="GetTable">返回Table</button>
    <button @click="SetTable">发送Table</button>

    <p>Dict</p>
    <button @click="GetBaseDict">返回基本类型Dict</button>
    <button @click="SetDict">发送基本类型Dict</button>

    <p>任意类型</p>
    <button @click="GetCustomBase">返回非内置对象</button>
    <button @click="SetCustomBase">发送非内置对象</button>
    <button @click="GetCustomList">返回非内置对象列表</button>
    <button @click="SetCustomList">发送非内置对象列表</button>
  </div>
</template>

<script setup lang="ts">
import { Kit, Table, Dict } from "../core/index";

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

const GetByteArray = async () => {
  const str = await Kit.rpc<string>(
    "cm",
    "TestSerialize.GetByteArray"
  );
  // base64转Uint8Array
  const uint8 = Uint8Array.from(atob(str), c => c.charCodeAt(0));
  console.log(uint8);
};

const SetByteArray = async () => {
  const success = await Kit.rpc<boolean>(
    "cm",
    "TestSerialize.SetByteArray",
    new Uint8Array([1, 2, 3, 4, 5])
  );
  console.log(success ? "调用成功！" : "调用不成功！");
};

const GetStringList = async () => {
  const str = await Kit.rpc<string[]>(
    "cm",
    "TestSerialize.GetStringList"
  );
  console.log(str);
};

const SetStringList = async () => {
  const success = await Kit.rpc<boolean>(
    "cm",
    "TestSerialize.SetStringList",
    ["这是一个测试字符串1", "这是一个测试字符串2", "这是一个测试字符串3"]
  );
  console.log(success ? "调用成功！" : "调用不成功！");
};

const GetBoolList = async () => {
  const str = await Kit.rpc<boolean>(
    "cm",
    "TestSerialize.GetBoolList"
  );
  console.log(str);
};

const SetBoolList = async () => {
  const success = await Kit.rpc<boolean>(
    "cm",
    "TestSerialize.SetBoolList",
    [true, false, true]
  );
  console.log(success ? "调用成功！" : "调用不成功！");
};

const GetNumberList = async () => {
  const str = await Kit.rpc<number>(
    "cm",
    "TestSerialize.GetIntList"
  );
  console.log(str);
};

const SetNumberList = async () => {
  const success = await Kit.rpc<boolean>(
    "cm",
    "TestSerialize.SetIntList",
    [1, 2, 3, 4, 5]
  );
  console.log(success ? "调用成功！" : "调用不成功！");
};

const GetDoubleList = async () => {
  const str = await Kit.rpc<number>(
    "cm",
    "TestSerialize.GetDoubleList"
  );
  console.log(str);
};

const SetDoubleList = async () => {
  const success = await Kit.rpc<boolean>(
    "cm",
    "TestSerialize.SetDoubleList",
    [1.1, 2.2, 3.3, 4.4, 5.5]
  );
  console.log(success ? "调用成功！" : "调用不成功！");
};

const GetDateTimeList = async () => {
  const str = await Kit.rpc<Date[]>(
    "cm",
    "TestSerialize.GetDateTimeList"
  );
  console.log(str);
};

const SetDateTimeList = async () => {
  const success = await Kit.rpc<boolean>(
    "cm",
    "TestSerialize.SetDateTimeList",
    [new Date(), new Date(), new Date()]
  );
  console.log(success ? "调用成功！" : "调用不成功！");
};

const GetObjectList = async () => {
  const str = await Kit.rpc<Date[]>(
    "cm",
    "TestSerialize.GetObjectList"
  );
  console.log(str);
};

const SetObjectList = async () => {
  const success = await Kit.rpc<boolean>(
    "cm",
    "TestSerialize.SetObjectList",
    [123, 'asdf', true, new Date()]
  );
  console.log(success ? "调用成功！" : "调用不成功！");
};

const GetTable = async () => {
  const table = await Kit.rpc<Table>(
    "cm",
    "TestSerialize.GetTable"
  );
  console.log(table);
};

const SetTable = async () => {
  const success = await Kit.rpc<boolean>(
    "cm",
    "TestSerialize.SetTable",
    new Dict([["string", "string value"], ["bool", true], ["number", 123], ["date", new Date()]])
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

const GetCustomBase = async () => {
  const dict = await Kit.rpc<Dict>(
    "cm",
    "TestSerialize.GetCustomBase"
  );
  console.log(dict);
};

const SetCustomBase = async () => {
  const success = await Kit.rpc<boolean>(
    "cm",
    "TestSerialize.SetCustomBase",
    { Name: "abv", ExpiryDate: new Date(), Sizes:[ "12", "abc", "wer" ] }
  );
  console.log(success ? "调用成功！" : "调用不成功！");
};

const GetCustomList = async () => {
  const dict = await Kit.rpc<Dict>(
    "cm",
    "TestSerialize.GetCustomList"
  );
  console.log(dict);
};

const SetCustomList = async () => {
  const success = await Kit.rpc<boolean>(
    "cm",
    "TestSerialize.SetCustomList",
    [{ Name: "1", ExpiryDate: new Date(), Sizes:[ "12", "abc", "wer" ] },
    { Name: "2", ExpiryDate: new Date(), Sizes:[ "ab", "cd", "ef" ]}]
  );
  console.log(success ? "调用成功！" : "调用不成功！");
};
</script>
