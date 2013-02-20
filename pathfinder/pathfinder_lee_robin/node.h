#ifndef NODE_H
#define NODE_H

typedef struct node Node;

//Set up the node structure
struct node {
	unsigned int id;
	int	label;

	Node **mines;
};

//Function prototypes
Node *newNode (unsigned int id);
Node *getNode (unsigned int id);
unsigned int getNodeX(Node *node);
unsigned int getNodeY(Node *node);
Node *getNodeByXY (unsigned int x, unsigned int y);
Node *getAdjNode (Node *node, char dir, char checkmine);
char isMine (Node *node1, Node *node2);
void addMine (Node *node1, Node *node2);

Node nullnode;

#endif