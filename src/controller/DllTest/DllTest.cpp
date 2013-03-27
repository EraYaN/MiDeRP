#include <windows.h>

BOOL __stdcall DllMain(HINSTANCE hInst, DWORD dwReason, LPVOID lpReserved) {
	return TRUE;
}

extern "C" __declspec(dllexport) void __stdcall Message(char* p_szMessage) {
	MessageBox(NULL, p_szMessage, "Message from DLL", MB_OK);
}