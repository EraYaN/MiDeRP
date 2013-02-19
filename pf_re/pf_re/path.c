#include <stdlib.h>
#include <stdio.h>

#include "path.h"
#include "util.h"
#include "line.h"
#include "node.h"

///     |
///  -------
/// _|  |  |_ n
///  |  |  |
///  -------
///     | m

void resetAllNodes(){
	long i;
	for(i=0; i<numNodes; i++)
	{
		nodes[i]->value = inf;
	}
}
Node **findShortestRoute(Node *start, Node *end, long *count){
	long j;	
	long length, wavenum; 
	long index = 0;
	Node *current;
	long num, conns;
	Node **neighbours;
	Line **linestoneighbours;
	Node **path = (Node**)safeMalloc(sizeof(Node*)*numNodes);	
	//Node **neigbours = (Node**)safeMalloc(sizeof(Node*)*numNodes);	
	*count = 0; //current index in path
	//reset all values
	resetAllNodes();
	//fill field with values (end first)
	//end->value = 0;
	path[(*count)++] = start; //add start to node list.
	length = setValuesofField(end,start);
	//printf("setValuesofFiels returned: %d.\n",length);
	//trace your path, starting at 'start'.
	current = start;
	for(wavenum = length; wavenum>=0;wavenum--){
		if(current == end) break;
		num = 0;
		conns = 0;
		linestoneighbours = getNodeConnectionsBackTrack(current,&conns);
		neighbours = (Node**)safeMalloc(sizeof(Node*)*conns);
		for(j=0;j<conns;j++){		
			if(linestoneighbours[j]->origin==current){
				neighbours[num] = linestoneighbours[j]->destination;			
				num++;
			} else if(linestoneighbours[j]->destination==current){			
				neighbours[num] = linestoneighbours[j]->origin;			
				num++;			
			}
		}
		safeFree(linestoneighbours);
		for(j=0;j<num;j++){		
			if(neighbours[j]->value==current->value-1){
				path[(*count)++]=neighbours[j];
				current = neighbours[j];
			}
		}
		safeFree(neighbours);
	}
	
	//return the path
	return path;
}
long setValuesofField(Node *start, Node *goal){
	long conns;
	long num = 0;
	long i,j;
	//long max;
	long currentNumber = 0;
	long done = 0;
	Node *current = start;
	Node **wave, **buwave;
	long inCurrentWave;
	long inPrevWave;
	Node **neighbours;
	Line **linestoneighbours;
	inCurrentWave = 1;
	start->value = currentNumber;
	wave = (Node**)safeMalloc(sizeof(Node*)*2);
	wave[0] = start;
	while(!done){
		inPrevWave = inCurrentWave;
		inCurrentWave = 0;
		buwave = wave;
		wave = (Node**)safeMalloc(sizeof(Node*)*inPrevWave*3);
		getNextWave(buwave,wave,inPrevWave,&inCurrentWave);
		safeFree(buwave);
		for(i=0;i<inCurrentWave&&!done;i++){
			
			current = wave[i];
			num = 0;
			conns = 0;
			linestoneighbours = getNodeConnections(current,&conns);
			neighbours = (Node**)safeMalloc(sizeof(Node*)*conns);
			for(j=0;j<conns;j++){		
				if(linestoneighbours[j]->origin==current){
					neighbours[num] = linestoneighbours[j]->destination;			
					num++;
				} else if(linestoneighbours[j]->destination==current){			
					neighbours[num] = linestoneighbours[j]->origin;			
					num++;			
				}
			}
			safeFree(linestoneighbours);
			for(j=0;j<num;j++){		
				if(neighbours[j]->value > currentNumber){
					neighbours[j]->value = currentNumber+1;
				}
				if(neighbours[j] == goal){
					done = 1;
					break;
				}
			}
			safeFree(neighbours);
		}
		//safeFree(wave);
		currentNumber++;
		//printField();
		//done with wave

	}
	//linestoneighbours = getNodeConnections(node,&conns);
	return currentNumber;
}
void markNeighbours(Node* node){
	long x = node->x, y = node->y;
	long value = node->value;
	Node *left = getNode(x-1,y);
	Node *right = getNode(x+1,y);
	Node *up = getNode(x,y+1);
	Node *down = getNode(x,y-1);
	if(left){
		if(left->value > value){
			left->value = value+1;
		}
	}
	if(right){
		if(right->value > value){
			right->value = value+1;
		}
	}
	if(up){
		if(up->value > value){
			up->value = value+1;
		}
	}
	if(down){
		if(down->value > value){
			down->value = value+1;
		}
	}
}
void getNextWave(Node** wave, Node** newwave, long length, long *count){
	long i;
	long x, y;
	long value;
	Node *left;
	Node *right;
	Node *up;
	Node *down;
	*count = 0;
	for(i=0;i<length;i++){
		x = wave[i]->x, y = wave[i]->y;
		value = wave[i]->value;
		left = getNode(x-1,y);
		right = getNode(x+1,y);
		up = getNode(x,y+1);
		down = getNode(x,y-1);
		if(left){
			if(left->value > value){
				newwave[(*count)++] = left;
			}
		}
		if(right){
			if(right->value > value){
				newwave[(*count)++] = right;
			}
		}
		if(up){
			if(up->value > value){
				newwave[(*count)++] = up;
			}
		}
		if(down){
			if(down->value > value){
				newwave[(*count)++] = down;
			}
		}
	}
}
long containsNode(Node* node, Node** collection, long count){
	long i;
	for(i = 0;i<count;i++){
		if(collection[i]==node){
			return i;
		}
	}
	return -1;
}