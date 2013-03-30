#include "util.h"


//Create a new node
Node *newNode (Node *current, unsigned int id)
{

	Node *node	= (Node*)safeMalloc (sizeof (Node));

	if (id < 0 || id > (numNodes - 1))
	{
		printf( "Error: tried to create node %ld out of bounds!\n", id);
		return NULL; //node is out of bounds
	}

	node->id = id;
	node->open = 0;
	node->close = 0;
	node->previous = current;
	node->previousDir = 0;
	node->f = 0;
	node->g = 0;

	node->mines[0] = 0;
	node->mines[1] = 0;
	node->mines[2] = 0;
	node->mines[3] = 0;
	
	node->neighbors[0] = 0;
	node->neighbors[1] = 0;
	node->neighbors[2] = 0;
	node->neighbors[3] = 0;

	nodes[id] = node;

	return node;
}

//Get node by id
Node *getNodeI(unsigned int id){
	if (id < 0 || id > (numNodes - 1) || !nodes[id])
	{
		//node does not exist
		return NULL;
	}
	else 
		return nodes[id];
}

Node *getNodeC(unsigned int x, unsigned int y)
{
	if(y>=n||x>=m||y*m+x>=numNodes){
		return NULL;
	}
	return nodes[y*m+x];
}

//Get node neighbours and create them if they do not exist
void setNeighbors (Node *node)
{
	if (getXY (node, 'X') != 0)
	{
		if (getNodeI(node->id - 1))
			node->neighbors[0] = getNodeI(node->id - 1);
		else
			node->neighbors[0] = newNode (node, node->id - 1);
	}
	else
		node->neighbors[0] = NULL;

	if (getXY (node, 'X') < m - 1)
	{
		if (getNodeI(node->id + 1))
		{
			node->neighbors[1] = getNodeI(node->id + 1);
		}
		else
			node->neighbors[1] = newNode (node, node->id + 1);
	}
	else
		node->neighbors[1] = NULL;

	if (getXY (node, 'Y') < n - 1)
	{
		if (getNodeI(node->id + m))
			node->neighbors[2] = getNodeI(node->id + m);
		else
			node->neighbors[2] = newNode (node, node->id + m);
	}
	else
		node->neighbors[2] = NULL;

	if (getXY (node, 'Y') != 0)
	{
		if (getNodeI(node->id - m))
			node->neighbors[3] = getNodeI(node->id - m);
		else
			node->neighbors[3] = newNode (node, node->id - m);
	}
	else
		node->neighbors[3] = NULL;

}

//Get node X or Y value //TODO make two function out of it is better performance wise and clearer.
unsigned int getXY (Node *node, char axis)
{
	if (!node)
	{
		printf("Error: tried to get coordinates of non-existing node!\n");
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
			printf("Error: entered invalid axis %c!\n", axis);
			return 0;
		}
	}
}

//Add mine to grid
int setMineI(unsigned int id1, unsigned int id2, char mine)
{
	Node *node1, *node2;
	if (!nodes[id1])
		node1 = newNode (NULL, id1);
	else
		node1 = getNodeI(id1);
	if (!nodes[id2])
		node2 = newNode (NULL, id2);
	else
		node2 = getNodeI(id2);

	return setMine (node1, node2, mine);	
}

int setMine(Node *node1, Node *node2, char mine)
{
	if (!node1 || !node2)
	{
		printf( "Error: tried to create mine out of bounds\n");
		return -1;
	}

	setNeighbors (node1);

	if (node2 == node1->neighbors[0])
	{
		//Node2 is left of node1
		node1->mines[0] = mine;
		node2->mines[1] = mine;
	}
	else if (node2 == node1->neighbors[1])
	{
		//Node2 is right of node1
		node1->mines[1] = mine;
		node2->mines[0] = mine;
	}
	else if (node2 == node1->neighbors[2])
	{
		//Node2 is above node1
		node1->mines[2] = mine;
		node2->mines[3] = mine;
	}
	else if (node2 == node1->neighbors[3])
	{
		//Node2 is under node1
		node1->mines[3] = mine;
		node2->mines[2] = mine;
	}
	else
	{
		printf( "Error: mines are not neighbours!\n");
		return -2;
	}
	return 0;
}

//Check for mine
char isMine (Node *node1, Node *node2)
{
	int i;

	if (!node1 || !node2)
	{
		printf( "Error: tried to check for mines near non-existing mine!\n");
		return 0;
	}

	setNeighbors (node1);

	for (i=0; i<4; i++)
	{
		if (node2 == node1->neighbors[i])
		{
			if (node1->mines[i])
				return 1;
			else
				return 0;
		}
	}

	printf( "Error: mines are not neighbours!\n");
	return 0;
}