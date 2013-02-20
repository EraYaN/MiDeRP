#ifndef NODE_H
#define NODE_H

typedef struct node Node;
#include "line.h"
struct node {
	long x, y;
    long value; //Lee
};

Node *newNode (long x, long y);
Node *getNode(long x, long y);
Node *getNodeFromControlPost(long controlPost);
Line **getNodeConnections(Node *node, long *count);
Line **getNodeConnectionsBackTrack(Node *node, long *count);
long getNodeX(long index);
long getNodeY(long index);



#endif
