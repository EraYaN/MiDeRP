#ifndef PATH_H
#define PATH_H

//Openlist
typedef struct openlist Openlist;

struct openlist {
	Node *node;
	struct openlist *next;
};

Openlist *head;

//Function prototypes
void findPath (Node * entryNode, Node * exitNode);
unsigned int getH(Node *node);
void addToOpenlist (Node *node);
void removeFromOpenlist (Node *node);
void makePath ();
void displayPath ();

#endif