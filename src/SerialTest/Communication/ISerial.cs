using System;
using System.IO.Ports;

namespace MiDeRP
{
    interface ISerial
    {
        event EventHandler<SerialDataEventArgs> SerialDataEvent;       
    }
}
