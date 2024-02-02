#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-01-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Tools
{
    public sealed partial class DbInit : Tab
    {
        DbInitInfo _info;

        public DbInit()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        protected override void OnFirstLoaded()
        {
            _info = (DbInitInfo)NaviParams;
            if (_info.DbType == DatabaseType.Oracle)
                _fv["NewDb"].Title = "表空间";

            _info.Tools = GetTools();
            _fv.Data = _info;
        }
        
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            // 从下一页面返回时也需设置
            _info.Log = AppendMsg;
        }

        async void OnConnect(object sender, RoutedEventArgs e)
        {
            if (_fv.ExistNull("NewDb", "NewUser", "NewPwd"))
                return;

            try
            {
                ToggleButton(true);
                var db = _info.Tools;
                bool existsDb = await db.ExistsDb();
                bool existsUser = await db.ExistsUser();
                string msg = null;
                if (!existsDb)
                {
                    msg = "数据库";
                }
                if (!existsUser)
                {
                    if (msg == null)
                        msg = "用户";
                    else
                        msg += "、用户";
                }

                if (msg != null)
                {
                    msg += "不存在！";
                }
                else if (!await db.IsPwdCorrect())
                {
                    msg = "密码不正确！";
                }

                if (msg != null)
                {
                    Kit.Error(msg);
                }
                else
                {
                    Forward(new DbImport(), _info);
                }
            }
            catch { }
            finally
            {
                ToggleButton(false);
            }
        }

        async void OnCreate(object sender, RoutedEventArgs e)
        {
            if (_fv.ExistNull("NewDb", "NewUser", "NewPwd"))
                return;

            if (!await Kit.Confirm("创建新库时若库或用户已存在将删除重建！！！\r\n是否继续？"))
                return;

            try
            {
                ToggleButton(true);
                _tbInfo.Text = "";
                var db = _info.Tools;
                bool existsDb = await db.ExistsDb();
                bool existsUser = await db.ExistsUser();
                string msg = null;

                if (existsDb)
                {
                    msg = "数据库";
                }
                if (existsUser)
                {
                    if (msg == null)
                        msg = "用户";
                    else
                        msg += "、用户";
                }

                if (msg != null)
                {
                    msg += "已存在，\r\n点击【确定】将删除重建！\r\n需要【确定】多次避免误操作！";
                    if (!await Kit.Confirm(msg + "\r\n3")
                        || !await Kit.Confirm(msg + "\r\n2")
                        || !await Kit.Confirm(msg + "\r\n1"))
                    {
                        ToggleButton(false);
                        return;
                    }
                    _tbInfo.Text = $"创建新库({_info.DbType})\r数据库、用户将删除重建";
                }
                else
                {
                    _tbInfo.Text = $"创建新库({_info.DbType})\r数据库、用户都不存在";
                }
                await _info.Tools.CreateDb();
            }
            catch { }
            finally
            {
                ToggleButton(false);
            }
        }

        async void OnDelete(object sender, RoutedEventArgs e)
        {
            if (_fv.ExistNull("NewDb", "NewUser", "NewPwd"))
                return;

            string alert = "删除数据库后不可恢复！！！\r\n是否继续？";
            if (!await Kit.Confirm(alert))
                return;

            try
            {
                ToggleButton(true);
                _tbInfo.Text = "";
                var db = _info.Tools;
                bool existsDb = await db.ExistsDb();
                bool existsUser = await db.ExistsUser();

                string msg = null;
                if (!existsDb)
                {
                    msg = "数据库";
                }
                if (!existsUser)
                {
                    if (msg == null)
                        msg = "用户";
                    else
                        msg += "、用户";
                }

                if (msg != null)
                {
                    Kit.Error(msg + "不存在！");
                }
                else
                {
                    alert += "\r\n需要【确定】多次避免误操作！";
                    if (await Kit.Confirm(alert + "\r\n3")
                        && await Kit.Confirm(alert + "\r\n2")
                        && await Kit.Confirm(alert + "\r\n1"))
                    {
                        _tbInfo.Text = "已确认删除";
                        await _info.Tools.DeleteDb();
                    }
                }
            }
            catch { }
            finally
            {
                ToggleButton(false);
            }
        }

        IDbTools GetTools()
        {
            switch (_info.DbType)
            {
                case DatabaseType.PostgreSql:
                    return new PostgreSqlTools(_info);

                case DatabaseType.MySql:
                    return new MySqlTools(_info);

                case DatabaseType.Oracle:
                    return new OracleTools(_info);

                case DatabaseType.SqlServer:
                    return new SqlServerTools(_info);

            }
            return new PostgreSqlTools(_info);
        }

        void ToggleButton(bool p_disabled)
        {
            if (p_disabled)
            {
                _btnConn.IsEnabled = false;
                _btnCreate.IsEnabled = false;
                _btnDel.IsEnabled = false;
            }
            else
            {
                _btnConn.IsEnabled = true;
                _btnCreate.IsEnabled = true;
                _btnDel.IsEnabled = true;
            }
        }

        void AppendMsg(string p_msg)
        {
            Kit.RunAsync(() =>
            {
                _tbInfo.Text = _tbInfo.Text + "\r" + p_msg;
                _tbInfo.Focus(FocusState.Programmatic);
                _tbInfo.Select(_tbInfo.Text.Length, 0);
            });
        }
    }
}
