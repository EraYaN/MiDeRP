#include "input.h"

//Get grid dimensions
void getDimensions ()
{
	//TODO get those from parameter to int(int, int) instead
	m = 5;
	n = 5;

}

//Get entry and exit points
void getPoints ()
{
	//Init
	unsigned int entry_id, exit_id;

	//Get entry node
	while (!entryNode)
	{
		entry_id = 0;
	}
	
	//Get exit node
	while (!exitNode)
	{
		exit_id = n*m-1;
	}
	//TODO: fix this, What da FUK? Is the ID not enough?
	//Actually create entryNode and exitNode
	entryNode = newNode (NULL, entry_id);
	exitNode = newNode (NULL, exit_id);

	print(1, 1, "\nPoints OK! Starting pathfinder...\n\n");
}
