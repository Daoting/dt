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
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class EntityDemo : Win
    {
        public EntityDemo()
        {
            InitializeComponent();

            _fv.Data = new MyEntityObj
            (
                ID: 100,
                MaxLength: "测试"
            );
        }
    }

    public partial class MyEntityObj
    {
        void SetMaxLength(string p_value)
        {
            Throw.If(p_value.Length > 3, "最大长度3");
        }

        void SetSrc(string p_value)
        {
            Tgt = p_value;
        }

        void SetIsCheck(bool p_value)
        {
            Throw.If(p_value && string.IsNullOrEmpty(Src), "联动源不为空");
        }
    }

    #region 自动生成
    [Tbl("pub_my")]
    public partial class MyEntityObj : Entity
    {
        #region 构造方法
        MyEntityObj() { }

        public MyEntityObj(
            long ID,
            string MaxLength = default,
            string Src = default,
            string Tgt = default,
            bool IsCheck = default)
        {
            AddCell<long>("ID", ID);
            AddCell<string>("MaxLength", MaxLength);
            AddCell<string>("Src", Src);
            AddCell<string>("Tgt", Tgt);
            AddCell<bool>("IsCheck", IsCheck);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        public string MaxLength
        {
            get { return (string)this["MaxLength"]; }
            set { this["MaxLength"] = value; }
        }

        public string Src
        {
            get { return (string)this["Src"]; }
            set { this["Src"] = value; }
        }

        public string Tgt
        {
            get { return (string)this["Tgt"]; }
            set { this["Tgt"] = value; }
        }

        public bool IsCheck
        {
            get { return (bool)this["IsCheck"]; }
            set { this["IsCheck"] = value; }
        }
        #endregion
    }
    #endregion
}