using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MiDeRP
{
    public class Navigation
    {
        public List<NodeConnection> mines = new List<NodeConnection>();
        public List<NodeConnection> visited = new List<NodeConnection>();
        public NodeConnection currentPos;
        public List<NodeConnection> fullPath = new List<NodeConnection>();
		public List<uint> targetCPs = new List<uint>((int)Data.numControlPosts);
		public int currentPath = 0;
		public List<NodeConnection>[] paths;

		public uint currentExitCP
		{
			get
			{
				return targetCPs[currentPath];
			}
		}

		public Coord currentExitCPCoord
		{
			get
			{
				return new Coord(currentExitCP);
			}
		}

		#region DLL imports
		/// <summary><c>initNavigation</c> is a method in the navigation.c file. Imported at runtime from the pure C dll navigation.dll.
        /// </summary>
        [DllImport("navigation.dll")]
        static extern int initNavigation(uint m, uint n);

        [DllImport("navigation.dll")]
        public static extern int updatePath(uint EntryID, uint ExitID);

        [DllImport("navigation.dll")]
        public static extern uint getPathLength();

        [DllImport("navigation.dll")]
        public static extern int extractPath(IntPtr path);

        [DllImport("navigation.dll")]
        static extern void closeNavigation();

        [DllImport("navigation.dll")]
        static extern int clearMines();

		[DllImport("navigation.dll")]
        static extern IntPtr extractPath();

        [DllImport("navigation.dll")]
        static extern int setMineC(uint X1, uint Y1, uint X2, uint Y2, byte mine);
		#endregion

		public Navigation()
        {
            //constructor
            initNavigation(Data.M, Data.N);
        }

        ~Navigation()
        {
            //destructor
            closeNavigation();
        }

		#region Path planning
		public void InitChallenge()
		{
			if (Data.challenge == Challenge.FindPath)
			{
				makePaths();
			}
			else if (Data.challenge == Challenge.FindTreasure)
			{
				Data.ctr.treasureSearchList.Clear();
				addAllNodeConnections(Data.M - 1, Data.N);
				addAllNodeConnections(Data.M, Data.N - 1);
                Data.nav.visited.Clear();
				findTreasure();
			}
			else
			{
				throw new ArgumentException("Invalid challenge...");
			}
		}

		public void makePaths()
		{
			if (targetCPs.Count < 1)
				throw new ArgumentOutOfRangeException("Need one or more controlposts");

			paths = new List<NodeConnection>[targetCPs.Count];
			fullPath.Clear();

			for (int i = 0; i < targetCPs.Count; i++)
			{
				currentPath = i;
				if (i == 0)
				{
					updateCurrentPath(currentPos.To, new Coord(targetCPs[0]));
				}
				else
				{
					updateCurrentPath(new Coord(targetCPs[i - 1]), new Coord(targetCPs[i]));
				}
                if(paths[currentPath]!=null)
				    fullPath.AddRange(paths[currentPath]);
			}

			currentPath = 0;
			Data.vis.DrawField();
		}

		public void updateCurrentPath(Coord entry, Coord exit)
		{
            try
            {
                paths[currentPath] = getPath(entry, exit);
                Data.vis.DrawField();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
		}

		private void addAllNodeConnections(uint xlim, uint ylim)
		{
			for (uint x = 0; x < xlim; x++)
			{
				for (uint y = 0; y < ylim; y++)
				{
					Coord N1 = new Coord();
					Coord N2 = new Coord();
					NodeConnection ml = new NodeConnection();
					N1.X = x;
					N1.Y = y;
					N2.X = x;
					N2.Y = y;
					if (xlim == Data.M && ylim != Data.N)					
						N2.Y++;					
					else
						N2.X++;						
					ml.To = N1;
					ml.From = N2;
					Data.ctr.treasureSearchList.Add(ml);
				}

			}
		}

        int getH (Coord node, Coord sourceNode)
        {
            return Math.Abs((int)sourceNode.X - (int)node.X) + Math.Abs((int)sourceNode.Y - (int)node.Y);           

        }

		public void findTreasure()
		{
            List<NodeConnection> visited = new List<NodeConnection>();

			Data.ctr.GetInitialRobotDirection();
			getNextAxis();
            
			Data.vis.DrawField();
		}

		public void getNextAxis()
		{
			const int notvisitedscore = 10, visitedpenalty = 6, turnpenalty = 20, fullturnpenalty = 24, minepenalty = 999; 
			int i, score = -999, temp_score, length = 0;
			Direction nextdir = Direction.Unknown, currentdir;
			NodeConnection currentnodeconnection = currentPos;
			Coord to = currentPos.To;

			for (i = 0; i < 4; i++)
			{
				currentdir = (Direction)i;
				temp_score = 0;

				//Add score for longer possible path
				if (currentdir == Direction.Left)
					length = (int)currentPos.To.X;
				else if (currentdir == Direction.Right)
					length = (int)Data.M - 1 - (int)currentPos.To.X;
				else if (currentdir == Direction.Up)
					length = (int)Data.N - 1 - (int)currentPos.To.Y;
				else if (currentdir == Direction.Down)
					length = (int)currentPos.To.Y;

				if (length == 0)
					continue;

				//Subtract score for any mines and visited nodes
				if (currentdir == Direction.Left)
					currentnodeconnection = new NodeConnection(new Coord(currentPos.To.X, currentPos.To.Y), new Coord(currentPos.To.X - 1, currentPos.To.Y));
				else if (currentdir == Direction.Right)
					currentnodeconnection = new NodeConnection(new Coord(currentPos.To.X, currentPos.To.Y), new Coord(currentPos.To.X + 1, currentPos.To.Y));
				else if (currentdir == Direction.Up)
					currentnodeconnection = new NodeConnection(new Coord(currentPos.To.X, currentPos.To.Y ), new Coord(currentPos.To.X , currentPos.To.Y + 1));
				else if (currentdir == Direction.Down)
					currentnodeconnection = new NodeConnection(new Coord(currentPos.To.X, currentPos.To.Y), new Coord(currentPos.To.X, currentPos.To.Y - 1));

				if (visited.Contains(currentnodeconnection) || visited.Contains(currentnodeconnection.Flipped))
					temp_score -= (visitedpenalty * ((int)Data.M - length) + (visited.IndexOf(currentnodeconnection) / 6));
				if (mines.Contains(currentnodeconnection) || mines.Contains(currentnodeconnection.Flipped))
					temp_score -= minepenalty;
				if (!(visited.Contains(currentnodeconnection) || visited.Contains(currentnodeconnection.Flipped)))
					temp_score += notvisitedscore;

				//Substract for making a turn
				if (currentdir != Data.ctr.RobotDirection)
					temp_score -= turnpenalty;

				//Substract for making a full turn
				if (((int)currentdir + 2) % 4 == (int)Data.ctr.RobotDirection)
					temp_score -= fullturnpenalty;

				//Is this path better?
				if (temp_score >= score)
				{
					score = temp_score;
					nextdir = currentdir;
				}
			}

			if (nextdir == Direction.Left)
			{
				to = new Coord(currentPos.To.X - 1, currentPos.To.Y);
			}
			else if (nextdir == Direction.Right)
			{
				to = new Coord(currentPos.To.X + 1, currentPos.To.Y);
			}
			else if (nextdir == Direction.Up)
			{
				to = new Coord(currentPos.To.X, currentPos.To.Y + 1);
			}
			else if (nextdir == Direction.Down)
			{
				to = new Coord(currentPos.To.X, currentPos.To.Y - 1);
			}
			
			fullPath = getPath(currentPos.To, to);
		}
		#endregion

		#region Pathfinder
		private int setMinesInDLL()
		{
			if (clearMines() != 0)
			{
				//error
				return -1;
			}
			foreach (NodeConnection m in mines)
			{
				setMineC(m.To.X, m.To.Y, m.From.X, m.From.Y, 1);
			}
			return 0;
		}

		public List<NodeConnection> getPath(Coord entry, Coord exit)
        {
			setMinesInDLL();
			int res = updatePath(entry.Id, exit.Id);
			if (res != 0)
				throw new ArgumentException("No path found");

       		List<NodeConnection> path = new List<NodeConnection>();
			uint len = getPathLength();
            IntPtr ptr = Marshal.AllocHGlobal(((int)len+1)*sizeof(int));
            int[] stage1 = new int[len+1];
            res = extractPath(ptr);

			if (res == 0 && len > 0 && ptr != null)
			{
				Marshal.Copy(ptr, stage1, 0, (int)len + 1);
				Marshal.FreeHGlobal(ptr);
				List<Coord> stage2 = new List<Coord>(); //node's
				for (int i = 0; i < len + 1; i++)
				{
					Coord c = new Coord();
					c.Id = (uint)stage1[i];
					stage2.Add(c);
				}
				Coord? prev = null;
				foreach (Coord c in stage2)
				{
					if (prev == null)
					{
						prev = c;
					}
					else
					{
						NodeConnection nc = new NodeConnection((Coord)c, (Coord)prev);
						path.Add(nc);
						prev = c;
					}
				}
				if (Data.challenge == Challenge.FindPath)
				{
					if (currentPath > 0)
						path.Insert(0, new NodeConnection(new Coord(targetCPs[(int)(currentPath - 1)]), false));
					path.Add(new NodeConnection(new Coord(currentExitCP), true));
				}
			}
             //Update UI
            Data.db.UpdateProperty("PathLength");
            Data.db.UpdateProperty("MineCount");
            Data.db.UpdateProperty("CurrentPosText");
			return path;
        }

		public void recalculatePath()
		{
			currentPos = currentPos.Flipped;
            if (Data.challenge == Challenge.FindPath)
            {
                makePaths();
            }
            else if (Data.challenge == Challenge.FindTreasure)
            {
                findTreasure();
            }
            else
            {
                throw new ArgumentException("Invalid challenge...");
            }			
		}
		#endregion

		public void updateCP(uint id)
		{
			if (targetCPs.Contains(id))
				targetCPs.Remove(id);
			else
				targetCPs.Add(id);
		}
	}
}
