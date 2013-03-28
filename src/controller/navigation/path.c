#include "util.h"

unsigned int getH();
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

	current->f = getH (current);
	addToOpenlist (current);
	currentItem = head;

	startStopwatch();

	length = 1;

	//Start finder
	while (head)
	{
		current = head->node;

#ifdef _DEBUG
		print(1, 1, "Current item: %d\n", current->id);
#endif

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

			if (!neighbor || isMine (current, neighbor))
			{
#ifdef _DEBUG
				print(1, 1, "Neighbor does not exist or found mine\n");
#endif
				continue; //neighbor does not exist
			}

#ifdef _DEBUG
			print(1, 1, "Now checking neighbor node %d\n", neighbor->id);
#endif

			pre_g = current->g + 1;
			if (neighbor->close && pre_g >= neighbor->g)
			{
				//Neighbor is further from target than current node
#ifdef _DEBUG
				print(1, 1, "Neighbor is closed or further away from target\n");
#endif
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
			}
		}
	}

	print (1, 1, "Error: failed to find a path!\n");

}

//Get an estimated heuristic cost to exitNode
unsigned int getH (Node *node)
{
	unsigned int H, straight = 0;

	H = abs ((int)(getXY(node, 'X') - getXY(exitNode, 'X'))) + abs ((int)(getXY(node, 'Y') - getXY(exitNode, 'Y')));

	return H;
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
#ifdef _DEBUG
		print (1, 1, "Added node %d as first item to openlist\n", node->id);
#endif
	}
	else
	{
		current = head;
		newItem = (Openlist*) safeMalloc (sizeof (Openlist));

		while (current)
		{
			if (node->f < current->node->f || !current->next)
			{
				newItem->node = node;
				newItem->next = current->next;
				current->next = newItem;
#ifdef _DEBUG
				print (1, 1, "Added node %d to openlist\n", node->id);
#endif
				return;
			}
			else
			{
				current = current->next;
				continue;
			}
		}

		print (1, 1, "Error: failed to add node %d to openlist\n", node->id);

	}
}

void removeFromOpenlist (Node *node)
{
	Openlist *current;
	current = head;

	if (node == head->node)
	{
		if (!head->next)
		{
			head = NULL;
		}
		else
		{
			current = head;
			head = head->next;
			free (current);
		}
#ifdef _DEBUG
		print (1, 1, "Removed node %d from openlist\n", node->id);
#endif
		return;
	}
	else
	{
		while (current)
		{
			if (!current->next)
			{
				//Last element in list
				print (1, 1, "Error: failed to remove node %d from open list, it probably wasn't in there in the first place!\n", node->id);
				break;
			}
			else if (current->next->node == node)
			{
				current->next = current->next->next;
				free (current->next);
#ifdef _DEBUG
				print (1, 1, "Removed node %d from open list", node->id);
#endif
				break;
			}
			current = current->next;
		}
	}
	
}

//Backtrace found path and save it to an array
void makePath ()
{
	Node *current = exitNode->previous;
	unsigned int i = 0;
	length = 0;

	while (current)
	{
		length++;
		current = current->previous;
	}

	path = (Node**) safeMalloc (sizeof (Node*) * (size_t) length);

	current = exitNode->previous;

	for (i=length; i>0; i--)
	{
		path[i] = current;

		current = current->previous;
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
		for (i=0; i<length; i++)
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