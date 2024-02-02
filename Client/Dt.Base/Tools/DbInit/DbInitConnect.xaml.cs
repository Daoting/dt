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
    public sealed partial class DbInitConnect : Tab
    {
        public DbInitConnect()
        {
            InitializeComponent();
            InitData();
        }

        void InitData()
        {
            _fv.Data = new DbInitInfo
            {
                DbType = DatabaseType.PostgreSql,
                Host = "10.10.1.2",
                Port = "5432",
                DefDb = "postgres",
                DefUser = "postgres",
            };
        }

        async void OnConnect(object sender, RoutedEventArgs e)
        {
            if (_fv.ExistNull("Host", "Port", "DefUser", "DefDb", "Pwd"))
                return;

            var info = _fv.Data.To<DbInitInfo>();
            bool suc = await DbKit.Test(info);
            if (suc)
            {
                Forward(new DbInit(), info);
            }
            else
            {
                Kit.Warn("数据库连接失败，请检查输入！");
            }
        }

        void OnAfterSelect(CList arg1, object arg2)
        {
            var info = _fv.Data.To<DbInitInfo>();
            var db = _fv["DefDb"];
            switch ((DatabaseType)arg2)
            {
                case DatabaseType.PostgreSql:
                    _fv["Port"].Val = "5432";
                    _fv["DefUser"].Val = "postgres";
                    db.IsEnabled = true;
                    db.Title = "默认库名";
                    db.Val = "postgres";
                    info.DbType = DatabaseType.PostgreSql;
                    break;

                case DatabaseType.MySql:
                    _fv["Port"].Val = "3306";
                    _fv["DefUser"].Val = "root";
                    db.IsEnabled = false;
                    db.Title = "无需填写";
                    db.Val = "  ";
                    info.DbType = DatabaseType.MySql;
                    break;

                case DatabaseType.Oracle:
                    _fv["Port"].Val = "1521";
                    _fv["DefUser"].Val = "system";
                    db.IsEnabled = true;
                    db.Title = "服务名";
                    db.Val = "orcl";
                    info.DbType = DatabaseType.Oracle;
                    break;

                case DatabaseType.SqlServer:
                    _fv["Port"].Val = "1433";
                    _fv["DefUser"].Val = "sa";
                    db.IsEnabled = true;
                    db.Title = "默认库名";
                    db.Val = "master";
                    info.DbType = DatabaseType.SqlServer;
                    break;

                default:
                    Throw.Msg($"不支持{(DatabaseType)arg2}数据库！");
                    break;
            }
        }
    }
}
