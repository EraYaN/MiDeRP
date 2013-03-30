using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Director
{
    public class Communications
    {
        IntPtr h;
        string port;
        uint baudrate;
        public Communications(string _port, uint _baudrate)
        {
            port = _port;
            baudrate = _baudrate;
            int res;
            if ((res = initCommunications(out h, port, baudrate)) != 0)
            {
                MessageBox.Show("Communications init returned error (" + res + ")","Communications Error",MessageBoxButton.OK,MessageBoxImage.Error);
                //TODO exception
            }
        }
        ~Communications()
        {
            int res;
            if ((res = closeCommunications(h)) != 0)
            {
                MessageBox.Show("Communications close returned error (" + res + ")", "Communications Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //TODO exception
            }
        }
        //Import all methods.

        /// <summary>
        /// The function the seeks to open the COM port and link to the serial xBee module.
        /// </summary>
        /// <param name="Handle">Output handle to serial xBee port.</param>
        /// <param name="ComPort">The name of the COM port. ex: COM1, COM5</param>
        /// <param name="BaudRate">Baud rate of the connection.
        /// Possible values:\n
        /// 110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 56000, 57600, 115200, 128000, 256000.
        /// </param>
        /// <returns>Return Code (0 = success)</returns>
        [DllImport("communications.dll")]
        static extern int initCommunications(out IntPtr Handle, string ComPort, uint BaudRate);
        /// <summary>
        /// The function the seeks to open the COM port and link to the serial xBee module.
        /// </summary>
        /// <param name="Handle">Handle to the serial port returned by <see cref="initCommunications"/></param>
        /// 
        /// <returns>Return Code (0 = success)</returns>
        [DllImport("communications.dll")]
        static extern int closeCommunications(IntPtr Handle);

        /// <summary>
        /// Writes one byte to Robot.
        /// </summary>
        /// <param name="Handle">Handle to the serial port returned by <see cref="initCommunications"/></param>
        /// <param name="ByteToWrite">The byte the needs to be written.</param>
        /// <returns>Return Code (0 = success)</returns>
        [DllImport("communications.dll")]
        static extern int writeToRobot(IntPtr Handle, Byte ByteToWrite);

        /// <summary>
        /// Read one byte from Robot.
        /// </summary>
        /// <param name="Handle">Handle to the serial port returned by <see cref="initCommunications"/></param>
        /// <param name="ByteRead">The byte was read from the robot.</param>
        /// <returns>Return Code (0 = success)</returns>
        [DllImport("communications.dll")]
        static extern int readFromRobot(IntPtr Handle, out Byte ByteRead);
    }
}
