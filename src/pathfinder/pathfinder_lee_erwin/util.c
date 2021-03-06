#include <stdlib.h>
#include <stdio.h>
#include <time.h>

#include "util.h"

const long maxConn = 4 /* We use it in a grid */;
clock_t begin = 0, end = 0;
long numberAlloc = 0;

void *safeMalloc (size_t size)
{
	void* p;
	#ifdef _DEBUG
	printf ("(%05ld/%05ld) safeMalloc: Allocating %ld bytes... ", numberAlloc,5*m*n,size);
	numberAlloc++;
	#endif
	p = malloc(size);
	if (p == NULL)
	{
		printf ("ERROR: Out of memory!\n");
		exit (1);
	}
	memusage += size;
	#ifdef _DEBUG
	printf ("Done at %p\n",p);
	#endif
	return p;
}
void printMemSize(void) {
	if(memusage>(8UL*1024UL)){
		printf("~~~~~Mem Size: %ld kiB~~~~~\n",memusage/(1024UL));
	} else if(memusage>(8UL*1024UL*1024UL)){
		printf("~~~~~Mem Size: %ld MiB~~~~~\n",memusage/(1024UL*1024UL));
	} else if(memusage>(2UL*1024UL*1024UL*1024UL)){
		printf("~~~~~Mem Size: %ld GiB~~~~~\n",memusage/(1024UL*1024UL*1024UL));
	} else {
		printf("~~~~~Mem Size: %ld B~~~~~\n",(long)memusage);
	}
}
void startStopwatch(void){
	begin = clock();
}
double stopStopwatch(void){
	end = clock();
	return ((double)end - (double)begin) / CLOCKS_PER_SEC;
}
void safeFree (void *ptr){

	#ifdef _DEBUG
	printf ("safeFree: Freeing at %p... ", ptr);
	#endif
	if(ptr){
		free(ptr);
	} else {
		printf("ERROR: Tried to free something not allocated.\n");
	}
	#ifdef _DEBUG
	printf ("Done\n");
	#endif
	return;
}

void printField(void){
	//line chars

	/*char pv = 186;
	char ph = 205;*/
	char v = 179;
	char h = 196;
	char mc = 'x';

	Node *tmp;
	Line *tmpline;
	char* spacing = "  ";
	long x;
	long y;
	//long i;
	printf("\n\n");
	for(y = n-1;y>=0;y--){
		for(x = 0;x<m;x++){
			tmp = getNode(x,y);
			printf("(%04ld)", tmp->value);
			if(x<m-1){
				//node to the left exists
				tmpline = getLineFilter(tmp,getNode(x+1,y),0);
				if(tmpline){
					if(tmpline->mine){
						printf("%c",mc);
					} else {
						printf("%c",h);
					}
				} else {
					printf(" ");
				}
			}
			//printf("(%d,%d;%03d)", tmp->x, tmp->y,tmp->value);
		}
		if(y>0){
			//there is a row under this one.

			printf("\n");
			for(x = 0;x<m;x++){
				tmpline = getLineFilter(getNode(x,y),getNode(x,y-1),0);
				printf("%s",spacing);
				if(tmpline){
					if(tmpline->mine == 1){
						printf("%c",mc);
					} else {
						printf("%c",v);
					}
				} else {
					printf(" ");
				}
				printf("%s",spacing);
				printf(" ");
			}
			printf("\n");
		}
	}
	printf("\n\n");
}
void saveField(char* filename){
	//line chars
	FILE *hf=fopen(filename,"wt");
	/*char pv = 186;
	char ph = 205;*/
	char v = '|';
	char h = '-';
	char mc = 'x';
	Node *tmp;
	Line *tmpline;
	long x;
	long y;
	//long i;
	fprintf(hf,"Header\n\n");
	for(y = n-1;y>=0;y--){
		for(x = 0;x<m;x++){
			tmp = getNode(x,y);
			fprintf(hf,"%4ld;%4ld;%4ld;", tmp->x,tmp->y,tmp->value);
			if(x<m-1){
				//node to the left exists
				tmpline = getLineFilter(tmp,getNode(x+1,y),0);
				if(tmpline){
					if(tmpline->mine==1){
						fprintf(hf,"%c;",mc);
					} else {
						fprintf(hf,"%c;",h);
					}
				} else {
					fprintf(hf,"0;");
				}
			}
			//printf("(%d,%d;%03d)", tmp->x, tmp->y,tmp->value);
		}
		if(y>0){
			//there is a row under this one.

			fprintf(hf,"\n");
			for(x = 0;x<m;x++){
				tmpline = getLineFilter(getNode(x,y),getNode(x,y-1),0);
				//fprintf(hf,"%s",spacing);
				if(tmpline){
					if(tmpline->mine == 1){
						fprintf(hf,"%c;",mc);
					} else {
						fprintf(hf,"%c;",v);
					}
				} else {
					fprintf(hf,"0;");
				}
				//fprintf(hf,"%s",spacing);
				//fprintf(hf," ");
			}
			fprintf(hf,"\n");
		}
	}
	fprintf(hf,"\n\n");
	fclose(hf);
}
void emptySTDIN(void){
	int c;
	while ((c = getchar()) != '\n' && c != EOF);
}



