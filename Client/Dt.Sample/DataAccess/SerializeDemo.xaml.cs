#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System.Text.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Text.Json.Serialization;
using Dt.Core.Rpc;
#endregion

namespace Dt.Sample
{
    public partial class SerializeDemo : Win
    {
        Table _tbl;
        public SerializeDemo()
        {
            InitializeComponent();
            _tbl = CreateTable();
        }

        #region 基本类型
        async void GetString(object sender, RoutedEventArgs e)
        {
            _tbInfo.Text = "返回值：" + await AtTest.GetString();
        }

        async void SetString(object sender, RoutedEventArgs e)
        {
            bool success = await AtTest.SetString("发送字符串");
            _tbInfo.Text = success ? "调用成功！" : "调用不成功！";
        }

        async void GetBool(object sender, RoutedEventArgs e)
        {
            bool result = await AtTest.GetBool();
            _tbInfo.Text = "返回值：" + result.ToString();
        }

        async void SetBool(object sender, RoutedEventArgs e)
        {
            bool result = await AtTest.SetBool(true);
            _tbInfo.Text = result ? "调用成功！" : "调用不成功！";
        }

        async void GetInt(object sender, RoutedEventArgs e)
        {
            int result = await AtTest.GetInt();
            _tbInfo.Text = "返回值：" + result.ToString();
        }

        async void SetInt(object sender, RoutedEventArgs e)
        {
            int val = 100;
            int result = await AtTest.SetInt(val);
            _tbInfo.Text = (result == val) ? "调用成功！" : "调用不成功！";
        }

        async void GetLong(object sender, RoutedEventArgs e)
        {
            var result = await AtTest.GetLong();
            _tbInfo.Text = "返回值：" + result.ToString();
        }

        async void SetLong(object sender, RoutedEventArgs e)
        {
            long val = 1234L;
            long result = await AtTest.SetLong(val);
            _tbInfo.Text = (result == val) ? "调用成功！" : "调用不成功！";
        }

        async void GetDouble(object sender, RoutedEventArgs e)
        {
            var result = await AtTest.GetDouble();
            _tbInfo.Text = "返回值：" + result.ToString();
        }

        async void SetDouble(object sender, RoutedEventArgs e)
        {
            double val = 1234.123d;
            double result = await AtTest.SetDouble(val);
            _tbInfo.Text = (result == val) ? "调用成功！" : "调用不成功！";
        }

        async void GetDateTime(object sender, RoutedEventArgs e)
        {
            var result = await AtTest.GetDateTime();
            _tbInfo.Text = "返回值：" + result.ToString();
        }

        async void SetDateTime(object sender, RoutedEventArgs e)
        {
            DateTime val = DateTime.Now;
            DateTime result = await AtTest.SetDateTime(val);
            TimeSpan span = val - result;
            _tbInfo.Text = (span.TotalSeconds < 1) ? "调用成功！" : "调用不成功！";
        }

        async void GetByteArray(object sender, RoutedEventArgs e)
        {
            var result = await AtTest.GetByteArray();
            _tbInfo.Text = "返回值：" + result.ToString();
        }

        async void SetByteArray(object sender, RoutedEventArgs e)
        {
            byte[] val = new byte[] { 10, 12, 14, 16 };
            byte[] result = await AtTest.SetByteArray(val);
            bool succ = true;
            if (val.Length != result.Length)
                succ = false;
            else
            {
                for (int i = 0; i < val.Length; i++)
                {
                    if (val[i] != result[i])
                        succ = false;
                }
            }
            _tbInfo.Text = succ ? "调用成功！" : "调用不成功！";
        }

        async void GetMsgInfo(object sender, RoutedEventArgs e)
        {
            var result = await AtTest.GetMsgInfo();
            _tbInfo.Text = result != null ? "调用成功！" : "调用不成功！";
        }

