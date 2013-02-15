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

#endif