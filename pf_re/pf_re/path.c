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
		free(linestoneighbours);
		for(j=0;j<num;j++){		
			if(neighbours[j]->value==current->value-1){
				path[(*count)++]=neighbours[j];
				current = neighbours[j];
			}
		}
		free(neighbours);
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
	Node **wave;
	long inCurrentWave;
	Node **neighbours;
	Line **linestoneighbours;
	
	start->value = currentNumber;
	while(!done){
		inCurrentWave = 0;
		wave = (Node**)safeMalloc(sizeof(Node*)*numNodes);
		for(i=0;i<numNodes;i++){
			if(nodes[i]->value == currentNumber){
				wave[inCurrentWave++] = nodes[i];
			}
		}
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
			free(linestoneighbours);
			for(j=0;j<num;j++){		
				if(neighbours[j]->value > currentNumber){
					neighbours[j]->value = currentNumber+1;
				}
				if(neighbours[j] == goal){
					done = 1;
					break;
				}
			}
			free(neighbours);
		}
		free(wave);
		currentNumber++;
		//printField();
		//done with wave

	}
	//linestoneighbours = getNodeConnections(node,&conns);
	return currentNumber;
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