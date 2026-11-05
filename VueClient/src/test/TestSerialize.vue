<template>
  <div class="column">
    <q-bar class="q-mt-md">基本类型</q-bar>
    <div class="row">
      <q-btn @click="GetString" label="返回字符串" />
      <q-btn label="发送字符串" @click="SetString" />
      <q-btn label="返回bool值" @click="GetBool" />
      <q-btn label="发送bool值" @click="SetBool" />
      <q-btn label="返回int值" @click="GetInt" />
      <q-btn label="发送int值" @click="SetInt" />
      <q-btn label="返回long值" @click="GetLong" />
      <q-btn label="发送long值" @click="SetLong" />
      <q-btn label="返回double值" @click="GetDouble" />
      <q-btn label="发送double值" @click="SetDouble" />
      <q-btn label="返回DateTime" @click="GetDateTime" />
      <q-btn label="发送DateTime" @click="SetDateTime" />
      <q-btn label="返回byte[]值" @click="GetByteArray" />
      <q-btn label="发送byte[]值" @click="SetByteArray" />
      <q-btn label="返回MsgInfo" @click="GetMsgInfo" />
      <q-btn label="发送MsgInfo" @click="SetMsgInfo" />
    </div>

    <q-bar class="q-mt-md">集合类型</q-bar>
    <div class="row">
      <q-btn @click="GetStringList" label="返回字符串列表" />
      <q-btn label="发送字符串列表" @click="SetStringList" />
      <q-btn label="返回bool列表" @click="GetBoolList" />
      <q-btn label="发送bool列表" @click="SetBoolList" />
      <q-btn label="返回int列表" @click="GetIntList" />
      <q-btn label="发送int列表" @click="SetIntList" />
      <q-btn label="返回long列表" @click="GetLongList" />
      <q-btn label="发送long列表" @click="SetLongList" />
      <q-btn label="返回double列表" @click="GetDoubleList" />
      <q-btn label="发送double列表" @click="SetDoubleList" />
      <q-btn label="返回DateTime列表" @click="GetDateTimeList" />
      <q-btn label="发送DateTime列表" @click="SetDateTimeList" />
    </div>

    <q-bar class="q-mt-md">Dict类型</q-bar>
    <div class="row">
      <q-btn label="返回基本类型Dict" @click="GetBaseDict" />
      <q-btn label="返回复杂类型Dict" @click="GetCombineDict" />
      <q-btn label="发送Dict" @click="SendDict" />
      <q-btn label="返回Dict列表" @click="GetDictList" />
      <q-btn label="发送Dict列表" @click="SendDictList" />
    </div>

    <q-bar class="q-mt-md">Table类型</q-bar>
    <div class="row">
      <q-btn label="返回Table" @click="GetTable" />
      <q-btn label="发送Table" @click="SetTable" />
      <q-btn label="返回Row" @click="GetRow" />
      <q-btn label="发送Row" @click="SetRow" />
    </div>

    <q-bar class="q-mt-md">SaveItem类型</q-bar>
    <div class="row">
      <q-btn label="返回Table的SaveItem" @click="GetTableSaveItem" />
      <q-btn label="发送Table的SaveItem" @click="SetTableSaveItem" />
      <q-btn label="返回Row的SaveItem" @click="GetRowSaveItem" />
      <q-btn label="发送Row的SaveItem" @click="SetRowSaveItem" />
      <q-btn label="返回SaveItem列表" @click="GetSaveItems" />
      <q-btn label="发送SaveItem列表" @click="SetSaveItems" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { AtTestCm } from "@/test/AtTestCm";
import { MsgInfo } from "@/core";

async function GetString() {
  const str = await AtTestCm.getString();
  kit.msg(str);
}

async function SetString() {
  const suc = await AtTestCm.setString("发送字符串");
  kit.msg(suc ? "调用成功" : "调用失败");
}

async function GetBool() {
  const str = await AtTestCm.getBool();
  kit.msg("返回值: " + str);
}

async function SetBool() {
  const suc = await AtTestCm.setBool(true);
  kit.msg(suc ? "调用成功" : "调用失败");
}

async function GetInt() {
  const str = await AtTestCm.getInt();
  kit.msg("返回值: " + str);
}

async function SetInt() {
  const str = await AtTestCm.setInt(500);
  kit.msg("发送成功: " + str);
}

async function GetLong() {
  const str = await AtTestCm.getLong();
  kit.msg("返回值: " + str);
}

async function SetLong() {
  const str = await AtTestCm.setLong(123456789012);
  kit.msg("发送成功: " + str);
}

async function GetDouble() {
  const str = await AtTestCm.getDouble();
  kit.msg("返回值: " + str);
}

async function SetDouble() {
  const str = await AtTestCm.setDouble(1234.567);
  kit.msg("发送成功: " + str);
}

async function GetDateTime() {
  const str = await AtTestCm.getDateTime();
  kit.msg("返回值: " + str);
}

async function SetDateTime() {
  const str = await AtTestCm.setDateTime(new Date());
  kit.msg("发送成功: " + str);
}

async function GetByteArray() {
  const str = await AtTestCm.getByteArray();
  kit.msg("返回值: " + str);
}

