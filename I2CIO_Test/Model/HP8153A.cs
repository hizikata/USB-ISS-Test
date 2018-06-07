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
        /// <summary>
        /// 设置波长
        /// </summary>
        /// <param name="waveLength">波长(形如:1310,单位nm)</param>
        public void SetWaveLength(string waveLength)
        {
            
            Status = visa32.viPrintf(Vi, "SENS:POW:WAVE "+waveLength+"NM\n");
            CheckStatus(Vi, Status);              
        }
        /// <summary>
        /// 设置cal值
        /// </summary>
        /// <param name="calibration">cal值(形如:10 单位dB)</param>
        public void SetCalibration(string calibration)
        {
            Status = visa32.viPrintf(Vi, "SENS:CORR:LOSS:INP:MAGN " + calibration + "DB\n");
        }
        /// <summary>
        /// 设置平均时间
        /// </summary>
        /// <param name="time">时间(形如:200,单位ms)</param>
        public void SetATime(string time)
        {
            Status = visa32.viPrintf(Vi, "SENS:POW:ATIME " + time + "MS\n");
            CheckStatus(Vi, Status);
        }
        /// <summary>
        /// 读取光功率计数据
        /// </summary>
        public string ReadData()
        {
            Status = visa32.viPrintf(Vi, "READ1:POW?\n");
            CheckStatus(Vi, Status);
            return ReadCommand();
        }
        /*
         * READ1:POW? //读取数据
         * SENS:POW:WAVE 1310NM //设置波长
         * SENS:POW:WAVE?  //查询当前波长
         * SENS:POW:RANG:AUTO ON //设置自动量程
         * SENS:POW:UNIT DBM/W //设置数据单位
         * SENS:POW:UNIT? DBM->0/W->1 //查询数据单位
         * INIT:CONT ON/OFF //设置连续测量
         * INIT:CONT?  //查询连续测量
         * SENS:POW:ATIME 200MS //设置平均时间
         * SENS:POW:ATIME?
         * SENS:CORR:LOSS:INP:MAGN 10DB //设置cal
         * SENS:CORR:LOSS:INP:MAGN?
         * DISP:STAT ON/OFF  //设置/关闭 显示
         * **/
    }
}
