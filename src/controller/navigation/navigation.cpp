#include "util.h"
#include "windows.h"

BOOL __stdcall DllMain(HINSTANCE hInst, DWORD dwReason, LPVOID lpReserved) {
	return TRUE;
}
//Wrappers
extern "C" __declspec(dllexport) void __stdcall initNavigation(int m, int n) {
	init(m, n);
}

extern "C" __declspec(dllexport) int __stdcall getNumberofMines(void) {
	//return int the is number of mines.
	return 0;
}

extern "C" __declspec(dllexport) int __stdcall getNumberofNodes(void) {
	//return int the is number of Nodes.
	return 0;
}

extern "C" __declspec(dllexport) char __stdcall getMine(int X1, int Y1, int X2, int Y2) {
	//return btween 2 nodes.
	//true if mine
	return 0;
}

extern "C" __declspec(dllexport) void __stdcall loopNavigation(void) {
	loop();
}

extern "C" __declspec(dllexport) void __stdcall closeNavigation(void) {
	quit();
}