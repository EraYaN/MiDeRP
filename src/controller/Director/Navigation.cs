using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Generic;

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

		public uint currentExitCP
		{
			get
			{
				return targetCPs[currentPath];
			}
		}

		#region DLL imports
		/// <summary><c>initPathfinder</c> is a method in the <c>Pathfinder</c> class. Imported at runtime from the pure C dll navigation.dll.
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
			if (Data.challenge == Challenge.FindTreasure)
			{
				findTreasure();
			}
		}

		private void makePaths()
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
			if (updatePath(entry.Id, exit.Id) != 0)
				throw new Exception();
			paths[currentPath] = getPath();
			Data.vis.DrawField();
		}

		private void findTreasure()
		{
			throw new NotImplementedException();
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

        public List<NodeConnection> getPath()
        {
       		List<NodeConnection> path = new List<NodeConnection>();
			setMinesInDLL();

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
