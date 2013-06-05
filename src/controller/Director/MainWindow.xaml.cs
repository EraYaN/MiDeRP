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

        private void startChallengeButton_Click(object sender, RoutedEventArgs e)
        {
			Data.nav.InitChallenge();
        }

        private void startInitButton_Click(object sender, RoutedEventArgs e)
        {
            //Init classes
			Data.nav = new Navigation();
            Data.vis = new Visualization(fieldmapcanvas);
            
            if (comPortsComboBox.SelectedItem != null && baudRateComboBox.SelectedItem != null)
            {
                Data.ComPort = (string)((ComboBoxItem)comPortsComboBox.SelectedItem).Content;
				Data.BaudRate = int.Parse((string)((ComboBoxItem)baudRateComboBox.SelectedItem).Content);
                if (Data.ComPort != "" && Data.BaudRate > 0)
                {
                    Data.com = new SerialInterface(Data.ComPort, Data.BaudRate);
                    int res = Data.com.OpenPort();
                    Data.ctr = new Controller();
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

			Data.challenge = (Challenge)challengeComboBox.SelectedIndex;
			Data.nav.currentPos = new NodeConnection(new Coord(Data.entryCP), false);
            Data.db.UpdateProperty("MineCount");
            Data.db.UpdateProperty("PathLength");
            Data.db.UpdateProperty("SerialPortStatus");
            Data.db.UpdateProperty("SerialPortStatusColor");
            Data.db.UpdateProperty("CurrentPosText");
            //enable buttons
            startChallengeButton.IsEnabled = true;
            TestDrawButton.IsEnabled = true;
            startInitButton.IsEnabled = false;
            destroyButton.IsEnabled = true;
            comPortsComboBox.IsEnabled = false;
            baudRateComboBox.IsEnabled = false;
			challengeComboBox.IsEnabled = false;
            if (Data.ctr != null)
			    startRobotButton.IsEnabled = true;

			Data.vis.DrawField();
        }

        private void startRobotButton_Click(object sender, RoutedEventArgs e)
        {
			if (Data.nav.targetCPs.Count == 0&&Data.challenge == Challenge.FindPath)
			{
				MessageBox.Show("Please choose one or more control points...", "No control points selected", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			if (Data.nav.fullPath.Count == 0)
				Data.nav.InitChallenge();

            //Enable controller
            Data.ctr.EnableRobotControl();
            startRobotButton.IsEnabled = false;
			resetRobotButton.IsEnabled = true;
            softResetRobotButton.IsEnabled = true;
        }

        private void destroyButton_Click(object sender, RoutedEventArgs e)
        {
            startChallengeButton.IsEnabled = false;
            TestDrawButton.IsEnabled = false;
            startInitButton.IsEnabled = true;
            destroyButton.IsEnabled = false;
            comPortsComboBox.IsEnabled = true;
            baudRateComboBox.IsEnabled = true;
			startRobotButton.IsEnabled = false;
			challengeComboBox.IsEnabled = true;
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
				Data.nav.fullPath.Clear();
				if (Data.nav.paths != null)
					Array.Clear(Data.nav.paths, 0, Data.nav.paths.Length);
				if (Data.nav.targetCPs != null)
					Data.nav.targetCPs.Clear();
			}

			if (Data.ctr != null)
			{
                Data.ctr.ResetRobotControl();
                startRobotButton.IsEnabled = true;
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

		private void resetRobotButton_Click(object sender, RoutedEventArgs e)
		{
			if (Data.nav != null)
			{
				Data.nav.currentPos = new NodeConnection(new Coord(Data.entryCP), false);
				Data.nav.mines.Clear();
				Data.nav.fullPath.Clear();
				if (Data.nav.paths != null)
					Array.Clear(Data.nav.paths, 0, Data.nav.paths.Length);
			}

			if (Data.ctr != null)
			{
				Data.ctr.ResetRobotControl();
				startRobotButton.IsEnabled = true;
			}

			Data.vis.DrawField();
		}

        private void softResetRobotButton_Click(object sender, RoutedEventArgs e)
        {
            if (Data.nav != null)
            {
                Data.nav.currentPos = new NodeConnection(new Coord(Data.entryCP), false);
                //Data.nav.mines.Clear();
                Data.nav.fullPath.Clear();
                if (Data.nav.paths != null)
                    Array.Clear(Data.nav.paths, 0, Data.nav.paths.Length);
            }

            if (Data.ctr != null)
            {
                Data.ctr.ResetRobotControl();
                startRobotButton.IsEnabled = true;
            }

            Data.vis.DrawField();
        }
    }
}
