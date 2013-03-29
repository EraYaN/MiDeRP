#ifndef SERIAL_H
#define SERIAL_H

#include <Windows.h>

//Function prototypes
int writeByte(HANDLE const hSerial, byte *buffWrite);
int readByte(HANDLE const hSerial, byte *buffRead);
int openSerial(HANDLE *hSerial, LPCWSTR const cPort, DWORD const bRate);
int closeSerial(HANDLE const hSerial);

#endif
