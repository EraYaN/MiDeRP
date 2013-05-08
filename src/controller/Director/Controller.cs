using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director
{
	public enum StatusByteCode : byte { 
		Unknown = 0x00,
		Forward = 0x46, 
		Stop = 0x53, 
		Left = 0x4c, 
		Right = 0x52, 
		Acknowledged = 0x06, 
		NotAcknowledged = 0x15,
		Enquiry = 0x05,
		MineDetected = 0x07,
		Done = 0x04
	};

	public enum Direction
	{
		Unknown,
		Left,
		Right,
		Up,
		Down
	};

	public class Controller
	{
		private StatusByteCode _receivedByte;
		private int _i = 0;
		
		public Controller()
		{
			Data.com.SerialDataEvent += com_SerialDataEvent;
		}

		private void com_SerialDataEvent(object sender, SerialDataEventArgs e)
		{
			Direction robotDirection; //current direction the robot is pointing in
			Direction nextAbsoluteDirection; //next direction in terms of the XY grid
			Direction nextRelativeDirection; //next direction relative to the robot's current one
			NodeConnection nextNodeConnection;

			_receivedByte = (StatusByteCode)e.DataByte;

			if (_i == 0)
			{
				//First run, robot is still outside of field

			}

			if (_receivedByte == StatusByteCode.Enquiry)
			{
				//Robot asks for new directions
				nextNodeConnection = Data.nav.path[_i];

				if ((int)nextNodeConnection.From.X - (int)nextNodeConnection.To.X >= 1)
				{
					//Next destination is left of origin
					nextAbsoluteDirection = Direction.Left;
				}
				if ((int)nextNodeConnection.From.X - (int)nextNodeConnection.To.X <= -1)
				{
					//Next destination is right of origin
					nextAbsoluteDirection = Direction.Right;
				}
				if ((int)nextNodeConnection.From.Y - (int)nextNodeConnection.To.Y <= -1)
				{
					//Next destination is above origin
					nextAbsoluteDirection = Direction.Up;
				}
				if ((int)nextNodeConnection.From.Y - (int)nextNodeConnection.To.Y >= 1)
				{
					//Next destination is under origin
					nextAbsoluteDirection = Direction.Down;
				}
				else
				{
					//Shit is pretty wrong
					nextAbsoluteDirection = Direction.Unknown;
				}

			}
		}
	}
}
