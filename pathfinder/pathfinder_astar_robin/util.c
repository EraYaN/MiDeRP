#include "util.h"

clock_t begin = 0, end = 0;

//Safe allocation of memory
void *safeMalloc (size_t size)
{
	void *p = malloc(size);	

	if (p == NULL)
	{
		print (1, 1, "Error: out of memory (tried to allocate %d bytes), terminating program!\n", size);
		fclose (file);
		exit (0);
	}
	return p;
}

//Prints to either console, logs or both
void print (char toconsole, char tofile, char *format, ...)
{
	va_list args;

	if (!toconsole && !tofile)
		return;

	va_start (args, format);
	if (toconsole)
		vprintf (format, args);
	if (tofile)
		vfprintf (file, format, args);
	va_end (args);
}

//Mark the beginning of a time count
void startStopwatch(void){
	begin = clock();
}

//End time count and return duration
double stopStopwatch(void){
	end = clock();
	return ((double)end - (double)begin) / CLOCKS_PER_SEC;
}

//Create grid
void createGrid ()
{
	unsigned int i;

	//Allocate memory and initialize every node to NULL
	nodes = (Node**) safeMalloc (sizeof (Node*) * (size_t)numNodes);
	for (i=0; i<numNodes; i++)
		nodes[i] = NULL;
}

//Display found path
void displayPath ()
{
	unsigned int i;

	if (!path)
	{
		print (1, 1, "Failed to find a path, quitting...\n");
		return;
	}

	print (1, 1, "Found a path from node %d to node %d, with length %d (took %.4lfs)!\n", entryNode->id, exitNode->id, length, findTime);

	if (length > 250)
	{
		print (1, 1, "Path length too high for display in console, will only print to logs\n\n");
		print (0, 1, "Entry node is %d\n", entryNode->id);
		for (i=1; i<length; i++)
		{
			print (0, 1, "Next node in path is node %d\n", path[i+1]->id);
		}
		print (0, 1, "Exit node (id: %d) reached!\n", exitNode->id);
	}
	else
	{
		print (1, 0, "Displaying result:\n\n");
		print (1, 1, "Entry node is %d\n", entryNode->id);
		for (i=1; i<length; i++)
		{
			print (1, 1, "Next node in path is node %d\n", path[i+1]->id);
		}
		print (1, 1, "Exit node (id: %d) reached!\n\n", exitNode->id);
	}


}