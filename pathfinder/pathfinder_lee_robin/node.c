#include <stdio.h>

#include "node.h"
#include "util.h"

//Initialize all node functionality

Node *newNode (unsigned int id)
{
	Node *node	= (Node*)safeMalloc (sizeof (Node));

	node->id = id;
	node->label = -1;

	node->mines = (Node**)safeMalloc(sizeof(Node*)*4);

	return node;
}

Node *getNode(unsigned int id){
	if (id < 0 || id > (numNodes - 1))
	{
		//node does not exist
		return NULL;
	}
	else 
		return nodes[id];
}

unsigned int getNodeX(Node *node)
{
	if (!node)
		return 0;
	else
		return (node->id % m);
}

unsigned int getNodeY(Node *node)
{
	if (!node)
		return 0;
	else	
		return ((node->id / m) % n);
}

Node *getNodeByXY(unsigned int x, unsigned int y)
{
	if (x > m || y > n)
		return NULL;
	else
		return getNode(y*m+x);
}

Node *getAdjNode (Node *node, char dir, char checkmine)
{
	if (dir > 3)
	{
		return NULL;
	}
	else if (dir == 0)
	{
		if (getNodeX(node) == 0 || (checkmine && isMine(node, getNode(node->id - 1))))
			return NULL;
		else
			return getNode(node->id - 1);
	}
	else if (dir == 1)
	{
		if (getNodeX(node) >= (m - 1) || (checkmine && isMine(node, getNode(node->id + 1))))
			return NULL;
		else
			return getNode(node->id + 1);
	}
	else if (dir == 2)
	{
		if (getNodeY(node) >= (n - 1) || (checkmine && isMine(node, getNode(node->id + m))))
			return NULL;
		else
			return getNode(node->id + m);
	}
	else 
	{
		if (getNodeY(node) == 0 || (checkmine && isMine(node, getNode(node->id - m))))
			return NULL;
		else
			return getNode(node->id - m);
	}
}

char isMine (Node *node1, Node *node2)
{
	if (node1 && node2 && (node1->mines[0] == node2 || node1->mines[1] == node2 || node1->mines[2] == node2 || node1->mines[3] == node2))
	{
		return 1;
	}
	else
		return 0;
}

void addMine (Node *node1 ,Node *node2)
{
	if (!node1 && !node2)
		print(1, 1, "Error: tried to create mine outside the grid!\n");
	else if (getAdjNode(node1, 0, 0) != node2 && getAdjNode(node1, 1, 0) != node2 && getAdjNode(node1, 2, 0) != node2 && getAdjNode(node1, 3, 0) != node2)
		print(1, 1, "Error: mines are not neighbours!\n");
	else
	{
		if (getAdjNode(node1, 0, 0) == node2)
		{
			node1->mines[0] = node2;
			node2->mines[1] = node1;
		}
		else if (getAdjNode(node1, 1, 0) == node2)
		{
			node1->mines[1] = node2;
			node2->mines[0] = node1;
		}
		else if (getAdjNode(node1, 2, 0) == node2)
		{
			node1->mines[2] = node2;
			node2->mines[3] = node1;
		}
		else if (getAdjNode(node1, 3, 0) == node2)
		{
			node1->mines[3] = node2;
			node2->mines[2] = node1;
		}		
	}
}