#ifndef LINE_H
#define LINE_H

typedef struct line Line;

#include "node.h"
#include "util.h"

struct line {
	Node *origin;
	Node *destination;

	long mine;
};

Line *newLineH (long i);
Line *newLineV (long i);
Line *getLine(Node *origin, Node *destination);
Line *getLineFilter(Node *origin, Node *destination, long filter);

long placeMine(Node *origin, Node *destination);
long removeMine(Node *origin, Node *destination);

#endif