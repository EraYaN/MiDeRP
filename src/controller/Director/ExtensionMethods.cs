using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiDeRP
{
    public static class ExtensionMethods
    {
        public static StatusByteCode ToStatusByteCode(this Byte b)
        {
            StatusByteCode? sbc = (StatusByteCode)b;
            if (sbc == null)
            {
                return StatusByteCode.Unknown;
            }
            else
            {
				return (StatusByteCode)sbc;
            }
        }
    }
}
