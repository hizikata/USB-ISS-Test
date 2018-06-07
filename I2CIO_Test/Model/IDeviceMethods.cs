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
        string ReadCommand();
        bool WriteCommand(string command);
       
    }
}
