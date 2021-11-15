using Dt.Core;
using Dt.Core.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dt.Sample
{
    public static class AtTest
    {
        #region TestSerialize
        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <returns></returns>
        public static Task<string> GetString()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetString"
            ).Call<string>();
        }

        /// <summary>
        /// 字符串参数
        /// </summary>
        /// <param name="p_str"></param>
        public static Task<bool> SetString(string p_str)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetString",
                p_str
            ).Call<bool>();
        }

        /// <summary>
        /// 返回bool值
        /// </summary>
        /// <returns></returns>
        public static Task<bool> GetBool()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetBool"
            ).Call<bool>();
        }

        /// <summary>
        /// bool参数
        /// </summary>
        /// <param name="p_val"></param>
        public static Task<bool> SetBool(bool p_val)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetBool",
                p_val
            ).Call<bool>();
        }

        /// <summary>
        /// 返回int值
        /// </summary>
        /// <returns></returns>
        public static Task<int> GetInt()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetInt"
            ).Call<int>();
        }

        /// <summary>
        /// int参数
        /// </summary>
        /// <param name="p_val"></param>
        public static Task<int> SetInt(int p_val)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetInt",
                p_val
            ).Call<int>();
        }

        /// <summary>
        /// 返回long值
        /// </summary>
        /// <returns></returns>
        public static Task<long> GetLong()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetLong"
            ).Call<long>();
        }

        /// <summary>
        /// long参数
        /// </summary>
        /// <param name="p_val"></param>
        public static Task<long> SetLong(long p_val)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetLong",
                p_val
            ).Call<long>();
        }

        /// <summary>
        /// 返回double值
        /// </summary>
        /// <returns></returns>
        public static Task<Double> GetDouble()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetDouble"
            ).Call<Double>();
        }

        /// <summary>
        /// double参数
        /// </summary>
        /// <param name="p_val"></param>
        public static Task<Double> SetDouble(Double p_val)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetDouble",
                p_val
            ).Call<Double>();
        }

        /// <summary>
        /// 返回DateTime值
        /// </summary>
        /// <returns></returns>
        public static Task<DateTime> GetDateTime()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetDateTime"
            ).Call<DateTime>();
        }

        /// <summary>
        /// DateTime参数
        /// </summary>
        /// <param name="p_val"></param>
        public static Task<DateTime> SetDateTime(DateTime p_val)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetDateTime",
                p_val
            ).Call<DateTime>();
        }

        /// <summary>
        /// 返回byte[]值
        /// </summary>
        /// <returns></returns>
        public static Task<Byte[]> GetByteArray()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetByteArray"
            ).Call<Byte[]>();
        }

        /// <summary>
        /// byte[]参数
        /// </summary>
        /// <param name="p_val"></param>
        public static Task<Byte[]> SetByteArray(Byte[] p_val)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetByteArray",
                p_val
            ).Call<Byte[]>();
        }

        /// <summary>
        /// 返回MsgInfo
        /// </summary>
        /// <returns></returns>
        public static Task<MsgInfo> GetMsgInfo()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetMsgInfo"
            ).Call<MsgInfo>();
        }

        /// <summary>
        /// MsgInfo参数
        /// </summary>
        /// <param name="p_msg"></param>
        public static Task<MsgInfo> SetMsgInfo(MsgInfo p_msg)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetMsgInfo",
                p_msg
            ).Call<MsgInfo>();
        }

        /// <summary>
        /// 返回字符串数组
        /// </summary>
        /// <returns></returns>
        public static Task<List<string>> GetStringList()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetStringList"
            ).Call<List<string>>();
        }

        /// <summary>
        /// 字符串列表
        /// </summary>
        /// <param name="p_ls"></param>
        public static Task<bool> SetStringList(List<string> p_ls)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetStringList",
                p_ls
            ).Call<bool>();
        }

        /// <summary>
        /// 返回bool值列表
        /// </summary>
        /// <returns></returns>
        public static Task<List<bool>> GetBoolList()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetBoolList"
            ).Call<List<bool>>();
        }

        /// <summary>
        /// bool值列表
        /// </summary>
        /// <param name="p_val"></param>
        public static Task<List<bool>> SetBoolList(List<bool> p_val)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetBoolList",
                p_val
            ).Call<List<bool>>();
        }

        /// <summary>
        /// 返回int值列表
        /// </summary>
        /// <returns></returns>
        public static Task<List<int>> GetIntList()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetIntList"
            ).Call<List<int>>();
        }

        /// <summary>
        /// int列表
        /// </summary>
        /// <param name="p_val"></param>
        public static Task<List<int>> SetIntList(List<int> p_val)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetIntList",
                p_val
            ).Call<List<int>>();
        }

        /// <summary>
        /// 返回long值列表
        /// </summary>
        /// <returns></returns>
        public static Task<List<long>> GetLongList()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetLongList"
            ).Call<List<long>>();
        }

        /// <summary>
        /// long列表
        /// </summary>
        /// <param name="p_val"></param>
        public static Task<List<long>> SetLongList(List<long> p_val)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetLongList",
                p_val
            ).Call<List<long>>();
        }

        /// <summary>
        /// 返回double值列表
        /// </summary>
        /// <returns></returns>
        public static Task<List<double>> GetDoubleList()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetDoubleList"
            ).Call<List<double>>();
        }

        /// <summary>
        /// double列表
        /// </summary>
        /// <param name="p_val"></param>
        public static Task<List<double>> SetDoubleList(List<double> p_val)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetDoubleList",
                p_val
            ).Call<List<double>>();
        }

        /// <summary>
        /// DateTime列表
        /// </summary>
        /// <returns></returns>
        public static Task<List<DateTime>> GetDateTimeList()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetDateTimeList"
            ).Call<List<DateTime>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_times"></param>
        /// <returns></returns>
        public static Task<bool> SetDateTimeList(List<DateTime> p_times)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetDateTimeList",
                p_times
            ).Call<bool>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Task<List<object>> GetObjectList()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetObjectList"
            ).Call<List<object>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_ls"></param>
        /// <returns></returns>
        public static Task<bool> SetObjectList(params object[] p_ls)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetObjectList",
                (p_ls == null || p_ls.Length == 0) ? null : p_ls.ToList()
            ).Call<bool>();
        }

        /// <summary>
        /// 返回Table到客户端
        /// </summary>
        /// <returns></returns>
        public static Task<Table> GetTable()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetTable"
            ).Call<Table>();
        }

        /// <summary>
        /// 由外部传递Table
        /// </summary>
        /// <param name="p_tbl"></param>
        public static Task<Table> SetTable(Table p_tbl)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetTable",
                p_tbl
            ).Call<Table>();
        }

        /// <summary>
        /// 返回Row到客户端
        /// </summary>
        /// <returns></returns>
        public static Task<Row> GetRow()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetRow"
            ).Call<Row>();
        }

        /// <summary>
        /// 由外部传递Row
        /// </summary>
        /// <param name="p_row"></param>
        public static Task<Row> SetRow(Row p_row)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetRow",
                p_row
            ).Call<Row>();
        }

        public static Task<Table<CustomEntityObj>> GetEntityTable()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetEntityTable"
            ).Call<Table<CustomEntityObj>>();
        }

        public static Task<bool> SetEntityTable(Table p_tbl)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetEntityTable",
                p_tbl
            ).Call<bool>();
        }

        public static Task<CustomEntityObj> GetEntity()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetEntity"
            ).Call<CustomEntityObj>();
        }

        public static Task<bool> SetEntity(Row p_entity)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetEntity",
                p_entity
            ).Call<bool>();
        }

        /// <summary>
        /// 返回多个Table到客户端
        /// </summary>
        /// <returns></returns>
        public static Task<Dict> GetTableDict()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetTableDict"
            ).Call<Dict>();
        }

        /// <summary>
        /// 由外部传递多个Table
        /// </summary>
        /// <param name="p_dict"></param>
        public static Task<Dict> SetTableDict(Dict p_dict)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetTableDict",
                p_dict
            ).Call<Dict>();
        }

        /// <summary>
        /// 返回多个Table到客户端
        /// </summary>
        /// <returns></returns>
        public static Task<List<Table>> GetTableList()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetTableList"
            ).Call<List<Table>>();
        }

        /// <summary>
        /// 由外部传递多个Table
        /// </summary>
        /// <param name="p_ls"></param>
        public static Task<List<Table>> SetTableList(List<Table> p_ls)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetTableList",
                p_ls
            ).Call<List<Table>>();
        }

        /// <summary>
        /// 返回基本数据类型的Dict
        /// </summary>
        /// <returns></returns>
        public static Task<Dict> GetBaseDict()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetBaseDict"
            ).Call<Dict>();
        }

        /// <summary>
        /// 返回基本数据类型的Dict
        /// </summary>
        /// <returns></returns>
        public static Task<Dict> GetCombineDict()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetCombineDict"
            ).Call<Dict>();
        }

        /// <summary>
        /// 本数据类型的Dict
        /// </summary>
        /// <param name="p_dict"></param>
        public static Task<Dict> SendDict(Dict p_dict)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SendDict",
                p_dict
            ).Call<Dict>();
        }

        /// <summary>
        /// 返回Dict列表
        /// </summary>
        /// <returns></returns>
        public static Task<List<Dict>> GetDictList()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetDictList"
            ).Call<List<Dict>>();
        }

        /// <summary>
        /// 发送Dict列表
        /// </summary>
        /// <param name="p_dicts"></param>
        public static Task<bool> SendDictList(List<Dict> p_dicts)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SendDictList",
                p_dicts
            ).Call<bool>();
        }

        /// <summary>
        /// 返回基础自定义类型
        /// </summary>
        /// <returns></returns>
        public static Task<Product> GetCustomBase()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetCustomBase"
            ).Call<Product>();
        }

        /// <summary>
        /// 由外部传递基础自定义类型
        /// </summary>
        /// <param name="p_product"></param>
        /// <returns></returns>
        public static Task<bool> SetCustomBase(Product p_product)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetCustomBase",
                p_product
            ).Call<bool>();
        }

        /// <summary>
        /// 返回复杂自定义类型
        /// </summary>
        /// <returns></returns>
        public static Task<Student> GetCustomCombine()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetCustomCombine"
            ).Call<Student>();
        }

        /// <summary>
        /// 由外部传递复杂自定义类型
        /// </summary>
        /// <param name="p_person"></param>
        /// <returns></returns>
        public static Task<bool> SetCustomCombine(Student p_person)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetCustomCombine",
                p_person
            ).Call<bool>();
        }

        /// <summary>
        /// 返回嵌套自定义类型
        /// </summary>
        /// <returns></returns>
        public static Task<Department> GetContainCustom()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetContainCustom"
            ).Call<Department>();
        }

        /// <summary>
        /// 由外部传递嵌套自定义类型
        /// </summary>
        /// <param name="p_dept"></param>
        /// <returns></returns>
        public static Task<bool> SetContainCustom(Department p_dept)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetContainCustom",
                p_dept
            ).Call<bool>();
        }

        public static Task AsyncVoid(string p_msg)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.AsyncVoid",
                p_msg
            ).Call<object>();
        }

        public static Task<Table> AsyncDb()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.AsyncDb"
            ).Call<Table>();
        }

        public static Task<int> AsyncWait()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.AsyncWait"
            ).Call<int>();
        }
        #endregion

        #region TestException
        public static Task<string> ThrowException()
        {
            return new UnaryRpc(
                "cm",
                "TestException.ThrowException"
            ).Call<string>();
        }

        public static Task<string> ThrowBusinessException()
        {
            return new UnaryRpc(
                "cm",
                "TestException.ThrowBusinessException"
            ).Call<string>();
        }

        public static Task<string> ThrowPostionException()
        {
            return new UnaryRpc(
                "cm",
                "TestException.ThrowPostionException"
            ).Call<string>();
        }

        public static Task<Dict> ThrowSerializeException()
        {
            return new UnaryRpc(
                "cm",
                "TestException.ThrowSerializeException"
            ).Call<Dict>();
        }
        #endregion
    }
}
