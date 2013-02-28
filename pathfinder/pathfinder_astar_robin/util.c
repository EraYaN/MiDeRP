#include "util.h"

//Safe allocation of memory
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