#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include "util.h"

const int	m = 5,
			n = 5;

int	numNodes,
	numVerticesH,
	numVerticesV,
	numVertices,
	numControlPosts;

node **nodes;
vertex **vertices;

void getNode();
void createGrid();

int main()
{
	numNodes =	m*n;
	numVerticesH = n*(m-1);
	numVerticesV = m*(n-1);
	numVertices = numVerticesH + numVerticesV;
	numControlPosts = 2*(m-2) + 2*(n-2);

	*nodes = (node*)malloc(sizeof(node)*numNodes);
	*vertices = (vertex*)malloc(sizeof(vertex)*numVertices);

	createGrid();
	return 0;
}

void createGrid()
{
	int i;

	for(i=0; i<numNodes; i++)
	{
		node *node = newNode(i % n, i % m);
		nodes[i] = node;
	}

	for(i=0; i<numVerticesH; i++)
	{
		vertex *vertex = newVertexH(i);
		vertices[i] = vertex;
	}
}