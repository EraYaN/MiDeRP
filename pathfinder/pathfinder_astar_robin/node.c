#include "util.h"

char nodeExists (unsigned int id)
{
	if (id > numNodes || id < 0)
		return 0;
	else
		return 1;
}

unsigned int nodeCoord (unsigned int id, char axis)
{
	if (!nodeExists (id))
	{
		print (1, 1, "Error: tried to get coordinates of non-existing node %d!\n", id);
		return 0;
	}
	else
	{
		if (axis == 'X')
			return id % m;
		else if (axis == 'Y')
			return id / m % n;
		else
		{
			print (1, 1, "Error: entered invalid axis %c!\n", axis);
			return 0;
		}
	}
}