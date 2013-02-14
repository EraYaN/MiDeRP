#ifndef _LINE_H_
#define _LINE_H_

typedef struct line Line;

#include "node.h"

struct line {
	Node *origin;
	Node *destination;
	
	int mine;
};

Line *newLineH (int i);
Line *newLineV (int i);

#endif