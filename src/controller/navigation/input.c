#include "util.h"

//Get grid dimensions (deprecated)
/*void getDimensions ()
{
	//TODO get those from parameter to int(int, int) instead
	m = 5;
	n = 5;

}*/

//Get entry and exit points
void getPoints (unsigned int entry_id, unsigned int exit_id)
{
	//Get entry node
	if(!entryNode||entryNode->id!=entry_id) //while = infinite loop //TODO geen entryNode meer alleen id?
	{
		entry_id = 0; //TODO parameters of loop or other function.
		entryNode = newNode (NULL, entry_id);//MOVED see below
	}
	
	//Get exit node
	if(!exitNode||exitNode->id!=exit_id) //while = infinite loop //TODO see above
	{
		exit_id = n*m-1; //TODO parameters of loop or other function.
		exitNode = newNode (NULL, exit_id);//MOVED that shit breaks it of ran twice.
	}
	//TODO: fix this, What da FUK? Is the ID not enough?
	//Actually create entryNode and exitNode NEIN breaks shit.
		

	printf( "\nPoints OK! Starting pathfinder...\n\n");
}
