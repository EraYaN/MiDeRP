using System;
using System.IO.Ports;

namespace MiDeRP
{
    public class SerialDataEventArgs : EventArgs
    {
        public readonly byte DataByte;
        public SerialDataReceivedEventArgs innerEvent;
        public SerialDataEventArgs(byte _DataByte, SerialDataReceivedEventArgs e)
        {
            DataByte = _DataByte;            
            innerEvent = e;
        }
    }
}
