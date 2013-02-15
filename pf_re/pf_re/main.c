#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include "util.h"
#include "node.h"
#include "line.h"

void getNode();
void createGrid();

int main()
{
	printf("Enter grid width: \n");
	scanf("%d", &m);
	printf("Enter grid height: \n");
	scanf("%d", &n);

	numNodes =	m*n;
	numLinesH = n*(m-1);
	numLinesV = m*(n-1);
	numLines = numLinesH + numLinesV;
	numControlPosts = 2*(m-2) + 2*(n-2);

	nodes = (Node**)safeMalloc(sizeof(Node*)*numNodes);
	lines = (Line**)safeMalloc(sizeof(Line*)*numLines);

	createGrid();
	return 0;
}

void createGrid()
{
	int i;

	for(i=0; i<numNodes; i++)
	{
		Node *node = newNode(i % n, i % m);
		nodes[i] = node;
	}

	for(i=0; i<numLinesH; i++)
	{
		Line *line = newLineH(i);
		lines[i] = line;
	}
}