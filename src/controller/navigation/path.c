#include "util.h"

void addToOpenlist ();
void removeFromOpenlist ();
void makePath ();
void displayPath ();

//Find a path from startingNode to exitNode
void findPath ()
{
	//Init
	unsigned int i, pre_g;
	Node *neighbor, *current, *next;
	Openlist *currentItem;
	current = next = entryNode; //The value of current actually matters, next just has to have a value

	addToOpenlist (current);
	currentItem = head;

	startStopwatch();

	length = 0;

	//Start finder
	while (currentItem)
	{
		length++;

		if (current == exitNode)
		{
			//Reached endpoint, trace back path
			makePath ();
			findTime = stopStopwatch ();
			displayPath ();
			return;
		}

		current->open = 0;
		removeFromOpenlist (current);
		current->close = 1;

		setNeighbors (current);
		for (i=0; i<4; i++)
		{
			neighbor = current->neighbors[i];
			printf("now checking neighbor %d\n", neighbor);

			if (!neighbor || isMine (current, neighbor))
				continue; //neighbor does not exist

			pre_g = current->g + 1;
			if (neighbor->close && pre_g >= neighbor->g)
			{
				//Neighbor is further from target than current node
				continue;
			}
			else
			{
				//Neighbor might be part of the shortest path to target
				neighbor->previous = current;
				neighbor->previousDir = i;
				neighbor->g = pre_g;
				neighbor->f = neighbor->g + getH (neighbor);
				neighbor->open = 1;
				addToOpenlist (neighbor);
				printf("added node %d to openlist\n", neighbor->id);
			}
		}
	}

	print (1, 1, "Error: failed to find a path!\n");

}

//Add item to sorted openlist
void addToOpenlist (Node *node)
{
	Openlist *current, *newItem;

	if (!head)
	{
		head = (Openlist*) safeMalloc (sizeof (Openlist));
		head->node = node;
		head->next = NULL;
	}
	else
	{
		current = head;
		newItem = (Openlist*) safeMalloc (sizeof (Openlist));

		while (current)
		{
			if (node->f <= current->node->f )
			{
				newItem->node = node;
				newItem->next = current->next;
				current->next = newItem;
				return;
			}
			else
			{
				current = current->next;
				continue;
			}
		}
		
	}
}

void removeFromOpenlist (Node *node)
{
	Openlist *current;
	current = head;

	while (current)
	{
		if (current->next && current->next->node == node)
		{
			current->next = current->next->next;
			free (current->next);
		}
		else
		{
			head = NULL;
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