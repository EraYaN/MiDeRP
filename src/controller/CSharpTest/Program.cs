using System;
using System.Runtime.InteropServices;

namespace CSharpTest
{
    class Program
    {
        /*[DllImport("pathfinder.dll")]
        static extern void init();
        [DllImport("pathfinder.dll")]
        static extern void loop();
        [DllImport("pathfinder.dll")]
        static extern void quit();*/
        [DllImport("DllTest.dll")]
        static extern void Message(string msg);

        static void Main(string[] args)
        {
            try
            {
                /*init();
                loop();
                quit();*/
                Message("Test");
            }
            catch (DllNotFoundException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (EntryPointNotFoundException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
