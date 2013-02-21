#include <stdlib.h>
#include <stdio.h>
#include <time.h>

#include "util.h"
#include "node.h"

clock_t begin = 0, end = 0;

void *safeMalloc (size_t size)
{
	void* p = malloc(size);
	if (p == NULL)
	{
		print (1, 1, "Error: out of memory, terminating program!\n");
		exit (0);
	}
	return p;
}

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

void startStopwatch(void){
	begin = clock();
}

double stopStopwatch(void){
	end = clock();
	return ((double)end - (double)begin) / CLOCKS_PER_SEC;
}

void createGrid()
{
	//Init
	unsigned int i;

	//Create node and add it to the array
	for(i=0; i<numNodes; i++)
	{
		nodes[i] = newNode(i);
	}
}

void displayGrid ()
{
	//Init
	unsigned int i, j;
	Node *current, *next;

	if (m > 20 || n > 20)
	{
		print (1, 1, "Wave expansion done (took %.4lfs), grid is too big to print\n", expansionTime);
		return;
	}

	print (1, 1, "Wave expansion done (took %.4lfs), displaying results!\n\n", expansionTime);

	//Loop through nodes top->down, left->right
	for (i=2*n-2; i!=-1; i--)
	{
		for (j=0; j<m; j++)
		{
			if (i % 2 == 0)
			{
				current = getNodeByXY(j, (i + 2)/2 - 1);
				next = getAdjNode(current, 1, 0);

				if (j < (m - 1))
				{
					//Check for mine
					if (!isMine (current, next))
					{
						if (current->label == -1)
							print(1, 1, "# - ");
						else
							print(1, 1, "%d - ", current->label);
					}
					else
					{
						if (current->label == -1)
							print(1, 1, "# X ");
						else
							print(1, 1, "%d X ", current->label);
					}
				}
				else
				{
					if (current->label == -1)
						print(1, 1, "#");
					else
						print(1, 1, "%d", current->label);
				}
			}
			else
			{
				if (i > 0)
				{
					current = getNodeByXY(j, (i + 1)/2);
					next = getAdjNode(current, 3, 0);

					if (j < m)
					{
						//Check for mine
						if (!isMine (current, next))
							print(1, 1, "|   ");
						else
							print(1, 1, "X   ");
					}
					else 
					{
						//Check for mine
						if (!isMine (current, next))
							print(1, 1, "|");
						else
							print(1, 1, "X");
					}
				}
			}
		}
		print(1, 1, "\n");
	}
	print(1, 1, "\n");
}

void displayPath ()
{
	int i;

	print (1, 1, "Found a path from node %d to node %d, with length %d (took %.4lfs)!\n", entryNode->id, exitNode->id, exitNode->label, backTraceTime);

	if (exitNode->label > 250)
	{
		print (1, 1, "Path length too high for display in console, will only print to logs\n\n");
		print (0, 1, "Entry node is %d\n", entryNode->id);
		for (i=1; i<exitNode->label; i++)
		{
			print (0, 1, "Next node in path is node %d, with label %d\n", path[i+1]->id, path[i+1]->label);
		}
		print (0, 1, "Exit node (id: %d) reached!\n", exitNode->id);
	}
	else
	{
		print (1, 0, "Displaying result:\n\n");
		print (1, 1, "Entry node is %d\n", entryNode->id);
		for (i=1; i<exitNode->label; i++)
		{
			print (1, 1, "Next node in path is node %d, with label %d\n", path[i+1]->id, path[i+1]->label);
		}
		print (1, 1, "Exit node (id: %d) reached!\n\n", exitNode->id);
	}


}