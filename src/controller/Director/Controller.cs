using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director
{
	public enum ByteCodes : byte { forward = 0x46, stop = 0x53, left = 0x4c, right = 0x52 };

	public class Controller
	{
		private byte _receivedByte;
		
		public void Controller()
		{
			Data.com.SerialDataEvent += com_SerialDataEvent;
		}

		private void com_SerialDataEvent(object sender, SerialDataEventArgs e)
		{
			//throw new NotImplementedException();
			_receivedByte = e.DataByte;
		}
	}
}
