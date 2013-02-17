#include <stdlib.h>

#include "line.h"

Line *newLineH (int i)
{
	Line *line	= (Line*)safeMalloc (sizeof (Line));
	int x_orig = i%(m-1);
	int y = (i/(m-1));
	int x_dest = x_orig+1;
	line->origin = getNode(x_orig,y);
	line->destination = getNode(x_dest, y);
	line->mine = 0;	

	return line;
}
Line *newLineV (int i)
{
	Line *line	= (Line*)safeMalloc (sizeof (Line));
	int x = i%(m);
	int y_orig = (i/(m));
	int y_dest = y_orig+1;
	line->origin = getNode(x,y_orig);
	line->destination = getNode(x, y_dest);
	line->mine = 0;	

	return line;
}
Line *getLine(Node *origin, Node *destination){
	int i;
	for(i=0; i<numLines; i++){
		if(lines[i]->origin == origin && lines[i]->destination == destination && lines[i]->mine == 0)
			return lines[i];
	}
	return NULL;
}