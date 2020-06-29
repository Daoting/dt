#if WASM
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-06-22 创建
******************************************************************************/
#endregion

#region 引用命名
using SQLitePCL;
using System;
using System.IO;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// https://github.com/unoplatform/Uno.SQLitePCLRaw.Wasm/blob/master/src/SQLitePCLRaw.provider.wasm/SQLite3Provider_WebAssembly.cs
    /// </summary>
    public class SQLite3Provider_WebAssembly : ISQLite3Provider
	{
        public SQLite3Provider_WebAssembly()
        {

        }
        public int sqlite3_open(string filename, out IntPtr db)
		{
			var res = Runtime.InvokeJS($"SQLiteWasm.sqlite3_open(\"{Path.GetFileName(filename)}\")");

			return ParseOpenREsult(out db, res);
		}

		public int sqlite3_open_v2(string filename, out IntPtr db, int flags, string vfs)
		{
			var res = Runtime.InvokeJS($"SQLiteWasm.sqlite3_open_v2(\"{Path.GetFileName(filename)}\", {flags}, {vfs})");

			return ParseOpenREsult(out db, res);
		}

		private static int ParseOpenREsult(out IntPtr db, string res)
		{
			var parts = res.Split(';');

			if (parts.Length == 2
				&& int.TryParse(parts[0], out var code)
				&& int.TryParse(parts[1], out var pDb)
			)
			{
				db = (IntPtr)pDb;
				return code;
			}

			db = IntPtr.Zero;
			return raw.SQLITE_ERROR;
		}

		public int sqlite3_close(IntPtr db)
			=> sqlite3_close_v2(db);

		public int sqlite3_close_v2(IntPtr db)
			=> InvokeJSInt($"SQLiteWasm.sqliteClose2({db})");

		public string sqlite3_db_filename(IntPtr db, string att)
			=> Runtime.InvokeJS($"SQLiteWasm.sqlite3_db_filename({db}, \"{att}\")");

		public int sqlite3_changes(IntPtr db)
			=> InvokeJSInt($"SQLiteWasm.sqliteChanges({db})");

		public int sqlite3_prepare_v2(IntPtr db, string sql, out IntPtr stmt, out string remain)
		{
			sql = sql.Replace("\n", " ");

			var res = Runtime.InvokeJS($"SQLiteWasm.sqlitePrepare2({db}, \"{Runtime.EscapeJs(sql)}\")");

			var parts = res.Split(';');

			if (parts.Length == 3
				&& int.TryParse(parts[0], out var code)
				&& int.TryParse(parts[1], out var pStatement)
				&& int.TryParse(parts[2], out var remainIndex)
			)
			{
				stmt = (IntPtr)pStatement;

				if (remainIndex != -1)
				{
					remain = sql.Substring(remainIndex);
				}
				else
				{
					remain = "";
				}
				return code;
			}

			remain = "";
			stmt = IntPtr.Zero;
			return raw.SQLITE_ERROR;
		}
		public int sqlite3_step(IntPtr stmt)
			=> InvokeJSInt($"SQLiteWasm.sqliteStep({stmt})");

		public int sqlite3_reset(IntPtr stmt)
			=> InvokeJSInt($"SQLiteWasm.sqliteReset({stmt})");

		public int sqlite3_finalize(IntPtr stmt)
			=> InvokeJSInt($"SQLiteWasm.sqliteFinalize({stmt})");

		public long sqlite3_last_insert_rowid(IntPtr db)
		{
			var res = Runtime.InvokeJS($"SQLiteWasm.sqliteLastInsertRowid({db})");

			if (int.TryParse(res, out var count))
			{
				return count;
			}

			throw new InvalidOperationException($"Invalid row if {res}");
		}

		public string sqlite3_errmsg(IntPtr db)
			=> Runtime.InvokeJS($"SQLiteWasm.sqliteErrMsg({db})");

		public int sqlite3_errcode(IntPtr db)
			=> InvokeJS($"SQLiteWasm.sqlite3_errcode({db})");

		public int sqlite3_extended_errcode(IntPtr db)
			=> InvokeJS($"SQLiteWasm.sqlite3_extended_errcode({db})");

		public int sqlite3_extended_result_codes(IntPtr db, int onoff)
			=> InvokeJS($"SQLiteWasm.sqlite3_extended_result_codes({db}, {onoff})");

		public int sqlite3_bind_parameter_index(IntPtr stmt, string strName)
			=> InvokeJSInt($"SQLiteWasm.sqlite3_bind_parameter_index({stmt}, \"{strName}\")");

		public int sqlite3_bind_null(IntPtr stmt, int index)
			=> InvokeJS($"SQLiteWasm.sqliteBindNull({stmt}, {index})");

		public int sqlite3_bind_int(IntPtr stmt, int index, int val)
			=> InvokeJS($"SQLiteWasm.sqliteBindInt({stmt}, {index}, {val})");

		public unsafe int sqlite3_bind_int64(IntPtr stmt, int index, long val)
		{
			return InvokeJS($"SQLiteWasm.sqliteBindInt64({stmt}, {index}, {(IntPtr)(&val)})");
		}

		public int sqlite3_bind_double(IntPtr stmt, int index, double val)
			=> InvokeJS($"SQLiteWasm.sqliteBindDouble({stmt}, {index}, {val})");

		public int sqlite3_bind_text(IntPtr stmt, int index, string text)
			=> InvokeJS($"SQLiteWasm.sqliteBindText({stmt}, {index}, \"{Runtime.EscapeJs(text)}\")");

		public int sqlite3_bind_blob(IntPtr stmt, int index, byte[] blob, int nSize)
		{
			var gch = GCHandle.Alloc(blob, GCHandleType.Pinned);

			try
			{
				var pinnedData = gch.AddrOfPinnedObject();

				return InvokeJS($"SQLiteWasm.sqlite3_bind_blob({stmt}, {index}, {pinnedData}, {blob.Length})");
			}
			finally
			{
				gch.Free();
			}
		}

		public int sqlite3_bind_blob(IntPtr stmt, int index, byte[] blob)
			=> sqlite3_bind_blob(stmt, index, blob, blob.Length);

		public int sqlite3_column_count(IntPtr stmt)
			=> InvokeJS($"SQLiteWasm.sqliteColumnCount({stmt})");

		public string sqlite3_column_name(IntPtr stmt, int index)
			=> Runtime.InvokeJS($"SQLiteWasm.sqliteColumnName({stmt}, {index})");

		public int sqlite3_column_type(IntPtr stmt, int index)
			=> InvokeJS($"SQLiteWasm.sqliteColumnType({stmt}, {index})");

		public int sqlite3_libversion_number()
			=> InvokeJS("SQLiteWasm.sqliteLibVersionNumber();");
		public int sqlite3_busy_timeout(IntPtr db, int ms)
			=> InvokeJS($"SQLiteWasm.sqlite3_busy_timeout({db}, {ms});");

		public string sqlite3_column_text(IntPtr stmt, int index)
			=> Runtime.InvokeJS($"SQLiteWasm.sqliteColumnString({stmt}, {index})");

		public int sqlite3_column_int(IntPtr stmt, int index)
			=> InvokeJS($"SQLiteWasm.sqliteColumnInt({stmt}, {index})");

		public unsafe long sqlite3_column_int64(IntPtr stmt, int index)
		{
			long data;

			InvokeJS($"SQLiteWasm.sqlite3_column_int64({stmt}, {index}, {(IntPtr)(&data)})");

			return data;
		}

		public double sqlite3_column_double(IntPtr stmt, int index)
			=> InvokeJS($"SQLiteWasm.sqlite3_column_double({stmt}, {index});");

		public byte[] sqlite3_column_blob(IntPtr stmt, int index)
		{
			var size = sqlite3_column_bytes(stmt, index);
			var buffer = new byte[size];

			var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);

			try
			{
				var pinnedData = gch.AddrOfPinnedObject();

				InvokeJS($"SQLiteWasm.sqlite3_column_blob({stmt}, {index}, {pinnedData}, {size})");

				return buffer;
			}
			finally
			{
				gch.Free();
			}
		}
		public int sqlite3_column_bytes(IntPtr stmt, int index)
			=> InvokeJS($"SQLiteWasm.sqlite3_column_bytes({stmt}, {index})");

		public int sqlite3_bind_parameter_count(IntPtr stmt)
			=> InvokeJS($"SQLiteWasm.sqlite3_bind_parameter_count({stmt})");

		public string sqlite3_bind_parameter_name(IntPtr stmt, int index)
			=> Runtime.InvokeJS($"SQLiteWasm.sqlite3_bind_parameter_name({stmt}, {index})");

		public int sqlite3_stmt_readonly(IntPtr stmt)
			=> InvokeJS($"SQLiteWasm.sqlite3_stmt_readonly({stmt})");

		public int sqlite3_backup_finish(IntPtr backup) => throw new NotImplementedException();
		public IntPtr sqlite3_backup_init(IntPtr destDb, string destName, IntPtr sourceDb, string sourceName) => throw new NotImplementedException();
		public int sqlite3_backup_pagecount(IntPtr backup) => throw new NotImplementedException();
		public int sqlite3_backup_remaining(IntPtr backup) => throw new NotImplementedException();
		public int sqlite3_backup_step(IntPtr backup, int nPage) => throw new NotImplementedException();
		public int sqlite3_bind_zeroblob(IntPtr stmt, int index, int size) => throw new NotImplementedException();
		public int sqlite3_blob_bytes(IntPtr blob) => throw new NotImplementedException();
		public int sqlite3_blob_close(IntPtr blob) => throw new NotImplementedException();
		public int sqlite3_blob_open(IntPtr db, byte[] db_utf8, byte[] table_utf8, byte[] col_utf8, long rowid, int flags, out IntPtr blob) => throw new NotImplementedException();
		public int sqlite3_blob_open(IntPtr db, string sdb, string table, string col, long rowid, int flags, out IntPtr blob) => throw new NotImplementedException();
		public int sqlite3_blob_read(IntPtr blob, byte[] b, int n, int offset) => throw new NotImplementedException();
		public int sqlite3_blob_read(IntPtr blob, byte[] b, int bOffset, int n, int offset) => throw new NotImplementedException();
		public int sqlite3_blob_write(IntPtr blob, byte[] b, int n, int offset) => throw new NotImplementedException();
		public int sqlite3_blob_write(IntPtr blob, byte[] b, int bOffset, int n, int offset) => throw new NotImplementedException();
		public int sqlite3_clear_bindings(IntPtr stmt) => throw new NotImplementedException();
		public int sqlite3_column_blob(IntPtr stm, int columnIndex, byte[] result, int offset) => throw new NotImplementedException();
		public string sqlite3_column_database_name(IntPtr stmt, int index) => throw new NotImplementedException();
		public string sqlite3_column_decltype(IntPtr stmt, int index) => throw new NotImplementedException();
		public string sqlite3_column_origin_name(IntPtr stmt, int index) => throw new NotImplementedException();
		public string sqlite3_column_table_name(IntPtr stmt, int index) => throw new NotImplementedException();
		public void sqlite3_commit_hook(IntPtr db, delegate_commit func, object v) => throw new NotImplementedException();
		public string sqlite3_compileoption_get(int n) => throw new NotImplementedException();
		public int sqlite3_compileoption_used(string sql) => throw new NotImplementedException();
		public int sqlite3_complete(string sql) => throw new NotImplementedException();
		public int sqlite3_config(int op) => throw new NotImplementedException();
		public int sqlite3_config(int op, int val) => throw new NotImplementedException();
		public int sqlite3_config_log(delegate_log func, object v) => throw new NotImplementedException();
		public int sqlite3_create_collation(IntPtr db, string name, object v, delegate_collation func) => throw new NotImplementedException();
		public int sqlite3_create_function(IntPtr db, string name, int nArg, object v, delegate_function_scalar func) => throw new NotImplementedException();
		public int sqlite3_create_function(IntPtr db, string name, int nArg, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final) => throw new NotImplementedException();
		public int sqlite3_create_function(IntPtr db, string name, int nArg, int flags, object v, delegate_function_scalar func) => throw new NotImplementedException();
		public int sqlite3_create_function(IntPtr db, string name, int nArg, int flags, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final) => throw new NotImplementedException();
		public int sqlite3_data_count(IntPtr stmt) => throw new NotImplementedException();
		public IntPtr sqlite3_db_handle(IntPtr stmt) => throw new NotImplementedException();
		public int sqlite3_db_readonly(IntPtr db, string dbName) => throw new NotImplementedException();
		public int sqlite3_db_status(IntPtr db, int op, out int current, out int highest, int resetFlg) => throw new NotImplementedException();
		public int sqlite3_enable_load_extension(IntPtr db, int enable) => throw new NotImplementedException();
		public int sqlite3_enable_shared_cache(int enable) => throw new NotImplementedException();
		public string sqlite3_errstr(int rc) => throw new NotImplementedException();
		public int sqlite3_exec(IntPtr db, string sql, delegate_exec callback, object user_data, out string errMsg) => throw new NotImplementedException();
		public int sqlite3_get_autocommit(IntPtr db) => throw new NotImplementedException();
		public int sqlite3_initialize() => throw new NotImplementedException();
		public void sqlite3_interrupt(IntPtr db) => throw new NotImplementedException();
		public string sqlite3_libversion() => throw new NotImplementedException();
		public long sqlite3_memory_highwater(int resetFlag) => throw new NotImplementedException();
		public long sqlite3_memory_used() => throw new NotImplementedException();
		public IntPtr sqlite3_next_stmt(IntPtr db, IntPtr stmt) => throw new NotImplementedException();
		public void sqlite3_profile(IntPtr db, delegate_profile func, object v) => throw new NotImplementedException();
		public void sqlite3_progress_handler(IntPtr db, int instructions, delegate_progress func, object v) => throw new NotImplementedException();
		public void sqlite3_result_blob(IntPtr context, byte[] val) => throw new NotImplementedException();
		public void sqlite3_result_double(IntPtr context, double val) => throw new NotImplementedException();
		public void sqlite3_result_error(IntPtr context, string strErr) => throw new NotImplementedException();
		public void sqlite3_result_error_code(IntPtr context, int code) => throw new NotImplementedException();
		public void sqlite3_result_error_nomem(IntPtr context) => throw new NotImplementedException();
		public void sqlite3_result_error_toobig(IntPtr context) => throw new NotImplementedException();
		public void sqlite3_result_int(IntPtr context, int val) => throw new NotImplementedException();
		public void sqlite3_result_int64(IntPtr context, long val) => throw new NotImplementedException();
		public void sqlite3_result_null(IntPtr context) => throw new NotImplementedException();
		public void sqlite3_result_text(IntPtr context, string val) => throw new NotImplementedException();
		public void sqlite3_result_zeroblob(IntPtr context, int n) => throw new NotImplementedException();
		public void sqlite3_rollback_hook(IntPtr db, delegate_rollback func, object v) => throw new NotImplementedException();
		public int sqlite3_set_authorizer(IntPtr db, delegate_authorizer authorizer, object user_data) => throw new NotImplementedException();
		public int sqlite3_shutdown() => throw new NotImplementedException();
		public string sqlite3_sourceid() => throw new NotImplementedException();
		public string sqlite3_sql(IntPtr stmt) => throw new NotImplementedException();
		public int sqlite3_status(int op, out int current, out int highwater, int resetFlag) => throw new NotImplementedException();
		public int sqlite3_stmt_busy(IntPtr stmt) => throw new NotImplementedException();
		public int sqlite3_stmt_status(IntPtr stmt, int op, int resetFlg) => throw new NotImplementedException();
		public int sqlite3_table_column_metadata(IntPtr db, string dbName, string tblName, string colName, out string dataType, out string collSeq, out int notNull, out int primaryKey, out int autoInc) => throw new NotImplementedException();
		public int sqlite3_threadsafe() => throw new NotImplementedException();
		public int sqlite3_total_changes(IntPtr db) => throw new NotImplementedException();
		public void sqlite3_trace(IntPtr db, delegate_trace func, object v) => throw new NotImplementedException();
		public void sqlite3_update_hook(IntPtr db, delegate_update func, object v) => throw new NotImplementedException();
		public byte[] sqlite3_value_blob(IntPtr p) => throw new NotImplementedException();
		public int sqlite3_value_bytes(IntPtr p) => throw new NotImplementedException();
		public double sqlite3_value_double(IntPtr p) => throw new NotImplementedException();
		public int sqlite3_value_int(IntPtr p) => throw new NotImplementedException();
		public long sqlite3_value_int64(IntPtr p) => throw new NotImplementedException();
		public string sqlite3_value_text(IntPtr p) => throw new NotImplementedException();
		public int sqlite3_value_type(IntPtr p) => throw new NotImplementedException();
		public int sqlite3_wal_autocheckpoint(IntPtr db, int n) => throw new NotImplementedException();
		public int sqlite3_wal_checkpoint(IntPtr db, string dbName) => throw new NotImplementedException();
		public int sqlite3_wal_checkpoint_v2(IntPtr db, string dbName, int eMode, out int logSize, out int framesCheckPointed) => throw new NotImplementedException();
		public int sqlite3_win32_set_directory(int typ, string path) => throw new NotImplementedException();
		public int sqlite3__vfs__delete(string vfs, string pathname, int syncDir) => throw new NotImplementedException();


		private static int InvokeJS(string statement)
		{
			var res = Runtime.InvokeJS(statement);

			if (int.TryParse(res, out var value))
			{
				return value;
			}

			return raw.SQLITE_ERROR;
		}

		private static int InvokeJSInt(string statement)
		{
			var res = Runtime.InvokeJS(statement);

			if (int.TryParse(res, out var value))
			{
				return value;
			}

			throw new InvalidOperationException($"Invalid result {res}");
		}

        string ISQLite3Provider.GetNativeLibraryName()
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_open(utf8z filename, out IntPtr db)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_open_v2(utf8z filename, out IntPtr db, int flags, utf8z vfs)
        {
            throw new NotImplementedException();
        }

        void ISQLite3Provider.sqlite3_interrupt(sqlite3 db)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3__vfs__delete(utf8z vfs, utf8z pathname, int syncDir)
        {
            throw new NotImplementedException();
        }

        utf8z ISQLite3Provider.sqlite3_libversion()
        {
            throw new NotImplementedException();
        }

        utf8z ISQLite3Provider.sqlite3_sourceid()
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_db_readonly(sqlite3 db, utf8z dbName)
        {
            throw new NotImplementedException();
        }

        utf8z ISQLite3Provider.sqlite3_db_filename(sqlite3 db, utf8z att)
        {
            throw new NotImplementedException();
        }

        utf8z ISQLite3Provider.sqlite3_errmsg(sqlite3 db)
        {
            throw new NotImplementedException();
        }

        long ISQLite3Provider.sqlite3_last_insert_rowid(sqlite3 db)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_changes(sqlite3 db)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_total_changes(sqlite3 db)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_get_autocommit(sqlite3 db)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_busy_timeout(sqlite3 db, int ms)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_extended_result_codes(sqlite3 db, int onoff)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_errcode(sqlite3 db)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_extended_errcode(sqlite3 db)
        {
            throw new NotImplementedException();
        }

        utf8z ISQLite3Provider.sqlite3_errstr(int rc)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_prepare_v2(sqlite3 db, ReadOnlySpan<byte> sql, out IntPtr stmt, out ReadOnlySpan<byte> remain)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_prepare_v3(sqlite3 db, ReadOnlySpan<byte> sql, uint flags, out IntPtr stmt, out ReadOnlySpan<byte> remain)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_prepare_v2(sqlite3 db, utf8z sql, out IntPtr stmt, out utf8z remain)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_prepare_v3(sqlite3 db, utf8z sql, uint flags, out IntPtr stmt, out utf8z remain)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_step(sqlite3_stmt stmt)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_reset(sqlite3_stmt stmt)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_clear_bindings(sqlite3_stmt stmt)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_stmt_status(sqlite3_stmt stmt, int op, int resetFlg)
        {
            throw new NotImplementedException();
        }

        utf8z ISQLite3Provider.sqlite3_sql(sqlite3_stmt stmt)
        {
            throw new NotImplementedException();
        }

        IntPtr ISQLite3Provider.sqlite3_next_stmt(sqlite3 db, IntPtr stmt)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_bind_zeroblob(sqlite3_stmt stmt, int index, int size)
        {
            throw new NotImplementedException();
        }

        utf8z ISQLite3Provider.sqlite3_bind_parameter_name(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_bind_blob(sqlite3_stmt stmt, int index, ReadOnlySpan<byte> blob)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_bind_double(sqlite3_stmt stmt, int index, double val)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_bind_int(sqlite3_stmt stmt, int index, int val)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_bind_int64(sqlite3_stmt stmt, int index, long val)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_bind_null(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_bind_text(sqlite3_stmt stmt, int index, ReadOnlySpan<byte> text)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_bind_text(sqlite3_stmt stmt, int index, utf8z text)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_bind_parameter_count(sqlite3_stmt stmt)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_bind_parameter_index(sqlite3_stmt stmt, utf8z strName)
        {
            throw new NotImplementedException();
        }

        utf8z ISQLite3Provider.sqlite3_column_database_name(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        utf8z ISQLite3Provider.sqlite3_column_name(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        utf8z ISQLite3Provider.sqlite3_column_origin_name(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        utf8z ISQLite3Provider.sqlite3_column_table_name(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        utf8z ISQLite3Provider.sqlite3_column_text(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_data_count(sqlite3_stmt stmt)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_column_count(sqlite3_stmt stmt)
        {
            throw new NotImplementedException();
        }

        double ISQLite3Provider.sqlite3_column_double(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_column_int(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        long ISQLite3Provider.sqlite3_column_int64(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        ReadOnlySpan<byte> ISQLite3Provider.sqlite3_column_blob(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_column_bytes(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_column_type(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        utf8z ISQLite3Provider.sqlite3_column_decltype(sqlite3_stmt stmt, int index)
        {
            throw new NotImplementedException();
        }

        sqlite3_backup ISQLite3Provider.sqlite3_backup_init(sqlite3 destDb, utf8z destName, sqlite3 sourceDb, utf8z sourceName)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_backup_step(sqlite3_backup backup, int nPage)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_backup_remaining(sqlite3_backup backup)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_backup_pagecount(sqlite3_backup backup)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_blob_open(sqlite3 db, utf8z db_utf8, utf8z table_utf8, utf8z col_utf8, long rowid, int flags, out sqlite3_blob blob)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_blob_bytes(sqlite3_blob blob)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_blob_reopen(sqlite3_blob blob, long rowid)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_blob_write(sqlite3_blob blob, ReadOnlySpan<byte> b, int offset)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_blob_read(sqlite3_blob blob, Span<byte> b, int offset)
        {
            throw new NotImplementedException();
        }

        void ISQLite3Provider.sqlite3_log(int errcode, utf8z s)
        {
            throw new NotImplementedException();
        }

        void ISQLite3Provider.sqlite3_commit_hook(sqlite3 db, delegate_commit func, object v)
        {
            throw new NotImplementedException();
        }

        void ISQLite3Provider.sqlite3_rollback_hook(sqlite3 db, delegate_rollback func, object v)
        {
            throw new NotImplementedException();
        }

        void ISQLite3Provider.sqlite3_trace(sqlite3 db, delegate_trace func, object v)
        {
            throw new NotImplementedException();
        }

        void ISQLite3Provider.sqlite3_profile(sqlite3 db, delegate_profile func, object v)
        {
            throw new NotImplementedException();
        }

        void ISQLite3Provider.sqlite3_progress_handler(sqlite3 db, int instructions, delegate_progress func, object v)
        {
            throw new NotImplementedException();
        }

        void ISQLite3Provider.sqlite3_update_hook(sqlite3 db, delegate_update func, object v)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_create_collation(sqlite3 db, byte[] name, object v, delegate_collation func)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_create_function(sqlite3 db, byte[] name, int nArg, int flags, object v, delegate_function_scalar func)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_create_function(sqlite3 db, byte[] name, int nArg, int flags, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_db_status(sqlite3 db, int op, out int current, out int highest, int resetFlg)
        {
            throw new NotImplementedException();
        }

        void ISQLite3Provider.sqlite3_result_blob(IntPtr context, ReadOnlySpan<byte> val)
        {
            throw new NotImplementedException();
        }

        void ISQLite3Provider.sqlite3_result_error(IntPtr context, ReadOnlySpan<byte> strErr)
        {
            throw new NotImplementedException();
        }

        void ISQLite3Provider.sqlite3_result_error(IntPtr context, utf8z strErr)
        {
            throw new NotImplementedException();
        }

        void ISQLite3Provider.sqlite3_result_text(IntPtr context, ReadOnlySpan<byte> val)
        {
            throw new NotImplementedException();
        }

        void ISQLite3Provider.sqlite3_result_text(IntPtr context, utf8z val)
        {
            throw new NotImplementedException();
        }

        ReadOnlySpan<byte> ISQLite3Provider.sqlite3_value_blob(IntPtr p)
        {
            throw new NotImplementedException();
        }

        utf8z ISQLite3Provider.sqlite3_value_text(IntPtr p)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_stmt_busy(sqlite3_stmt stmt)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_stmt_readonly(sqlite3_stmt stmt)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_exec(sqlite3 db, utf8z sql, delegate_exec callback, object user_data, out IntPtr errMsg)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_complete(utf8z sql)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_compileoption_used(utf8z sql)
        {
            throw new NotImplementedException();
        }

        utf8z ISQLite3Provider.sqlite3_compileoption_get(int n)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_wal_autocheckpoint(sqlite3 db, int n)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_wal_checkpoint(sqlite3 db, utf8z dbName)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_wal_checkpoint_v2(sqlite3 db, utf8z dbName, int eMode, out int logSize, out int framesCheckPointed)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_table_column_metadata(sqlite3 db, utf8z dbName, utf8z tblName, utf8z colName, out utf8z dataType, out utf8z collSeq, out int notNull, out int primaryKey, out int autoInc)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_set_authorizer(sqlite3 db, delegate_authorizer authorizer, object user_data)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_stricmp(IntPtr p, IntPtr q)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_strnicmp(IntPtr p, IntPtr q, int n)
        {
            throw new NotImplementedException();
        }

        void ISQLite3Provider.sqlite3_free(IntPtr p)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_key(sqlite3 db, ReadOnlySpan<byte> key)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_key_v2(sqlite3 db, utf8z dbname, ReadOnlySpan<byte> key)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_rekey(sqlite3 db, ReadOnlySpan<byte> key)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_rekey_v2(sqlite3 db, utf8z dbname, ReadOnlySpan<byte> key)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_enable_load_extension(sqlite3 db, int enable)
        {
            throw new NotImplementedException();
        }

        int ISQLite3Provider.sqlite3_win32_set_directory(int typ, utf8z path)
        {
            throw new NotImplementedException();
        }
    }

    public class SQLite3P1 : ISQLite3Provider
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
#endif