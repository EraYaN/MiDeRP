using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MiDeRP;

namespace SerialTestWPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		static public SerialInterface com;
		public MainWindow()
		{
			com = new SerialInterface("COM4 ", 9600);
			
			InitializeComponent();
		}

		private void com_SerialDataEvent(object sender, SerialDataEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("Serial data byte received; {0}", e.DataByte);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (com.OpenPort() != 0)
			{
				System.Diagnostics.Debug.WriteLine("Error: {0}", com.lastError);
			}
			if (com.IsOpen)
			{
				System.Diagnostics.Debug.WriteLine("Port is open.");
			}
			com.SerialDataEvent += com_SerialDataEvent;
		}
	}
}
