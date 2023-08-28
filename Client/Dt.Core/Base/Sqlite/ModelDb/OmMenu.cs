#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Data;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 业务菜单，在模型库中
    /// </summary>
    [Bindable]
    public class OmMenu : Entity
    {
        #region 构造方法
        OmMenu() { }

        public OmMenu(
            long ID,
            long? ParentID = default,
            string Name = default,
            bool IsGroup = default,
            string ViewName = default,
            string Params = default,
            string Icon = default,
            string Note = default,
            int Dispidx = default,
            string Warning = default)
        {
            Add("ID", ID);
            Add("ParentID", ParentID);
            Add("Name", Name);
            Add("IsGroup", IsGroup);
            Add("ViewName", ViewName);
            Add("Params", Params);
            Add("Icon", Icon);
            Add("Note", Note);
            Add("Dispidx", Dispidx);
            Add("Warning", Warning);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 标识ID
        /// </summary>
        new public long ID
        {
            get { return (long)this["ID"]; }
            set { this["ID"] = value; }
        }

        /// <summary>
        /// 父菜单项ID
        /// </summary>
        public long? ParentID
        {
            get { return (long?)this["ParentID"]; }
            set { this["ParentID"] = value; }
        }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 菜单类型是否为分组
        /// </summary>
        public bool IsGroup
        {
            get { return (bool)this["IsGroup"]; }
            set { this["IsGroup"] = value; }
        }

        /// <summary>
        /// 菜单对应的视图名称
        /// </summary>
        public string ViewName
        {
            get { return (string)this["ViewName"]; }
            set { this["ViewName"] = value; }
        }

        /// <summary>
        /// 菜单参数
        /// </summary>
        public string Params
        {
            get { return (string)this["Params"]; }
            set { this["Params"] = value; }
        }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon
        {
            get
            {
                var icon = (string)this["Icon"];
                if (string.IsNullOrEmpty(icon))
                    icon = IsGroup ? "文件夹" : "文件";
                return icon;
            }
            set { this["Icon"] = value; }
        }

        /// <summary>
        /// 菜单描述
        /// </summary>
        public string Note
        {
            get { return (string)this["Note"]; }
            set { this["Note"] = value; }
        }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int DispIdx
        {
            get { return (int)this["Dispidx"]; }
            set { this["Dispidx"] = value; }
        }

        /// <summary>
        /// 菜单项醒目提示的数字
        /// </summary>
        public string Warning
        {
            get
            {
                if (!Contains("Warning"))
                    Add("Warning", "");
                return (string)this["Warning"];
            }
        }

        /// <summary>
        /// 设置菜单项醒目提示的数字
        /// </summary>
        /// <param name="p_num"></param>
        public void SetWarningNum(int p_num)
        {
            string msg;
            if (p_num == 0)
                msg = null;
            else if (p_num < 100)
                msg = p_num.ToString();
            else
                msg = "┅";

            if (!Contains("Warning"))
                Add("Warning", msg);
            else
                this["Warning"] = msg;
        }
    }
}
