#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Reflection;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Sample
{
    public partial class CellsDemo : Win
    {
        Table _tbl;
        int _rowNum;

        public CellsDemo()
        {
            InitializeComponent();
            _fv.CellClick += OnCellClick;
            _pbFv.Data = _fv;
            CreateTable();
            OnRow(null, null);
        }

        void CreateTable()
        {
            _tbl = new Table
            {
                { "txt" },
                { "txtlong" },
                { "txtpy" },
                { "num", typeof(double) },
                { "mask" },
                { "pwd" },

                { "date", typeof(DateTime) },
                { "time", typeof(DateTime) },
                { "datetime", typeof(DateTime) },
                { "datestr" },
                { "datetouch", typeof(DateTime) },

                { "check", typeof(bool) },
                { "checkint", typeof(int) },
                { "checkstr" },
                { "checkswitch", typeof(bool) },

                { "liststr" },
                { "listenum1", typeof(Base.SelectionMode) },
                { "listenum2" },
                { "listint" },
                { "listobj", typeof(Person) },
                { "listrow" },
                { "sex" },
                { "sexname" },

                { "img1" },

                { "tree" },
                { "treedata" },
                { "treefill" },

                { "icon", typeof(Icons) },
                { "iconint", typeof(int) },
                { "iconstr" },

                { "color", typeof(Color) },
                { "colorstr" },
                { "colorbrush", typeof(SolidColorBrush) },

                { "listrowid" },
                {  "tgtpy" },
                { "treefillid" },
            };
        }

        void OnRow(object sender, RoutedEventArgs e)
        {
            _rowNum++;
            DateTime now = DateTime.Now;
            _fv.Data = _tbl.AddRow(new
            {
                txt = $"文本{_rowNum}",
                txtlong = "",
                txtpy = "",
                num = _rowNum,
                mask = "",
                pwd = "",

                date = now,
                time = now,
                datetime = now,
                datestr = "2015-06-19",
                datetouch = now,

                check = true,
                checkint = 1,
                checkstr = "男",
                checkswitch = true,

                liststr = "选项三",
                listenum1 = Base.SelectionMode.Multiple,
                listenum2 = "None",
                listint = "2",
                listobj = default(Person),
                listrow = "",
                sex = 1,
                sexname = "男",

                img1 = "[[\"sys/photo/profilephoto.jpg\", \"头像\", \".jpg\", 1140, \"hdt\", null]]",

                tree = "抗微生物药物",
                treedata = "消化系统药",
                treefill = "肛肠科",

                icon = Icons.主页,
                iconint = 10,
                iconstr = "主页",

                color = Colors.Green,
                colorstr = "#FF1BA1E2",
                colorbrush = AtRes.YellowBrush
            });
        }

        void OnObj(object sender, RoutedEventArgs e)
        {
            _rowNum++;
            DateTime now = DateTime.Now;
            _fv.Data = new CellData
            {
                Txt = $"文本{_rowNum}",
                TxtLong = "长文本",
                Txtpy = "",
                Num = _rowNum,
                Pwd = "123",

                Date = now,
                Time = now,
                DateTime = now,
                DateStr = "2015-06-19",
                DateTouch = now,

                Check = true,
                CheckInt = 1,
                Checkstr = "男",
                CheckSwitch = true,

                Liststr = "选项二",
                ListEnum1 = SelectionMode.Single,
                ListEnum2 = "None",
                Listint = "3",

                Tree = "抗微生物药物",
                TreeData = "消化系统药",
                TreeFill = "肛肠科",

                Icon = Icons.主页,
                Iconint = 10,
                Iconstr = "主页",

                Color = Colors.Tomato,
                Colorstr = "#FFFFA1E2",
                Colorbrush = AtRes.BrownBrush,
            };
        }

        void OnNull(object sender, RoutedEventArgs e)
        {
            _fv.Data = null;
        }

        void OnChanged(object sender, ICell e)
        {
            FvCell cell = _fv[e.ID];
            if (cell != null)
                AtKit.Msg($"{cell.Title}：{(e.Val != null ? e.Val : "空")}");
            else
                AtKit.Msg($"{e.ID}：{(e.Val != null ? e.Val : "空")}");

            if (e.ID == "txtpy")
                _fv.Row["tgtpy"] = AtKit.GetPinYin(e.GetVal<string>());
        }

        void OnCellClick(object sender, FvCell e)
        {
            if (_pbCell.Data == e)
                return;

            var items = _pbCell.Items;
            using (items.Defer())
            {
                items.Clear();
                FvCell cell = new CBool();
                cell.ID = "IsReadOnly";
                cell.Title = "只读";
                items.Add(cell);

                foreach (var info in e.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    // 可设置属性
                    var attr = (CellParamAttribute)info.GetCustomAttribute(typeof(CellParamAttribute), false);
                    if (attr == null)
                        continue;

                    cell = Fv.CreateCell(info.PropertyType, info.Name);
                    cell.Title = attr.Title;
                    items.Add(cell);
                }
            }
            _pbCell.Data = e;
        }

        void OnLoadPersons(object sender, AsyncEventArgs e)
        {
            ((CList)sender).Data = SampleData.CreatePersonsList(20);
        }

        void OnLoadDataTable(object sender, AsyncEventArgs e)
        {
            Table tbl = new Table { { "id" }, { "name" } };
            tbl.AddRow(new { id = "1", name = "李全亮" });
            tbl.AddRow(new { id = "2", name = "杨乐" });
            tbl.AddRow(new { id = "3", name = "任艳莉" });
            tbl.AddRow(new { id = "4", name = "潘洋" });
            tbl.AddRow(new { id = "5", name = "李妍" });
            tbl.AddRow(new { id = "6", name = "尚涛" });
            ((CList)sender).Data = tbl;
        }

        void OnSelectedRow(object sender, object e)
        {
            AtKit.Msg($"填充ID：{((Row)_fv.Data).Str("listrowid")}");
        }

        class CellData
        {
            public string Txt { get; set; }
            public string TxtLong { get; set; }
            public string Txtpy { get; set; }
            public double Num { get; set; }
            public string Mask { get; set; }
            public string Pwd { get; set; }

            public DateTime Date { get; set; }
            public DateTime Time { get; set; }
            public DateTime DateTime { get; set; }
            public string DateStr { get; set; }
            public DateTime DateTouch { get; set; }

            public bool Check { get; set; }
            public int CheckInt { get; set; }
            public string Checkstr { get; set; }
            public bool CheckSwitch { get; set; }

            public string Liststr { get; set; }
            public SelectionMode ListEnum1 { get; set; }
            public string ListEnum2 { get; set; }
            public string Listint { get; set; }
            public Person Listobj { get; set; }
            public Person Listrow { get; set; }

            public string Tree { get; set; }
            public string TreeData { get; set; }
            public string TreeFill { get; set; }

            public Icons Icon { get; set; }
            public int Iconint { get; set; }
            public string Iconstr { get; set; }

            public Color Color { get; set; }
            public string Colorstr { get; set; }
            public SolidColorBrush Colorbrush { get; set; }

            public string Listrowid { get; set; }
            public string Tgtpy { get; set; }
            public string TreeFillid { get; set; }
        }

        void OnLoadTreeData(object sender, AsyncEventArgs e)
        {
            ((CTree)sender).Data = TvData.GetTbl();
        }

        void OnWarn(object sender, RoutedEventArgs e)
        {
            _fv["txt"].Warn("警告提示信息内容警告提示信息内容警告提示信息内容警告提示信息内容");
        }

        void OnMsg(object sender, RoutedEventArgs e)
        {
            _fv["date"].Msg("提示消息内容");
        }

        int _cnt = 0;
        void OnToggleLink(object sender, EventArgs e)
        {
            _link.Title = $"切换内容{++_cnt}";
        }
    }
}