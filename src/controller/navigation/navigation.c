#include "util.h"
#include "windows.h"

/*BOOL __stdcall DllMain(HINSTANCE hInst, DWORD dwReason, LPVOID lpReserved) {
	return TRUE;
}*/
//Wrappers
#ifdef __cplusplus
extern "C"{
#endif
__declspec(dllexport) int __stdcall initNavigation(unsigned int m, unsigned int n) {
	return init(m, n);
}

__declspec(dllexport) int __stdcall clearMines() {
	unsigned int i;
	for(i=0;i<numNodes;i++){
		if(nodes[i]){
			nodes[i]->mines[0] = 0;
			nodes[i]->mines[1] = 0;
			nodes[i]->mines[2] = 0;
			nodes[i]->mines[3] = 0;
		}
	}
	return 0;
}

__declspec(dllexport) int __stdcall clearVisited() {
	unsigned int i;
	for(i=0;i<numNodes;i++){
		if(nodes[i]){
			nodes[i]->visited[0] = 0;
			nodes[i]->visited[1] = 0;
			nodes[i]->visited[2] = 0;
			nodes[i]->visited[3] = 0;
		}
	}
	return 0;
}


__declspec(dllexport) int __stdcall setMineC(unsigned int X1, unsigned int Y1, unsigned int X2, unsigned int Y2, char mine) {
	
	unsigned int id1 = Y1*m+X1;
	unsigned int id2 = Y2*m+X2;
	printf("Adding mine between %d;%d (%d) and %d;%d (%d).\n",X1,Y1,id1,X2,Y2,id2);
	return setMineI(id1,id2,mine);
}

__declspec(dllexport) int __stdcall setVisitedC(unsigned int X1, unsigned int Y1, unsigned int X2, unsigned int Y2, char visited) {
	
	unsigned int id1 = Y1*m+X1;
	unsigned int id2 = Y2*m+X2;
	printf("Adding visited node between %d;%d (%d) and %d;%d (%d).\n",X1,Y1,id1,X2,Y2,id2);
	return setVisitedI(id1,id2,visited);
}

__declspec(dllexport) int __stdcall updatePath(unsigned int entry, unsigned int exit) {
	printf("Finding path between ID's: %d and %d.\n",entry, exit);
	return loop(entry, exit);
}

__declspec(dllexport) unsigned int __stdcall getPathLength() {	
	return length;
}

__declspec(dllexport) int __stdcall extractPath(int *pathout) {
	unsigned int i;
	if(path){
		printf("Extracting path....\n");		
		for (i=0; i<=length; i++)
		{
			pathout[i]=path[i]->id;
		}
		
		return 0;
	} else {
		printf("No path to extract....\n");
		return -1;
	}
}

__declspec(dllexport) void __stdcall closeNavigation(void) {
	quit();
}

#ifdef __cplusplus
}
#endif