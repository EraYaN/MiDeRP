using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
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

namespace Director
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int M = 5;
        const int N = 5;        
        //Random rand = new Random();
        static public Visualization vis;
        static public Navigation nav;
        static public Communications com;
  
        public MainWindow()
        {
            InitializeComponent();
            statusBarMineNumber.DataContext = Data.db;
        }

        private void TestDrawButton_Click(object sender, RoutedEventArgs e)
        {
            vis.DrawField();            
        }

        private void findPathButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.SetMinesInDLL();
            int res;
            if ((res = Navigation.loopNavigation(0, M * N - 1)) != 0)
            {
                MessageBox.Show("Error during path finding. #" + res, "Path Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //TODO exception
            }
            else
            {
                MessageBox.Show("Path found!", "Path Found!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void startInitButton_Click(object sender, RoutedEventArgs e)
        {
            //Init classes
            vis = new Visualization(fieldmapcanvas, M, N);
            nav = new Navigation(M, N);
            com = new Communications(Data.ComPort, Data.BaudRate);
            //enable buttons
            findPathButton.IsEnabled = true;
            TestDrawButton.IsEnabled = true;
            startInitButton.IsEnabled = false;
            destroyButton.IsEnabled = true;
            comPortsComboBox.IsEnabled = false;
        }

        private void destroyButton_Click(object sender, RoutedEventArgs e)
        {
            findPathButton.IsEnabled = false;
            TestDrawButton.IsEnabled = false;
            startInitButton.IsEnabled = true;
            destroyButton.IsEnabled = false;
            comPortsComboBox.IsEnabled = true;
            nav = null;
            com = null;
            vis = null;
        }               

        private void comPortsComboBox_DropDownOpened(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            comPortsComboBox.Items.Clear();
            foreach (string s in ports)
            {
                ComboBoxItem extra = new ComboBoxItem();
                extra.Content = s;
                
                if (Data.ComPort == s)
                {
                    extra.IsSelected = true;
                }
                comPortsComboBox.Items.Add(extra);
            }
        }   
    }
}
