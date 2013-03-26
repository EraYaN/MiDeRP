#include "util.h"

int main()
{
	//Init
	file = fopen ("log.txt", "a");
	print (0, 1, "##########################################\nProgram started\n\n");

	getDimensions ();
	
	//Create the grid	
	createGrid ();
	
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

	getPoints ();

	//Find path
	findPath ();	

	//Exit program
	totalTime = gridTime + findTime;
	print (1, 1, "\nDone! Total calculation time is %.4lfs\n", totalTime);
	print (0, 1, "All done!\n##########################################\n\n");
	fclose (file);

	return 0;
}