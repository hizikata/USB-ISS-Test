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
        /// <summary>
        /// 设置波长
        /// </summary>
        /// <param name="waveLength">波长(形如:1310，单位nm)</param>
        public void SetWaveLength(string waveLength)
        {
            Status = visa32.viPrintf(Vi, ":INP:WAV " + waveLength + "NM\n");
            CheckStatus(Vi, Status);
        }
        /// <summary>
        /// 设置cal
        /// </summary>
        /// <param name="cal">cal</param>
        public void SetCal(string cal)
        {
            Status = visa32.viPrintf(Vi, ":INP:OFFS " + cal + "dB\n");
            CheckStatus(Vi, Status);
        }
        /// <summary>
        /// 设置衰减
        /// </summary>
        /// <param name="att">衰减</param>
        public void SetAtt(string att)
        {
            Status = visa32.viPrintf(Vi, ":INP:ATT " + att + "dB\n");
            CheckStatus(Vi, Status);
        }
        /// <summary>
        /// 打开设备
        /// </summary>
        public void Open()
        {
            Status = visa32.viPrintf(Vi, "OUTP ON\n");
            CheckStatus(Vi, Status);
        }
        /// <summary>
        /// 关闭设备
        /// </summary>
        public void Close()
        {
            Status = visa32.viPrintf(Vi, "OUTP OFF\n");
            CheckStatus(Vi, Status);
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
