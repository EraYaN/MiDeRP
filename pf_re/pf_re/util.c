#include <stdlib.h>
#include <stdio.h>

#include "util.h"
const int m = 5, n = 5;
void *safeMalloc (size_t size)
{
	void* p = malloc(size);
	if (p == NULL)
	{
		printf ("Error: out of memory!\n");
		exit (1);
	}
	return p;
}