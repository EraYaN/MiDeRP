void *safeMalloc (size_t size)
{
    void *p = malloc(size);
	if (p == NULL)
	{
		printf ("Error: out of memory!\n");
		exit (1);
	}
	return p;
}


node *newNode (int x, int y)
{
	node *n		= safeMalloc(sizeof (node));

	n->x		= x;
	n->y	= y;
	n->visited = 0;
	n->previous		= NULL;
	n->distance = inf;

	return n;
}
