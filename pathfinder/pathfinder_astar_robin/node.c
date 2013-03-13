#include "util.h"

//Create a new node
Node *newNode (Node *current, unsigned int id)
{

	Node *node	= (Node*)safeMalloc (sizeof (Node));

	if (id < 0 || id > (numNodes - 1))
	{
		print (1, 1, "Error: tried to create node %ld out of bounds!\n", id);
		return NULL; //node is out of bounds
	}

	node->id = id;
	node->open = 1;
	node->previous = current;
	node->f = 0;
	node->g = 0;

	nodes[id] = node;

	return node;
}

//Get node by id
Node *getNode (unsigned int id){
	if (id < 0 || id > (numNodes - 1) || !nodes[id])
	{
		//node does not exist
		return NULL;
	}
	else 
		return nodes[id];
}

//Get node neighbours and create them if they do not exist
Node **getNeighbors (Node *node)
{
	Node **neighbors = (Node**) safeMalloc (sizeof (Node*) * 4);

	if (getXY (node, 'X') != 0)
	{
		if (getNode (node->id - 1))
			neighbors[0] = getNode (node->id - 1);
		else
			neighbors[0] = newNode (node, node->id - 1);
	}
	else
		neighbors[0] = NULL;

	if (getXY (node, 'X') < m - 1)
	{
		if (getNode (node->id + 1))
		{
			neighbors[1] = getNode (node->id + 1);
		}
		else
			neighbors[1] = newNode (node, node->id + 1);
	}
	else
		neighbors[1] = NULL;

	if (getXY (node, 'Y') < n - 1)
	{
		if (getNode (node->id + m))
			neighbors[2] = getNode (node->id + m);
		else
			neighbors[2] = newNode (node, node->id + m);
	}
	else
		neighbors[2] = NULL;

	if (getXY (node, 'Y') != 0)
	{
		if (getNode (node->id - m))
			neighbors[3] = getNode (node->id - m);
		else
			neighbors[3] = newNode (node, node->id - m);
	}
	else
		neighbors[3] = NULL;

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