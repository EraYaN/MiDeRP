#include <stdlib.h>
#include <stdio.h>

#include "path.h"
#include "util.h"
#include "node.h"

void expand (Node **node, int count);
void backTrace();

//path finding algorithm based on Lee's algorithm
void findPath ()
{
	//Init
	entryNode->label = 0;

	//Wave expansion
	print(1, 1, "Pathfinder initialized, starting wave expansion...\n");
	startStopwatch ();
	expand (&entryNode, 1); //use wave expansion
	expansionTime = stopStopwatch ();
	displayGrid (); //print results

	//Backtrace
	print(1, 1, "Tracing back path...\n\n");
	startStopwatch ();
	backTrace ();
	backTraceTime = stopStopwatch ();
	displayPath (); //print results

}

void expand (Node **xnodes, int count)
{
	//Init
	int i, k;
	Node *node, **adjnodes;
	adjnodes = (Node**)safeMalloc( sizeof (Node*)*(count + 4));
	k = 0;

	//Loop through each nodes to expand
	for (i=0; i<count; i++)
	{
		node = xnodes[i];
		
		if (getAdjNode(node, 0, 1) && getAdjNode(node, 0, 1)->label == -1) //Node exists
		{
			getAdjNode(node, 0, 1)->label = node->label + 1;

			if (getAdjNode(node, 0, 1) == exitNode)
				return;

			adjnodes[k++] = getAdjNode(node, 0, 1); //Add adjacent node to array with nodes to process next
		}
		if (getAdjNode(node, 1, 1) && getAdjNode(node, 1, 1)->label == -1) //Node exists
		{
			getAdjNode(node, 1, 1)->label = node->label + 1;

			if (getAdjNode(node, 1, 1) == exitNode)
				return;

			adjnodes[k++] = getAdjNode(node, 1, 1); //Add adjacent node to array with nodes to process next
		}
		if (getAdjNode(node, 2, 1) && getAdjNode(node, 2, 1)->label == -1) //Node exists
		{
			getAdjNode(node, 2, 1)->label = node->label + 1; 

			if (getAdjNode(node, 2, 1) == exitNode)
				return;

			adjnodes[k++] = getAdjNode(node, 2, 1); //Add adjacent node to array with nodes to process next
		}
		if (getAdjNode(node, 3, 1) && getAdjNode(node, 3, 1)->label == -1) //Node exists
		{
			getAdjNode(node, 3, 1)->label = node->label + 1;

			if (getAdjNode(node, 3, 1) == exitNode)
				return;

			adjnodes[k++] = getAdjNode(node, 3, 1); //Add adjacent node to array with nodes to process next
		}
	}

	//Recursive
	expand (adjnodes, k);
}

void backTrace()
{
	//Init
	Node *node, *nextNode;
	int i, direction = 0;
	path = (Node**)safeMalloc (sizeof (Node*) * (exitNode->label));
	node = exitNode;
	nextNode = NULL;
	i = exitNode->label;

	//Keep tracing until we reach the entry node
	while (node != entryNode)
	{
		nextNode = getAdjNode (node, direction, 0);

		if (nextNode && !isMine(node, nextNode) && nextNode->label >= 0 && nextNode->label < node->label) 
		{
			path[i--] = node = nextNode; 
		} 
		else 
		{ 
			direction = (direction + 1)%4; 
		}
	}
}