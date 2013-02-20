#ifndef UTIL_H
#define UTIL_H

#include <stdlib.h>
#include <stdarg.h>

#include "node.h"

//Init global variables
unsigned int m, n, numNodes;
FILE *file;
Node *entryNode, *exitNode, **nodes, **path;

//Function prototypes
void *safeMalloc (size_t size);
void print (char toconsole, char tofile, char *format, ...);
void createGrid ();
void displayGrid ();
void displayPath ();

#endif