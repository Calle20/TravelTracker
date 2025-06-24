using BruTile.Predefined;
using BruTile.Web;
using Mapsui;
using Mapsui.Limiting;
using Mapsui.Projections;
using Mapsui.Providers;
using Mapsui.Tiling;
using Mapsui.Tiling.Layers;
using Mapsui.UI;
using Mapsui.UI.Maui;
using Mapsui.Utilities;
using System.Globalization;
using Map = Mapsui.Map;

namespace TravelTracker
{
    public partial class MapPage : ContentPage
    {
        readonly MapControl _mapControl = new MapControl();
        private static readonly Dictionary<string, MRect> CountryEnvelopes = new()
        {
            // Germany (approx)
            ["DE"] = new MRect(561000, 5930000, 1300000, 7000000),
            // France (approx)
            ["FR"] = new MRect(-6000000, 450000, -200000, 7000000),
            // … add your other countries …
        };

        public MapPage()
        {
            InitializeComponent();

            _mapControl.Map = CreateMapAsnyc();
            Content = _mapControl;
        }
        public Map CreateMapAsnyc()
        {
            var map = new Map
            {
                CRS = "EPSG:3857" // ensure WebMercator projection
            };

            // OSM base layer
            var osmLayer = OpenStreetMap.CreateTileLayer();
            osmLayer.Name = "OSM Base";
            osmLayer.Opacity = 0.5;
            map.Layers.Add(osmLayer);

            map.Navigator.Limiter = new ViewportLimiterKeepWithinExtent();
            map.Navigator.PanLock = false;
            map.Navigator.ZoomLock = false;
            map.Navigator.RotationLock = true;

            // 4) Zoom to user's country or world
            var countryCode = new RegionInfo(CultureInfo.CurrentCulture.Name).TwoLetterISORegionName.ToString();
            Console.WriteLine($"Detected country code: {countryCode}");
            Console.WriteLine($"Country envelope: {CountryEnvelopes.ContainsKey(countryCode)}");
            if (CountryEnvelopes.TryGetValue(countryCode, out var bbox))
                map.Navigator.ZoomToBox(bbox, MBoxFit.Fit);
            else
                map.Navigator.ZoomToBox(new MRect(-20_037_508, -20_037_508, 20_037_508, 20_037_508), MBoxFit.Fit);


            return map;
        }
    }
}