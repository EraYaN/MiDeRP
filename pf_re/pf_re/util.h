#ifndef UTIL_H
#define UTIL_H

#include <stdlib.h>

#include "node.h"
#include "line.h"

#define inf 999;

//Function prototypes
void *safeMalloc (size_t size);
Node *getNode(int x, int y);
Node *getNodeFromControlPost(int controlPost);
Line *getLine(Node *origin, Node *destination);

//Common variables
const int m, n;

int	numNodes,
	numLinesH,
	numLinesV,
	numLines,
	numControlPosts;

Node **nodes;
Line **lines;

#endif