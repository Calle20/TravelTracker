using BruTile.Predefined;
using BruTile.Web;
using BruTile.Wms;
using Mapsui;
using Mapsui.Limiting;
using Mapsui.Projections;
using Mapsui.Providers;
using Mapsui.Tiling;
using Mapsui.Tiling.Layers;
using Mapsui.UI;
using Mapsui.UI.Maui;
using Mapsui.Utilities;
using Microsoft.Maui.ApplicationModel;
using System.Globalization;
using Map = Mapsui.Map;

namespace TravelTracker
{
    public partial class MapPage : ContentPage
    {
        readonly MapControl _mapControl = new MapControl();
        static readonly Dictionary<string, MRect> CountryEnvelopes = new Dictionary<string, MRect>
        {
            { "DE", new MRect(667510, 5984026, 1670585, 7269661) },   // Germany
            { "FR", new MRect(-556597, 5181236, 1064703, 6665628) },  // France
            { "US", new MRect(-13957016, 2881529, -7453304, 6449785) }, // USA
            { "GB", new MRect(-842586, 6446275, 187058, 7820567) },   // United Kingdom
            { "IN", new MRect(7594069, 887586, 10886340, 4213004) },
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
            osmLayer.Opacity = 1;
            map.Layers.Add(osmLayer);

            map.Navigator.Limiter = new ViewportLimiterKeepWithinExtent();
            map.Navigator.PanLock = false;
            map.Navigator.ZoomLock = false;
            map.Navigator.RotationLock = true;

            // 4) Zoom to user's country or world
            var countryCode = new RegionInfo(CultureInfo.CurrentCulture.Name).TwoLetterISORegionName.ToString();
            if (CountryEnvelopes.TryGetValue(countryCode, out var bbox))
            {
                _mapControl.SizeChanged += (_, __) =>
                {
                    _mapControl.Map.Navigator.ZoomToBox(bbox, MBoxFit.Fit);
                };
                
            }
            else
                map.Navigator.ZoomToBox(new MRect(-20_037_508, -20_037_508, 20_037_508, 20_037_508), MBoxFit.Fit);


            return map;
        }
    }
}