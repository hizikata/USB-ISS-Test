using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace I2CIO_Test.Model
{
    /// <summary>
    /// GPIB设备基类
    /// </summary>
    public class DeviceBase : IDeviceMethods,IDisposable
    {
        #region Fields
        /// <summary>
        /// 设备名称
        /// </summary>
        protected string DeviceName;
        /// <summary>
        /// 设备连接字符串
        /// </summary>
        protected readonly string DeviceConn;
        protected int DefRM, Status, Vi;
        #endregion
        #region Constructors
        /// <summary>
        /// DevieBase 构造函数
        /// </summary>
        /// <param name="add"></param>
        public DeviceBase(string add)
        {
            DeviceConn = string.Format(@"GPIB0::{0}::INSTR", add);
            Status = visa32.viOpenDefaultRM(out DefRM);
            Status = visa32.viOpen(DefRM, DeviceConn, visa32.VI_NO_LOCK, visa32.VI_TMO_IMMEDIATE, out Vi);
            CheckStatus(Vi, Status);


        }


        #endregion
        #region Methods
        /// <summary>
        /// 验证SCPI语句是否执行成功
        /// </summary>
        /// <param name="vi"></param>
        /// <param name="status"></param>
        protected void CheckStatus(int vi, int status)
        {
            if (status != visa32.VI_SUCCESS)
            {
                StringBuilder err = new StringBuilder(256);
                visa32.viStatusDesc(vi, status, err);
                throw new Exception(err.ToString());
            }
        }
        #endregion
        #region IDeviceMethods
        /// <summary>
        /// 重启设备
        /// </summary>
        /// <returns></returns>
        public bool Reset()
        {
            Status = visa32.viPrintf(Vi, "*RST\n");
            CheckStatus(Vi, Status);
            return true;
        }
        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <returns></returns>
        public string GetIdn()
        {
            Status = visa32.viPrintf(Vi, "*IDN?\n");
            CheckStatus(Vi, Status);
            Status = visa32.viRead(Vi, out string result, 100);
            CheckStatus(Vi, Status);
            return result;
        }
        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <returns></returns>
        public virtual bool Initialize()
        {
            return true;
        }
        /// <summary>
        /// 读取指令信息
        /// </summary>
        /// <returns></returns>
        public virtual string ReadCommand()
        {
            Status = visa32.viRead(Vi, out string result, 100);
            CheckStatus(Vi, Status);
            return result;

        }
        /// <summary>
        /// 向仪器发送指令
        /// </summary>
        /// <returns></returns>
        public virtual bool WriteCommand(string command)
        {
            string strCmd = command + "\n";
            Status = visa32.viPrintf(Vi, strCmd);
            CheckStatus(Vi, Status);
            return true;
        }
        /// <summary>
        /// 释放VISA资源
        /// </summary>
        public void Dispose()
        {
            //关闭GPIB 通信
            if (DefRM != 0)
                visa32.viClose(DefRM);
            if (Vi != 0)
                visa32.viClose(Vi);
        }
        #endregion
    }
}
