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
    /// 
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private Map _map;

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

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
            FeatureCollectionTable ohioTable = new FeatureCollectionTable(ohioData);
            SimpleLineSymbol countyOutline = new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, Color.Black, 1);
            UniqueValueRenderer countyLineRenderer = new UniqueValueRenderer
            {
                DefaultSymbol = countyOutline
            };
            ohioTable.Renderer = countyLineRenderer;

            FeatureCollectionTable ohioTable2 = new FeatureCollectionTable(ohioData);
            //await ohioTable.LoadAsync();
            FeatureCollection ohioColl = new FeatureCollection();
            ohioColl.Tables.Add(ohioTable);
            ohioColl.Tables.Add(ohioTable2);
            FeatureCollectionLayer collLay = new FeatureCollectionLayer(ohioColl);

            //_map = new Map(Basemap.CreateDarkGrayCanvasVector());
            _map = new Map(new Uri("https://www.arcgis.com/home/item.html?id=979c6cc89af9449cbeb5342a439c6a76"));
            await _map.LoadAsync();
            _map.OperationalLayers.Add(collLay);
            _map.InitialViewpoint = new Viewpoint(40.170479, -82.608932, 2500000);
            createCountyNameGraphics(ohioTable);
            mapView.Map = _map;
            

        }

        private void createCountyNameGraphics(FeatureCollectionTable _ohioTable)
        {
            List<Feature> features = _ohioTable.ToList();
            GraphicsOverlay nameOverlay = new GraphicsOverlay();
            foreach (Feature f in features)
            {
                Esri.ArcGISRuntime.Geometry.Geometry geometry = f.Geometry;
                TextSymbol name = new TextSymbol();
                name.Text = (string)f.Attributes["NAME"];
                name.Size = 12;
                name.Color = Color.White;
                name.BackgroundColor = Color.Black;
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
