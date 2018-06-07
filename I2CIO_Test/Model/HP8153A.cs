using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace I2CIO_Test.Model
{
    /// <summary>
    /// HP8153A光功率计
    /// </summary>
    public class HP8153A:DeviceBase
    {
        /// <summary>
        /// hp8153A构造函数
        /// </summary>
        /// <param name="add"></param>
        public HP8153A(string add):base(add)
        {
            DeviceName = "Hp8153A";
        }
        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <returns></returns>
        public override bool Initialize()
        {
            //设置自动量程
            Status = visa32.viPrintf(Vi, "SENS:POW:RANG:AUTO ON\n");
            CheckStatus(Vi,Status);
            //设置测量单位dBm
            Status = visa32.viPrintf(Vi, "SENS:POW:UNIT DBM\n");
            CheckStatus(Vi, Status);
            //设置连续测量
            Status = visa32.viPrintf(Vi, "INIT:CONT ON\n");
            return true;
        }
        /*
         * READ1:POW? //读取数据
         * SENS:POW:WAVE 1310NM //设置波长
         * SENS:POW:WAVE?  //查询当前波长
         * SENS:POW:RANG:AUTO ON
         * SENS:POW:UNIT DBM/W
         * SENS:POW:UNIT? DBM->0/W->1
         * INIT:CONT ON/OFF
         * INIT:CONT?
         * SENS:POW:ATIME 200MS
         * SENS:POW:ATIME?
         * SENS:CORR:LOSS:INP:MAGN 10DB
         * SENS:CORR:LOSS:INP:MAGN?
         * DISP:STAT ON/OFF
         * **/
    }
}
