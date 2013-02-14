#include "node.h"
#include "util.h"

Node *newNode (int x, int y)
{
	Node *node	= (Node*)safeMalloc (sizeof (Node));

	node->x = x;
	node->y = y;
	node->visited = 0;
	node->previous	= 0;
	node->distance = inf;

	return node;
}