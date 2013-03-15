#ifndef UTIL_H
#define UTIL_H

#include <stdlib.h>
#include <stdarg.h>
#include <stdio.h>
#include <time.h>

#include "node.h"
#include "path.h"

//Init variables
char **mines;
unsigned int m, n, numNodes, length;
double gridTime, findTime, totalTime;
Node *entryNode, *exitNode, **nodes, **path;
FILE *file;

//Function prototypes
void *safeMalloc (size_t size);
void print (char toconsole, char tofile, char *format, ...);
void startStopwatch(void);
double stopStopwatch(void);
void createGrid ();
void displayPath ();
#endif