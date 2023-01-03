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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
                MaxLength: "测试",
                NotNull: "非空"
            );
        }

        void OnNoBinding(object sender, RoutedEventArgs e)
        {
            _fv.Data.To<MyEntityObj>().NoBinding = new Random().Next(100);
        }

        void OnNoHook(object sender, RoutedEventArgs e)
        {
            _fv.Data.To<MyEntityObj>().NoHook = new Random().Next(100);
        }
    }

    public partial class MyEntityObj
    {
        protected override void OnHook()
        {
            Hook<string>(nameof(MaxLength), (v) => Throw.If(v.Length > 3, "最大长度3"));
            Hook<string>(nameof(NotNull), (v) => Throw.If(string.IsNullOrEmpty(v), "不可为空"));
            Hook<string>(nameof(Src), (v) => Tgt = v);
            Hook<bool>(nameof(IsCheck), (v) => Throw.If(v && string.IsNullOrEmpty(Src), "联动源不为空"));

            Hook<int>(nameof(NoBinding), (v) =>
            {
                Kit.Msg("新值：" + v.ToString());
                Throw.If(v > 50, "最大值不可超过50");
            });
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
            string NotNull = default,
            string Src = default,
            string Tgt = default,
            bool IsCheck = default,
            int NoBinding = default,
            int NoHook = default)
        {
            AddCell<long>("ID", ID);
            AddCell<string>("MaxLength", MaxLength);
            AddCell<string>("NotNull", NotNull);
            AddCell<string>("Src", Src);
            AddCell<string>("Tgt", Tgt);
            AddCell<bool>("IsCheck", IsCheck);
            AddCell<int>("NoBinding", NoBinding);
            AddCell<int>("NoHook", NoHook);
            IsAdded = true;
        }
        #endregion

        #region 属性
        public string MaxLength
        {
            get { return (string)this["MaxLength"]; }
            set { this["MaxLength"] = value; }
        }

        public string NotNull
        {
            get { return (string)this["NotNull"]; }
            set { this["NotNull"] = value; }
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

        public int NoBinding
        {
            get { return (int)this["NoBinding"]; }
            set { this["NoBinding"] = value; }
        }

        public int NoHook
        {
            get { return (int)this["NoHook"]; }
            set { this["NoHook"] = value; }
        }
        #endregion
    }
    #endregion
}