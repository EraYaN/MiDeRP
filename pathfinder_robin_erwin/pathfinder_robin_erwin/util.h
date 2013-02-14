//Struct initialisations
struct node {
	const int x, y; //For debugging

	int visited,
		previous,
		distance;
};

struct vertex {
	struct node *origin;
	struct node *destination;
	
	int mine;
};

//Function prototypes
void *safe_malloc (size_t size);
node *newNode (int x, int y);
vertex *newVertexH (int i);
vertex *newVertexV (int i);

