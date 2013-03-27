#include "util.h"

void makePath ();
void displayPath ();

//Find a path from startingNode to exitNode
void findPath ()
{
	//Init
	unsigned int i, pre_g;
	Node **open, *neighbor, *current, *next;
	current = next = entryNode; //The value of current actually matters, next just has to have a value

	startStopwatch();

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

	findTime = stopStopwatch ();

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

	displayPath ();

}

//Display found path
void displayPath ()
{
	unsigned int i;

	if (!path)
	{
		print (1, 1, "Failed to find a path, quitting...\n");
		return;
	}

	print (1, 1, "Found a path from node %d to node %d, with length %d (took %.4lfs)!\n", entryNode->id, exitNode->id, length, findTime);

	if (length > 250)
	{
		print (1, 1, "Path length too high for display in console, will only print to logs\n\n");
		print (0, 1, "Entry node is %d\n", entryNode->id);
		for (i=1; i<length; i++)
		{
			print (0, 1, "Next node in path is node %d\n", path[i+1]->id);
		}
		print (0, 1, "Exit node (id: %d) reached!\n", exitNode->id);
	}
	else
	{
		print (1, 0, "Displaying result:\n\n");
		print (1, 1, "Entry node is %d\n", entryNode->id);
		for (i=1; i<length; i++)
		{
			print (1, 1, "Next node in path is node %d\n", path[i+1]->id);
		}
		print (1, 1, "Exit node (id: %d) reached!\n\n", exitNode->id);
	}


}