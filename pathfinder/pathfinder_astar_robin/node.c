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
	node->neighbors = (Node**) safeMalloc (sizeof (Node*) * 4);

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
void setNeighbors (Node *node)
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

	node->neighbors = neighbors;
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

//Get an estimated heuristic cost to exitNode
unsigned int getH (Node *node)
{
	return abs ((int)(getXY(node, 'X') - getXY(exitNode, 'X'))) + abs ((int)(getXY(node, 'Y') - getXY(exitNode, 'Y')));
}

void addMine (unsigned int id1, unsigned int id2)
{
	Node *node1, *node2;

	if (!nodes[id1])
		node1 = newNode (NULL, id1);
	else
		node1 = getNode (id1);
	if (!nodes[id2])
		node2 = newNode (NULL, id2);
	else
		node2 = getNode (id2);

	setNeighbors (node1);

	if (!mines)
		mines = (char**) safeMalloc (sizeof (char) * (size_t) numNodes);

	if (node2 == node1->neighbors[0])
	{
		//Node2 is left of node1
		mines[node1->id][0] = 1;
		mines[node2->id][1] = 1;
	}
	else if (node2 == node1->neighbors[1])
	{
		//Node2 is right of node1
		mines[node1->id][1] = 1;
		mines[node2->id][0] = 1;
	}
	else if (node2 == node1->neighbors[2])
	{
		//Node2 is above node1
		mines[node1->id][2] = 1;
		mines[node2->id][3] = 1;
	}
	else if (node2 == node1->neighbors[3])
	{
		//Node2 is under node1
		mines[node1->id][3] = 1;
		mines[node2->id][2] = 1;
	}
	else
	{
		print(1, 1, "Error: mines are not neighbours!\n");
	}

}

char isMine (Node *node1, Node *node2)
{
	int i;

	if (!node1 || !node2)
	{
		print(1, 1, "Error: tried to check for mines near non-existing mine!\n");
		return 0;
	}

	setNeighbors (node1);

	for (i=0; i<4; i++)
	{
		if (node2 == node1->neighbors[i])
		{
			if (mines[node1->id][i])
				return 1;
			else
				return 0;
		}
	}

	print(1, 1, "Error: mines are not neighbours!\n");
	return 0;
}