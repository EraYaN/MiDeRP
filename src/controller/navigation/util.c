#include "util.h"

clock_t begin = 0, end = 0;

//Safe allocation of memory
void *safeMalloc (size_t size)
{
	void *p = malloc(size);	

	if (p == NULL)
	{
		printf("Error: out of memory (tried to allocate %d bytes), terminating program!\n", size);
		quit ();
	}
	return p;
}

//Prints to either console, logs or both
/*void print (char toconsole, char tofile, char *format, ...)
{
	va_list args;

	if (!toconsole && !tofile)
		return;
	if(format==NULL){
		return;
	}
	if(file==NULL)
		return;
	va_start (args, format);
	if (toconsole)
		//vprintf (format, args);
	if (tofile&&args!=NULL)
		vfprintf (file, format, args);
	else if (tofile)
		printf(file,format);
	va_end (args);
}*/

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
void createGrid (int m, int n)
{
	unsigned int i;
	numNodes = m * n;

	printf( "Init successful, creating grid...\n");
	startStopwatch ();

	//Allocate memory and initialize every node to NULL
	nodes = (Node**) safeMalloc (sizeof (Node*) * (size_t)numNodes);
	for (i=0; i<numNodes; i++)
		nodes[i] = NULL;

	gridTime = stopStopwatch ();
	printf( "Grid created successfully (took %.4lfs), ready to calculate paths!\n", gridTime);
}

