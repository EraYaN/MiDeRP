#include "util.h"

void makePath ();

void findPath ()
{
	//Init
	unsigned int i, pre_g;
	Node **neighbors, **open, *neighbor, *current, *next;
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
			makePath ();
			break;
		}

		current->open = 0;

		neighbors = getNeighbors (current);
		for (i=0; i<4; i++)
		{
			neighbor = neighbors[i];
			if (!neighbor)
				continue; //neighbor does not exist

			pre_g = current->g + 1;
			if (!neighbor->open && pre_g >= neighbor->g)
			{
				continue;
			}
			else
			{
				neighbor->previous = current;
				neighbor->g = pre_g;
				neighbor->f = neighbor->g + getH (neighbor);
				neighbor->open = 1;

				if (!next || next->f >= neighbor->f)
				{
					next = neighbor;
				}
			}
		}
	}
}

void makePath ()
{
	Node *current = exitNode;
	unsigned int i;

	if (!current->previous)
	{
		print (1, 1, "Failed to find a path, quitting...\n");
	}
	else
	{
		path = (Node**) safeMalloc (sizeof (Node*) * (size_t) length);

		for (i=length; i>0; i--)
		{
			path[i] = current;
			current = current->previous;
		}
	}
}