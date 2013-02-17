#ifndef PATH_H
#define PATH_H

#include "util.h"
#include "node.h"
#include "line.h"

void resetAllNodes();
Node **findShortestRoute(Node *start, Node *end, int *count);
Node **setValuesofNeighbours(Node* node, int *num);
int setValuesofField(Node *node, Node *goal);
int containsNode(Node* node, Node** collection, int count);

#endif