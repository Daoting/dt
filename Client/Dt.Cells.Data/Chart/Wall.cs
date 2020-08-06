#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using System.Xml;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// 
    /// </summary>
    internal class Wall : SpreadChartElement
    {
        int _thickness;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Wall" /> class.
        /// </summary>
        public Wall()
        {
            this._thickness = 1;
        }

        internal Wall(SpreadChart owner) : base(owner)
        {
            this._thickness = 1;
        }

        internal override void ReadXmlInternal(XmlReader reader)
        {
            string str;
            base.ReadXmlInternal(reader);
            if (((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null)) && (str == "FloatingObjectStyleInfo"))
            {
                this._thickness = (int) ((int) Serializer.DeserializeObj(typeof(int), reader));
            }
        }

        internal override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);
            if (this._thickness != 1)
            {
                Serializer.SerializeObj((int) this._thickness, "Thickness", writer);
            }
        }

        internal SpreadChart Chart
        {
            get { return (((ISpreadChartElement)this).Chart as SpreadChart); }
            set { ((ISpreadChartElement)this).Chart = value; }
        }

        internal override Dt.Cells.Data.ChartArea ChartArea
        {
            get
            {
                if (this.WallType == Dt.Cells.Data.WallType.FloorWall)
                {
                    return Dt.Cells.Data.ChartArea.FloorWall;
                }
                if (this.WallType == Dt.Cells.Data.WallType.SideWall)
                {
                    return Dt.Cells.Data.ChartArea.SideWall;
                }
                return Dt.Cells.Data.ChartArea.BackWall;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Thickness
        {
            get { return  this._thickness; }
            set
            {
                if (value != this.Thickness)
                {
                    this._thickness = value;
                    ((ISpreadChartElement) this).NotifyElementChanged("Thickness");
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the wall.
        /// </summary>
        /// <value>
        /// The type of the wall.
        /// </value>
        public Dt.Cells.Data.WallType WallType { get; set; }
    }
}

