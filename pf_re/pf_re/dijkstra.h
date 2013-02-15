#ifndef DIJKSTRA_H
#define DIJKSTRA_H


#include "util.h"
#include "node.h"
#include "line.h"

Node *getNode(int x, int y);
Node *getNodeFromControlPost(int controlPost);
Line *getLine(Node *origin, Node *destination);

#endif