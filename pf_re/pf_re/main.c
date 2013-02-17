#include <stdlib.h>
#include <stdio.h>
#include <string.h>
//#include <iostream>

#include "util.h"
#include "node.h"
#include "line.h"

void createGrid();

int main()
{
	int i;
	numNodes =	m*n;
	numLinesH = m*(n-1);
	numLinesV = n*(m-1);
	numLines = numLinesH + numLinesV;
	numControlPosts = 2*(m-2) + 2*(n-2);

	nodes = (Node**)safeMalloc(sizeof(Node*)*numNodes);
	lines = (Line**)safeMalloc(sizeof(Line*)*numLines);

	createGrid();
	
	for(i=0; i<numNodes; i++)
	{
		printf("Node %3.0d = (x,y) => (%d,%d)\n", i, nodes[i]->x,nodes[i]->y);
	}
	
	for(i=1; i<=numControlPosts; i++)
	{
		Node *node = getNodeFromControlPost(i);
		printf("CoPo %3.0d = node at (x,y) => (%d,%d)\n", i, node->x,node->y);
	}
	for(i=0; i<numLines; i++)
	{
		Line *line = lines[i];
		printf("Line %3.0d = origin at (x,y) => (%d,%d); destination at (x,y) => (%d,%d)\n", i, line->origin->x, line->origin->y,line->destination->x, line->destination->y);
	}
	system("Pause");
	return 0;
}

void createGrid()
{
	int i;

	for(i=0; i<numNodes; i++)
	{
		Node *node = newNode(getNodeX(i), getNodeY(i));
		nodes[i] = node;
	}

	for(i=0; i<numLinesH; i++)
	{
		Line *line = newLineH(i);		
		lines[i] = line;
	}
	for(i=0; i<numLinesV; i++)
	{
		Line *line = newLineV(i);		
		lines[i+numLinesH] = line;
	}
}