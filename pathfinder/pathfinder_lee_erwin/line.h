#ifndef LINE_H
#define LINE_H

typedef struct line Line;

#include "node.h"
#include "util.h"

struct line {
	char mine;
	Node *origin;
	Node *destination;	
};

Line *newLineH (long i);
Line *newLineV (long i);
Line *getLine(Node *origin, Node *destination);
Line *getLineFilter(Node *origin, Node *destination, int filter);
Line *getLineFB(Node *origin, Node *destination);
Line *getLineFilterFB(Node *origin, Node *destination, int filter);

int checkLine(Node *origin, Node *destination, Line* line);
int placeMine(Node *origin, Node *destination);
int removeMine(Node *origin, Node *destination);

#endif
