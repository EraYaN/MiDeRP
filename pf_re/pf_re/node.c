#include "node.h"
#include "line.h"
#include "util.h"
#include <stdio.h>

Node *newNode (long x, long y)
{
	Node *node	= (Node*)safeMalloc (sizeof (Node));

	node->x = x;
	node->y = y;
	//node->visited = 0;
	//node->previous	= 0;
	//node->distance = inf;
	node->value = inf;

	return node;
}

Node *getNode(long x, long y)
{
	Node *selected = nodes[y*m+x];
	return selected;
}

long getNodeX(long index)
{
	return index % m;
}

long getNodeY(long index)
{
	return (index/m) % n;
}

Node *getNodeFromControlPost(long controlPost)
{
	//TODO
	//long i;
	const long hori = m-2; //controlPosts per horizontale zijde van het veld
	const long vert = n-2; //controlPosts per verticale zijde van het veld
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

Line **getNodeConnections(Node *node, long *count)
{	
	long i;	
	Line **connected = (Line**)safeMalloc(sizeof(Line*)*maxConn);	
	*count = 0;
	for(i = 0; i<numLines; i++){
		if(lines[i]->mine) continue; //there is a mine on this line don't include it.
		if(lines[i]->destination==node && lines[i]->origin->value > node->value){ //filter out marked nodes
			//printf("Values: %d and %d",lines[i]->origin->value,node->value);
			connected[*count]=lines[i];
			(*count)++;
		}
		if(lines[i]->origin==node && lines[i]->destination->value > node->value){ //filter out marked nodes
			//printf("Values: %d and %d",lines[i]->destination->value,node->value);
			connected[*count]=lines[i];
			(*count)++;
		}
	}	
	return connected;
}
Line **getNodeConnectionsBackTrack(Node *node, long *count)
{	
	long i;	
	Line **connected = (Line**)safeMalloc(sizeof(Line*)*maxConn);	
	*count = 0;
	for(i = 0; i<numLines; i++){
		if(lines[i]->mine) continue; //there is a mine on this line don't include it.
		if(lines[i]->destination==node && lines[i]->origin->value < node->value){ //filter out unmarked nodes
			//printf("Values: %d and %d",lines[i]->origin->value,node->value);
			connected[*count]=lines[i];
			(*count)++;
		}
		if(lines[i]->origin==node && lines[i]->destination->value < node->value){ //filter out unmarked nodes
			//printf("Values: %d and %d",lines[i]->destination->value,node->value);
			connected[*count]=lines[i];
			(*count)++;
		}
	}	
	return connected;
}