#pragma once
#ifndef _UTIL_H_
#define _UTIL_H_

#include <stdlib.h>

#include "node.h"
#include "line.h"

#define inf 999;

//Function prototypes
void *safeMalloc (size_t size);

//Common variables
const int	m = 5,
			n = 5;

int	numNodes,
	numLinesH,
	numLinesV,
	numLines,
	numControlPosts;

Node **nodes;
Line **lines;

#endif