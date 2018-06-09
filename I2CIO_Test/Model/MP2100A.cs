using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace I2CIO_Test.Model
{
    /// <summary>
    /// MP2100A眼图仪
    /// </summary>
    public class MP2100A : DeviceBase
    {
        /// <summary>
        /// MP2100A 眼图仪
        /// </summary>
        /// <param name="add"></param>
        public MP2100A(string add) : base(add)
        {
            DeviceName = "Anritsu MP2100A";
        }
        /// <summary>
        /// 获取ER
        /// </summary>
        /// <returns></returns>
        public string GetER()
        {
            Status = visa32.viPrintf(Vi, ":FETC:AMPL:EXTR?\n");
            CheckStatus(Vi, Status);
            return ReadCommand();

        }
        /// <summary>
        /// 获取Crossing
        /// </summary>
        /// <returns></returns>
        public string GetCrossing()
        {
            Status = visa32.viPrintf(Vi, ":FETC:AMPL:CROS?\n");
            CheckStatus(Vi, Status);
            return ReadCommand();
        }
        public string GetErrorRate()
        {
            Status = visa32.viPrintf(Vi, "ER?\n");
            CheckStatus(Vi, Status);
            return ReadCommand();
        }

    }
}