async function SetByteArray() {
  const str = await AtTestCm.setByteArray(new Uint8Array([1, 2, 3, 4]));
  kit.msg("发送成功: " + str);
}

async function GetMsgInfo() {
  const str = await AtTestCm.getMsgInfo();
  kit.msg("返回值: " + str);
}

async function SetMsgInfo() {
  const str = await AtTestCm.setMsgInfo(
    {
      MethodName: "abc.fun",
      Params: [1, "a", true],
      Title: "",
      Content: "",
    },
  );
  kit.msg("发送成功: " + str);
}

async function GetStringList() {
  const str = await AtTestCm.getStringList();
  kit.msg("返回值: " + str.join(", "));
}

async function SetStringList() {
  const suc = await AtTestCm.setStringList(["first", "second"]);
  kit.msg(suc ? "调用成功" : "调用失败");
}

async function GetBoolList() {
  const str = await AtTestCm.getBoolList();
  kit.msg("返回值: " + str.join(", "));
}

async function SetBoolList() {
  const suc = await AtTestCm.setBoolList([true, false, true]);
  kit.msg(suc ? "调用成功" : "调用失败");
}

async function GetIntList() {
  const str = await AtTestCm.getIntList();
  kit.msg("返回值: " + str.join(", "));
}

async function SetIntList() {
  const str = await AtTestCm.setIntList([1, 2, 3, 4]);
  kit.msg("发送成功: " + str.join(", "));
}

async function GetLongList() {
  const str = await AtTestCm.getLongList();
  kit.msg("返回值: " + str.join(", "));
}

async function SetLongList() {
  const str = await AtTestCm.setLongList([12, 2, 3, 4]);
  kit.msg("发送成功: " + str.join(", "));
}

async function GetDoubleList() {
  const str = await AtTestCm.getDoubleList();
  kit.msg("返回值: " + str.join(", "));
}

async function SetDoubleList() {
  const str = await AtTestCm.setDoubleList([1, 2, 3, 4]);
  kit.msg("发送成功: " + str.join(", "));
}

async function GetDateTimeList() {
  const str = await AtTestCm.getDateTimeList();
  kit.msg("返回值: " + str.join(", "));
}

async function SetDateTimeList() {
  const suc = await AtTestCm.setDateTimeList([new Date(), new Date()]);
  kit.msg(suc ? "调用成功" : "调用失败");
}

async function GetBaseDict() {
  const dt = await AtTestCm.getBaseDict();
  let msg = "";
  for (const [key, val] of dt) {
    msg += key + " = " + val + "<br>";
  }
  kit.msg(msg);
}

async function GetCombineDict() {
  const suc = await AtTestCm.getCombineDict();
  kit.msg(suc ? "调用成功" : "调用失败");
}

async function SendDict() {
  const map = new Dict([
    ["a", 1],
    ["b", true],
    ["c", "abc"],
  ]);
  const dt = await AtTestCm.sendDict(map);
  let msg = "";
  for (const [key, val] of dt) {
    msg += key + " = " + val + "<br>";
  }
  kit.msg(msg);
}

async function GetDictList() {
  const ls = await AtTestCm.getDictList();
  let msg = "返回：<br>";
  for (const dt of ls) {
    for (const [key, val] of dt) {
      msg += key + " = " + val + "<br>";
    }
    msg += "<br>";
  }
  kit.msg(msg);
}

async function SendDictList() {
  const map1 = new Dict([
    ["a", 1],
    ["b", true],
    ["c", "abc"],
  ]);

  const map2 = new Dict([
    ["a", 1],
    ["b", true],
    ["c", "abc"],
  ]);
  
  const suc = await AtTestCm.sendDictList([map1, map2]);
  kit.msg(suc ? "调用成功" : "调用失败");
}

async function GetTable() {
  const tbl = await AtTestCm.getTable();
  kit.msg(tbl.toString());
}

async function SetTable() {
  const tbl = await AtTestCm.getTable();
  const tbl2 = await AtTestCm.setTable(tbl);
  kit.msg(tbl2.toString());
}

async function GetRow() {
  const row = await AtTestCm.getRow();
  kit.msg(row.toString());
}

async function SetRow() {
  const row = await AtTestCm.getRow();
  const row2 = await AtTestCm.setRow(row);
  kit.msg(row2.toString());
}

async function GetTableSaveItem() {
  const si = await AtTestCm.getTableSaveItem();
  kit.msg(si.toString());
}

async function SetTableSaveItem() {
  const si = await AtTestCm.getTableSaveItem();
  const si2 = await AtTestCm.setTableSaveItem(si);
  kit.msg(si2.toString());
}

async function GetRowSaveItem() {
  const si = await AtTestCm.getRowSaveItem();
  kit.msg(si.toString());
}

async function SetRowSaveItem() {
  const row = await AtTestCm.getRow();
  const row2 = await AtTestCm.setRow(row);
  kit.msg(row2.toString());
}

async function GetSaveItems() {
  const sis = await AtTestCm.getSaveItems();
  kit.msg(sis.toString());
}

async function SetSaveItems() {
  const row = await AtTestCm.getRow();
  const row2 = await AtTestCm.setRow(row);
  kit.msg(row2.toString());
}
</script>
