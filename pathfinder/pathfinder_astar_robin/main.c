#include "util.h"

int main()
{
	//Init
	unsigned int entry_id, exit_id;

	file = fopen ("log.txt", "a");
	print (0, 1, "##########################################\nProgram started\n\n");

	//Get grid dimensions
	while (!m)
	{
		print(1, 0, "Enter grid width: \n");
		scanf("%d", &m);
		if (m <= 1)
		{
			print(1, 0, "Width has to be more than 1\n");
			print(0, 1, "Entered invalid value for width\n");
			m = 0;
		}
	}
	print (1, 1, "Grid width is %d\n", m);
	while (!n)
	{
		print(1, 0, "Enter grid height: \n");
		scanf("%d", &n);
		if (n <= 1)
		{
			print(1, 0, "Height has to be more than 1\n");
			print(0, 1, "Entered invalid value for height\n");
			n = 0;
		}
	}
	print (1, 1, "Grid height is %d\n", n);

	//Calculate useful values
	print(1, 1, "\nInitializing variables...\n");
	numNodes =	m*n;

	print(1, 1, "Init successful, creating grid...\n");

	//Create the grid
	startStopwatch ();
	createGrid ();
	gridTime = stopStopwatch ();
	print(1, 1, "Grid created successfully (took %.4lfs), ready to calculate paths!\n", gridTime);

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

	///Get entryNode and exitNode id
	while (!entryNode)
	{
		print(1, 0, "\nEnter entry node id: \n");
		scanf("%d", &entry_id);
		if (entry_id < 0 || entry_id > numNodes)
			print(1, 1, "Error: tried to use non-existing node!\n");
		else
		{
			print (1, 1, "Entry node is %d\n", entry_id);
			break;
		}
	}
	
	while (!exitNode)
	{
		print(1, 0, "Enter exit node id: \n");
		scanf("%d", &exit_id);
		if (exit_id == entry_id)
			print(1, 1, "Error: entry node cannot be same as exit node!\n");
		else if (exit_id < 0 || exit_id > numNodes)
			print(1, 1, "Error: tried to use non-existing node!\n");
		else
		{
			print (1, 1, "Exit node is %d\n", exit_id);
			break;
		}
	}

	//Actually create entryNode and exitNode
	entryNode = newNode (NULL, entry_id);
	exitNode = newNode (NULL, exit_id);

	//Find path!
	print(1, 1, "\nPoints OK! Starting pathfinder...\n\n");
	startStopwatch();
	findPath ();
	findTime = stopStopwatch ();
	displayPath ();

	//Exit program
	totalTime = gridTime + findTime;
	print (1, 1, "\nDone! Total calculation time is %.4lfs\n", totalTime);
	print (0, 1, "All done!\n##########################################\n\n");
	fclose (file);

	return 0;
}