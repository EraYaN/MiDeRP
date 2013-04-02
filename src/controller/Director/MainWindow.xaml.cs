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
            
        //Random rand = new Random();
        
  
        public MainWindow()
        {
            InitializeComponent();
            statusBarMineNumber.DataContext = Data.db;
            statusBarComPort.DataContext = Data.db;
            statusBarPathLength.DataContext = Data.db;
            statusBarRobotPos.DataContext = Data.db;
        }

        private void TestDrawButton_Click(object sender, RoutedEventArgs e)
        {
            Data.vis.DrawField();            
        }

        private void findPathButton_Click(object sender, RoutedEventArgs e)
        {
            Data.nav.SetMinesInDLL();
            int res;
            if ((res = Data.nav.findPath()) != 0)
            {
                MessageBox.Show("Error during path finding. #" + res, "Path Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //TODO exception
            }
            else
            {
                //MessageBox.Show("Path found!", "Path Found!", MessageBoxButton.OK, MessageBoxImage.Information);
                Data.vis.DrawField();
            }
        }

        private void startInitButton_Click(object sender, RoutedEventArgs e)
        {
            //Init classes
            Data.vis = new Visualization(fieldmapcanvas);
            Data.nav = new Navigation();
            Data.com = new SerialInterface(Data.ComPort, Data.BaudRate);
            //subscribe to events
            Data.com.SerialDataEvent += com_SerialDataEvent;
            if (comPortsComboBox.SelectedItem != null && baudRateComboBox.SelectedItem != null)
            {
                Data.ComPort = (string)((ComboBoxItem)comPortsComboBox.SelectedItem).Content;
                Data.BaudRate = (int)((ComboBoxItem)comPortsComboBox.SelectedItem).Content;
                if (Data.ComPort != "" && Data.BaudRate > 0)
                {
                    int res = Data.com.OpenPort();
                    if (res != 0)
                        MessageBox.Show("SerialInterface Error: #" + res + "\n" + Data.com.lastError, "SerialInterface Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("COM Port or Baud Rate not valid.", "SerialInterface Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("No COM Port or Baud Rate chosen.", "SerialInterface Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
                 
            Data.db.UpdateProperty("MineCount");
            Data.db.UpdateProperty("PathLength");
            Data.db.UpdateProperty("SerialPortStatus");
            Data.db.UpdateProperty("SerialPortStatusColor");
            Data.db.UpdateProperty("CurrentPosText");
            //enable buttons
            findPathButton.IsEnabled = true;
            TestDrawButton.IsEnabled = true;
            startInitButton.IsEnabled = false;
            destroyButton.IsEnabled = true;
            comPortsComboBox.IsEnabled = false;
            baudRateComboBox.IsEnabled = false;
        }

        void com_SerialDataEvent(object sender, SerialDataEventArgs e)
        {
            //Test respons
            //TODO proper respons.
            if (e.innerEvent.EventType == SerialData.Chars)
            {
                //echo die shit.
                Data.com.SendByte(e.DataByte);
            }
            else
            {
                //EOF niks terug sturen
                //com.SendByte(e.DataByte);
            }
        }

        private void destroyButton_Click(object sender, RoutedEventArgs e)
        {
            findPathButton.IsEnabled = false;
            TestDrawButton.IsEnabled = false;
            startInitButton.IsEnabled = true;
            destroyButton.IsEnabled = false;
            comPortsComboBox.IsEnabled = true;
            baudRateComboBox.IsEnabled = true;
            Data.nav = null;
            Data.com = null;
            Data.vis = null;
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
