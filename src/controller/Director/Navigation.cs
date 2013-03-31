using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Director
{
    public class Navigation
    {
        public List<NodeConnection> mines = new List<NodeConnection>();
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
        public int SetMinesInDLL(){
            if (clearMines() != 0)
            {
                //error
                return -1;
            }
            foreach(NodeConnection m in mines){
                setMineC(m.N1.X, m.N1.Y, m.N2.X, m.N2.Y, 1);
            }
            return 0;
        }
        /*Node* getNodeFromControlPost(long controlPost)
        {
            //TODO
            //long i;
            const long hori = m - 2; //controlPosts per horizontale zijde van het veld
            const long vert = n - 2; //controlPosts per verticale zijde van het veld
            if (controlPost > numControlPosts || controlPost < 1)
            {
                //bestaat niet!
                return NULL;
            }
            if (controlPost <= hori)
            {
                //onder
                return getNode(controlPost, 0);
            }
            else if (controlPost > hori && controlPost <= hori + vert)
            {
                //rechts
                return getNode(m - 1, controlPost - hori);
            }
            else if (controlPost > hori + vert && controlPost <= 2 * hori + vert)
            {
                //boven
                return getNode((m - 1) - (controlPost - hori - vert), n - 1);
            }
            else if (controlPost > 2 * hori + vert && controlPost <= 2 * hori + 2 * vert)
            {
                //links
                return getNode(0, (n - 1) - (controlPost - 2 * hori - vert));
            }
            else
            {
                //bestaat niet!
                return NULL;
            }
            return NULL;
        }*/
    }
}
