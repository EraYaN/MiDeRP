#include <stdlib.h>
#include <stdio.h>

#include "util.h"
#include "node.h"
#include "path.h"

int main()
{
	//Init
	int entry_id, exit_id;
	entryNode = NULL;
	exitNode = NULL;

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

	//Preallocate data
	nodes = (Node**)safeMalloc(sizeof(Node*)*numNodes);
	print(1, 1, "Init successful, creating grid...\n");

	//Create the grid
	startStopwatch ();
	createGrid ();
	gridTime = stopStopwatch();
	print(1, 1, "Grid created successfully (took %.4lfs), ready to calculate paths!\n", gridTime);

	//Get entryNode and exitNode nodes
	while (!entryNode)
	{
		print(1, 0, "\nEnter entry node id: \n");
		scanf("%d", &entry_id);
		if (!getNode(entry_id))
			print(1, 1, "Error: tried to use non-existing node!\n");
		else
			entryNode = getNode(entry_id);
	}
	print (1, 1, "Entry node is %d\n", entryNode->id);

	while (!exitNode)
	{
		print(1, 0, "Enter exit node id: \n");
		scanf("%d", &exit_id);
		if (exit_id == entry_id)
			print(1, 1, "Error: entry node cannot be same as exit node!\n");
		else if (!getNode(exit_id))
			print(1, 1, "Error: tried to use non-existing node!\n");
		else
			exitNode = getNode(exit_id);
	}
	print (1, 1, "Exit node is %d\n", exitNode->id);

	//Find path!
	print(1, 1, "\nPoints OK! Starting pathfinder...\n\n");
	findPath ();

	//Exit program
	totalTime = gridTime + expansionTime + backTraceTime;
	print (1, 1, "\nDone! Total calculation time is %.4lfs\n", totalTime);
	print (0, 1, "All done!\n##########################################\n\n");
	fclose (file);

	return 0;
}
