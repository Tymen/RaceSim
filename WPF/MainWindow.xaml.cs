using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using Controller;
using Controller.EventArgs;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RaceController _raceController { get; set; }
        private DataContext _dataContext { get; set; }
        public MainWindow(RaceController raceController)
        {
            InitializeComponent();
            _raceController = raceController;
            _dataContext = new DataContext(_raceController);
            // Set the DataContext of MainWindow to the DataContextClass instance
            this.DataContext = _dataContext;
            _raceController.DriversChanged += UpdateDriverInfo;
            DataController.IsNextRace += IsNextRace;
        }
        public void IsNextRace(object sender, NextRaceEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                _raceController.DriversChanged -= UpdateDriverInfo;
                _raceController = e.RaceController;
                _raceController.DriversChanged += UpdateDriverInfo;
                _dataContext = new DataContext(e.RaceController);
                this.DataContext = _dataContext;
            });
        }
        public void UpdateDriverInfo(object sender, DriversChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                // MyLabel.Content = e.track.Name;
            });
        }

        private void OpenScreen1_Click(object sender, RoutedEventArgs e)
            {
                Screen1 screen1 = new Screen1();
                screen1.Show();
            }

            private void OpenScreen2_Click(object sender, RoutedEventArgs e)
            {
                Screen2 screen2 = new Screen2();
                screen2.Show();
            }

            private void Exit_Click(object sender, RoutedEventArgs e)
            {
                this.Close();
            }
        }
}