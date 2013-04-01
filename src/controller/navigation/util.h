#ifndef UTIL_H
#define UTIL_H

#include <stdlib.h>
#include <stdarg.h>
#include <stdio.h>
#include <time.h>

#include "main.h"
#include "node.h"
#include "path.h"

//Init variables
unsigned int m, n, numNodes, length;
double gridTime, findTime, totalTime;
Node **nodes, **path;

//Function prototypes
void *safeMalloc (size_t size);
void startStopwatch(void);
double stopStopwatch(void);
void createGrid (int m, int n);
#endif