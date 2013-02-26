#ifndef UTIL_H
#define UTIL_H

#include <stdlib.h>
#include <stdarg.h>
#include <stdio.h>

#include "node.h"
#include "path.h"

//Init variables
unsigned int m, n, numNodes, entryNode, exitNode;
FILE *file;

//Function prototypes
void *safeMalloc (size_t size);
void print (char toconsole, char tofile, char *format, ...);

#endif