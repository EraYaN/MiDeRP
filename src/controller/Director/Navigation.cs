using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Director
{
    public class Navigation
    {
        //TODO static dll import all function from c dll pathfinder.dll
        //Import all functions
        /// <summary><c>initPathfinder</c> is a method in the <c>Pathfinder</c> class. Imported at runtime from the pure C dll navigation.dll.
        /// </summary>
        [DllImport("navigation.dll")]
        static extern int initNavigation(int m, int n);

        [DllImport("navigation.dll")]
        public static extern int loopNavigation(uint EntryID, uint ExitID);

        [DllImport("navigation.dll")]
        static extern void closeNavigation();

        [DllImport("navigation.dll")]
        static extern int clearMines();

        [DllImport("navigation.dll")]
        static extern int setMineC(uint X1, uint Y1, uint X2, uint Y2, byte mine);

        public Navigation(int _M, int _N)
        {
            //constructor
            initNavigation(_M,_N);
        }
        ~Navigation()
        {
            //constructor
            closeNavigation();
        }
        static public int SetMinesInDLL(){
            if (clearMines() != 0)
            {
                //error
                return -1;
            }
            foreach(MineLocation m in Data.mines){
                setMineC(m.N1.X, m.N1.Y, m.N2.X, m.N2.Y, 1);
            }
            return 0;
        }
        
    }
}
