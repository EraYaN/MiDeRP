#ifndef UTIL_H
#define UTIL_H
/*#ifndef _DEBUG
//#define _DEBUG
#endif*/
#include <stdlib.h>
#include <time.h>

#include "node.h"
#include "line.h"

#define inf 999;

size_t memusage;


//Function prototypes
void *safeMalloc (size_t size);
void safeFree (void *ptr);
void startStopwatch();
double stopStopwatch();
void printMemSize();
void printField();
void emptySTDIN();

//Common variables
long m, n, numberAlloc;
const long maxConn;

long	numNodes,
	numLinesH,
	numLinesV,
	numLines,
	numControlPosts;

Node **nodes;
Line **lines;

#endif
