using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace I2CIO_Test.Model
{
    /// <summary>
    /// HP衰减器 8156A
    /// </summary>
    public class HP8156A:DeviceBase
    {
        
        /// <summary>
        /// HP8156A构造函数
        /// </summary>
        /// <param name="add"></param>
        public HP8156A(string add) : base(add)
        {
            DeviceName = "HP8156A";
        }


        /******
         * :INP:WAV 1490NM  //设置波长
         * :INP:OFFS 4.123dB //设置cal
         * :INP:ATT 10dB  // 设置衰减
         * :OUTP ON    //打开
         * :OUTP OFF    //关闭
         **/
    }
}
