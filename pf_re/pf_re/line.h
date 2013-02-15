#ifndef LINE_H
#define LINE_H

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