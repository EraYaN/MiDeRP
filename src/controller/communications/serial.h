#ifndef SERIAL_H
#define SERIAL_H
#include <stdio.h>
#include <stdlib.h>
#include <Windows.h>
#include <string.h>

//Function prototypes
extern int writeByte(HANDLE hSerial, char *buffWrite);
extern int readByte(HANDLE hSerial, char *buffRead);
extern HANDLE openSerial(LPCSTR cPort, DWORD bRate);
extern void closeSerial(HANDLE hSerial);

#endif
