#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.Docking;
using Dt.Base.FormView;
using Dt.Core;
using Dt.Core.Model;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Fz
{
    /// <summary>
    /// 系统存根
    /// </summary>
    public class Startup
    {
        public static void Run()
        {
            //var desk = new Desktop();
            //desk.MainWin = new Win { Title = "标题" };
            //SysVisual.RootContent = desk;

            //SysVisual.RootContent = frame;
#if WASM
            //SQLitePCL.raw.SetProvider(new SQLite3P2());
#endif

            var frame = new Frame();
            Windows.UI.Xaml.Window.Current.Content = frame;
            frame.Navigate(typeof(Playground));

            //SysVisual();
        }

        static Grid _rootGrid;

        static void SysVisual()
        {
            // 根Grid
            _rootGrid = new Grid();

            // 桌面层/页面层，此层调整为动态添加！为uno节省级数！启动时为临时提示信息
            TextBlock tb = new TextBlock
            {
                Text = "正在启动...",
                FontSize = 15,
                Foreground = new SolidColorBrush(Windows.UI.Colors.White),
                Margin = new Thickness(40, 0, 40, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            _rootGrid.Children.Add(tb);

            
            _rootGrid.SizeChanged += OnSizeChanged;
            // 主题蓝色
            _rootGrid.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0x1B, 0xA1, 0xE2));
            Window.Current.Content = _rootGrid;
        }

        static void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            bool isPhoneUI = e.NewSize.Width < 600;
            if (isPhoneUI != AtSys.IsPhoneUI)
            {
                
            }
        }
    }

    public class SQLite3P2 : ISQLite3Provider
    {
        public string GetNativeLibraryName()
        {
            throw new NotImplementedException();
        }

        public int sqlite3_backup_finish(IntPtr backup)
        {
            throw new NotImplementedException();
        }

        public sqlite3_backup sqlite3_backup_init(sqlite3 destDb, utf8z destName, sqlite3 sourceDb, utf8z sourceName)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_backup_pagecount(sqlite3_backup backup)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_backup_remaining(sqlite3_backup backup)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_backup_step(sqlite3_backup backup, int nPage)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_bind_blob(sqlite3_stmt stmt, int index, ReadOnlySpan<byte> blob)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_bind_double(sqlite3_stmt stmt, int index, double val)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_bind_int(sqlite3_stmt stmt, int index, int val)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_bind_int64(sqlite3_stmt stmt, int index, long val)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_bind_null(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_bind_parameter_count(sqlite3_stmt stmt)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_bind_parameter_index(sqlite3_stmt stmt, utf8z strName)
        {
            throw new NotImplementedException();
        }

        public utf8z sqlite3_bind_parameter_name(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_bind_text(sqlite3_stmt stmt, int index, ReadOnlySpan<byte> text)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_bind_text(sqlite3_stmt stmt, int index, utf8z text)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_bind_zeroblob(sqlite3_stmt stmt, int index, int size)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_blob_bytes(sqlite3_blob blob)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_blob_close(IntPtr blob)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_blob_open(sqlite3 db, utf8z db_utf8, utf8z table_utf8, utf8z col_utf8, long rowid, int flags, out sqlite3_blob blob)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_blob_read(sqlite3_blob blob, Span<byte> b, int offset)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_blob_reopen(sqlite3_blob blob, long rowid)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_blob_write(sqlite3_blob blob, ReadOnlySpan<byte> b, int offset)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_busy_timeout(sqlite3 db, int ms)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_changes(sqlite3 db)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_clear_bindings(sqlite3_stmt stmt)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_close(IntPtr db)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_close_v2(IntPtr db)
        {
            throw new NotImplementedException();
        }

        public ReadOnlySpan<byte> sqlite3_column_blob(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_column_bytes(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_column_count(sqlite3_stmt stmt)
        {
            throw new NotImplementedException();
        }

        public utf8z sqlite3_column_database_name(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        public utf8z sqlite3_column_decltype(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        public double sqlite3_column_double(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_column_int(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        public long sqlite3_column_int64(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        public utf8z sqlite3_column_name(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        public utf8z sqlite3_column_origin_name(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        public utf8z sqlite3_column_table_name(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        public utf8z sqlite3_column_text(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_column_type(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        public void sqlite3_commit_hook(sqlite3 db, delegate_commit func, object v)
        {
            throw new NotImplementedException();
        }

        public utf8z sqlite3_compileoption_get(int n)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_compileoption_used(utf8z sql)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_complete(utf8z sql)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_config(int op)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_config(int op, int val)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_config_log(delegate_log func, object v)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_create_collation(sqlite3 db, byte[] name, object v, delegate_collation func)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_create_function(sqlite3 db, byte[] name, int nArg, int flags, object v, delegate_function_scalar func)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_create_function(sqlite3 db, byte[] name, int nArg, int flags, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_data_count(sqlite3_stmt stmt)
        {
            throw new NotImplementedException();
        }

        public utf8z sqlite3_db_filename(sqlite3 db, utf8z att)
        {
            throw new NotImplementedException();
        }

        public IntPtr sqlite3_db_handle(IntPtr stmt)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_db_readonly(sqlite3 db, utf8z dbName)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_db_status(sqlite3 db, int op, out int current, out int highest, int resetFlg)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_enable_load_extension(sqlite3 db, int enable)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_enable_shared_cache(int enable)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_errcode(sqlite3 db)
        {
            throw new NotImplementedException();
        }

        public utf8z sqlite3_errmsg(sqlite3 db)
        {
            throw new NotImplementedException();
        }

        public utf8z sqlite3_errstr(int rc)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_exec(sqlite3 db, utf8z sql, delegate_exec callback, object user_data, out IntPtr errMsg)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_extended_errcode(sqlite3 db)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_extended_result_codes(sqlite3 db, int onoff)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_finalize(IntPtr stmt)
        {
            throw new NotImplementedException();
        }

        public void sqlite3_free(IntPtr p)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_get_autocommit(sqlite3 db)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_initialize()
        {
            throw new NotImplementedException();
        }

        public void sqlite3_interrupt(sqlite3 db)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_key(sqlite3 db, ReadOnlySpan<byte> key)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_key_v2(sqlite3 db, utf8z dbname, ReadOnlySpan<byte> key)
        {
            throw new NotImplementedException();
        }

        public long sqlite3_last_insert_rowid(sqlite3 db)
        {
            throw new NotImplementedException();
        }

        public utf8z sqlite3_libversion()
        {
            throw new NotImplementedException();
        }

        public int sqlite3_libversion_number()
        {
            throw new NotImplementedException();
        }

        public void sqlite3_log(int errcode, utf8z s)
        {
            throw new NotImplementedException();
        }

        public long sqlite3_memory_highwater(int resetFlag)
        {
            throw new NotImplementedException();
        }

        public long sqlite3_memory_used()
        {
            throw new NotImplementedException();
        }

        public IntPtr sqlite3_next_stmt(sqlite3 db, IntPtr stmt)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_open(utf8z filename, out IntPtr db)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_open_v2(utf8z filename, out IntPtr db, int flags, utf8z vfs)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_prepare_v2(sqlite3 db, ReadOnlySpan<byte> sql, out IntPtr stmt, out ReadOnlySpan<byte> remain)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_prepare_v2(sqlite3 db, utf8z sql, out IntPtr stmt, out utf8z remain)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_prepare_v3(sqlite3 db, ReadOnlySpan<byte> sql, uint flags, out IntPtr stmt, out ReadOnlySpan<byte> remain)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_prepare_v3(sqlite3 db, utf8z sql, uint flags, out IntPtr stmt, out utf8z remain)
        {
            throw new NotImplementedException();
        }

        public void sqlite3_profile(sqlite3 db, delegate_profile func, object v)
        {
            throw new NotImplementedException();
        }

        public void sqlite3_progress_handler(sqlite3 db, int instructions, delegate_progress func, object v)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_rekey(sqlite3 db, ReadOnlySpan<byte> key)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_rekey_v2(sqlite3 db, utf8z dbname, ReadOnlySpan<byte> key)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_reset(sqlite3_stmt stmt)
        {
            throw new NotImplementedException();
        }

        public void sqlite3_result_blob(IntPtr context, ReadOnlySpan<byte> val)
        {
            throw new NotImplementedException();
        }

        public void sqlite3_result_double(IntPtr context, double val)
        {
            throw new NotImplementedException();
        }

        public void sqlite3_result_error(IntPtr context, ReadOnlySpan<byte> strErr)
        {
            throw new NotImplementedException();
        }

        public void sqlite3_result_error(IntPtr context, utf8z strErr)
        {
            throw new NotImplementedException();
        }

        public void sqlite3_result_error_code(IntPtr context, int code)
        {
            throw new NotImplementedException();
        }

        public void sqlite3_result_error_nomem(IntPtr context)
        {
            throw new NotImplementedException();
        }

        public void sqlite3_result_error_toobig(IntPtr context)
        {
            throw new NotImplementedException();
        }

        public void sqlite3_result_int(IntPtr context, int val)
        {
            throw new NotImplementedException();
        }

        public void sqlite3_result_int64(IntPtr context, long val)
        {
            throw new NotImplementedException();
        }

        public void sqlite3_result_null(IntPtr context)
        {
            throw new NotImplementedException();
        }

        public void sqlite3_result_text(IntPtr context, ReadOnlySpan<byte> val)
        {
            throw new NotImplementedException();
        }

        public void sqlite3_result_text(IntPtr context, utf8z val)
        {
            throw new NotImplementedException();
        }

        public void sqlite3_result_zeroblob(IntPtr context, int n)
        {
            throw new NotImplementedException();
        }

        public void sqlite3_rollback_hook(sqlite3 db, delegate_rollback func, object v)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_set_authorizer(sqlite3 db, delegate_authorizer authorizer, object user_data)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_shutdown()
        {
            throw new NotImplementedException();
        }

        public utf8z sqlite3_sourceid()
        {
            throw new NotImplementedException();
        }

        public utf8z sqlite3_sql(sqlite3_stmt stmt)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_status(int op, out int current, out int highwater, int resetFlag)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_step(sqlite3_stmt stmt)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_stmt_busy(sqlite3_stmt stmt)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_stmt_readonly(sqlite3_stmt stmt)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_stmt_status(sqlite3_stmt stmt, int op, int resetFlg)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_stricmp(IntPtr p, IntPtr q)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_strnicmp(IntPtr p, IntPtr q, int n)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_table_column_metadata(sqlite3 db, utf8z dbName, utf8z tblName, utf8z colName, out utf8z dataType, out utf8z collSeq, out int notNull, out int primaryKey, out int autoInc)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_threadsafe()
        {
            throw new NotImplementedException();
        }

        public int sqlite3_total_changes(sqlite3 db)
        {
            throw new NotImplementedException();
        }

        public void sqlite3_trace(sqlite3 db, delegate_trace func, object v)
        {
            throw new NotImplementedException();
        }

        public void sqlite3_update_hook(sqlite3 db, delegate_update func, object v)
        {
            throw new NotImplementedException();
        }

        public ReadOnlySpan<byte> sqlite3_value_blob(IntPtr p)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_value_bytes(IntPtr p)
        {
            throw new NotImplementedException();
        }

        public double sqlite3_value_double(IntPtr p)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_value_int(IntPtr p)
        {
            throw new NotImplementedException();
        }

        public long sqlite3_value_int64(IntPtr p)
        {
            throw new NotImplementedException();
        }

        public utf8z sqlite3_value_text(IntPtr p)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_value_type(IntPtr p)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_wal_autocheckpoint(sqlite3 db, int n)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_wal_checkpoint(sqlite3 db, utf8z dbName)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_wal_checkpoint_v2(sqlite3 db, utf8z dbName, int eMode, out int logSize, out int framesCheckPointed)
        {
            throw new NotImplementedException();
        }

        public int sqlite3_win32_set_directory(int typ, utf8z path)
        {
            throw new NotImplementedException();
        }

        public int sqlite3__vfs__delete(utf8z vfs, utf8z pathname, int syncDir)
        {
            throw new NotImplementedException();
        }
    }
}
