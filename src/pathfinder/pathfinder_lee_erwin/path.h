#ifndef PATH_H
#define PATH_H

#include "util.h"
#include "node.h"
#include "line.h"

void resetAllNodes(void);
Node **findShortestRoute(Node *start, Node *end, long *count);
Node **setValuesofNeighbours(Node* node, long *num);
long setValuesofField(Node *node, Node *goal);
long containsNode(Node* node, Node** collection, long count);

#endif