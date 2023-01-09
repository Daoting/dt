using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Xml;

namespace Dt.MySqlBak
{
    public partial class MySqlBakService : ServiceBase
    {
        private Timer _timer = null;
        private string _destTime;
        private string _path;
        private string _logFile;
        private FileSystemWatcher _watcher;
        string _conFile;
        string _conPath;

        #region 构造函数
        public MySqlBakService()
        {
            InitializeComponent();

            _destTime = ConfigurationManager.AppSettings["ExportTime"];
            _path = ConfigurationManager.AppSettings["ExportPath"];
            _logFile = Path.Combine(_path, "mySqlTrace.log");
            _conFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
            _conPath = _conFile.Substring(0, _conFile.LastIndexOf(Path.DirectorySeparatorChar));
            _watcher = new FileSystemWatcher(_conPath);
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
        }
        #endregion

        #region 服务入口点
        public void Start(string[] args)
        {
            OnStart(args);
        }
        #endregion

        #region 事件处理方法
        protected override void OnStart(string[] args)
        {
            _timer = new Timer();
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _timer.Interval = 10000;
            _watcher.BeginInit();
            _watcher.EnableRaisingEvents = true;
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.Filter = "Dt.MySqlBak.exe.config";
            _watcher.Changed += ConfigChanged;
            _watcher.EndInit();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("※ 服务启动成功，每天【{0}】备份：", _destTime);
            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                if (key != "ExportTime" && key != "ExportPath")
                {
                    sb.Append(key);
                    sb.Append(" ");
                }
            }
            Log(sb.ToString());
            _timer.Enabled = true;
        }

        private void ConfigChanged(object sender, FileSystemEventArgs e)
        {
            FileStream fs = new FileStream(_conFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (XmlReader reader = XmlReader.Create(fs))
            {
                while (reader.Read())
                {
                    if(reader.Name == "add" && reader.AttributeCount > 0 && reader.GetAttribute("key") == "ExportTime")
                    {
                        if (!string.IsNullOrEmpty(reader.GetAttribute("value")))
                        {
                            string newTime = reader.GetAttribute("value");
                            if (newTime != _destTime)
                            {
                                _destTime = newTime;
                                Log("注意：数据库备份时间改为每天" + _destTime);
                            }
                        }
                        break;
                    }
                }
                reader.Close();
            }
            fs.Close();
        }

        protected override void OnStop()
        {
            _timer.Stop();
            _timer.Dispose();
            _watcher.Dispose();
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;
            if (_destTime == now.ToString("HH:mm"))
            {
                DoDbBak();
            }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 数据库备份
        /// </summary>
        private void DoDbBak()
        {
            string bakDir = "";
            string conOps = "";
            string dumpOps = "";
            string cmd = "";

            FileStream fs = new FileStream(_conFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (XmlReader reader = XmlReader.Create(fs))
            {
                while (reader.Read())
                {
                    if (reader.Name == "add" && reader.AttributeCount > 0 && reader.GetAttribute("key") != "ExportTime" && reader.GetAttribute("key") != "ExportPath")
                    {
                        if (!string.IsNullOrEmpty(reader.GetAttribute("value")))
                        {
                            string dbName = reader.GetAttribute("key");
                            conOps = reader.GetAttribute("value").Trim();
                            if (string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(conOps))
                                continue;

                            bakDir = _path + "\\MySql_" + dbName + "_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".sql";

                            // -R -E 导出所有(结构&数据&存储过程&函数&事件&触发器)
                            // --no-create-db 禁止生成创建数据库语句
                            // --hex-blob 将:BINARY, VARBINARY, BLOB, BIT类型导出为十六进制
                            // --force 即使在一个表导出期间得到一个SQL错误,继续
                            // --single-transaction 导出时会加上事务,不会锁表
                            dumpOps = string.Format(" -R -E --no-create-db  --default-character-set=utf8 --hex-blob --force --single-transaction --log-error={0} ", _logFile);
                            cmd = "mysqldump " + conOps + dumpOps + " > \"" + bakDir + "\"";
                            Log("数据库" + dbName + "备份开始!");
                            Log("数据库备份命令：" + cmd);
                            string result = RunCmd(cmd).Trim();

                            if (string.IsNullOrEmpty(result))
                            {
                                Log("数据库备份成功!");
                            }
                            else
                            {
                                Log("数据库备份可能未成功：" + result);
                            }
                            Log("数据库" + dbName + "备份结束。\r\n");
                        }
                    }
                }
                reader.Close();
            }
            fs.Close();
        }

        /// <summary>
        /// 执行备份的具体执行命令过程
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private string RunCmd(string command)
        {
            Process p = new Process();
            String appDirecroty = _conPath + "\\";       // cmd命令行执行的路径
            p.StartInfo.FileName = "cmd.exe";           //确定程序名
            p.StartInfo.Arguments = "/c " + command;    //确定程式命令行
            p.StartInfo.WorkingDirectory = appDirecroty;// 指定cmd命令行执行的路径，防止找不到mysqldump执行文件
            p.StartInfo.UseShellExecute = false;        //Shell的使用
            p.StartInfo.RedirectStandardInput = true;   //重定向输入
            p.StartInfo.RedirectStandardOutput = true; //重定向输出
            p.StartInfo.RedirectStandardError = true;   //重定向输出错误
            p.StartInfo.CreateNoWindow = true;
            p.Start();                                  //00
            p.StandardInput.WriteLine(command);       //也可以用这种方式输入入要行的命令
            p.StandardInput.WriteLine("exit");
            return p.StandardError.ReadToEnd();
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="p_msg"></param>
        /// <param name="type"></param>
        public void Log(string p_msg,string type = null)
        {
            using (FileStream fs = File.Open(_logFile, FileMode.OpenOrCreate))
            {
                fs.Seek(0, SeekOrigin.End);
                using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                {
                    sw.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + (string.IsNullOrEmpty(type)? " 提示：":type) +  p_msg);
                    sw.WriteLine();
                    sw.WriteLine();
                    sw.Flush();
                }
            }
        }
        #endregion
    }
}
