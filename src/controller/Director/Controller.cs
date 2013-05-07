using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director
{
	public enum StatusByteCode : byte { 
		Unknown = 0x00,
		Forward = 0x46, 
		Stop = 0x53, 
		Left = 0x4c, 
		Right = 0x52, 
		Acknowledged = 0x06, 
		NotAcknowledged = 0x15,
		Enquiry = 0x05,
		MineDetected = 0x07,
		Done = 0x04
	};

	public class Controller
	{
		private StatusByteCode _receivedByte;
		
		public Controller()
		{
			Data.com.SerialDataEvent += com_SerialDataEvent;
		}

		private void com_SerialDataEvent(object sender, SerialDataEventArgs e)
		{
			uint i = 0;
			_receivedByte = (StatusByteCode)e.DataByte;

			if (_receivedByte == StatusByteCode.Enquiry)
			{
				//Robot asks for new directions
			}
		}
	}
}
