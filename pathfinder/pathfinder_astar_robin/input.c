#include "input.h"

//Get grid dimensions
void getDimensions ()
{
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
}

//Get entry and exit points
void getPoints ()
{
	//Init
	unsigned int entry_id, exit_id;

	//Get entry node
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
	
	//Get exit node
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

	print(1, 1, "\nPoints OK! Starting pathfinder...\n\n");
}
