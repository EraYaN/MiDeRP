using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Director
{
    public class Navigation
    {
        public List<NodeConnection> mines = new List<NodeConnection>();
        public NodeConnection currentPos;
        public List<NodeConnection> path = new List<NodeConnection>();
        //TODO static dll import all function from c dll pathfinder.dll
        //Import all functions
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

        public int SetMinesInDLL(){
            if (clearMines() != 0)
            {
                //error
                return -1;
            }
            foreach(NodeConnection m in mines){
                setMineC(m.To.X, m.To.Y, m.From.X, m.From.Y, 1);
            }
            return 0;
        }

        public int getPath()
        {
            //TODO             
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
				Data.nav.path.Clear();
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
				Data.nav.path.Add(new NodeConnection(new Coord(Data.exitCP), true));
			}
             //Update UI
            Data.db.UpdateProperty("PathLength");
            return res;
        }

        public int findPath()
        {
            Coord entry = currentPos.To;
            Coord exit = new Coord(Data.exitCP);
            int res = updatePath(entry.Id, exit.Id);
            getPath();
            return res;
        }
    }
}