        async void SetMsgInfo(object sender, RoutedEventArgs e)
        {
            var result = await AtTest.SetMsgInfo(new MsgInfo { Title = "abc", Content = "message" });
            _tbInfo.Text = result != null ? "调用成功！" : "调用不成功！";
        }
        #endregion

        #region 集合类型
        async void GetStringList(object sender, RoutedEventArgs e)
        {
            List<string> ls = await AtTest.GetStringList();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("调用成功，共有{0}个字符串：\r\n", ls.Count);
            foreach (string item in ls)
            {
                sb.AppendLine(item);
            }
            _tbInfo.Text = sb.ToString();
        }

        async void SetStringList(object sender, RoutedEventArgs e)
        {
            bool success = await AtTest.SetStringList(new List<string>() { "first", "second" });
            _tbInfo.Text = success ? "调用成功！" : "调用不成功！";
        }

        async void GetBoolList(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("调用成功：");
            foreach (var item in await AtTest.GetBoolList())
            {
                sb.AppendLine(item.ToString());
            }
            _tbInfo.Text = sb.ToString();
        }

        async void SetBoolList(object sender, RoutedEventArgs e)
        {
            var ls = await AtTest.SetBoolList(new List<bool>() { true, false, true });
            _tbInfo.Text = ls.Count == 3 ? "调用成功！" : "调用不成功！";
        }

        async void GetIntList(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("调用成功：");
            foreach (var item in await AtTest.GetIntList())
            {
                sb.AppendLine(item.ToString());
            }
            _tbInfo.Text = sb.ToString();
        }

        async void SetIntList(object sender, RoutedEventArgs e)
        {
            var ls = await AtTest.SetIntList(new List<int>() { 1, 2, 3, 4 });
            _tbInfo.Text = ls.Count == 4 ? "调用成功！" : "调用不成功！";
        }

        async void GetLongList(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("调用成功：");
            foreach (var item in await AtTest.GetLongList())
            {
                sb.AppendLine(item.ToString());
            }
            _tbInfo.Text = sb.ToString();
        }

        async void SetLongList(object sender, RoutedEventArgs e)
        {
            var ls = await AtTest.SetLongList(new List<long>() { 1, 2, 3, 4 });
            _tbInfo.Text = ls.Count == 4 ? "调用成功！" : "调用不成功！";
        }

        async void GetDoubleList(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("调用成功：");
            foreach (var item in await AtTest.GetDoubleList())
            {
                sb.AppendLine(item.ToString());
            }
            _tbInfo.Text = sb.ToString();
        }

        async void SetDoubleList(object sender, RoutedEventArgs e)
        {
            var ls = await AtTest.SetDoubleList(new List<double>() { 200.0d, 100d, 50.123d, 123.45d });
            _tbInfo.Text = ls.Count == 4 ? "调用成功！" : "调用不成功！";
        }

        async void GetDateTimeList(object sender, RoutedEventArgs e)
        {
            List<DateTime> dts = await AtTest.GetDateTimeList();
            _tbInfo.Text = (dts != null) ? string.Format("调用成功: {0}, {1}", dts[0], dts[1]) : "调用不成功！";
        }

        async void SetDateTimeList(object sender, RoutedEventArgs e)
        {
            List<DateTime> tms = new List<DateTime>();
            tms.Add(DateTime.Now);
            tms.Add(DateTime.Now.AddDays(-1));
            _tbInfo.Text = (await AtTest.SetDateTimeList(tms)) ? "调用成功！" : "调用不成功！";
        }

        async void GetObjectList(object sender, RoutedEventArgs e)
        {
            List<object> ls = await AtTest.GetObjectList();
            _tbInfo.Text = (ls != null && ls.Count > 0) ? "调用成功！" : "调用不成功！";
        }

