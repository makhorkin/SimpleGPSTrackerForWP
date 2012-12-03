using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using Microsoft.Phone.Controls.Maps;

using Microsoft.Devices.Sensors;

using Microsoft.Xna.Framework;

using System.Device.Location;
using System.Threading;

namespace ExploreMapControl
{
    public partial class MainPage : PhoneApplicationPage
    {

        private Vector3 currentValues;

        private GeoCoordinateWatcher MainGeoWatcher;
        private Pushpin MapWndPushpin;

        private Boolean FirstRun;
        private Boolean SecondRun;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            MainGeoWatcher = new GeoCoordinateWatcher();
            MainGeoWatcher.MovementThreshold = 100.0f;

            MainGeoWatcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(MainGeoWatcher_StatusChanged);

            MainGeoWatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(MainGeoWatcher_PositionChanged);
            
            MapWndPolyline.Locations = new LocationCollection { };
            
            FirstRun = true;

            SecondRun = true;
            
            new Thread(startMyGeoWotcher).Start();

            MapWndPushpin = new Pushpin();
            MapWndPushpin.Opacity = 0.2;
            MapWndPushpin.Width = 10;
            MapWndPushpin.Height = 20;
        }

        void startMyGeoWotcher()
        {
            MainGeoWatcher.TryStart(false, TimeSpan.FromSeconds(60));
            
        }
        
        
        void MainGeoWatcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (FirstRun) FirstRun = false;
            else if (SecondRun)
            {
                SecondRun = false;
            }
            else MapWndPolyline.Locations.Add(e.Position.Location);
            
            MapWnd.Center = e.Position.Location;
            MapWndPushpin.Location = e.Position.Location;

            if (!MapWnd.Children.Contains(MapWndPushpin)) MapWnd.Children.Add(MapWndPushpin);
        }

        void MainGeoWatcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    if (MainGeoWatcher.Permission == GeoPositionPermission.Denied)
                    {
                        GeoStatus.Text = "Сервис выключен";
                    }
                    else
                    {
                        GeoStatus.Text = "На этом устройстве сервис недоступен";
                    }
                    break;
                case GeoPositionStatus.Initializing:
                    GeoStatus.Text = "Сервис инициализируется";
                    break;
                case GeoPositionStatus.NoData:
                    GeoStatus.Text = "Данные о месположении недоступны";
                    break;
                case GeoPositionStatus.Ready:
                    GeoStatus.Text = "Данные о местоположении доступны";
                    break;
            }
            
        }
      

        private void HandleZoomIn()
        {
            MapWnd.ZoomLevel += 1; 
        }

        private void HandleZoomOut()
        {
            MapWnd.ZoomLevel -= 1;
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            MapWnd.ZoomLevel += 1; 
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            MapWnd.ZoomLevel -= 1; 
        }

        private void LayoutChange_Click(object sender, RoutedEventArgs e)
        {
            if (MapWnd.Mode is RoadMode)
            {
                MapWnd.Mode = new AerialMode(true);
            }
            else
            {
                MapWnd.Mode = new RoadMode();
            }         

        }

        
    }
}