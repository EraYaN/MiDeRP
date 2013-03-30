#include "util.h"
#include "windows.h"

/*BOOL __stdcall DllMain(HINSTANCE hInst, DWORD dwReason, LPVOID lpReserved) {
	return TRUE;
}*/
//Wrappers
#ifdef __cplusplus
extern "C"{
#endif
__declspec(dllexport) int __stdcall initNavigation(int m, int n) {
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
	int i;
	for(i=0;i<numNodes;i++){
		nodes[i]->mines[0] = 0;
		nodes[i]->mines[1] = 0;
		nodes[i]->mines[2] = 0;
		nodes[i]->mines[3] = 0;
	}
	return 0;
}

__declspec(dllexport) int __stdcall setMineC(unsigned int X1, unsigned int Y1, unsigned int X2, unsigned int Y2, char mine) {
	return setMine(getNodeC(X1,Y1),getNodeC(X2,Y2),mine);	
}

__declspec(dllexport) void __stdcall loopNavigation(unsigned int entry, unsigned int exit) {
	loop(entry, exit);
}

__declspec(dllexport) void __stdcall closeNavigation(void) {
	quit();
}

#ifdef __cplusplus
}
#endif