#include "util.h"

//Create a new node
Node *newNode (Node *currentNode, unsigned int id)
{
	Node *node	= (Node*)safeMalloc (sizeof (Node));

	node->id = id;
	node->open = 1;
	node->previous = currentNode;
	node->f = 0;
	node->g = 0;
	setScores (node);

	nodes[id] = node;

	print (0, 1, "Created new node with id %d\n", node->id);
	return node;
}

//Get node by id
Node *getNode (unsigned int id){
	if (id < 0 || id > (numNodes - 1))
	{
		//node does not exist
		return NULL;
	}
	else 
		return nodes[id];
}

Node **getNeighbors (Node *node)
{
	Node **neighbors = (Node**) safeMalloc (sizeof (Node*) * 4);
	neighbors[0] = getNode (node->id - 1);
	neighbors[1] = getNode (node->id + 1);
	neighbors[2] = getNode (node->id + m);
	neighbors[3] = getNode (node->id - m);

	return neighbors;
}

//Get node X or Y value
unsigned int getXY (Node *node, char axis)
{
	if (!node)
	{
		print (1, 1, "Error: tried to get coordinates of non-existing node %d!\n", node->id);
		return 0;
	}
	else
	{
		if (axis == 'X')
			return node->id % m;
		else if (axis == 'Y')
			return node->id / m % n;
		else
		{
			print (1, 1, "Error: entered invalid axis %c!\n", axis);
			return 0;
		}
	}
}

//Set node F and G scores
void setScores (Node *node)
{
	if (node->previous)
	{
		node->g = node->previous->g + 1;
		node->f = node->g + getH (node);
	}
}

//Get the heuristic cost to exitNod
unsigned int getH (Node *node)
{
	return abs ((int)(getXY(node, 'X') - getXY(exitNode, 'X'))) + abs ((int)(getXY(node, 'Y') - getXY(exitNode, 'Y')));
}