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
        static extern int clearVisited();

        [DllImport("navigation.dll")]
        static extern IntPtr extractPath();

        [DllImport("navigation.dll")]
        static extern int setMineC(uint X1, uint Y1, uint X2, uint Y2, byte mine);

        [DllImport("navigation.dll")]
        static extern int setVisitedC(uint X1, uint Y1, uint X2, uint Y2, byte mine);
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
				throw new NotImplementedException();
			}
		}

		public void makePaths()
		{
			if (targetCPs.Count < 1)
				return; //exception maybe?

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

		public void updateCurrentPath(Coord entry, Coord exit)
		{
            setMinesInDLL();
            if (Data.challenge == Challenge.FindTreasure)
                setVisitedInDLL();
            int res = updatePath(entry.Id, exit.Id);
            if (res != 0)
                return;//throw new Exception();
            try
            {
                paths[currentPath] = getPath();
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
			
			fullPath.Clear();
			
			paths = new List<NodeConnection>[Data.ctr.treasureSearchList.Count];
            List<Coord> nodes = new List<Coord>();
            List<NodeConnection> visited = new List<NodeConnection>();
            List<NodeConnection> open = new List<NodeConnection>();

            foreach (NodeConnection nc in Data.ctr.treasureSearchList)
            {
                if (!nodes.Contains(nc.To))
                {
                    nodes.Add(nc.To);
                }
                if (!nodes.Contains(nc.From))
                {
                    nodes.Add(nc.From);
                }
            }
            var queryFarthestNode =
                from c in Data.ctr.treasureSearchList
                where !c.IsSame(Data.nav.currentPos) && !visited.Contains(c) && !visited.Contains(c.Flipped)
                orderby getH(c.To,Data.nav.currentPos.To) descending
                select c;

            List<NodeConnection> tempVisited = Data.nav.visited;
			/*for (int i = 0; i < Data.ctr.treasureSearchList.Count; i++)
			{
				currentPath = i;
				if (i == 0)
				{
					updateCurrentPath(currentPos.To, Data.ctr.treasureSearchList[i].To);
				}
				else
				{
					updateCurrentPath(Data.ctr.treasureSearchList[i].To, Data.ctr.treasureSearchList[i].From);
				}
				fullPath.AddRange(paths[currentPath]);
			}*/
            open.AddRange(queryFarthestNode);
            
            currentPath = 0;
            NodeConnection cPos = currentPos;
            Boolean first = true;
            NodeConnection n;
            while (open.Count()>0)
            {
                if (first)
                {
                    n = open.First();
                }
                else
                {
                    n = open.Last();
                }
                open.Remove(n);
                Data.nav.visited = visited;
                updateCurrentPath(cPos.To, n.To);
                if (paths[currentPath] != null)
                {
                    fullPath.AddRange(paths[currentPath]);
                    foreach (NodeConnection nc in paths[currentPath])
                    {
                        nodes.Remove(nc.To);
                        nodes.Remove(nc.From);
                        if (!visited.Contains(nc) && !visited.Contains(nc.Flipped))
                        {
                            visited.Add(nc);
                        }
                    }
                    
                    cPos = n;
                    
                    first = !first;
                }
                else
                {
                    break;
                }
                              
            }            
			currentPath = 0;
            Data.nav.visited = tempVisited;
			Data.vis.DrawField();
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

        private int setVisitedInDLL()
        {
            if (clearVisited() != 0)
            {
                //error
                return -1;
            }
            foreach (NodeConnection m in visited)
            {
                setVisitedC(m.To.X, m.To.Y, m.From.X, m.From.Y, 1);
            }
            return 0;
        }

        public List<NodeConnection> getPath()
        {
       		List<NodeConnection> path = new List<NodeConnection>();
			uint len = getPathLength();
            IntPtr ptr = Marshal.AllocHGlobal(((int)len+1)*sizeof(int));
            int[] stage1 = new int[len+1];
            int res = extractPath(ptr);

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
