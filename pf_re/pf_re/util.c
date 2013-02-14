#include <stdlib.h>
#include <stdio.h>

#include "util.h"

static void *checkPointer (void *p)
{
	if (p == NULL)
	{
		printf ("Error: out of memory!\n");
		exit (1);
	}
	return p;
}

void *safeMalloc (size_t size)
{
	return checkPointer (malloc(size));
}