#include "util.h"

void makePath ();

//Find a path from startingNode to exitNode
void findPath ()
{
	//Init
	unsigned int i, pre_g;
	Node **open, *neighbor, *current, *next;
	current = next = entryNode; //The value of current actually matters, next just has to have a value

	open = (Node**) safeMalloc (sizeof (Node*) * (size_t)numNodes);
	open[0] = entryNode;
	length = 0;

	//Start finder
	while (next)
	{
		current = next;
		next = NULL;
		length++;

		if (current == exitNode)
		{
			//Reached endpoint, trace back path
			makePath ();
			break;
		}

		current->open = 0;

		setNeighbors (current);
		for (i=0; i<4; i++)
		{
			neighbor = current->neighbors[i];
			if (!neighbor || isMine (current, neighbor))
				continue; //neighbor does not exist

			pre_g = current->g + 1;
			if (!neighbor->open && pre_g >= neighbor->g)
			{
				//Neighbor is further from target than current node
				continue;
			}
			else
			{
				//Neighbor might be part of the shortest path to target
				neighbor->previous = current;
				neighbor->g = pre_g;
				neighbor->f = neighbor->g + getH (neighbor);
				neighbor->open = 1;

				if (!next || next->f >= neighbor->f)
				{
					//Last node will be picked as next node in path
					next = neighbor;
				}
			}
		}
	}
}

//Backtrace found path and save it to an array
void makePath ()
{
	Node *current = exitNode;
	unsigned int i;

	if (current->previous)
	{
		path = (Node**) safeMalloc (sizeof (Node*) * (size_t) length);

		for (i=length; i>0; i--)
		{
			path[i] = current;
			current = current->previous;
		}
	}
}