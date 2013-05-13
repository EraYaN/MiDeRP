using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiDeRP;

namespace SerialTest
{
	class Program
	{
		static public SerialInterface com;
		static void Main(string[] args)
		{
			com = new SerialInterface("COM4", 9600);
			com.SerialDataEvent += com_SerialDataEvent;
			if (com.OpenPort() != 0)
			{
				Console.WriteLine("Error: {0}", com.lastError);
			}
			if (com.IsOpen)
			{
				Console.WriteLine("Port is open.");
			}
			Console.ReadKey();
		}

		static void com_SerialDataEvent(object sender, SerialDataEventArgs e)
		{
			Console.WriteLine(e.DataByte);
		}
	}
}
