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

namespace MiDeRP
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
            if (comPortsComboBox.SelectedItem != null && baudRateComboBox.SelectedItem != null)
            {
                Data.ComPort = (string)((ComboBoxItem)comPortsComboBox.SelectedItem).Content;
				Data.BaudRate = int.Parse((string)((ComboBoxItem)baudRateComboBox.SelectedItem).Content);
                if (Data.ComPort != "" && Data.BaudRate > 0)
                {
					Data.com = new SerialInterface(Data.ComPort, Data.BaudRate);
					int res = Data.com.OpenPort();
					if (res != 0)
						MessageBox.Show("SerialInterface Error: #" + res + "\n" + Data.com.lastError, "SerialInterface Error", MessageBoxButton.OK, MessageBoxImage.Error);
					else
					{
						Data.com.SerialDataEvent += com_SerialDataEvent;
					}
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
			
			System.Diagnostics.Debug.WriteLine("COM Port status: {0}",Data.com.IsOpen);
			if(Data.com.IsOpen)
				Data.com.SendByte(25);
            Data.nav.currentPos = new NodeConnection(new Coord(Data.entryCP), false);
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
			startRobotButton.IsEnabled = true;
        }

		private void startRobotButton_Click(object sender, RoutedEventArgs e)
		{
			if (Data.com == null)
			{
				MessageBox.Show("No serial connection found, no robot", "SerialInterface Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			
			if (Data.nav == null || Data.nav.path == null || Data.nav.path.Count == 0)
			{
				MessageBox.Show("No path, robot will not start", "Path Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			if (Data.ctr != null)
			{
				MessageBoxResult result = MessageBox.Show("The controller is still running, exit and restart?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
				{

					Data.ctr.Reset();
					return;
				}
				else
				{
					return;
				}
			}

			//Start controller
			Data.ctr = new Controller();
			startRobotButton.IsEnabled = false;
		}

        void com_SerialDataEvent(object sender, SerialDataEventArgs e)
        {
			//Data.vis.DrawField();

			System.Diagnostics.Debug.WriteLine("Serial byte received: {0}", e.DataByte);
        }

        private void destroyButton_Click(object sender, RoutedEventArgs e)
        {
            findPathButton.IsEnabled = false;
            TestDrawButton.IsEnabled = false;
            startInitButton.IsEnabled = true;
            destroyButton.IsEnabled = false;
            comPortsComboBox.IsEnabled = true;
            baudRateComboBox.IsEnabled = true;
			startRobotButton.IsEnabled = false;
            Data.nav = null;
            Data.com = null;
            Data.vis = null;
			Data.ctr = null;
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

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
			if (Data.nav != null)
			{
				Data.nav.currentPos = new NodeConnection(new Coord(Data.entryCP), false);
				Data.nav.mines.Clear();
				Data.nav.path.Clear();
			}

			if (Data.ctr != null)
			{
				Data.ctr.Reset();
			}

			if (Data.vis != null)
			{
				Data.vis.DrawField();
			}

			if (Data.db != null)
			{
				Data.db.UpdateProperty("MineCount");
				Data.db.UpdateProperty("PathLength");
				Data.db.UpdateProperty("SerialPortStatus");
				Data.db.UpdateProperty("SerialPortStatusColor");
				Data.db.UpdateProperty("CurrentPosText");
			}
        }   
    }
}
