#include <stdlib.h>
#include <stdio.h>

#include "util.h"

#define inf 999;

void *safeMalloc (size_t size)
{
    void *p = malloc(size);
	if (p == NULL)
	{
		printf ("Error: out of memory!\n");
		exit (1);
	}
	return p;
}


node *newNode (int x, int y)
{
	node *n		= (node*)safeMalloc(sizeof (node));

	n->x = x;
	n->y = y;
	n->visited = 0;
	n->previous		= NULL;
	n->distance = inf;

	return n;
}
