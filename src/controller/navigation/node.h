#ifndef NODE_H
#define NODE_H

typedef struct node Node;

//Set up the node structure
struct node {
	unsigned int id, f, g;
	char open, close, mines[4], previousDir;
	Node *previous, *neighbors[4];
};

//Function prototypes
Node *newNode (Node *current, unsigned int id);
Node *getNode (unsigned int id);
void setNeighbors (Node *node);
unsigned int getXY (Node *node, char axis);
void setScores (Node *node);
unsigned int getH (Node *node);
void addMine (unsigned int id1, unsigned int id2);
char isMine (Node *node1, Node *node2);

#endif