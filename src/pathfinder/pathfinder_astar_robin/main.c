#include "util.h"

int main ()
{
	init ();
	loop ();
	quit ();

	return 0;
}

//Init
void init ()
{
	//Open log file
	remove ("log_old.txt");
	rename ("log.txt", "log_old.txt");
	file = fopen ("log.txt", "a");
	print (0, 1, "##########################################\nProgram started\n\n");

	//Get grid size
	getDimensions ();

	//Create the grid
	createGrid ();

	//Add mines for debugging
#ifdef _DEBUG
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
#endif

	//Get entry and exit nodes
	getPoints ();
}

void loop ()
{
	findPath ();

	totalTime = gridTime + findTime;
	print (1, 1, "\nDone! Total calculation time is %.4lfs\n", totalTime);
}

void quit ()
{
	print (0, 1, "Quitting...\n##########################################\n\n");
	fclose (file);
}