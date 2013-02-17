#include "node.h"
#include "line.h"
#include "util.h"

Node *newNode (int x, int y)
{
	Node *node	= (Node*)safeMalloc (sizeof (Node));

	node->x = x;
	node->y = y;
	node->visited = 0;
	node->previous	= 0;
	node->distance = inf;

	return node;
}

Node *getNode(int x, int y)
{
	Node *selected = nodes[y*m+x];
	return selected;
}

int getNodeX(int index)
{
	return index % m;
}

int getNodeY(int index)
{
	return (index/m) % n;
}

Node *getNodeFromControlPost(int controlPost)
{
	//TODO
	//int i;
	const int hori = m-2; //controlPosts per horizontale zijde van het veld
	const int vert = n-2; //controlPosts per verticale zijde van het veld
	if(controlPost > numControlPosts||controlPost<1){
		//bestaat niet!
		return NULL;
	}
	if(controlPost<=hori){
		//onder
		return getNode(controlPost,0);
	} else if(controlPost>hori&&controlPost<=hori+vert){
		//rechts
		return getNode(m-1,controlPost-hori);
	} else if(controlPost>hori+vert&&controlPost<=2*hori+vert){
		//boven
		return getNode((m-1)-(controlPost-hori-vert),n-1);
	} else if(controlPost>2*hori+vert&&controlPost<=2*hori+2*vert){
		//links
		return getNode(0,(n-1)-(controlPost-2*hori-vert));
	} else {
		//bestaat niet!
		return NULL;
	}	
	return NULL;
}

Line *getLinesFromNode(Node node, int count)
{
	
	
}