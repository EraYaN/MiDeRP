#ifndef NODE_H
#define NODE_H

typedef struct node Node;

struct node {
	int x,
		y,
		visited,
		previous,
		distance;
};

Node *newNode (int x, int y);
Node *getNode(int x, int y);
Node *getNodeFromControlPost(int controlPost);
int getNodeX(int index);
int getNodeY(int index);


#endif