        async void SetObjectList(object sender, RoutedEventArgs e)
        {
            List<object> ls = new List<object>();
            ls.Add("asdf");
            ls.Add(123);
            ls.Add(DateTime.Now);
            ls.Add(100.23d);
            _tbInfo.Text = (await AtTest.SetObjectList(ls)) ? "调用成功！" : "调用不成功！";
        }
        #endregion

        #region Table
        async void GetTable(object sender, RoutedEventArgs e)
        {
            var tbl = await AtTest.GetTable();
            StringBuilder sb = new StringBuilder("调用成功：\r\n");
            foreach (var row in tbl)
            {
                foreach (var cell in row.Cells)
                {
                    sb.AppendFormat("{0}：{1}    ", cell.ID, cell.Val);
                }
                sb.AppendLine();
            }
            _tbInfo.Text = sb.ToString();
        }

        async void SetTable(object sender, RoutedEventArgs e)
        {
            var tbl = await AtTest.SetTable(_tbl);
            _tbInfo.Text = tbl != null ? "调用成功！" : "调用不成功！";
        }

        async void GetRow(object sender, RoutedEventArgs e)
        {
            var row = await AtTest.GetRow();
            StringBuilder sb = new StringBuilder("调用成功：\r\n");
            foreach (var cell in row.Cells)
            {
                sb.AppendFormat("{0}：{1}    ", cell.ID, cell.Val);
            }
            _tbInfo.Text = sb.ToString();
        }

        async void SetRow(object sender, RoutedEventArgs e)
        {
            var row = await AtTest.SetRow(_tbl[0]);
            _tbInfo.Text = row != null ? "调用成功！" : "调用不成功！";
        }

        async void GetEntityRowTable(object sender, RoutedEventArgs e)
        {
            var tbl = await AtTest.GetEntityTable();
            StringBuilder sb = new StringBuilder("调用成功：\r\n");
            foreach (var r in tbl)
            {
                sb.AppendLine($"Col1:  {r.Col1}");
                sb.AppendLine($"Col2:  {r.Col2}");
                sb.AppendLine($"Col3:  {r.Col3}");
                sb.AppendLine($"Col4:  {r.Col4}");
                sb.AppendLine($"Col5:  {r.Col5}");
                sb.AppendLine($"Col6:  {r.Col6}");
                sb.AppendLine($"Col7:  {r.Col7}");
                sb.AppendLine();
            }
            _tbInfo.Text = sb.ToString();
        }

        async void SetEntityTable(object sender, RoutedEventArgs e)
        {
            var suc = await AtTest.SetEntityTable(CreateEntityTable());
            _tbInfo.Text = suc ? "调用成功！" : "调用不成功！";
        }

        async void GetEntityRow(object sender, RoutedEventArgs e)
        {
            var row = await AtTest.GetEntity();
            _tbInfo.Text = row.Col1 + " " + row.Col2;
        }

        async void SetEntityRow(object sender, RoutedEventArgs e)
        {
            var suc = await AtTest.SetEntity(CreateEntityTable()[0]);
            _tbInfo.Text = suc ? "调用成功！" : "调用不成功！";
        }

        /// <summary>
        /// 返回多个Table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void GetTableDict(object sender, RoutedEventArgs e)
        {
            Dict tbls = await AtTest.GetTableDict();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("调用成功，共有{0}个Table：\r\n", tbls.Count);
            foreach (var item in tbls)
            {
                var tbl = item.Value as Table;
                foreach (var row in tbl)
                {
                    foreach (var cell in row.Cells)
                    {
                        sb.AppendFormat("{0}：{1}    ", cell.ID, cell.Val);
                    }
                    sb.AppendLine();
                }
                sb.AppendLine();
                sb.AppendLine();
            }
            _tbInfo.Text = sb.ToString();
        }

        /// <summary>
        /// 传递多个Table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void SetTableDict(object sender, RoutedEventArgs e)
        {
            Dict dtl = new Dict();
            var tbl = _tbl.Clone();
            tbl.AcceptChanges();
            dtl["table2"] = tbl;
            Dict result = await AtTest.SetTableDict(dtl);
            _tbInfo.Text = result.Count == 1 ? "调用成功！" : "调用不成功！";
        }

