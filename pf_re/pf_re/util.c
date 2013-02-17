#include <stdlib.h>
#include <stdio.h>

#include "util.h"

const int m = 5 /* x */, n = 5 /* y */, maxConn = 4 /* We use it in a grid */;



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

void printField(){
	//line chars
	
	/*char pv = 186;
	char ph = 205;*/
	char v = 179;
	char h = 196;
	char mc = 'x';
	Node *tmp;
	Line *tmpline;
	char* spacing = "  ";
	int x;
	int y;
	//int i;
	printf("\n\n");	
	for(y = n-1;y>=0;y--){
		for(x = 0;x<m;x++){
			tmp = getNode(x,y);
			printf("(%03d)", tmp->value);
			if(x<m-1){
				//node to the left exists
				tmpline = getLineFilter(tmp,getNode(x+1,y),0);
				if(tmpline->mine){
					printf("%c",mc);
				} else {
					printf("%c",h);
				}
			}
			//printf("(%x,%d;%03d)", tmp->x, tmp->y,tmp->value);
		}		
		if(y>0){
			//there is a row under this one.
			
			printf("\n");
			for(x = 0;x<m;x++){
				tmpline = getLineFilter(getNode(x,y),getNode(x,y-1),0);
				printf("%s",spacing);
				if(tmpline->mine == 1){
					printf("%c",mc);
				} else {
					printf("%c",v);
				}
				printf("%s",spacing);
				printf(" ");
			}
			printf("\n",spacing);
		}
	}
	printf("\n\n");
}




