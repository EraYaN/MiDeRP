using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Director
{
    class Navigation
    {
        //TODO static dll import all function from c dll pathfinder.dll
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
        //Import all functions
        /// <summary><c>initPathfinder</c> is a method in the <c>Pathfinder</c> class. Imported at runtime from the pure C dll navigation.dll.
        /// </summary>
        [DllImport("navigation.dll")]
        static extern void initNavigation(int m, int n);

        //Import all functions
        [DllImport("navigation.dll")]
        public static extern void loopNavigation();

        //Import all functions
        [DllImport("navigation.dll")]
        static extern void closeNavigation();
    }
}
