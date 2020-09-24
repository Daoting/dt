#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml;

#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 矩阵行头列头基类
    /// </summary>
    internal abstract class RptMtxHeader : RptItemBase
    {
        #region 变量
        readonly RptMatrix _matrix;
        List<RptMtxLevel> _levels = new List<RptMtxLevel>();
        #endregion

        #region 构造
        public RptMtxHeader(RptMatrix p_matrix)
        {
            _matrix = p_matrix;
        }
        #endregion

        #region 外部属性
        /// <summary>
        /// 获取报表模板根对象
        /// </summary>
        public override RptRoot Root
        {
            get { return _matrix.Root; }
        }

        /// <summary>
        /// 获取报表项所属容器
        /// </summary>
        public override RptPart Part
        {
            get { return _matrix.Part; }
        }

        /// <summary>
        /// 获取报表项所属父项
        /// </summary>
        public override RptItemBase Parent
        {
            get { return _matrix; }
        }

        /// <summary>
        /// 获取设置报表元素起始行索引
        /// </summary>
        public override int Row
        {
            get 
            { 
                return this is RptMtxRowHeader ? _matrix.Row + _matrix.ColHeader.RowSpan : _matrix.Row; 
            }
            set { }
        }

        /// <summary>
        /// 获取设置报表元素起始列索引
        /// </summary>
        public override int Col
        {
            get 
            { 
                return this is RptMtxRowHeader ? _matrix.Col : _matrix.Col + _matrix.RowHeader.ColSpan; 
            }
            set { }
        }

        /// <summary>
        /// 层次集合
        /// </summary>
        public List<RptMtxLevel> Levels
        {
            get { return _levels; }
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 读取子元素xml，结束时定位在该子元素的末尾元素上
        /// </summary>
        /// <param name="p_reader"></param>
        protected override void ReadChildXml(XmlReader p_reader)
        {
            if (p_reader.Name == "Level")
            {
                RptMtxLevel level = new RptMtxLevel(this);
                level.ReadXml(p_reader);
                _levels.Add(level);
            }
        }

        /// <summary>
        /// 序列化子元素
        /// </summary>
        /// <param name="p_writer"></param>
        protected void WriteChildXml(XmlWriter p_writer)
        {
            if (Levels.Count > 0)
            {
                foreach (RptMtxLevel level in Levels)
                {
                    level.WriteXml(p_writer);
                }
            }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 获取层次对应数据列
        /// </summary>
        /// <returns></returns>
        public List<string> GetFieldNames()
        {
            List<string> fields = new List<string>();
            foreach (RptMtxLevel level in Levels)
            {
                fields.Add(level.Field);
            }
            return fields;
        }
        #endregion
    }

    /// <summary>
    /// 行头样式
    /// </summary>
    public enum RptMtxHeaderType
    {
        /// <summary>
        /// 行
        /// </summary>
        Row,

        /// <summary>
        /// 列
        /// </summary>
        Col
    }
}