        /// <summary>
        /// 返回多个Table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void GetTableList(object sender, RoutedEventArgs e)
        {
            List<Table> tbls = await AtTest.GetTableList();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("调用成功，共有{0}个DataTable：\r\n", tbls.Count);
            foreach (var tbl in tbls)
            {
                foreach (var row in tbl)
                {
                    foreach (var cell in row.Cells)
                    {
                        sb.AppendFormat("{0}：{1}    ", cell.ID, cell.Val);
                    }
                    sb.AppendLine();
                }
                sb.AppendLine();
                sb.AppendLine();
            }
            _tbInfo.Text = sb.ToString();
        }

        /// <summary>
        /// 传递多个Table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void SetTableList(object sender, RoutedEventArgs e)
        {
            List<Table> tbls = new List<Table>();
            tbls.Add(_tbl);
            tbls.Add(_tbl.Clone());
            tbls = await AtTest.SetTableList(tbls);
            _tbInfo.Text = tbls.Count == 2 ? "调用成功！" : "调用不成功！";
        }
        #endregion

        #region Dict
        /// <summary>
        /// 返回基本数据类型的Dict
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void GetBaseDict(object sender, RoutedEventArgs e)
        {
            OutDict(await AtTest.GetBaseDict());
        }

        /// <summary>
        /// 返回复杂类型Dict
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void GetCombineDict(object sender, RoutedEventArgs e)
        {
            OutDict(await AtTest.GetCombineDict());
        }

        /// <summary>
        /// 传送字符串对
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void SendDict(object sender, RoutedEventArgs e)
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

            var tbl = _tbl.Clone();
            tbl.AcceptChanges();
            dt["DataTable"] = tbl;

            dt["字符串列表"] = new List<string>() { "first", "second" };
            dt["bool列表"] = new List<bool>() { true, false, false, true };
            dt["int列表"] = new List<int>() { 100, 123, 432, 78 };

            OutDict(await AtTest.SendDict(dt));
        }

        async void GetDictList(object sender, RoutedEventArgs e)
        {
            List<Dict> dts = await AtTest.GetDictList();
            _tbInfo.Text = string.Format("调用成功：\r\n列表中有 {0} 个Dict", dts.Count);
        }

