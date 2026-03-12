#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using BruTile;
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using Dt.Base;
using Dt.Core;
using Mapsui;
using Mapsui.Tiling;
using Mapsui.Tiling.Fetcher;
using Mapsui.Tiling.Layers;
using Mapsui.Widgets.ButtonWidgets;
using Mapsui.Widgets.InfoWidgets;
using Mapsui.Widgets.ScaleBar;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

#endregion

namespace Demo.UI
{
    public sealed partial class MapHome : Win
    {
        public MapHome()
        {
            InitializeComponent();
            Jz();
        }

        void Jz()
        {
            Table tbl = new Table
            {
                { "group" },
                { "name" },
                { "note" },
            };

            tbl.AddRow(new { group = "", name = "OpenStreetMap", note = "" });
            tbl.AddRow(new { group = "", name = "", note = "" });
            tbl.AddRow(new { group = "", name = "", note = "" });

            _lv.Data = tbl;
        }

        void OnItemClick(ItemClickArgs e)
        {
            _map.Map = CreateMap();
        }

        public static Map CreateMap()
        {
            var map = new Map();

            map.Layers.Add(CreateLayer());
            map.Navigator.CenterOnAndZoomTo(new MPoint(1059114.80157058, 5179580.75916194), map.Navigator.Resolutions[14]);
            map.BackColor = Mapsui.Styles.Color.FromString("#000613");

            return map;
        }

        private static TileLayer CreateLayer()
        {
            var tileSource = KnownTileSources.Create(KnownTileSource.BingAerial, "");
            return new TileLayer(tileSource, dataFetchStrategy: new DataFetchStrategy()) // DataFetchStrategy prefetches tiles from higher levels
            {
                Name = "Bing Aerial",
            };
        }
    }



    // 1. 定义百度地图瓦片模式（适配BD09MC坐标系和瓦片范围）
    public class BaiduTileSchema : TileSchema
    {
        public BaiduTileSchema()
        {
            // 百度地图瓦片的核心参数
            Name = "Baidu";
            Format = "png";
            Extent = new Extent(-20037508.342789244, -20037508.342789244, 20037508.342789244, 20037508.342789244);
            OriginX = -20037508.342789244;
            OriginY = -20037508.342789244;
            // 百度地图缩放级别（1-19）
            //for (int i = 1; i <= 19; i++)
            //{
            //    var resolution = 20037508.342789244 * 2 / (Math.Pow(2, i) * 256);
            //    Resolutions.Add(i, resolution);
            //}
        }
    }

    // 2. 自定义百度地图瓦片源
    public class BaiduTileSource : ITileSource
    {
        private readonly TileSchema _schema;
        private readonly string[] _servers = { "0", "1", "2", "3" }; // 百度地图子域名

        public BaiduTileSource()
        {
            _schema = new BaiduTileSchema();
        }

        public ITileSchema Schema => _schema;

        public string Name => "BaiduMap";

        public Attribution Attribution { get; }

        // 构建百度地图瓦片请求URL
        public Uri GetUri(TileInfo tileInfo)
        {
            // 随机选择子域名，避免单域名请求限制
            var random = new Random();
            var server = _servers[random.Next(_servers.Length)];

            // 百度地图瓦片URL模板
            var url = string.Format(
                "http://online{0}.map.bdimg.com/onlinelabel/?qt=tile&x={1}&y={2}&z={3}&styles=pl&scaler=1&p=1",
                server,
                tileInfo.Index.Col,
                tileInfo.Index.Row,
                tileInfo.Index.Level
            );
            return new Uri(url);
        }
    }
}
