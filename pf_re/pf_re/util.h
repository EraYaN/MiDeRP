#ifndef UTIL_H
#define UTIL_H

#include <stdlib.h>

#include "node.h"
#include "line.h"

#define inf 999;

//Function prototypes
void *safeMalloc (size_t size);
void printField();

//Common variables
const long m, n, maxConn;

long	numNodes,
	numLinesH,
	numLinesV,
	numLines,
	numControlPosts;

Node **nodes;
Line **lines;

#endif