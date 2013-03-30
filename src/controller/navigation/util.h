#ifndef UTIL_H
#define UTIL_H

#include <stdlib.h>
#include <stdarg.h>
#include <stdio.h>
#include <time.h>

#include "main.h"
#include "node.h"
#include "path.h"
#include "input.h"

//Init variables
unsigned int m,n, numNodes, length;
double gridTime, findTime, totalTime;
Node *entryNode, *exitNode, **nodes, **path;
//Function prototypes
void *safeMalloc (size_t size);
//void print (char toconsole, char tofile, char *format, ...);
void startStopwatch(void);
double stopStopwatch(void);
void createGrid (int m, int n);
#endif