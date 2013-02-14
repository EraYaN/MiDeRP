#ifndef _NODE_H_
#define _NODE_H_

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