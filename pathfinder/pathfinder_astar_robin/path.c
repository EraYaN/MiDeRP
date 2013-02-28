#include "util.h"

void findPath ()
{
	unsigned int i;
	Node *currentNode = entryNode;
	Node **neighbors, *neighbor;

	while(1)
	{
		if (currentNode == exitNode)
		{
			//ADD ACTUAL CODE HERE
			break;
		}

		currentNode->open = 0;

		neighbors = getNeighbors (currentNode);
		for (i=0; i<4; i++)
		{
			neighbor = neighbors[i];

		}
		free (neighbors);
	}
}
