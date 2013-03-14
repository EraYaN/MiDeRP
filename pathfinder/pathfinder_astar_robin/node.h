#ifndef NODE_H
#define NODE_H

typedef struct node Node;

//Set up the node structure
struct node {
	unsigned int id, f, g;
	char open;
	Node *previous;
};

//Function prototypes
Node *newNode (Node *current, unsigned int id);
Node *getNode (unsigned int id);
Node **getNeighbors (Node *node);
unsigned int getXY (Node *node, char axis);
void setScores (Node *node);
unsigned int getH (Node *node);

#endif