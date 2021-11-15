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
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Cells.Data
{
    internal sealed class CollectionViewConnection : ConnectionBase
    {
        ICollectionView collectionView;
        Dictionary<string, BindingField> fields = new Dictionary<string, BindingField>();

        static ICollectionView BuildView(object source)
        {
            if ((source is IEnumerable) && !((IEnumerable) source).GetEnumerator().MoveNext())
            {
                return null;
            }
            ICollectionView view = source as ICollectionView;
            if (view != null)
            {
                return view;
            }
            ICollectionViewFactory factory = source as ICollectionViewFactory;
            if (factory != null)
            {
                return factory.CreateView();
            }
            var viewSource = new CollectionViewSource();
            viewSource.Source = source;
            view = viewSource.View;
            return view;
        }

        public override bool CanOpen()
        {
            return (base.DataSource is IEnumerable);
        }

        public override void Close()
        {
            if (this.fields != null)
            {
                this.fields.Clear();
            }
            this.collectionView = null;
            base.Close();
        }

        void CreateAvailableFields()
        {
            if ((this.Context != null) && ((this.fields == null) || (this.fields.Count == 0)))
            {
                Dictionary<string, BindingField> fieldsTemp = new Dictionary<string, BindingField>();
                if ((this.fields == null) || (this.fields.Count == 0))
                {
                    while (this.Context.CurrentItem != null)
                    {
                        object currentItem = this.Context.CurrentItem;
                        if (currentItem != null)
                        {
                            // hdt
                            foreach (PropertyInfo info in currentItem.GetType().GetRuntimeProperties())
                            {
                                ParameterInfo[] indexParameters = info.GetIndexParameters();
                                if ((indexParameters == null) || (indexParameters.Length == 0))
                                {
                                    fieldsTemp.Add(info.Name, new BindingField(info.Name, info));
                                }
                            }
                            this.fields = fieldsTemp;
                            return;
                        }
                        this.Context.MoveCurrentToNext();
                    }
                }
            }
        }

        public override Type GetColumnDataType(string field)
        {
            if (this.fields != null)
            {
                BindingField field2 = this.fields[field];
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
                if ((this.Context != null) && this.Context.MoveCurrentToPosition(recordIndex))
                {
                    return this.Context.CurrentItem;
                }
            }
            catch
            {
            }
            return null;
        }

        public override int GetRecordCount()
        {
            if (base.DataSource == null)
            {
                return 0;
            }
            if (this.Context == null)
            {
                return 0;
            }
            return this.Context.Count;
        }

        protected override object GetRecordValue(object record, string field)
        {
            try
            {
                if ((record != null) && (this.fields != null))
                {
                    BindingField bindingField = null;
                    this.fields.TryGetValue(field, out bindingField);
                    if (bindingField != null)
                    {
                        return GetFiledValueFromRecord(bindingField, record);
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        public override void Open()
        {
            if (!base.IsOpen)
            {
                base.Open();
                if (base.IsOpen)
                {
                    this.collectionView = BuildView(base.DataSource);
                    this.CreateAvailableFields();
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
            if ((record != null) && (this.fields != null))
            {
                BindingField bindingField = null;
                this.fields.TryGetValue(field, out bindingField);
                if (bindingField != null)
                {
                    SetFiledValueFromRecord(bindingField, record, value);
                }
            }
        }

        internal override void UpdateCollectionView()
        {
            this.collectionView = BuildView(base.DataSource);
        }

        ICollectionView Context
        {
            get { return  this.collectionView; }
        }

        public override string[] DataFields
        {
            get
            {
                if (this.fields != null)
                {
                    return Enumerable.ToArray<string>((IEnumerable<string>) this.fields.Keys);
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
                this.propertyInfo = pi;
            }

            public object Clone()
            {
                return new CollectionViewConnection.BindingField { name = this.name, index = this.index, propertyInfo = this.propertyInfo };
            }

            public Type DataType
            {
                get
                {
                    if (this.propertyInfo != null)
                    {
                        return this.propertyInfo.PropertyType;
                    }
                    return typeof(object);
                }
            }

            public int Index
            {
                get { return  this.index; }
                set { this.index = value; }
            }

            public string Name
            {
                get { return  this.name; }
            }
        }
    }
}

