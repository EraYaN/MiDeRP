#include "util.h"

//Get entry and exit points
void getPoints (unsigned int entry_id, unsigned int exit_id)
{
	//Get entry node
	if(!entryNode||entryNode->id!=entry_id) //while = infinite loop //TODO geen entryNode meer alleen id?
	{
		//entry_id = 0; //TODO parameters of loop or other function.
		entryNode = newNode (entry_id);//MOVED see below
	}
	
	//Get exit node
	if(!exitNode||exitNode->id!=exit_id) //while = infinite loop //TODO see above
	{
		//exit_id = n*m-1; //TODO parameters of loop or other function.
		exitNode = newNode (exit_id);//MOVED that shit breaks it of ran twice.
	}	

	printf( "\nPoints OK! Starting pathfinder...\n\n");
}
