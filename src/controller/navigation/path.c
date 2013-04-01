#include "util.h"

unsigned int getH(Node *node, Node *exitNode);
void addToOpenlist (Node *node);
void makePath (Node *entryNode, Node *exitNode);
int displayPath (Node *entryNode, Node *exitNode);

//Find a path from startingNode to exitNode
int findPath (Node * entryNode, Node * exitNode)
{
	//Init
	unsigned int i, pre_g, displayedPath;
	Node *neighbor, *current;
	Openlist *tmp;

	head = NULL;

	//prepare entryNode
	entryNode->f = getH (entryNode, exitNode);
	addToOpenlist (entryNode);
	current = entryNode;
	//determine initial direction
	if (getXY (entryNode, 'Y') == 0)
		entryNode->previousDir = 2;
	if (getXY (entryNode, 'Y') == (n - 1))
		entryNode->previousDir = 3;
	if (getXY (entryNode, 'X') == 0)
		entryNode->previousDir = 1;
	if (getXY (entryNode, 'X') == (m - 1))
		entryNode->previousDir = 0;

	startStopwatch();

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
			makePath (entryNode, exitNode);
			findTime = stopStopwatch ();
			displayedPath = displayPath (entryNode, exitNode);
			
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
			if (current->previousDir != -1 && current->previousDir != i)
				pre_g = pre_g + 2;

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
				neighbor->previousDir = i;
				neighbor->g = pre_g;
				neighbor->f = neighbor->g + getH (neighbor, exitNode);
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
unsigned int getH (Node *node, Node *exitNode)
{
	unsigned int H, straight = 0;

	H = abs ((int)(getXY(node, 'X') - getXY(exitNode, 'X'))) + abs ((int)(getXY(node, 'Y') - getXY(exitNode, 'Y')));

	//if (node->previous && node->previousDir != -1 && node->previous->previousDir != node->previousDir)
	//	H = H + 1;

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
void makePath (Node *entryNode, Node *exitNode)
{
	Node *current = exitNode;
	unsigned int i;
	length = 0;

	while (current->previous)
	{
		length++;
		current = current->previous;
	}

	path = (Node**) safeMalloc (sizeof (Node*) * (size_t) (length+1));

	current = exitNode;

	for (i=length; i>0; i--)
	{
		path[i] = current;

		current = current->previous;
	}
	path[0] = entryNode;
}

//Display found path
int displayPath (Node *entryNode, Node *exitNode)
{
	unsigned int i;

	if (!path)
	{
		printf( "Error: called displayPath (), but no path found, this should not be happening!\n\n");
		return -1;
	}

	printf( "Found a path from node %d to node %d, with length %d (took %.4lfs)!\n", entryNode->id, exitNode->id, length, findTime);

	printf( "Displaying result:\n\n");
	
	for (i=0; i<=length; i++)
	{
		if (i == 0)
			printf( "Entry node is %d\n", path[i]->id);
		else if (i == length)
			printf( "Exit node (id: %d) reached!\n\n", path[i]->id);
		else
			printf( "Next node in path is node %d\n", path[i]->id);
	}
	
	return 0;
}

void cleanup ()
{
	unsigned int i;
	Openlist *tmp;

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
		if (nodes[i])
		{
			nodes[i]->f = 0;
			nodes[i]->g = 0;
			nodes[i]->open = 0;
			nodes[i]->close = 0;
			nodes[i]->previous = NULL;
			nodes[i]->previousDir = -1;
		}
	}
}