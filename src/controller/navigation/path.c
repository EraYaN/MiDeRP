#include "util.h"

unsigned int getH(Node *node);
void addToOpenlist (Node *node);
void makePath ();
int displayPath ();

//Find a path from startingNode to exitNode
int findPath (Node * entryNode, Node * exitNode)
{
	//Init
	unsigned int i, pre_g, displayedPath;
	Node *neighbor, *current;
	Openlist *tmp;

	head = NULL;

	entryNode->f = getH (entryNode);
	addToOpenlist (entryNode);
	current = entryNode;

	startStopwatch();

	length = 1;

	//Start finder
	while (head)
	{
		current = head->node;

#ifdef _DEBUG
		printf( "\nCurrent item: %d\n", current->id);
#endif

		if (current == exitNode)
		{
			//Reached endpoint, trace back path
			makePath ();
			findTime = stopStopwatch ();
			displayedPath = displayPath ();
			
			if (displayedPath == 0)
			{
				//Succesfully found and displayed a path
				return 0;
			}
			else
			{
				//Path was found but not displayed, something is very wrong
				return -2;
			}
		}

		//Remove current node from open list so it will not be processed again
		current->open = 0;
		current->close = 1;
		tmp = head;
		head = head->next;
		free (tmp);

		setNeighbors (current);
		for (i=0; i<4; i++)
		{
			neighbor = current->neighbors[i];

			if (!neighbor || isMine (current, neighbor))
			{
#ifdef _DEBUG
				printf( "Neighbor does not exist or found mine\n");
#endif
				continue; //neighbor does not exist
			}

#ifdef _DEBUG
			printf( "Now checking neighbor node %d\n", neighbor->id);
#endif

			pre_g = current->g + 1;
			if (neighbor->close && pre_g >= neighbor->g)
			{
				//Neighbor is further from target than current node
#ifdef _DEBUG
				printf( "Neighbor is closed or further away from target\n");
#endif
				continue;
			}
			else if (!neighbor->open || pre_g < neighbor->g)
			{
				//Neighbor might be part of the shortest path to target
				neighbor->previous = current;
				neighbor->g = pre_g;
				neighbor->f = neighbor->g + getH (neighbor);
				if (!neighbor->open)
				{
					neighbor->open = 1;
					addToOpenlist (neighbor);
				}
			}
		}
	}

	return -1;

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
		printf( "Added node %d as first item to openlist\n", node->id);
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
				printf( "Added node %d to openlist\n", node->id);
#endif
				return;
			}
			else
			{
				current = current->next;
				continue;
			}
		}

		//Fail, openlist was empty before exitNode was reached
		printf( "Error: failed to add node %d to openlist\n", node->id);

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
int displayPath ()
{
	unsigned int i;

	if (!path)
	{
		printf( "Error: called displayPath (), but no path found, this should not be happening!\n\n");
		return -1;
	}

	printf( "Found a path from node %d to node %d, with length %d (took %.4lfs)!\n", entryNode->id, exitNode->id, length, findTime);

	if (length > 250)
	{
		printf( "Path length too high for display in console, will only print to logs\n\n");
		printf( "Entry node is %d\n", entryNode->id);
		for (i=0; i<length; i++)
		{
			printf( "Next node in path is node %d\n", path[i+1]->id);
		}
		printf( "Exit node (id: %d) reached!\n", exitNode->id);
	}
	else
	{
		printf( "Displaying result:\n\n");
		printf( "Entry node is %d\n", entryNode->id);
		for (i=1; i<length; i++)
		{
			printf( "Next node in path is node %d\n", path[i+1]->id);
		}
		printf( "Exit node (id: %d) reached!\n\n", exitNode->id);
	}

	return 0;
}

void cleanup ()
{
	unsigned int i;
	Openlist *tmp, *current;

	//Empty openlist completely
	while (head)
	{
		tmp = head;
		head = head->next;
		free (tmp);
	}

	free (head);

	//Reset node values
	for (i=0; i<numNodes; i++)
	{
		nodes[i]->f = 0;
		nodes[i]->g = 0;
		nodes[i]->open = 0;
		nodes[i]->close = 0;
		nodes[i]->previous = NULL;
	}
}