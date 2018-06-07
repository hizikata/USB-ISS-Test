using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace I2CIO_Test.Model
{
    interface IDeviceMethods
    {
        bool Reset();
        string Identification();
        bool Initialize();
        bool Read();
        bool Write();
       
    }
}
