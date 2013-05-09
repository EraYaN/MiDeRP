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
		Left,
		Up,
		Right,
		Down,
		Unknown
	};

	public class Controller
	{
		private bool _sentDirectiveIsUnacknowledged = false;
		private StatusByteCode _receivedByte;
		NodeConnection nextNodeConnection;
		private Direction _nextAbsoluteDirection = Direction.Unknown; //next direction in terms of the XY grid
		private Direction _robotDirection = Direction.Unknown; //current direction the robot is pointing in
		private StatusByteCode _nextDirective = StatusByteCode.Unknown; //next directive to be sent to robot
		private int _i = 0;
		
		public Controller()
		{
			Data.com.SerialDataEvent += com_SerialDataEvent;
		}

		private void com_SerialDataEvent(object sender, SerialDataEventArgs e)
		{	
			_receivedByte = (StatusByteCode)e.DataByte;

			if (_receivedByte == StatusByteCode.Enquiry)
			{
				if (_sentDirectiveIsUnacknowledged == true)
				{
					//Out of sync
					//TODO: Handle this
					return;
				}

				getNextDirective();
				Data.com.SendByte((byte)_nextDirective);
				_sentDirectiveIsUnacknowledged = true;
			}
			else if (_receivedByte == StatusByteCode.Acknowledged)
			{
				_i++; //Advance to next item in path
				_robotDirection = _nextAbsoluteDirection;
				_sentDirectiveIsUnacknowledged = false;
			}
			else if (_receivedByte == StatusByteCode.NotAcknowledged)
			{
				//Resend directives
				Data.com.SendByte((byte)_nextDirective);
			}
			else if (_receivedByte == StatusByteCode.MineDetected)
			{
				//Detected mine, add to list
				recalculatePath();
				getNextDirective();
				Data.com.SendByte((byte)_nextDirective);
			}
			else if (_receivedByte == StatusByteCode.NotAcknowledged)
			{
				//Finished
				//TODO: Handle this
			}
			else
			{
				//Ignore all others?
				return;
			}
		}

		private void getNextDirective()
		{
			//Robot asks for new directions
			nextNodeConnection = Data.nav.path[_i];

			//Find initial direction
			if (_i == 0 && _robotDirection == Direction.Unknown)
			{
				//Amount of control posts on horizontal sides is m - 2, on vertical sides n - 2, placing is counterclockwise, starting from bottom left (1)
				if (Data.entryCP > (Data.numControlPosts - (Data.N - 2)))
				{
					//entryCP is at left side
					_robotDirection = Direction.Right;
				}
				else if (Data.entryCP > (Data.numControlPosts - ((Data.N - 2) + (Data.M - 2))))
				{
					//entryCP is at top side
					_robotDirection = Direction.Down;
				}
				else if (Data.entryCP > (Data.numControlPosts - (2 * (Data.N - 2) + (Data.M - 2))))
				{
					//entryCP is at right side
					_robotDirection = Direction.Left;
				}
				else if (Data.entryCP > (Data.numControlPosts - (2 * (Data.N - 2) + 2 * (Data.M - 2))))
				{
					//entryCP is at bottom side
					_robotDirection = Direction.Up;
				}
				else
				{
					_robotDirection = Direction.Unknown;
				}
			}

			//Determine next direction in the field
			if ((int)nextNodeConnection.From.X - (int)nextNodeConnection.To.X >= 1)
			{
				//Next destination is left of origin
				_nextAbsoluteDirection = Direction.Left;
			}
			else if ((int)nextNodeConnection.From.X - (int)nextNodeConnection.To.X <= -1)
			{
				//Next destination is right of origin
				_nextAbsoluteDirection = Direction.Right;
			}
			else if ((int)nextNodeConnection.From.Y - (int)nextNodeConnection.To.Y <= -1)
			{
				//Next destination is above origin
				_nextAbsoluteDirection = Direction.Up;
			}
			else if ((int)nextNodeConnection.From.Y - (int)nextNodeConnection.To.Y >= 1)
			{
				//Next destination is under origin
				_nextAbsoluteDirection = Direction.Down;
			}
			else
			{
				//Shit is pretty wrong
				_nextAbsoluteDirection = Direction.Unknown;
			}

			//Determine the robot's next turn
			if (_nextAbsoluteDirection - _robotDirection == 1 || _nextAbsoluteDirection - _robotDirection == -3)
			{
				_nextDirective = StatusByteCode.Right;
			}
			else if (_nextAbsoluteDirection - _robotDirection == -1 || _nextAbsoluteDirection - _robotDirection == 3)
			{
				_nextDirective = StatusByteCode.Left;
			}
			else
			{
				_nextDirective = StatusByteCode.Forward;
			}
		}

		private void recalculatePath()
		{
			Data.nav.mines.Add(Data.nav.path[_i]);
			Data.nav.SetMinesInDLL();
			_i = 0;
			Data.nav.currentPos = new NodeConnection(Data.nav.currentPos.To, Data.nav.currentPos.From);
			Data.nav.findPath();
			_robotDirection = (Direction)(((int)_robotDirection + 2) % 4); //Bacon flips
			//Data.vis.DrawField(); //BROKEN
		}
	}
}
