using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Color = System.Drawing.Color;

namespace covid_dashboard
{
    /// <summary>
    /// Methods for interacting with UI and model for map view of Ohio counties.
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //The Map object to be displayed in the MainWindow, displays Ohio counties
        private Map _map;
        //FeatureCollectionTable to hold Ohio county data queried from a service Url
        private FeatureCollectionTable _ohioTable;

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }
        /// <summary>
        /// Set up map of Ohio with county outlines and names
        /// </summary>
        private async void Initialize()
        {
            //table to hold US County Data
            ServiceFeatureTable countyDataTable = new ServiceFeatureTable(new Uri("https://services.arcgis.com/P3ePLMYs2RVChkJx/arcgis/rest/services/USA_Counties_Generalized/FeatureServer/0"));
            //Query Parameter to use on table. Will only pull Ohio counties data
            QueryParameters ohioDataParameters = new QueryParameters()
            {
                WhereClause = "STATE_NAME='Ohio' and POPULATION > 0 and MALES > 0 and FEMALES > 0"
            };
            //perform query 
            FeatureQueryResult ohioData = await countyDataTable.QueryFeaturesAsync(ohioDataParameters);
            //feature collection table to store just Ohio county data
            _ohioTable = new FeatureCollectionTable(ohioData);
            SimpleLineSymbol countyOutline = new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, Color.Black, 1);
            UniqueValueRenderer countyLineRenderer = new UniqueValueRenderer
            {
                DefaultSymbol = countyOutline
            };
            _ohioTable.Renderer = countyLineRenderer;
            
            FeatureCollection ohioColl = new FeatureCollection();
            ohioColl.Tables.Add(_ohioTable);
            FeatureCollectionLayer collLay = new FeatureCollectionLayer(ohioColl);

            //_map = new Map(Basemap.CreateDarkGrayCanvasVector());
            _map = new Map(new Uri("https://www.arcgis.com/home/item.html?id=979c6cc89af9449cbeb5342a439c6a76"));
            await _map.LoadAsync();
            _map.OperationalLayers.Add(collLay);
            _map.InitialViewpoint = new Viewpoint(40.170479, -82.608932, 2500000);
            _map.MaxScale = 2500000;
            _map.MinScale = 2500000;
            createCountyNameGraphics();
            mapView.Map = _map;
            

        }

        /// <summary>
        /// Creates and applies a GraphicsOverlay to the mapView that displays Ohio county names
        /// </summary>
        private void createCountyNameGraphics()
        {
            List<Feature> features = _ohioTable.ToList();
            GraphicsOverlay nameOverlay = new GraphicsOverlay();
            foreach (Feature f in features)
            {
                Esri.ArcGISRuntime.Geometry.Geometry geometry = f.Geometry;
                TextSymbol name = new TextSymbol();
                name.Text = (string)f.Attributes["NAME"];
                name.Size = 12;
                name.Color = Color.FromArgb(33, 71, 112);
                name.OutlineColor = Color.White;
                name.FontWeight = Esri.ArcGISRuntime.Symbology.FontWeight.Bold;
                if (name.Text.Equals("Lucas"))
                {
                    name.VerticalAlignment = Esri.ArcGISRuntime.Symbology.VerticalAlignment.Bottom;
                    name.HorizontalAlignment = Esri.ArcGISRuntime.Symbology.HorizontalAlignment.Right;
                }
                Graphic g = new Graphic(geometry, name);
                nameOverlay.Graphics.Add(g);
            }
            
            mapView.GraphicsOverlays.Add(nameOverlay);
        }

        /// <summary>
        /// Raises the <see cref="MapViewModel.PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
