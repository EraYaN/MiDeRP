#ifndef NODE_H
#define NODE_H

typedef struct node Node;
#include "line.h"
struct node {
	int x,
		y,
		/*visited,*/ //Dijkstra
		/*previous,*/ //Dijkstra
		/*distance*/ //Dijkstra
		value; //Lee
};

Node *newNode (int x, int y);
Node *getNode(int x, int y);
Node *getNodeFromControlPost(int controlPost);
Line **getNodeConnections(Node *node, int *count);
Line **getNodeConnectionsBackTrack(Node *node, int *count);
int getNodeX(int index);
int getNodeY(int index);



#endif