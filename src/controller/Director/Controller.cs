using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiDeRP    
{
	public enum StatusByteCode : byte { 
		Unknown = 0x00,
		Forward = 0x46, 
		Stop = 0x53, 
		Left = 0x4c, 
		Right = 0x52,
		Turn = 0x54,
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
		private NodeConnection _nextNodeConnection;
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
			
            _receivedByte = e.DataByte.ToStatusByteCode();

			if (_receivedByte == StatusByteCode.Acknowledged)
			{
				if (_sentDirectiveIsUnacknowledged)
				{
					_i++; //Advance to next item in path
					_robotDirection = _nextAbsoluteDirection;
					Data.nav.currentPos = _nextNodeConnection;
					_sentDirectiveIsUnacknowledged = false;
				}
				else
				{
					return;
				}
			}
			else if (_receivedByte == StatusByteCode.NotAcknowledged)
			{
				//Resend directives
				Data.com.SendByte((byte)_nextDirective);
			}
			else if (_sentDirectiveIsUnacknowledged == false)
			{
				if (_receivedByte == StatusByteCode.Enquiry)
				{
                    getNextDirective();
					Data.com.SendByte((byte)_nextDirective);
                    if (_i == (Data.nav.path.Count - 1))
                    {
                        _i++;
                        getNextDirective();
                        Data.com.SendByte((byte)_nextDirective);
                    }
					_sentDirectiveIsUnacknowledged = true;
				}
				else if (_receivedByte == StatusByteCode.MineDetected)
				{
					//Detected mine, add to list
					Data.nav.mines.Add(Data.nav.path[_i - 1]);
					Data.nav.SetMinesInDLL();

					recalculatePath();

					//getNextDirective();
					Data.com.SendByte((byte)StatusByteCode.Acknowledged);
					//_sentDirectiveIsUnacknowledged = true;
				}
				else if (_receivedByte == StatusByteCode.Done)
				{
					//Finished
					//TODO: Handle this
				}
			}
			else
			{
				//Invalid bytecode or out of sync
				return;
            }
            Data.vis.DrawField();
            System.Diagnostics.Debug.WriteLine("Serial byte received: {0}", e.DataByte);

        }

		private void getNextDirective()
		{
			//Robot asks for new directions
			if (Data.nav.path.Count > 0 && Data.nav.path.Count > _i)
			{
				_nextNodeConnection = Data.nav.path[_i];
			}
			else if (Data.nav.path.Count==_i)
			{
				_nextDirective = StatusByteCode.Done;
                return;
			}
			else
			{
				//No path
				_nextDirective = StatusByteCode.Unknown;
				return;
			}

			//Exit point
			if (_i == Data.nav.path.Count - 1)
			{
				if (Data.exitCP > (Data.numControlPosts - (Data.N - 2)))
				{
					//exitCP is at left side
					_nextAbsoluteDirection = Direction.Left;
				}
				else if (Data.exitCP > (Data.numControlPosts - ((Data.N - 2) + (Data.M - 2))))
				{
					//exitCP is at top side
					_nextAbsoluteDirection = Direction.Up;
				}
				else if (Data.exitCP > (Data.numControlPosts - (2 * (Data.N - 2) + (Data.M - 2))))
				{
					//exitCP is at right side
					_nextAbsoluteDirection = Direction.Right;
				}
				else if (Data.exitCP > (Data.numControlPosts - (2 * (Data.N - 2) + 2 * (Data.M - 2))))
				{
					//exitCP is at bottom side
					_nextAbsoluteDirection = Direction.Down;
				}
				else
				{
					_nextAbsoluteDirection = Direction.Unknown;
				}
			}
			else
			{
				//Determine next direction in the field
				if ((int)_nextNodeConnection.From.X - (int)_nextNodeConnection.To.X >= 1)
				{
					//Next destination is left of origin
					_nextAbsoluteDirection = Direction.Left;
				}
				else if ((int)_nextNodeConnection.From.X - (int)_nextNodeConnection.To.X <= -1)
				{
					//Next destination is right of origin
					_nextAbsoluteDirection = Direction.Right;
				}
				else if ((int)_nextNodeConnection.From.Y - (int)_nextNodeConnection.To.Y <= -1)
				{
					//Next destination is above origin
					_nextAbsoluteDirection = Direction.Up;
				}
				else if ((int)_nextNodeConnection.From.Y - (int)_nextNodeConnection.To.Y >= 1)
				{
					//Next destination is under origin
					_nextAbsoluteDirection = Direction.Down;
				}
				else
				{
					//Shit is pretty wrong
					_nextAbsoluteDirection = Direction.Unknown;
				}
			}

			//Find initial direction
			if (_robotDirection == Direction.Unknown)
			{
				if (Data.nav.currentPos.FromCPoint == true)
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
				else if (Data.nav.currentPos.FromCPoint == false && Data.nav.currentPos.ToCPoint == false)
				{
					if (Data.nav.currentPos.From.Y > Data.nav.currentPos.To.Y)
					{
						_robotDirection = Direction.Down;
					}
					else if (Data.nav.currentPos.From.Y < Data.nav.currentPos.To.Y)
					{
						_robotDirection = Direction.Up;
					}
					else if (Data.nav.currentPos.From.X > Data.nav.currentPos.To.X)
					{
						_robotDirection = Direction.Left;
					}
					else if (Data.nav.currentPos.From.X > Data.nav.currentPos.To.X)
					{
						_robotDirection = Direction.Right;
					}
					else
					{
						_robotDirection = Direction.Unknown;
					}
				}
				else
				{
					_robotDirection = Direction.Unknown;
				}
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
			else if (_nextAbsoluteDirection - _robotDirection == 2 || _nextAbsoluteDirection - _robotDirection == -2)
			{
				_nextDirective = StatusByteCode.Turn;
			}
			else
			{
				_nextDirective = StatusByteCode.Forward;
			}
		}

		private void recalculatePath()
		{
			_i = 0;
			Data.nav.currentPos = new NodeConnection(Data.nav.currentPos.From, Data.nav.currentPos.To);
			_robotDirection = (Direction)(((int)_robotDirection + 2) % 4); //Bacon flips
			Data.nav.findPath();
		}

		public void Reset()
		{
			_sentDirectiveIsUnacknowledged = false;
			_nextAbsoluteDirection = Direction.Unknown; //next direction in terms of the XY grid
			_robotDirection = Direction.Unknown; //current direction the robot is pointing in
			_nextDirective = StatusByteCode.Unknown; //next directive to be sent to robot
			_i = 0;
		}
	}
}
