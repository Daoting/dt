﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-06-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 功能测试Api
    /// </summary>
    [Api(GroupName = "功能测试")]
    public class TestSerialize : BaseApi
    {
        #region 基本类型
        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <returns></returns>
        public string GetString()
        {
            return "字符串结果";
        }

        /// <summary>
        /// 字符串参数
        /// </summary>
        /// <param name="p_str"></param>
        public bool SetString(string p_str)
        {
            return !string.IsNullOrEmpty(p_str);
        }

        /// <summary>
        /// 返回bool值
        /// </summary>
        /// <returns></returns>
        public bool GetBool()
        {
            return true;
        }

        /// <summary>
        /// bool参数
        /// </summary>
        /// <param name="p_val"></param>
        public bool SetBool(bool p_val)
        {
            return p_val;
        }

        /// <summary>
        /// 返回int值
        /// </summary>
        /// <returns></returns>
        public int GetInt()
        {
            return 100;
        }

        /// <summary>
        /// int参数
        /// </summary>
        /// <param name="p_val"></param>
        public int SetInt(int p_val)
        {
            return p_val;
        }

        /// <summary>
        /// 返回long值
        /// </summary>
        /// <returns></returns>
        public long GetLong()
        {
            return long.MaxValue;
        }

        /// <summary>
        /// long参数
        /// </summary>
        /// <param name="p_val"></param>
        public long SetLong(long p_val)
        {
            return p_val;
        }

        /// <summary>
        /// 返回double值
        /// </summary>
        /// <returns></returns>
        public double GetDouble()
        {
            return 200.0d;
        }

        /// <summary>
        /// double参数
        /// </summary>
        /// <param name="p_val"></param>
        public double SetDouble(double p_val)
        {
            return p_val;
        }

        /// <summary>
        /// 返回DateTime值
        /// </summary>
        /// <returns></returns>
        public DateTime GetDateTime()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// DateTime参数
        /// </summary>
        /// <param name="p_val"></param>
        public DateTime SetDateTime(DateTime p_val)
        {
            return p_val;
        }

        /// <summary>
        /// 返回byte[]值
        /// </summary>
        /// <returns></returns>
        public byte[] GetByteArray()
        {
            return new byte[] { 10, 12, 14, 16 };
        }

        /// <summary>
        /// byte[]参数
        /// </summary>
        /// <param name="p_val"></param>
        public byte[] SetByteArray(byte[] p_val)
        {
            return p_val;
        }

        /// <summary>
        /// 返回MsgInfo
        /// </summary>
        /// <returns></returns>
        public MsgInfo GetMsgInfo()
        {
            return new MsgInfo
            {
                MethodName = "abc.fun",
                Params = new List<object> { 1, "a", true },
                PushMode = MsgPushMode.Online,
            };
        }

        /// <summary>
        /// MsgInfo参数
        /// </summary>
        /// <param name="p_msg"></param>
        public MsgInfo SetMsgInfo(MsgInfo p_msg)
        {
            return p_msg;
        }
        #endregion

        #region 集合类型
        /// <summary>
        /// 返回字符串数组
        /// </summary>
        /// <returns></returns>
        public List<string> GetStringList()
        {
            return new List<string>() { "first", "second" };
        }

        /// <summary>
        /// 字符串列表
        /// </summary>
        /// <param name="p_ls"></param>
        public bool SetStringList(List<string> p_ls)
        {
            return p_ls != null && p_ls.Count > 0;
        }

        /// <summary>
        /// 返回bool值列表
        /// </summary>
        /// <returns></returns>
        public List<bool> GetBoolList()
        {
            List<bool> ls = new List<bool>();
            ls.Add(true);
            ls.Add(false);
            ls.Add(false);
            return ls;
        }

        /// <summary>
        /// bool值列表
        /// </summary>
        /// <param name="p_val"></param>
        public List<bool> SetBoolList(List<bool> p_val)
        {
            return p_val;
        }

        /// <summary>
        /// 返回int值列表
        /// </summary>
        /// <returns></returns>
        public List<int> GetIntList()
        {
            List<int> ls = new List<int>() { 1, 2, 3, 4 };
            return ls;
        }

        /// <summary>
        /// int列表
        /// </summary>
        /// <param name="p_val"></param>
        public List<int> SetIntList(List<int> p_val)
        {
            return p_val;
        }

        /// <summary>
        /// 返回long值列表
        /// </summary>
        /// <returns></returns>
        public List<long> GetLongList()
        {
            List<long> ls = new List<long>() { 1, 2, 3, 4 };
            return ls;
        }

        /// <summary>
        /// long列表
        /// </summary>
        /// <param name="p_val"></param>
        public List<long> SetLongList(List<long> p_val)
        {
            return p_val;
        }

        /// <summary>
        /// 返回double值列表
        /// </summary>
        /// <returns></returns>
        public List<double> GetDoubleList()
        {
            return new List<double>() { 200.0d, 100d, 50.123d, 123.45d };
        }

        /// <summary>
        /// double列表
        /// </summary>
        /// <param name="p_val"></param>
        public List<double> SetDoubleList(List<double> p_val)
        {
            return p_val;
        }

        /// <summary>
        /// DateTime列表
        /// </summary>
        /// <returns></returns>
        public List<DateTime> GetDateTimeList()
        {
            List<DateTime> tms = new List<DateTime>();
            tms.Add(DateTime.Now);
            tms.Add(DateTime.Now.AddDays(-1));
            return tms;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_times"></param>
        /// <returns></returns>
        public bool SetDateTimeList(List<DateTime> p_times)
        {
            return p_times != null && p_times.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<object> GetObjectList()
        {
            List<object> ls = new List<object>();
            ls.Add("asdf");
            ls.Add(123);
            ls.Add(DateTime.Now);
            ls.Add(100.23d);
            return ls;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_ls"></param>
        /// <returns></returns>
        public bool SetObjectList(List<object> p_ls)
        {
            return p_ls != null && p_ls.Count > 0;
        }
        #endregion

        #region Table
        /// <summary>
        /// 返回Table到客户端
        /// </summary>
        /// <returns></returns>
        public Table GetTable()
        {
            return CreateTable();
        }

        /// <summary>
        /// 由外部传递Table
        /// </summary>
        /// <param name="p_tbl"></param>
        public Table SetDataTable(Table p_tbl)
        {
            if (p_tbl != null)
                p_tbl.AcceptChanges();
            return p_tbl;
        }

        /// <summary>
        /// 返回Row到客户端
        /// </summary>
        /// <returns></returns>
        public Row GetRow()
        {
            return CreateTable()[0];
        }

        /// <summary>
        /// 由外部传递Row
        /// </summary>
        /// <param name="p_row"></param>
        public Row SetRow(Row p_row)
        {
            return p_row;
        }

        /// <summary>
        /// 返回多个Table到客户端
        /// </summary>
        /// <returns></returns>
        public Dict GetTableDict()
        {
            Dict dict = new Dict();
            var tbl = CreateTable();
            tbl.Name = "tbl1";
            dict[tbl.Name] = tbl;

            tbl = CreateTable();
            tbl.Name = "tbl2";
            dict[tbl.Name] = tbl;
            return dict;
        }

        /// <summary>
        /// 由外部传递多个Table
        /// </summary>
        /// <param name="p_dict"></param>
        public Dict SetTableDict(Dict p_dict)
        {
            return p_dict;
        }

        /// <summary>
        /// 返回多个Table到客户端
        /// </summary>
        /// <returns></returns>
        public List<Table> GetTableList()
        {
            List<Table> ls = new List<Table>();
            Table tbl = CreateTable();
            tbl.Name = "tbl1";
            ls.Add(tbl);

            tbl = CreateTable();
            tbl.Name = "tbl2";
            ls.Add(tbl);
            return ls;
        }

        /// <summary>
        /// 由外部传递多个Table
        /// </summary>
        /// <param name="p_ls"></param>
        public List<Table> SetTableList(List<Table> p_ls)
        {
            foreach (Table tbl in p_ls)
            {
                tbl.AcceptChanges();
            }
            return p_ls;
        }

        Table CreateTable()
        {
            Table tbl = new Table { { "col1" }, { "col2" } };
            tbl.NewRow("列值11", "列值12");
            tbl.NewRow("列值21", "列值22");
            return tbl;
        }
        #endregion

        #region Dict
        /// <summary>
        /// 返回基本数据类型的Dict
        /// </summary>
        /// <returns></returns>
        public Dict GetBaseDict()
        {
            Dict dict = new Dict();
            dict["string"] = "string value";
            dict["bool"] = true;
            dict["int"] = 100;
            dict["long"] = 123234L;
            dict["double"] = 1234.456d;
            dict["datetime"] = DateTime.Now;
            dict["bytearray"] = new byte[] { 10, 12, 14, 16 };
            dict["null"] = null;
            return dict;
        }

        /// <summary>
        /// 返回基本数据类型的Dict
        /// </summary>
        /// <returns></returns>
        public Dict GetCombineDict()
        {
            Dict dt = new Dict();

            Dict cls = new Dict();
            cls["string"] = "string value";
            cls["bool"] = true;
            cls["int"] = 100;
            cls["long"] = 123234L;
            cls["double"] = 1234.456d;
            cls["datetime"] = DateTime.Now;
            cls["bytearray"] = new byte[] { 10, 12, 14, 16 };
            cls["null"] = null;
            dt["基本数据类型"] = cls;

            Dict temp = new Dict();
            dt["第一级"] = temp;
            cls = new Dict();
            cls["第三级"] = "节点值";
            temp["第二级"] = cls;


            dt["DataTable"] = CreateTable();
            dt["字符串列表"] = new List<string>() { "first", "second" };
            dt["bool列表"] = new List<bool>() { true, false, false, true };
            dt["int列表"] = new List<int>() { 100, 123, 432, 78 };
            return dt;
        }

        /// <summary>
        /// 本数据类型的Dict
        /// </summary>
        /// <param name="p_dict"></param>
        public Dict SendDict(Dict p_dict)
        {
            return p_dict;
        }

        /// <summary>
        /// 返回Dict列表
        /// </summary>
        /// <returns></returns>
        public List<Dict> GetDictList()
        {
            List<Dict> dts = new List<Dict>();
            Dict dict = new Dict();
            dict["string"] = "string value";
            dict["bool"] = true;
            dict["int"] = 100;
            dict["long"] = 123234L;
            dict["double"] = 1234.456d;
            dict["datetime"] = DateTime.Now;
            dict["bytearray"] = new byte[] { 10, 12, 14, 16 };
            dict["null"] = null;
            dts.Add(dict);

            dict = new Dict();
            dict["string"] = "string value";
            dict["bool"] = true;
            dts.Add(dict);
            return dts;
        }

        /// <summary>
        /// 发送Dict列表
        /// </summary>
        /// <param name="p_dicts"></param>
        public bool SendDictList(List<Dict> p_dicts)
        {
            return p_dicts != null && p_dicts.Count > 0;
        }
        #endregion

        #region 自定义类型
        /// <summary>
        /// 返回基础自定义类型
        /// </summary>
        /// <returns></returns>
        public Product GetCustomBase()
        {
            Product product = new Product();
            product.Name = "Apple";
            product.ExpiryDate = new DateTime(2016, 12, 28);
            product.Price = 3.99M;
            product.Sizes = new string[] { "Small", "Medium", "Large" };
            return product;
        }

        /// <summary>
        /// 由外部传递基础自定义类型
        /// </summary>
        /// <param name="p_product"></param>
        /// <returns></returns>
        public bool SetCustomBase(Product p_product)
        {
            return (p_product != null && p_product.Sizes.Length > 0);
        }

        /// <summary>
        /// 返回复杂自定义类型
        /// </summary>
        /// <returns></returns>
        public Student GetCustomCombine()
        {
            Student person = new Student();
            person.Name = "John Smith";
            person.LastModified = DateTime.Now;
            person.Salary = new Dict();
            person.Salary.Add("一月", 2000);
            person.Salary.Add("二月", 3000);
            person.Info = CreateTable();
            return person;
        }

        /// <summary>
        /// 由外部传递复杂自定义类型
        /// </summary>
        /// <param name="p_person"></param>
        /// <returns></returns>
        public bool SetCustomCombine(Student p_person)
        {
            return (p_person != null && !string.IsNullOrEmpty(p_person.Name));
        }

        /// <summary>
        /// 返回嵌套自定义类型
        /// </summary>
        /// <returns></returns>
        public Department GetContainCustom()
        {
            Department dept = new Department() { Name = "人事部" };
            Student employee = new Student();
            employee.Name = "John Smith";
            employee.LastModified = DateTime.Now;
            employee.Salary = new Dict();
            employee.Salary.Add("一月", 2000);
            employee.Salary.Add("二月", 3000);
            employee.Info = CreateTable();
            dept.Employee = employee;
            return dept;
        }

        /// <summary>
        /// 由外部传递嵌套自定义类型
        /// </summary>
        /// <param name="p_dept"></param>
        /// <returns></returns>
        public bool SetContainCustom(Department p_dept)
        {
            return (p_dept != null && p_dept.Employee != null);
        }
        #endregion

        #region 异步
        public Task AsyncVoid(string p_msg)
        {
            return Task.Run(() => _.Log.Information(p_msg));
        }

        public Task<Table> AsyncDb()
        {
            return Task.Run(() => CreateTable());
        }

        public async Task<int> AsyncWait()
        {
            int cnt = await Task.Run<int>(() => 10);
            return cnt;
        }
        #endregion
    }

    [JsonObj("产品")]
    public class Product
    {
        public string Name { get; set; }

        public DateTime ExpiryDate { get; set; }

        [JsonIgnore]
        public decimal Price { get; set; }

        public string[] Sizes { get; set; }
    }

    [JsonObj("学生")]
    public class Student
    {
        public string Name { get; set; }

        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime LastModified { get; set; }

        [JsonConverter(typeof(RpcJson))]
        public Dict Salary { get; set; }

        [JsonConverter(typeof(RpcJson))]
        public Table Info { get; set; }
    }

    [JsonObj("部门")]
    public class Department
    {
        public string Name { get; set; }

        public Student Employee { get; set; }
    }
}