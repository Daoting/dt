#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Caches;
using Dt.Core.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm
{
    [Tbl("cm_user")]
    public class User : Root
    {
        public User()
        {
            ID = Id.New(0);
        }

        /// <summary>
        /// 手机号，唯一
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 密码的MD5
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public bool Sex { get; private set; }

        /// <summary>
        /// 分组id
        /// </summary>
        public long? GroupID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime MTime { get; set; }

    }

    [Tbl("cm_menu")]
    public partial class Menu : Root
    {
        public Menu()
        { }

        public Menu(long p_parentid, string p_name, bool p_isgroup, string p_viewname, string p_params, string p_icon, string p_srvname, string p_note, int p_dispidx, bool p_islocked, DateTime p_ctime, DateTime p_mtime)
        {
            Parentid = p_parentid;
            Name = p_name;
            Isgroup = p_isgroup;
            Viewname = p_viewname;
            Params = p_params;
            Icon = p_icon;
            Srvname = p_srvname;
            Note = p_note;
            Dispidx = p_dispidx;
            Islocked = p_islocked;
            Ctime = p_ctime;
            Mtime = p_mtime;
        }

        /// <summary>
        /// 父菜单标识
        /// </summary>
        public long? Parentid { get; private set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 分组或实例。0表实例，1表分组
        /// </summary>
        public bool Isgroup { get; private set; }

        /// <summary>
        /// 视图名称
        /// </summary>
        public string Viewname { get; private set; }

        /// <summary>
        /// 传递给菜单程序的参数
        /// </summary>
        public string Params { get; private set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; private set; }

        /// <summary>
        /// 提供提示信息的服务名称，空表示无提示信息
        /// </summary>
        public string Srvname { get; private set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; private set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx { get; private set; }

        /// <summary>
        /// 定义了菜单是否被锁定。0表未锁定，1表锁定不可用
        /// </summary>
        public bool Islocked { get; private set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime { get; private set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime Mtime { get; private set; }
    }
}
