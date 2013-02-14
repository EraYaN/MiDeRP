#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#define inf 999;

const int	m = 5,
			n = 5;

int	numNodes,
	numVerticesH,
	numVerticesV,
	numVertices,
	numControlPosts;

struct node {
	const int x, y; //For debugging

	int visited,
		previous,
		distance;
};

struct vertex {
	struct node *origin;
	struct node *destination;
	
	int mine;
};

node *nodes;
vertex *vertices;

void getNode();
void createGrid();

int main()
{
	numNodes =	m*n;
	numVerticesH = n*(m-1);
	numVerticesV = m*(n-1);
	numVertices = numVerticesH + numVerticesV;
	numControlPosts = 2*(m-2) + 2*(n-2);

	nodes = (node*)malloc(sizeof(node)*numNodes);
	vertices = (vertex*)malloc(sizeof(vertex)*numVertices);

	createGrid();
	return 0;
}

void createGrid()
{
	int i;

	for(i=0; i<numNodes; i++)
	{
		//*nodes[i]->x = i % n;
		//*nodes[i]->x = i % n;
		//// {i % n, i % m, 0, -1, inf};
	}

	for(i=0; i<numVerticesH; i++)
	{
		//vertices[i]->origin = 
	}
}