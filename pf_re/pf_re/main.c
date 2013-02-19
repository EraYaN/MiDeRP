#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <limits.h>
//#include <iostream>
#include "util.h"
#include "node.h"
#include "line.h"
#include "path.h"

void createGrid();

int main()
{
	long i,j = 0;
	long count;
	long start = 1, end = 1;
	char nodataerror = 1;
	Node **path;
	#ifdef _DEBUG
	printf("ULONG_MAX: %ld (%d), UINT_MAX: %d (%d),\nUSHRT_MAX: %d (%d), UCHAR_MAX: %d (%d)\n\n",ULONG_MAX,sizeof(unsigned long),UINT_MAX,sizeof(unsigned int),USHRT_MAX,sizeof(unsigned short),UCHAR_MAX,sizeof(unsigned char));
	printMemSize();
	#endif
	printf("Enter m and n (number of nodes on the x-axis and y-axis,\n\tthe width and height of the grid)\n\tlike so:\n\t\"x\ty\" (without the quotes)\n");
	scanf("%ld%ld",&m,&n);
	if(m<3){
		printf("ERROR: 3 is the minimun for m, your value of: %ld doesn't qualify.\n",m);
		nodataerror = 0;
	}
	if(n<3){
		printf("ERROR: 3 is the minimun for n, your value of: %ld doesn't qualify.\n",n);
		nodataerror = 0;
	}
	#ifdef _DEBUG
	printMemSize();
	#endif
	if(nodataerror){
		numNodes =	m*n;
		numLinesH = m*(n-1);
		numLinesV = n*(m-1);
		numLines = numLinesH + numLinesV;
		numControlPosts = 2*(m-2) + 2*(n-2);
		printf("Enter start and end controlpost (starting from the bottom left numbered CCW)\n\tlike so:\n\"start\tend\" (without the quotes)\n");
		scanf("%ld%ld",&start,&end);
		if(start>numControlPosts||start<1){
			printf("ERROR: Start point not on the grid, for these dimensions (%ldx%ld)\n\tthe control posts on the grid are numbered %ld through %ld.\n",m,n,1L,numControlPosts);
			nodataerror = 0;
		}
		if(end>numControlPosts||end<1){
			printf("ERROR: End point not on the grid, for these dimensions (%ldx%ld)\n\tthe control posts on the grid are numbered %ld through %ld.\n",m,n,1L,numControlPosts);
			nodataerror = 0;
		}
		if(start==end){
			printf("ERROR: Start point is the same as endpoint you won't be needing any navigation.\n");
			nodataerror = 0;
		}
		#ifdef _DEBUG
		printMemSize();
		#endif
		if(nodataerror){
			nodes = (Node**)safeMalloc(sizeof(Node*)*numNodes);
			#ifdef _DEBUG
			printMemSize();
			#endif
			lines = (Line**)safeMalloc(sizeof(Line*)*numLines);
			#ifdef _DEBUG
			printMemSize();
			#endif
			createGrid();
			#ifdef _DEBUG
			printMemSize();
			#endif
			//create mines
			/*placeMine(getNode(2,0),getNode(3,0));
			placeMine(getNode(2,1),getNode(3,1));
			placeMine(getNode(2,2),getNode(3,2));
			placeMine(getNode(2,3),getNode(3,3));
			placeMine(getNode(2,4),getNode(3,4));*/
			//testing fpor mem leaks, done -> none
			//for(j=0;j<1000000;j++){
				printf("#%ld; CPs: %ld and %ld\n",j,start,end);
				startStopwatch();
				//get actual path.
				path = findShortestRoute(getNodeFromControlPost(start),getNodeFromControlPost(end),&count);
				printf("Finding the path took %0.4lf seconds.\n",stopStopwatch());
				printf("Path length: %ld\n",count);
				for(i=0;i<count;i++){
					printf("\tPath node #%ld at (x,y) => (%ld,%ld)\n",i,path[i]->x,path[i]->y);
				}
				#ifdef _DEBUG
				printMemSize();
				#endif
				safeFree(path);

			//}
			#ifdef _DEBUG
				printField();
			#endif
		} else {
			printf("There were errors, can not proceed.\n");
		}
	}
	#ifdef _DEBUG
	printMemSize();
	#endif
	printf("Hit enter to exit\n");
	//empty stdin and wait
	emptySTDIN();
	getchar();
	//
	return 0;
}
void createGrid()
{
	long i;

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
