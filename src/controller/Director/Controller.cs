using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiDeRP
{
    

    public class Controller
	{
		private bool _sentDirectiveIsUnacknowledged = false;
		private bool _done = false;
		private bool _continue = false;
		private StatusByteCode _receivedByte;
		private NodeConnection _nextNodeConnection;
		private Direction _nextAbsoluteDirection = Direction.Unknown; //next direction in terms of the XY grid
		private StatusByteCode _nextDirective = StatusByteCode.Unknown; //next directive to be sent to robot	
		private int _i = 0;

		public List<NodeConnection> treasureSearchList = new List<NodeConnection>();

		private Direction _robotDirection = Direction.Unknown; //current direction the robot is pointing in
		public Direction RobotDirection
		{
			get
			{
				return _robotDirection;
			}
		}

		private bool _halfway = false;
		public bool Halfway
		{
			get
			{
				return _halfway;
			}
		}

        private bool _robotControlIsEnabled;
        public bool RobotControlIsEnabled
        {
            get
            {
                return _robotControlIsEnabled;
            }
        }
		
		public Controller()
		{
			Data.com.SerialDataEvent += com_SerialDataEvent;
            _robotControlIsEnabled = false;
		}

		#region Robot control
		private void com_SerialDataEvent(object sender, SerialDataEventArgs e)
		{
			if (!_robotControlIsEnabled)
				return;

			_receivedByte = e.DataByte.ToStatusByteCode();

			if (_receivedByte == StatusByteCode.Acknowledged)
			{
				if (_sentDirectiveIsUnacknowledged)
				{
					if (_done == true)
						DisableRobotControl();

					if (_nextDirective == StatusByteCode.Back)
					{
						Data.nav.currentPos = new NodeConnection(Data.nav.currentPos.From, true);
						_halfway = true;
					}
					else
					{
						_halfway = false;
						_robotDirection = _nextAbsoluteDirection;
						Data.nav.currentPos = _nextNodeConnection;
					}

					_i++; //Advance to next item in path					
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
					if (_i > 0 && (!_halfway && _nextDirective != StatusByteCode.Turn) && !Data.nav.fullPath[_i].FromPoint)
						return;

					getNextDirective();
					Data.com.SendByte((byte)_nextDirective);

					if (Data.challenge == Challenge.FindPath)
					{
						if (_done == true)
							Data.com.SendByte((byte)StatusByteCode.Done);

						if (_continue == true)
							Data.com.SendByte((byte)StatusByteCode.Continue);

						_continue = false;
					}

					_sentDirectiveIsUnacknowledged = true;
				}
				else if (_receivedByte == StatusByteCode.MineDetected)
				{
					if (_i == 0)
						return;

					//Detected mine, add to list
					if (Data.challenge == Challenge.FindTreasure)
					{
						if (!treasureSearchList.Remove(Data.nav.currentPos))
							treasureSearchList.Remove(Data.nav.currentPos.Flipped);
						Data.nav.visited.Add(Data.nav.currentPos);
					}

					Data.nav.mines.Add(Data.nav.fullPath[_i - 1]);

					_i = 0;
					Data.nav.recalculatePath();
					Data.com.SendByte((byte)StatusByteCode.Acknowledged);
					_halfway = true;
				}
				else if (_receivedByte == StatusByteCode.Halfway)
				{
					if (Data.challenge == Challenge.FindTreasure)
					{
						if (!treasureSearchList.Remove(Data.nav.currentPos))
							treasureSearchList.Remove(Data.nav.currentPos.Flipped);
						Data.nav.visited.Add(Data.nav.currentPos);
					}

					_halfway = true;
					Data.com.SendByte((byte)StatusByteCode.Acknowledged);
				}
			}
			else
			{
				//Invalid bytecode or out of sync
				return;
			}

			Data.vis.DrawField();
		}

		private void getNextDirective()
		{
			//Robot asks for new directions
			if (Data.challenge == Challenge.FindPath)
			{
				if (Data.nav.fullPath.Count > 0 && Data.nav.fullPath.Count > _i)
				{
					_nextNodeConnection = Data.nav.fullPath[_i];
				}

				if ((Data.nav.fullPath.Count - 1) == _i || Data.nav.fullPath.Count == 0)
				{
					_done = true;
				}
				else if (Data.nav.fullPath[_i].FromPoint == true)
				{
					//TargetCP visited
					_nextDirective = StatusByteCode.Turn;
					_nextAbsoluteDirection = (Direction)(((int)_robotDirection + 2) % 4);

					if (Data.nav.targetCPs != null && Data.challenge == Challenge.FindPath)
						Data.nav.targetCPs.RemoveAt(Data.nav.currentPath);
					return;
				}
			}

			if (Data.nav.fullPath[_i].ToPoint == true)
			{
				//TargetCP reached
				if (Data.nav.currentExitCPCoord.X == 0)
				{
					//nav.currentExitCP is at left side
					_nextAbsoluteDirection = Direction.Left;
				}
				else if (Data.nav.currentExitCPCoord.Y == Data.N - 1)
				{
					//nav.currentExitCP is at top side
					_nextAbsoluteDirection = Direction.Up;
				}
				else if (Data.nav.currentExitCPCoord.X == Data.M - 1)
				{
					//nav.currentExitCP is at right side
					_nextAbsoluteDirection = Direction.Right;
				}
				else if (Data.nav.currentExitCPCoord.Y == 0)
				{
					//nav.currentExitCP is at bottom side
					_nextAbsoluteDirection = Direction.Down;
				}
				else
				{
					_nextAbsoluteDirection = Direction.Unknown;
				}

				if ((Data.nav.fullPath.Count - 1) != _i)
				{
					_continue = true;
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
				if (Data.nav.currentPos.FromPoint == true)
				{
					//Amount of control posts on horizontal sides is m - 2, on vertical sides n - 2, placing is counterclockwise, starting from bottom left (1)
					if (Data.entryCPCoord.X == 0)
					{
						//entryCP is at left side
						_robotDirection = Direction.Right;
					}
					else if (Data.entryCPCoord.Y == Data.N - 1)
					{
						//entryCP is at top side
						_robotDirection = Direction.Down;
					}
					else if (Data.entryCPCoord.X == Data.M - 1)
					{
						//entryCP is at right side
						_robotDirection = Direction.Left;
					}
					else if (Data.entryCPCoord.Y == 0)
					{
						//entryCP is at bottom side
						_robotDirection = Direction.Up;
					}
					else
					{
						_robotDirection = Direction.Unknown;
					}
				}
				else if (Data.nav.currentPos.FromPoint == false && Data.nav.currentPos.ToPoint == false)
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

		public void DisableRobotControl()
		{
			_robotControlIsEnabled = false;
		}

        public void EnableRobotControl()
        {
            _robotControlIsEnabled = true;
        }
		
		public void ResetRobotControl()
		{
			_done = false;
			_halfway = false;
            _robotControlIsEnabled = false;
			_sentDirectiveIsUnacknowledged = false;
			_nextAbsoluteDirection = Direction.Unknown; //next direction in terms of the XY grid
			_robotDirection = Direction.Unknown; //current direction the robot is pointing in
			_nextDirective = StatusByteCode.Unknown; //next directive to be sent to robot
			_i = 0;
			Data.nav.currentPath = 0;
		}
		#endregion
	}
}
