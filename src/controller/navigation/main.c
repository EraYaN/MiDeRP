#include "util.h"

//TODO make one cpp file to include the project and make this into a properly interfaced class or pure C dll

/*int main ()
{
	init ();
	loop ();
	quit ();

	return 0;
}*/

//Init
int init (int _m, int _n)
{
	//Open log file
	m = _m;
	n=_n;
	

	remove ("log_old.txt");
	rename ("log.txt", "log_old.txt");
	//file = fopen ("log.txt", "a");

	//reroute stdout
	freopen("log.txt", "a", stdout);
	//if(file==NULL)
	//return -1;
	printf( "##########################################\nProgram started\n\n");

	//Get grid size, NEIN nu argumenten
	//getDimensions ();

	//Create the grid
	createGrid(m,n);

	//Add mines for debugging
/*#ifdef _DEBUG
	addMine (12, 13);
	addMine (12, 11);
	addMine (12, 7);
	addMine (12, 17);
	addMine (10, 5);
	addMine (1, 6);
	addMine (15, 16);
	addMine (8, 9);
	addMine (22, 23);
	addMine (19, 14);
	addMine (13, 18);
#endif*/

	

	return 0;
}

void loop (unsigned int entry, unsigned int exit)
{
	//Get entry and exit nodes
	getPoints(entry, exit);
	findPath (entryNode, exitNode);//TODO need class to store m and n

	totalTime = gridTime + findTime;
	printf( "\nDone! Total calculation time is %.4lfs\n", totalTime);
}

void quit ()
{
	printf( "Quitting...\n##########################################\n\n");
	fclose (stdout);
}