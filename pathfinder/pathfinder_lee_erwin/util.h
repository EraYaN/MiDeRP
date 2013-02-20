#ifndef UTIL_H
#define UTIL_H
/*#ifndef _DEBUG
//#define _DEBUG //define if you want verbose output
#endif*/
#include <stdlib.h>
#include <limits.h>
#include <time.h>

#include "node.h"
#include "line.h"

#define inf LONG_MAX;

size_t memusage;


//Function prototypes
void *safeMalloc (size_t size);
void safeFree (void *ptr);
void startStopwatch(void);
double stopStopwatch(void);
void printMemSize(void);
void printField(void);
void saveField(char* filename);
void emptySTDIN(void);

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
