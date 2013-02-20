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
	//long index = 0;
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
	printf("Starting floodfill...\n");
	length = setValuesofField(end,start);
	printf("Done\n");
	//printf("setValuesofFiels returned: %d.\n",length);
	//trace your path, starting at 'start'.
	printf("Backtracing...");
	current = start;
	for(wavenum = length; wavenum>=0;wavenum--){
		if(current == end) break;
		num = 0;
		conns = 0;
		linestoneighbours = getNodeConnectionsBackTrack(current,&conns);
		neighbours = (Node**)safeMalloc(sizeof(Node*)*conns);
		for(j=0;j<conns;j++){
			if(linestoneighbours[j]->destination==current){
				neighbours[num] = linestoneighbours[j]->origin;
				num++;
			} else if(linestoneighbours[j]->origin==current){
				neighbours[num] = linestoneighbours[j]->destination;
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
	printf("Done\n");
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
	//Node **buwave;
	long inCurrentWave = 1;
	//long inPrevWave = 1;
	Node **neighbours;
	Line **linestoneighbours;

	start->value = currentNumber;
	neighbours = (Node**)safeMalloc(sizeof(Node*));
	wave = (Node**)safeMalloc(sizeof(Node*));
	neighbours[0] = start;
	num = 1;
	while(!done){
	   // printField();
	    //inPrevWave = inCurrentWave;
		inCurrentWave = 0;
		inCurrentWave = num;
		num = 0;
		//buwave = wave;
		if(!neighbours) {
            done = 1;
            break;
		}
		safeFree(wave);
		wave = neighbours;//(Node**)safeMalloc(sizeof(Node*)*((num*3)+1));
		/*for(i=0;i<num;i++){
			if(neighbours[i]->value == currentNumber){
				wave[inCurrentWave++] = getNode(neighbours[i]->x,neighbours[i]->y);
			}
		}*/
		//safeFree(neighbours);
		if(currentNumber%((n+m)/200)==0){
			printf("Making new wave (#%ld) of %ld elements, last wave: %ld items.\n",currentNumber,inCurrentWave+4, inCurrentWave);
		}
		/*if(currentNumber>995){
			printf("Going to fail?. (Hit enter once or twice)\n");
			saveField("iterationfile.txt");
			getchar();
			getchar();
		}*/
		if(inCurrentWave==0){
			done = 1;
			break;
		}
		//printf("Making new wave of ");
        neighbours = (Node**)safeMalloc(sizeof(Node*)*(inCurrentWave+4));
        num = 0;
		for(i=0;i<inCurrentWave&&!done;i++){

			current = wave[i];
			conns = 0;
			linestoneighbours = getNodeConnections(current,&conns);

			for(j=0;j<conns;j++){
				if(linestoneighbours[j]->origin==current && linestoneighbours[j]->destination->value > current->value && containsNode(linestoneighbours[j]->destination,neighbours,num)<0){
					linestoneighbours[j]->destination->value = currentNumber+1;
					neighbours[num] = linestoneighbours[j]->destination;
					num++;
				} else if(linestoneighbours[j]->destination==current && linestoneighbours[j]->origin->value > current->value && containsNode(linestoneighbours[j]->origin,neighbours,num)<0){
					linestoneighbours[j]->origin->value = currentNumber+1;
					neighbours[num] = linestoneighbours[j]->origin;
					num++;
				}
			}
			safeFree(linestoneighbours);

			//safeFree(neighbours);
		}
		for(j=0;j<num;j++){
            if(neighbours[j] == goal){
                done = 1;
                break;
            }
        }
		//safeFree(wave);
		currentNumber++;
		//printField();
		//done with wave

	}
	//linestoneighbours = getNodeConnections(node,&conns);
	safeFree(wave);
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
