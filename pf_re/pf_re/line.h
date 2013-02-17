#ifndef LINE_H
#define LINE_H

typedef struct line Line;

#include "node.h"
#include "util.h"

struct line {
	Node *origin;
	Node *destination;

	int mine;
};

Line *newLineH (int i);
Line *newLineV (int i);
Line *getLine(Node *origin, Node *destination);
Line *getLineFilter(Node *origin, Node *destination, int filter);

int placeMine(Node *origin, Node *destination);
int removeMine(Node *origin, Node *destination);

#endif