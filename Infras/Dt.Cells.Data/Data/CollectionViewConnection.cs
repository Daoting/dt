#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.UI.Xaml.Data;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// AOT时出错，不使用CollectionViewSource
    /// </summary>
    internal sealed class CollectionViewConnection : ConnectionBase
    {
        Dictionary<string, BindingField> _fields;
        bool _isValueCollection;
        Type _valType;

        public override bool CanOpen()
        {
            return (base.DataSource is IEnumerable);
        }

        public override void Close()
        {
            if (_fields != null)
                _fields.Clear();
            base.Close();
        }

        public override Type GetColumnDataType(string field)
        {
            if (_isValueCollection && _valType != null)
                return _valType;
            
            if (_fields != null)
            {
                BindingField field2 = _fields[field];
                if (field2 != null)
                {
                    return field2.DataType;
                }
            }
            return base.GetColumnDataType(field);
        }

        static object GetFiledValueFromRecord(BindingField bindingField, object record)
        {
            if ((bindingField.propertyInfo != null) && bindingField.propertyInfo.CanRead)
            {
                return bindingField.propertyInfo.GetValue(record, null);
            }
            return null;
        }

        protected override object GetRecord(int recordIndex)
        {
            try
            {
                if (DataSource is IList ic)
                {
                    return ic[recordIndex];
                }

                if (DataSource is IEnumerable ie)
                {
                    int count = 0;
                    foreach (var item in ie)
                    {
                        if (count == recordIndex)
                            return item;
                        count++;
                    }
                    return null;
                }
            }
            catch { }
            return null;
        }

        public override int GetRecordCount()
        {
            if (base.DataSource == null)
            {
                return 0;
            }
            if (DataSource is ICollection ic)
            {
                return ic.Count;
            }

            if (DataSource is IEnumerable ie)
            {
                int count = 0;
                foreach (var item in ie)
                {
                    count++;
                }
                return count;
            }
            return 0;
        }

        protected override object GetRecordValue(object record, string field)
        {
            if (_isValueCollection)
            {
                return record;
            }
            
            try
            {
                if ((record != null) && (_fields != null))
                {
                    BindingField bindingField = null;
                    _fields.TryGetValue(field, out bindingField);
                    if (bindingField != null)
                    {
                        return GetFiledValueFromRecord(bindingField, record);
                    }
                }
            }
            catch { }
            return null;
        }

        public override void Open()
        {
            if (DataSource is IEnumerable && !IsOpen)
            {
                base.Open();
                CreateAvailableFields();
            }
        }

        void CreateAvailableFields()
        {
            var ls = DataSource as IEnumerable;
            foreach (var item in ls)
            {
                if (item != null)
                {
                    var tp = item.GetType();
                    if (tp == typeof(string) || tp.IsValueType)
                    {
                        // 值类型或字符串
                        _isValueCollection = true;
                        _valType = tp;
                    }
                    else
                    {
                        Dictionary<string, BindingField> fieldsTemp = new Dictionary<string, BindingField>();
                        // hdt
                        foreach (PropertyInfo info in tp.GetRuntimeProperties())
                        {
                            ParameterInfo[] indexParameters = info.GetIndexParameters();
                            if ((indexParameters == null) || (indexParameters.Length == 0))
                            {
                                fieldsTemp.Add(info.Name, new BindingField(info.Name, info));
                            }
                        }
                        _fields = fieldsTemp;
                        _isValueCollection = false;
                    }
                    return;
                }
            }
        }

        static void SetFiledValueFromRecord(BindingField bindingField, object record, object value)
        {
            if ((bindingField.propertyInfo != null) && bindingField.propertyInfo.CanWrite)
            {
                bindingField.propertyInfo.SetValue(record, value, null);
            }
        }

        protected override void SetRecordValue(object record, string field, object value)
        {
            if ((record != null) && (_fields != null))
            {
                BindingField bindingField = null;
                _fields.TryGetValue(field, out bindingField);
                if (bindingField != null)
                {
                    SetFiledValueFromRecord(bindingField, record, value);
                }
            }
        }

        internal override void UpdateCollectionView()
        {
        }

        public override string[] DataFields
        {
            get
            {
                if (_isValueCollection)
                {
                    return new string[] { "#a" };
                }
                
                if (_fields != null)
                {
                    return Enumerable.ToArray<string>((IEnumerable<string>)_fields.Keys);
                }
                return new string[0];
            }
        }

        internal sealed class BindingField : ICloneable
        {
            int index;
            string name;
            public PropertyInfo propertyInfo;

            BindingField()
            {
            }

            public BindingField(string name, PropertyInfo pi)
            {
                this.name = name;
                propertyInfo = pi;
            }

            public object Clone()
            {
                return new CollectionViewConnection.BindingField { name = name, index = index, propertyInfo = propertyInfo };
            }

            public Type DataType
            {
                get
                {
                    if (propertyInfo != null)
                    {
                        return propertyInfo.PropertyType;
                    }
                    return typeof(object);
                }
            }

            public int Index
            {
                get { return index; }
                set { index = value; }
            }

            public string Name
            {
                get { return name; }
            }
        }
    }
}