        async void SendDictList(object sender, RoutedEventArgs e)
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
            bool succ = await AtTest.SendDictList(dts);
            _tbInfo.Text = succ ? "调用成功！" : "调用不成功！";
        }
        #endregion

        #region 自定义类型
        async void GetCustomBase(object sender, RoutedEventArgs e)
        {
            Product product = await AtTest.GetCustomBase();
            if (product == null)
                _tbInfo.Text = "调用不成功！";
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Name: {0}\r\n", product.Name);
                sb.AppendFormat("ExpiryDate: {0}\r\n", product.ExpiryDate.ToString("yyyy-MM-dd"));
                sb.AppendFormat("Price : {0}\r\n", product.Price);
                sb.Append("Sizes:  ");
                foreach (string size in product.Sizes)
                {
                    sb.Append(size + "  ");
                }
                _tbInfo.Text = string.Format("调用成功：\r\n{0}", sb);
            }
        }

        async void SetCustomBase(object sender, RoutedEventArgs e)
        {
            Product product = new Product();
            product.Name = "Apple";
            product.ExpiryDate = new DateTime(2016, 12, 28);
            product.Price = 3.99M;
            product.Sizes = new string[] { "Small", "Medium", "Large" };
            _tbInfo.Text = (await AtTest.SetCustomBase(product)) ? "调用成功！" : "调用不成功！";
        }

        async void GetCustomCombine(object sender, RoutedEventArgs e)
        {
            var person = await AtTest.GetCustomCombine();
            if (person == null)
                _tbInfo.Text = "调用不成功！";
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Name: {0}\r\n", person.Name);
                sb.AppendFormat("LastModified: {0}\r\n", person.LastModified.ToString("yyyy-MM-hh HH:mm:ss"));
                sb.AppendLine("Salary:");
                foreach (var s in person.Salary)
                {
                    sb.AppendFormat("  {0} -> {1}\r\n", s.Key, s.Value);
                }
                sb.AppendLine("Info:");
                foreach (var row in person.Info)
                {
                    foreach (var cell in row.Cells)
                    {
                        sb.AppendFormat("  {0} -> {1}\r\n", cell.ID, cell.Val);
                    }
                }
                _tbInfo.Text = string.Format("调用成功：\r\n{0}", sb);
            }
        }

        async void SetCustomCombine(object sender, RoutedEventArgs e)
        {
            var person = new Student();
            person.Name = "John Smith";
            person.LastModified = Kit.Now;
            person.Salary = new Dict();
            person.Salary.Add("一月", 2000);
            person.Salary.Add("二月", 3000);
            person.Info = _tbl;
            _tbInfo.Text = (await AtTest.SetCustomCombine(person)) ? "调用成功！" : "调用不成功！";
        }

        async void GetContainCustom(object sender, RoutedEventArgs e)
        {
            Department dept = await AtTest.GetContainCustom();
            if (dept == null)
                _tbInfo.Text = "调用不成功！";
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("DepartmentName: {0}\r\n", dept.Name);
                sb.AppendFormat("Name: {0}\r\n", dept.Employee.Name);
                sb.AppendFormat("LastModified: {0}\r\n", dept.Employee.LastModified.ToString("yyyy-MM-hh HH:mm:ss"));
                sb.AppendLine("Salary:");
                foreach (var s in dept.Employee.Salary)
                {
                    sb.AppendFormat("  {0} -> {1}\r\n", s.Key, s.Value);
                }
                sb.AppendLine("Info:");
                foreach (var row in dept.Employee.Info)
                {
                    foreach (var cell in row.Cells)
                    {
                        sb.AppendFormat("  {0} -> {1}\r\n", cell.ID, cell.Val);
                    }
                }
                _tbInfo.Text = string.Format("调用成功：\r\n{0}", sb);
            }
        }

        async void SetContainCustom(object sender, RoutedEventArgs e)
        {
            Department dept = new Department() { Name = "人事部" };
            var employee = new Student();
            employee.Name = "John Smith";
            employee.LastModified = Kit.Now;
            employee.Salary = new Dict();
            employee.Salary.Add("一月", 2000);
            employee.Salary.Add("二月", 3000);
            employee.Info = _tbl;
            dept.Employee = employee;
            _tbInfo.Text = (await AtTest.SetContainCustom(dept)) ? "调用成功！" : "调用不成功！";
        }
        #endregion

        void OutDict(Dict p_dt)
        {
            int level = 0;
            StringBuilder sb = new StringBuilder();
            BuildDict(sb, p_dt, level);
            _tbInfo.Text = string.Format("调用成功：\r\n{0}", sb);
        }

        void BuildDict(StringBuilder p_sb, Dict p_dt, int p_level)
        {
            foreach (var item in p_dt)
            {
                for (int i = 0; i < p_level; i++)
                    p_sb.Append("   ");
                p_sb.Append(item.Key);

                if (item.Value == null)
                {
                    p_sb.Append(" → null");
                    p_sb.AppendLine();
                    continue;
                }

                Type type = item.Value.GetType();
                if (type == typeof(Dict))
                {
                    p_sb.AppendLine();
                    BuildDict(p_sb, (Dict)item.Value, p_level + 1);
                }
                else if (type == typeof(Table))
                {
                    p_sb.Append(" → ");
                    p_sb.AppendFormat("Table共{0}行", (item.Value as Table).Count);
                    p_sb.AppendLine();
                }
                else if (type != typeof(string) && item.Value is IEnumerable)
                {
                    p_sb.Append(" → ");
                    foreach (object obj in (item.Value as IEnumerable))
                    {
                        p_sb.Append(obj.ToString());
                        p_sb.Append(", ");
                    }
                    p_sb.AppendLine();
                }
                else
                {
                    p_sb.Append(" → ");
                    p_sb.Append(item.Value);
                    p_sb.AppendLine();
                }
            }
        }

        void OutProperty(StringBuilder p_sb, object p_obj)
        {
            if (p_obj == null)
                return;

            foreach (PropertyInfo property in p_obj.GetType().GetRuntimeProperties())
            {
                object val = property.GetValue(p_obj, null);
                p_sb.AppendFormat("{0}：{1}\r\n", property.Name, val);
            }
        }

        public static Table CreateTable()
        {
            Table tbl = new Table
            {
                { "col1" },
                { "col2", typeof(bool) },
                { "col3", typeof(long) },
                { "col4", typeof(DateTime) },
                { "col5", typeof(Gender) },
                { "col6", typeof(byte) },
                { "col7", typeof(byte[]) }
            };
            tbl.AddRow(new { col1 = "原始值", col2 = true, col3 = 100L, col4 = DateTime.Now, col5 = Gender.男, col6 = 23, col7 = new byte[] { 10, 20, 30, 40 } });
            tbl.AddRow(new { col1 = "列值21", col4 = DateTime.Now });
            tbl[0]["col1"] = "当前值";
            return tbl;
        }

        public static Table<CustomEntityObj> CreateEntityTable()
        {
            var tbl = Table<CustomEntityObj>.Create();
            tbl.Add(new CustomEntityObj(
                Col1: "原始值",
                Col2: true,
                Col3: 100L,
                Col4: DateTime.Now,
                Col5: Gender.男,
                Col6: 23,
                Col7: new byte[] { 10, 20, 30, 40 }));

            tbl.Add(new CustomEntityObj(
                Col1: "列值21",
                Col4: DateTime.Now));

            tbl[0].Col1 = "当前值";
            return tbl;
        }
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

        public DateTime LastModified { get; set; }

        [RpcJson]
        public Dict Salary { get; set; }

        [RpcJson]
        public Table Info { get; set; }
    }

    [JsonObj("部门")]
    public class Department
    {
        public string Name { get; set; }

        public Student Employee { get; set; }
    }

    public class CustomEntityObj : Entity
    {
        CustomEntityObj()
        { }

        public CustomEntityObj(
            string Col1 = default,
            bool Col2 = default,
            long Col3 = default,
            DateTime Col4 = default,
            Gender Col5 = default,
            byte Col6 = default,
            byte[] Col7 = default)
        {
            AddCell("Col1", Col1);
            AddCell("Col2", Col2);
            AddCell("Col3", Col3);
            AddCell("Col4", Col4);
            AddCell("Col5", Col5);
            AddCell("Col6", Col6);
            AddCell("Col7", Col7);
            IsAdded = true;
        }

        public string Col1
        {
            get { return (string)this["col1"]; }
            set { this["col1"] = value; }
        }

        public bool Col2
        {
            get { return (bool)this["col2"]; }
            set { this["col2"] = value; }
        }

        public long Col3
        {
            get { return (long)this["col3"]; }
            set { this["col3"] = value; }
        }

        public DateTime Col4
        {
            get { return (DateTime)this["col4"]; }
            set { this["col4"] = value; }
        }

        public Gender Col5
        {
            get { return (Gender)this["col5"]; }
            set { this["col5"] = value; }
        }

        public byte Col6
        {
            get { return (byte)this["col6"]; }
            set { this["col6"] = value; }
        }

        public byte[] Col7
        {
            get { return (byte[])this["col7"]; }
            set { this["col7"] = value; }
        }
    }
}