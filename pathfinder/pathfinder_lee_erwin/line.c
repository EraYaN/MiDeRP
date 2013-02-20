#include <stdlib.h>

#include "line.h"

Line *newLineH (long i)
{
	Line *line	= (Line*)safeMalloc (sizeof (Line));
	long x_orig = i%(m-1);
	long y = (i/(m-1));
	long x_dest = x_orig+1;
	line->origin = getNode(x_orig,y);
	line->destination = getNode(x_dest, y);
	line->mine = 0;

	return line;
}
Line *newLineV (long i)
{
	Line *line	= (Line*)safeMalloc (sizeof (Line));
	long x = i%(m);
	long y_orig = (i/(m));
	long y_dest = y_orig+1;
	line->origin = getNode(x,y_orig);
	line->destination = getNode(x, y_dest);
	line->mine = 0;

	return line;
}
Line *getLine(Node *origin, Node *destination){
	return getLineFilter(origin, destination, 1);
}
Line *getLineFilter(Node *origin, Node *destination, int filter){
	long index = 0;
	long perRowH = m-1, perRowV = m;
	//horizontal lines were created first. up to index numLinesH-1;
	if(!origin || !destination || origin==destination) return NULL;
	if(origin->x==destination->x&&origin->y==destination->y){
		//same	
		return NULL;
	}else if(origin->x==destination->x){
		//Y is different, vertical lines
		index = numLinesH; //starting point for vertical
		index += origin->x;
		if(origin->y>destination->y){
			//origin above dest
			index += destination->y * perRowV;
		} else {
			index += origin->y * perRowV;
		}
	} else if(origin->y==destination->y){
		//X is different, horizontal lines
		index = 0; //starting point for horizontal
		index += origin->y * perRowH;
		if(origin->x>destination->x){
			//dest left from origin			
			index += destination->x;
		} else {
			index += origin->x;
		}
		
	} else {
		//notConnected;
		return NULL;
	}
	if(checkLine(origin, destination, lines[index])){
		if(lines[index]->mine==0||filter==0){
			return lines[index];
		}
	} else {
		return getLineFilterFB(origin, destination,filter);
	}
	return NULL; //all else failed
}
//old fallback
Line *getLineFB(Node *origin, Node *destination){
	return getLineFilterFB(origin, destination, 1);
}
//old fallback
Line *getLineFilterFB(Node *origin, Node *destination, int filter){
	long i;
	for(i=0; i<numLines; i++){
		if(checkLine(origin, destination, lines[i]) && (lines[i]->mine == 0||!filter))
			return lines[i];
	}
	return NULL;
}
int checkLine(Node *origin, Node *destination, Line* line){
	if((line->origin == origin && line->destination == destination) || (line->origin == destination && line->destination == origin))
		return 1;
	else 
		return 0;
}

int placeMine(Node *origin, Node *destination){
	Line *line = getLine(origin,destination);
	if(line){
		line->mine = 1;
		return 1;
	}
	return 0;
}

int removeMine(Node *origin, Node *destination){
	Line *line = getLine(origin,destination);
	if(line){
		line->mine = 0;
		return 1;
	}
	return 0;
}
