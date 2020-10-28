#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Publish
{
    #region 自动生成
    [Tbl("pub_postkeyword", "pub")]
    public partial class Postkeyword : Entity
    {
        #region 构造方法
        Postkeyword() { }

        public Postkeyword(
            long PostID,
            string Keyword)
        {
            AddCell<long>("PostID", PostID);
            AddCell<string>("Keyword", Keyword);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 文章标识
        /// </summary>
        public long PostID
        {
            get { return (long)this["PostID"]; }
            set { this["PostID"] = value; }
        }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keyword
        {
            get { return (string)this["Keyword"]; }
            set { this["Keyword"] = value; }
        }

        new public long ID { get { return -1; } }
        #endregion

        #region 可复制
        /*
        void OnSaving()
        {
        }

        void OnDeleting()
        {
        }

        void SetPostID(long p_value)
        {
        }

        void SetKeyword(string p_value)
        {
        }
        */
        #endregion
    }
    #endregion
}
