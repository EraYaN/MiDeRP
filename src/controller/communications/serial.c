#include "serial.h"
#include <windows.h>
//--------------------------------------------------------------
// Function: initSio
// Description: intializes the parameters as Baudrate, Bytesize,
//           Stopbits, Parity and Timeoutparameters of
//           the COM port
//--------------------------------------------------------------
int initSio(HANDLE hSerial, DWORD bRate){

    COMMTIMEOUTS timeouts ={0};
    DCB dcbSerialParams = {0};

    dcbSerialParams.DCBlength = sizeof(dcbSerialParams);

    if (!GetCommState(hSerial, &dcbSerialParams)) {
    //error getting state
        //printf("error getting state \n");
		return -1;
    }

    dcbSerialParams.BaudRate = bRate;
    dcbSerialParams.ByteSize = 8;
    dcbSerialParams.StopBits = ONESTOPBIT;
    dcbSerialParams.Parity   = NOPARITY;

    if(!SetCommState(hSerial, &dcbSerialParams)){
        //error setting serial port state
        //printf("error setting state \n");
		return -2;
    }

    timeouts.ReadIntervalTimeout = 50;
    timeouts.ReadTotalTimeoutConstant = 50;
    timeouts.ReadTotalTimeoutMultiplier = 10;

    timeouts.WriteTotalTimeoutConstant = 50;
    timeouts.WriteTotalTimeoutMultiplier = 10;

    if(!SetCommTimeouts(hSerial, &timeouts)){
    //error occureed. Inform user
        //printf("error setting timeout state \n");
		return -3;
    }
	return 0;
}

//--------------------------------------------------------------
// Function: readByte
// Description: reads a single byte from the COM port into
//              buffer buffRead
//--------------------------------------------------------------
int readByte(HANDLE const hSerial, byte *buffRead) {

    DWORD dwBytesRead = 0;

    if (!ReadFile(hSerial, buffRead, 1, &dwBytesRead, NULL))
    {
        return -1;
    }
    //printf("Byte read from read buffer is: %x \n", buffRead[0]);
    return 0;
}

//--------------------------------------------------------------
// Function: writeByte
// Description: writes a single byte stored in buffRead to
//              the COM port
//--------------------------------------------------------------
int writeByte(HANDLE const hSerial, byte *buffWrite){

    DWORD dwBytesWritten = 0;

    if (!WriteFile(hSerial, buffWrite, 1, &dwBytesWritten, NULL))
    {
        return -1;
    }
    //printf("Byte written to write buffer is: %x \n", buffWrite[0]);

    return 0;
}

//For example hSerial = openSerial("COM6", CBR_9600);
int openSerial(HANDLE *_hSerial, LPCWSTR const cPort, DWORD const bRate)
{
    HANDLE hSerial; 
	int res;
    //----------------------------------------------------------
    // Open COMPORT for reading and writing
    //----------------------------------------------------------
    hSerial = CreateFile(cPort,
        GENERIC_READ | GENERIC_WRITE,
        0,
        0,
        OPEN_EXISTING,
        FILE_ATTRIBUTE_NORMAL,
        0
    );

    if(hSerial == INVALID_HANDLE_VALUE){
        if(GetLastError()== ERROR_FILE_NOT_FOUND){
            //serial port does not exist. Inform user.
			//TODO make proper error handling class.
            //printf(" serial port does not exist \n");
			return -1;
        }
        //some other error occurred. Inform user.
		//TODO make proper error handling class.
        //printf(" some other error occured. Inform user.\n");
		return -2;
    }
	//Init parameters
	
    if(res = initSio(hSerial,bRate)!=0)
		return -2+res;
	_hSerial = &hSerial; 
	return 0;
}
int closeSerial(HANDLE const hSerial)
{
    return CloseHandle(hSerial);
}