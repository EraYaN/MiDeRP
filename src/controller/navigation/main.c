#include "util.h"

//TODO make one cpp file to include the project and make this into a properly interfaced class or pure C dll

//Init
int init (unsigned int _m, unsigned int _n)
{
	//Open log file
	m = _m;
	n = _n;

	remove ("log_old.txt");
	rename ("log.txt", "log_old.txt");

	//reroute stdout to print to log.txt
	freopen("log.txt", "a", stdout);
	printf( "##########################################\nProgram started\n\n");

	//Create the grid
	createGrid(m,n);

	return 0;
}

int loop (unsigned int entry, unsigned int exit)
{
	int foundPath;
	Node *entryNode, *exitNode;

	//Find path
	if (!getNodeI (entry))
		entryNode = newNode (entry);
	else
		entryNode = getNodeI (entry);
	if (!getNodeI (exit))
		exitNode = newNode (exit);
	else
		exitNode = getNodeI (exit);

	foundPath = findPath (entryNode, exitNode); //TODO need class to store m and n
	cleanup ();

	if (foundPath == 0)
	{
		//Success
		//Display calculation time cause it's hot
		totalTime = gridTime + findTime;
		printf( "\nDone! Total calculation time is %.4lfs\n", totalTime);
		return 0;
	}
	else if (foundPath == -1)
	{
		//Fail, openlist was empty before exitNode was reached
		printf( "\nFailed to find a path, quitting..\n");
		return -1;
	}
	else if (foundPath == -2)
	{
		//Fail, path was found but not displayed, something is very wrong
		printf( "\nApparently found a path, but could not display, something is very wrong, quitting..\n");
		return -2;
	}
	else
	{
		//Fail, unknown error
		printf( "\nUnknown error while finding path, quitting..\n");
		return 1;
	}

}

void quit ()
{
	printf( "Quitting...\n##########################################\n\n");
	fclose (stdout);
}