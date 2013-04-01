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

/*__declspec(dllexport) int __stdcall getNumberofMines(void) {
	//return int the is number of mines.
	return 0;
}

__declspec(dllexport) int __stdcall getNumberofNodes(void) {
	//return int the is number of Nodes.
	return 0;
}*/

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

__declspec(dllexport) int __stdcall setMineC(unsigned int X1, unsigned int Y1, unsigned int X2, unsigned int Y2, char mine) {
	
	unsigned int id1 = Y1*m+X1;
	unsigned int id2 = Y2*m+X2;
	printf("Adding mine between %d;%d (%d) and %d;%d (%d).\n",X1,Y1,id1,X2,Y2,id2);
	//return setMine(getNodeC(X1,Y1),getNodeC(X2,Y2),mine);	
	return setMineI(id1,id2,mine);
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
		//pathout = (unsigned int*)safeMalloc(sizeof(unsigned int)*(length+1));		
		//pathout[0]=entryNode->id;
		for (i=0; i<length; i++)
		{
			//printf( "Next node in path is node %d\n", path[i+1]->id);
			pathout[i]=path[i+1]->id;
		}
		pathout[length]=exitNode->id;	
		
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