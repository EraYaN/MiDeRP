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
void findPath ();

#endif