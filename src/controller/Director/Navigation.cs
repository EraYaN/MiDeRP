﻿using System;
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
        public NodeConnection currentPos;
        public List<NodeConnection> fullPath = new List<NodeConnection>();
		public List<uint> targetCPs = new List<uint>((int)Data.numControlPosts);
		public int currentPath = 0;
		public List<NodeConnection>[] paths;
		public List<int> visitedYAxes = new List<int>(), visitedXAxes = new List<int>();
	
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
				fullPath.AddRange(paths[currentPath]);
			}

			currentPath = 0;
			Data.vis.DrawField();
		}

		public void findTreasure()
		{
			bool entryCPIsVertical;
			Coord currentNode, nextNode;

			if (Data.entryCPCoord.X % (Data.M - 1) == 0)
				entryCPIsVertical = true; //vertical axis
			else
				entryCPIsVertical = false; //horizontal axis

			currentNode = Data.entryCPCoord;
			currentPath = 0;
			paths = new List<NodeConnection>[Data.N * 2 + Data.M * 2 - 1];

			if (entryCPIsVertical)
			{
				//Get to corner if not already
				if (Data.entryCPCoord.Y % (Data.N - 1) != 0)
				{
					if (Data.entryCPCoord.Y > (Data.N / 2))
						nextNode = new Coord(Data.entryCPCoord.X, Data.N - 1);
					else
						nextNode = new Coord(Data.entryCPCoord.X, 0);

					updateCurrentPath(Data.entryCPCoord, nextNode);
					fullPath.AddRange(paths[currentPath]);
				}
				else
				{
					nextNode = Data.entryCPCoord;
				}

				nextNode = horizontalSweep(nextNode, 1);

				verticalSweep(nextNode, (int)Data.N * 2);

			}
			else
			{
				//Get to corner if not already
				if (Data.entryCPCoord.X % (Data.M - 1) != 0)
				{
					if (Data.entryCPCoord.X > (Data.M / 2))
						nextNode = new Coord(Data.M - 1, Data.entryCPCoord.Y);
					else
						nextNode = new Coord(0, Data.entryCPCoord.Y);

					updateCurrentPath(Data.entryCPCoord, nextNode);
					fullPath.AddRange(paths[currentPath]);
				}
				else
				{
					nextNode = Data.entryCPCoord;
				}

				nextNode = verticalSweep(nextNode, 1);
				horizontalSweep(nextNode, (int)Data.M * 2);
			
			}

			currentPath = 0;
			Data.vis.DrawField();
		}

		private Coord verticalSweep(Coord initialNode, int initialI)
		{
			int i;
			Coord currentNode, nextNode;

			currentNode = nextNode = initialNode;

			for (i = initialI; i < (initialI + Data.M * 2 - 1); i++)
			{
				currentPath = i;

				if ((i - (initialI - 1)) % 2 == 1)
				{
					//Cross over
					if (currentNode.Y == 0)
						nextNode = new Coord(currentNode.X, Data.N - 1);
					else
						nextNode = new Coord(currentNode.X, 0);
				}
				else
				{
					//Increase/decrease Y
					if (initialNode.X > (Data.M / 2))
						nextNode = new Coord(currentNode.X - 1, currentNode.Y);
					else
						nextNode = new Coord(currentNode.X + 1, currentNode.Y);
				}

				updateCurrentPath(currentNode, nextNode);
				fullPath.AddRange(paths[currentPath]);
				currentNode = nextNode;
			}

			return nextNode;
		}

		private Coord horizontalSweep(Coord initialNode, int initialI)
		{
			int i;
			Coord currentNode, nextNode;

			currentNode = nextNode = initialNode;

			for (i = initialI; i < (initialI + Data.N * 2 - 1); i++)
			{
				currentPath = i;
				
				if ((i - (initialI - 1)) % 2 == 1)
				{
					//Cross over
					if (currentNode.X == 0)
						nextNode = new Coord(Data.M - 1, currentNode.Y);
					else
						nextNode = new Coord(0, currentNode.Y);
				}
				else
				{
					//Increase/decrease Y
					if (initialNode.Y > (Data.N / 2))
						nextNode = new Coord(currentNode.X, currentNode.Y - 1);
					else
						nextNode = new Coord(currentNode.X, currentNode.Y + 1);
				}

				updateCurrentPath(currentNode, nextNode);
				fullPath.AddRange(paths[currentPath]);
				currentNode = nextNode;
			}

			return nextNode;
		}
		#endregion

		#region Pathfinder

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
				throw new InvalidOperationException("No path found");

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
			return path;
        }

		public void recalculatePath()
		{
			if (Data.challenge == Challenge.FindPath)
			{
				currentPos = currentPos.Flipped;
				Data.nav.makePaths();
			}
			else if (Data.challenge == Challenge.FindTreasure)
			{
				List<NodeConnection> tempPath = new List<NodeConnection>();
				Coord from = currentPos.From;
				Coord to = currentPos.To;

				//Determine initial path piece
				if (Data.ctr.RobotDirection == Direction.Left)
					from = new Coord(Data.M - 1, currentPos.From.Y);
				else if (Data.ctr.RobotDirection == Direction.Right)
					from = new Coord(0, currentPos.From.Y);
				else if (Data.ctr.RobotDirection == Direction.Up)
					from = new Coord(currentPos.From.X, 0);
				else if (Data.ctr.RobotDirection == Direction.Down)
					from = new Coord(currentPos.From.X, Data.N - 1);

				tempPath.AddRange(getPath(from, currentPos.From));

				//Add mine evasion maneuver
				tempPath.AddRange(getPath(currentPos.From, currentPos.To));

				//Add final path piece
				if (Data.ctr.RobotDirection == Direction.Left)
					to = new Coord(0, currentPos.To.Y);
				else if (Data.ctr.RobotDirection == Direction.Right)
					to = new Coord(Data.M - 1, currentPos.To.Y);
				else if (Data.ctr.RobotDirection == Direction.Up)
					to = new Coord(currentPos.To.X, Data.N - 1);
				else if (Data.ctr.RobotDirection == Direction.Down)
					to = new Coord(currentPos.To.X, 0);

				tempPath.AddRange(getPath(currentPos.To, to));

				//Replace current path segment
				paths[currentPath] = tempPath;
				fullPath.Clear();
				foreach (List<NodeConnection> path in paths)
					fullPath.AddRange(path);
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
