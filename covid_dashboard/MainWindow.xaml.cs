using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
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
using Polygon = Esri.ArcGISRuntime.Geometry.Polygon;

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
        //Dictionary to hold all county data
        private Dictionary<string, CountyData> data;

        public MainWindow()
        {
            InitializeComponent();
            InitializeMap();
            mapView.GeoViewTapped += MapView_GeoViewTapped;
            //data = OhioCovidDataService.getDataStartUp(); for progress bar implementation
            data = OhioCovidDataService.getData(DateTime.Now, true);

        }

        /// <summary>
        /// Set up map of Ohio with county outlines and names
        /// </summary>
        private async void InitializeMap()
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

        private void InitializeDataStartUp()
        {
            int caseCount = 0, hospCount = 0, deathCount = 0, maleSexCaseCount = 0, femaleSexCaseCount = 0,
                unknownSexCaseCount = 0, age0To19CaseCount = 0, age20To29CaseCount = 0, age30To39CaseCount = 0, age40To49CaseCount = 0,
                age50To59CaseCount = 0, age60To69CaseCount = 0, age70To79CaseCount = 0, age80PlusCaseCount = 0, unknownAgeCaseCount = 0;
            int maleSexHospCount = 0, femaleSexHospCount = 0, unknownSexHospCount = 0, age0To19HospCount = 0, age20To29HospCount = 0,
                age30To39HospCount = 0, age40To49HospCount = 0, age50To59HospCount = 0, age60To69HospCount = 0, age70To79HospCount = 0,
                age80PlusHospCount = 0, unknownAgeHospCount = 0;
            int maleSexDeathCount = 0, femaleSexDeathCount = 0, unknownSexDeathCount = 0, age0To19DeathCount = 0, age20To29DeathCount = 0,
                age30To39DeathCount = 0, age40To49DeathCount = 0, age50To59DeathCount = 0, age60To69DeathCount = 0, age70To79DeathCount = 0,
                age80PlusDeathCount = 0, unknownAgeDeathCount = 0;
            foreach (KeyValuePair<string, CountyData> c in data)
            {
                CountyData data = c.Value;
                Dictionary<string, int> counts = data.getCounts();
                caseCount += counts["TotalCaseCount"];
                hospCount += counts["TotalHospCount"];
                deathCount += counts["TotalDeathCount"];
                maleSexCaseCount += counts["MaleCaseCount"];
                maleSexHospCount += counts["MaleHospCount"];
                maleSexDeathCount += counts["MaleDeathCount"];
                femaleSexCaseCount += counts["FemaleCaseCount"];
                femaleSexHospCount += counts["FemaleHospCount"];
                femaleSexDeathCount += counts["FemaleDeathCount"];
                unknownSexCaseCount += counts["UnknownSexCaseCount"];
                unknownSexHospCount += counts["UnknownSexHospCount"];
                unknownSexDeathCount += counts["UnknownSexDeathCount"];
                age0To19CaseCount += counts["0-19CaseCount"];
                age0To19HospCount += counts["0-19HospCount"];
                age0To19DeathCount += counts["0-19DeathCount"];
                age20To29CaseCount += counts["20-29CaseCount"];
                age20To29HospCount += counts["20-29HospCount"];
                age20To29DeathCount += counts["20-29DeathCount"];
                age30To39CaseCount += counts["30-39CaseCount"];
                age30To39HospCount += counts["30-39HospCount"];
                age30To39DeathCount += counts["30-39DeathCount"];
                age40To49CaseCount += counts["40-49CaseCount"];
                age40To49HospCount += counts["40-49HospCount"];
                age40To49DeathCount += counts["40-49DeathCount"];
                age50To59CaseCount += counts["50-59CaseCount"];
                age50To59HospCount += counts["50-59HospCount"];
                age50To59DeathCount += counts["50-59DeathCount"];
                age60To69CaseCount += counts["60-69CaseCount"];
                age60To69HospCount += counts["60-69HospCount"];
                age60To69DeathCount += counts["60-69DeathCount"];
                age70To79CaseCount += counts["70-79CaseCount"];
                age70To79HospCount += counts["70-79HospCount"];
                age70To79DeathCount += counts["70-79DeathCount"];
                age80PlusCaseCount += counts["80+CaseCount"];
                age80PlusHospCount += counts["80+HospCount"];
                age80PlusDeathCount += counts["80+DeathCount"];
                unknownAgeCaseCount += counts["UnknownAgeCaseCount"];
                unknownAgeHospCount += counts["UnknownAgeHospCount"];
                unknownAgeDeathCount += counts["UnknownAgeDeathCount"];
            }

            totalCases.Text = caseCount.ToString();
            totalHospitalizations.Text = hospCount.ToString();
            totalDeaths.Text = deathCount.ToString();

            chartCasesSex.Series = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Male Cases",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(maleSexCaseCount) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Female Cases",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(femaleSexCaseCount) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Unknown Sex Cases",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(unknownSexCaseCount) },
                    DataLabels = true
                }
            };


        }

        private void MapView_GeoViewTapped(object sender, Esri.ArcGISRuntime.UI.Controls.GeoViewInputEventArgs e)
        {
            double xValue = e.Location.X;
            double yValue = e.Location.Y;
            int population = 0;
            int malePop = 0;
            int femalePop = 0;

            foreach (Feature f in _ohioTable)
            {
                List<string> potentialCounties = new List<string>();
                double countyXMin = f.Geometry.Extent.XMin;
                double countyYMin = f.Geometry.Extent.YMin;
                double countyXMax = f.Geometry.Extent.XMax;
                double countyYMax = f.Geometry.Extent.YMax;
                if (xValue > countyXMin && xValue < countyXMax && yValue > countyYMin && yValue < countyYMax)
                {
                    potentialCounties.Add((string)f.Attributes["NAME"]);
                    population = (int)f.Attributes["POPULATION"];
                    malePop = (int)f.Attributes["MALES"];
                    femalePop = (int)f.Attributes["FEMALES"];
                }
                else
                {
                    continue;
                }
                if (potentialCounties.Count == 1)
                {
                    regionName.Text = potentialCounties.ElementAt(0);
                    tbPopulation.Text = population.ToString();
                    tbMalePopulation.Text = malePop.ToString();
                    tbFemalePopulation.Text = femalePop.ToString();
                }
                else
                {
                    return;
                }
                    
            }
        }

        private void btnOhioData_Click(object sender, RoutedEventArgs e)
        {
            regionName.Text = "Ohio";
            tbPopulation.Text = "11730000";
            tbMalePopulation.Text = "5512000";
            tbFemalePopulation.Text = "5840000";
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
