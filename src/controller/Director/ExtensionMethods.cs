using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director
{
    class ExtensionMethods
    {
        public static StatusByteCode ToStatusByteCode(this Byte b)
        {
            var sbc = (StatusByteCode)b;
            if (sbc == null)
            {
                return ToStatusByteCode.Unknown;
            }
            else
            {
                return sbc;
            }
        }
    }
}
