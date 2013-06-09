using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MiDeRP;

namespace pcrobotsim
{
	class Program
	{
		static string comPort;
		const int baudRate = 9600;
		static SerialInterface com;
		static bool minehere = false;

		public enum StatusByteCode : byte
		{
			Unknown = 0x00,
			Continue = 0x01,
			Back = 0x42,
			Forward = 0x46,
			Stop = 0x53,
			Left = 0x4c,
			Right = 0x52,
			Turn = 0x54,
			Acknowledged = 0x06,
			NotAcknowledged = 0x15,
			Halfway = 0x48,
			Enquiry = 0x05,
			MineDetected = 0x07,
			Done = 0x04
		};

		public enum Direction
		{
			Left,
			Up,
			Right,
			Down,
			Unknown
		};

		static void Main(string[] args)
		{
			Console.WriteLine("Enter desired com port: \n");
			comPort = "COM" + Console.ReadLine();
			comInit();

			while (true)
			{
				ConsoleKey key = Console.ReadKey().Key;

				switch (key)
				{
					case ConsoleKey.R:
						com.SendByte((byte)StatusByteCode.Enquiry);
						break;
					case ConsoleKey.M:
						com.SendByte((byte)StatusByteCode.MineDetected);
						minehere = true;
						break;
					case ConsoleKey.Escape:
						return;
					default:
						continue;
				}
			}

		}

		static void comInit()
		{
			if (comPort != "" && baudRate > 0)
			{
				com = new SerialInterface(comPort, baudRate);
				int res = com.OpenPort();

				if (res != 0)
					Console.WriteLine("SerialInterface Error: #" + res + "\n" + com.lastError);
				else
					com.SerialDataEvent += com_SerialDataEvent;
			}
			else
			{
				Console.WriteLine("COM Port or Baud Rate not valid.");
			}

			com.SendByte((byte)StatusByteCode.Enquiry);
			System.Threading.Thread.Sleep(200);
		}

		private static void com_SerialDataEvent(object sender, SerialDataEventArgs e)
		{
			StatusByteCode _receivedByte = (StatusByteCode)e.DataByte;

			if (_receivedByte == 0x00)
				return;

			switch (_receivedByte)
			{
				case StatusByteCode.Left:
					com.SendByte((byte)StatusByteCode.Acknowledged);
					System.Threading.Thread.Sleep(500);
					if (!minehere)
					{
						com.SendByte((byte)StatusByteCode.Halfway);
						System.Threading.Thread.Sleep(500);
					}
					com.SendByte((byte)StatusByteCode.Enquiry);
					break;
				case StatusByteCode.Right:
					com.SendByte((byte)StatusByteCode.Acknowledged);
					System.Threading.Thread.Sleep(500);
					if (!minehere)
					{
						com.SendByte((byte)StatusByteCode.Halfway);
						System.Threading.Thread.Sleep(500);
					}
					com.SendByte((byte)StatusByteCode.Enquiry);
					break;
				case StatusByteCode.Forward:
					com.SendByte((byte)StatusByteCode.Acknowledged);
					System.Threading.Thread.Sleep(500);
					if (!minehere)
					{
						com.SendByte((byte)StatusByteCode.Halfway);
						System.Threading.Thread.Sleep(500);
					}
					com.SendByte((byte)StatusByteCode.Enquiry);
					break;
				case StatusByteCode.Turn:
					com.SendByte((byte)StatusByteCode.Acknowledged);
					System.Threading.Thread.Sleep(500);
					com.SendByte((byte)StatusByteCode.Enquiry);
					break;
				case StatusByteCode.Acknowledged:
					break;
				default:
					return;
			}

			minehere = false;

		}
	}
}
