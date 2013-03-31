using System;
using System.IO.Ports;

namespace Director
{
    interface ISerial
    {
        event EventHandler<SerialDataEventArgs> SerialDataEvent;       
    }
}
