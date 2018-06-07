using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace I2CIO_Test.Model
{
    /// <summary>
    /// 电源供应器Keithley
    /// </summary>
    public class Keithley:DeviceBase
    {
        /// <summary>
        /// Keithley 构造函数
        /// </summary>
        /// <param name="add"></param>
        public Keithley(string add) : base(add)
        {
            DeviceName = "Keithley";
        }

        
    }
}
