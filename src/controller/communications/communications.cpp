#include "serial.h"
#include "serial.c" // Don't know why this one need to be here. Otherwise I get unresolved externals.
#include "windows.h"

BOOL __stdcall DllMain(HINSTANCE hInst, DWORD dwReason, LPVOID lpReserved) {
	return TRUE;
}
//Wrappers
extern "C" __declspec(dllexport) int __stdcall initCommunications(HANDLE *hSerial, LPCWSTR ComPort, DWORD BaudRate) {
	return openSerial(hSerial,ComPort, BaudRate);
}

extern "C" __declspec(dllexport) int __stdcall closeCommunications(HANDLE hSerial) {
	//returns 0 on success
	//returns -1 on fail
	return closeSerial(hSerial);	
}

extern "C" __declspec(dllexport) int __stdcall writeToRobot(HANDLE hSerial, byte Byte) {
	//returns -1 on error
	return writeByte(hSerial,&Byte);	
}

extern "C" __declspec(dllexport) int __stdcall readFromRobot(HANDLE hSerial, byte * Byte) {
	//returns -1 on error
	return readByte(hSerial, Byte);	
}