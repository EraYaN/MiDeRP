#include "util.h"

int main()
{
	//Init
	int entry_id, exit_id;

	file = fopen ("log.txt", "a");
	print (0, 1, "##########################################\nProgram started\n\n");

	//Get grid dimensions
	while (!m)
	{
		print(1, 0, "Enter grid width: \n");
		scanf("%d", &m);
		if (m <= 1)
		{
			print(1, 0, "Width has to be more than 1\n");
			print(0, 1, "Entered invalid value for width\n");
			m = 0;
		}
	}
	print (1, 1, "Grid width is %d\n", m);
	while (!n)
	{
		print(1, 0, "Enter grid height: \n");
		scanf("%d", &n);
		if (n <= 1)
		{
			print(1, 0, "Height has to be more than 1\n");
			print(0, 1, "Entered invalid value for height\n");
			n = 0;
		}
	}
	print (1, 1, "Grid height is %d\n", n);

	//Calculate useful values
	print(1, 1, "\nInitializing variables...\n");
	numNodes =	m*n;

	//Get entryNode and exitNode nodes
	print(1, 1, "Init successful, ready for path variables\n\n");
	while (!entryNode)
	{
		print(1, 0, "Enter entry node id: \n");
		scanf("%d", &entry_id);
		if (!nodeExists(entry_id))
			print(1, 1, "Error: tried to use non-existing node!\n");
		else
			entryNode = entry_id;
	}
	print (1, 1, "Entry node is %d\n", entryNode);

	while (!exitNode)
	{
		print(1, 0, "Enter exit node id: \n");
		scanf("%d", &exit_id);
		if (exit_id == entry_id)
			print(1, 1, "Error: entry node cannot be same as exit node!\n");
		else if (!nodeExists(exit_id))
			print(1, 1, "Error: tried to use non-existing node!\n");
		else
			exitNode = exit_id;
	}
	print (1, 1, "Exit node is %d\n", exitNode);

	//Find path!
	print(1, 1, "\nPoints OK! Starting pathfinder...\n\n");
	findPath ();

	//Exit program
	print (0, 1, "All done!\n##########################################\n\n");
	fclose (file);

	return 0;
}