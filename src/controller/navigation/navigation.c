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

__declspec(dllexport) int __stdcall getNumberofMines(void) {
	//return int the is number of mines.
	return 0;
}

__declspec(dllexport) int __stdcall getNumberofNodes(void) {
	//return int the is number of Nodes.
	return 0;
}

__declspec(dllexport) char __stdcall getMine(int X1, int Y1, int X2, int Y2) {
	//return btween 2 nodes.
	//true if mine
	return 0;
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