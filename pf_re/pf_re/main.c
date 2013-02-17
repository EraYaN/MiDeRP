#include <stdlib.h>
#include <stdio.h>
#include <string.h>
//#include <iostream>

#include "util.h"
#include "node.h"
#include "line.h"
#include "path.h"

void createGrid();

int main()
{
	int i,j;
	int count;
	Node **path;
	numNodes =	m*n;
	numLinesH = m*(n-1);
	numLinesV = n*(m-1);
	numLines = numLinesH + numLinesV;
	numControlPosts = 2*(m-2) + 2*(n-2);
	
	nodes = (Node**)safeMalloc(sizeof(Node*)*numNodes);
	
	lines = (Line**)safeMalloc(sizeof(Line*)*numLines);

	createGrid();
	placeMine(getNode(1,3),getNode(1,4));
	placeMine(getNode(2,3),getNode(2,4));
	placeMine(getNode(3,3),getNode(3,4));
	placeMine(getNode(4,3),getNode(4,4));
	placeMine(getNode(1,2),getNode(1,3));
	placeMine(getNode(1,3),getNode(1,4));
	placeMine(getNode(3,0),getNode(3,1));
	placeMine(getNode(3,1),getNode(3,2));
	placeMine(getNode(3,2),getNode(3,3));
	placeMine(getNode(3,3),getNode(3,4));
	placeMine(getNode(2,3),getNode(3,3));
	placeMine(getNode(1,3),getNode(2,3));
	//Grid check
	/*for(i=0; i<numNodes; i++)
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
		printf("Line %3.0d = orig at (x,y) => (%d,%d); dest at (x,y) => (%d,%d)\n", i, line->origin->x, line->origin->y,line->destination->x, line->destination->y);
	}*/
	// End grid checks	
	//for(j=2;j<=numControlPosts;j++){
	j=6;
		printf("CPs: %d and %d\n",1,j);	
		path = findShortestRoute(getNodeFromControlPost(1),getNodeFromControlPost(j),&count);
		printf("Path length: %d\n",count);	
		for(i=0;i<count;i++){
			printf("\tPath node #%d at (x,y) => (%d,%d)\n",i,path[i]->x,path[i]->y);
		}
		free(path);
	//}
	//printField();
	printField();
	printf("Hit enter to exit\n");
	getchar();
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