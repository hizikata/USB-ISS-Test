using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace I2CIO_Test.Methods
{
    enum ISS_mode
    {
        IO_MODE = 0x00,
        IO_CHANGE = 0x10,
        I2C_S_20KHZ = 0x20,         // Software I2C (bit-bashed) modes
        I2C_S_50KHZ = 0x30,
        I2C_S_100KHZ = 0x40,
        I2C_S_400KHZ = 0x50,
        I2C_H_100KHZ = 0x60,        // Hardware I2C peripheral modes
        I2C_H_400KHZ = 0x70,
        I2C_H_1000KHZ = 0x80,
        SPI_MODE = 0x90,
        SERIAL = 0x01,
    };


    enum IssCmds
    {
        ISS_VER = 1,            // returns version num, 1 byte
        ISS_MODE,               // returns ACK, NACK, 1 byte
        GET_SER_NUM,

        I2C_SGL = 0x53,         // 0x53 Read/Write single byte for non-registered devices  
        I2C_AD0,                // 0x54 Read/Write multiple bytes for devices without internal address register 
        I2C_AD1,                // 0x55 Read/Write multiple bytes for 1 byte addressed devices  
        I2C_AD2,                // 0x56 Read/Write multiple bytes for 2 byte addressed devices 
        I2C_DIRECT,             // 0x57 Direct control of I2C start, stop, read, write.  
        ISS_CMD = 0x5A,         // 0x5A 
        SPI_IO = 0x61,          // 0x61 SPI I/O
        SERIAL_IO,              // 0x62
        SETPINS,                // 0x63 [SETPINS] [pin states]  
        GETPINS,                // 0x64 
        GETAD,                  // 0x65 [GETAD] [pin to convert]   
    };

    // I2C DIRECT commands
    enum I2Cdirect
    {
        I2CSRP = 0x00,          // Start/Stop Codes - 0x01=start, 0x02=restart, 0x03=stop, 0x04=nack  
        I2CSTART,               // send start sequence  
        I2CRESTART,             // send restart sequence   
        I2CSTOP,                // send stop sequence   
        I2CNACK,                // send NACK after next read  
        I2CREAD = 0x20,         // 0x20-0x2f, reads 1-16 bytes
        I2CWRITE = 0x30,        // 0x30-0x3f, writes next 1-16 bytes
    };
    enum Reason
    {
        DEVICE = 0x01,              // no ack from device			
        BUF_OVRFLOW,                // buffer overflow (>60)    
        RD_OVERFLOW,                // no room in buffer to read data  
        WR_UNDERFLOW,               // not enough data provided  
    };
    /// <summary>
    /// I2C数据传输类
    /// </summary>
    public class TransmitBase
    {
        /// <summary>
        /// 将指定byte数组数据写入设备中
        /// </summary>
        /// <param name="serBuf">要写入的byte 数组</param>
        /// <param name="write_bytes">数组的长度</param>
        /// <param name="USB_ISS">通信串口</param>
        private void iss_transmit(byte[] serBuf, byte write_bytes, SerialPort USB_ISS)
        {
            try
            {
                USB_ISS.Write(serBuf, 0, write_bytes);      // writes specified amount of SerBuf out on COM port
            }
            catch (Exception)
            {
                throw new Exception("串口写入失败!!");
            }
        }
        /// <summary>
        /// 从串口的输入缓冲区读取字节数据，写入指定的字节数组
        /// </summary>
        /// <param name="serBuf">要写入数据的byte数组</param>
        /// <param name="read_bytes">要读取的字节数</param>
        /// <param name="USB_ISS">指定通信窗口</param>
        private void iss_recieve(byte[] serBuf, byte read_bytes, SerialPort USB_ISS)
        {
            byte x;

            for (x = 0; x < read_bytes; x++)       // this will call the read function for the passed number times,  
            {                                      // this way it ensures each byte has been correctly recieved while  
                try                                // still using timeouts 
                {
                    USB_ISS.Read(serBuf, x, 1);   // retrieves 1 byte at a time and places in SerBuf at position x 
                }
                catch (Exception)                   // timeout or other error occured, set lost comms indicator 
                {
                    throw new Exception("串口读取失败!!");
                }
            }
            //？？
            byte[] serBufss = serBuf;
        }
        /// <summary>
        /// 读取A2H数据到指定字节数组
        /// </summary>
        /// <param name="SerBuf">指定字节数组</param>
        /// <param name="port">通信COM口</param>
        /// <param name="count">要读取的字节数</param>
        /// <param name="add">读取的开始地址</param>
        /// <returns></returns>
        public List<byte> MyI2C_ReadA2HByte(byte[] SerBuf, SerialPort port,byte add,byte count)  
        {
            #region 密码解锁
            //密码写入，都不变
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7E;                   // Device internal register 设备内部地址(a2 126位）
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x01;                   // The data bytes
            iss_transmit(SerBuf, 5, port);
            iss_recieve(SerBuf, 1, port);
            //USBISStextBox.Text += string.Format("write 126 1:{0:X02} \r\n", SerBuf[0]);

            //选择 Table-configure
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7f;                   // Device internal register 设备内部地址(a2 127位 写1）
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x01;                   // The data bytes
            iss_transmit(SerBuf, 5, port);
            iss_recieve(SerBuf, 1, port);
            #endregion
            #region 读取数据
            List<byte> data = new List<byte>();
            //读到A2 176位,读取两个字节的高低位温度，温度值 =((HEX2DEC(A2)*187.4/65536-53.7)+40)/2，单位度 
            SerBuf[0] = (byte)IssCmds.I2C_AD1;  //  Read/Write 1 byte addressed devices (the majority of devices will use this one)
            SerBuf[1] = 0xa3;  //选择A2H区域                  //  I2C address of EEPROM for reading，I2C数据区块，0xa1为Read A0; 0xa3为Read A2;0xb1为Read B0; 0xb3为Read B2;
            SerBuf[2] = add;                   //  Device internal register 设备内部地址  地址96,97(对应0x60,ox61)读取温度
            SerBuf[3] = count;                      // 读取的个数设定
            iss_transmit(SerBuf, 4, port);
            //iss_recieve(SerBuf, 1);          //  读的位数，目前一次性读1位
            iss_recieve(SerBuf, count, port);           //  读的位数，目前一次性读51位
            StringBuilder sb = new StringBuilder(20);
            for (int i = 0; i < count; i++)      //   i 的长度应该为iss_recieve里的read_bytes的长度
            {
                data.Add(SerBuf[i]);
            }
            #endregion
            #region 密码上锁
            //密码保存，都不变
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7E;                   // Device internal register 设备内部地址
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x00;                   // The data bytes
            iss_transmit(SerBuf, 5, port);
            iss_recieve(SerBuf, 1, port);
            return data;
            #endregion
          
        }
        /// <summary>
        /// 读取A0H数据
        /// </summary>
        /// <param name="SerBuf"></param>
        /// <param name="port"></param>
        /// <param name="add"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public string MyI2C_ReadA0HByte(byte[] SerBuf, SerialPort port, byte add, byte count)
        {
            #region 密码解锁
            //密码写入，都不变
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7E;                   // Device internal register 设备内部地址(a2 126位）
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x01;                   // The data bytes
            iss_transmit(SerBuf, 5, port);
            iss_recieve(SerBuf, 1, port);
            //USBISStextBox.Text += string.Format("write 126 1:{0:X02} \r\n", SerBuf[0]);

            //选择 Table-configure
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7f;                   // Device internal register 设备内部地址(a2 127位 写1）
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x01;                   // The data bytes
            iss_transmit(SerBuf, 5, port);
            iss_recieve(SerBuf, 1, port);
            #endregion
            //读到A2 176位,读取两个字节的高低位温度，温度值 =((HEX2DEC(A2)*187.4/65536-53.7)+40)/2，单位度 
            SerBuf[0] = (byte)IssCmds.I2C_AD1;  //  Read/Write 1 byte addressed devices (the majority of devices will use this one)
            SerBuf[1] = 0xa1;  //选择A0H区域                  //  I2C address of EEPROM for reading，I2C数据区块，0xa1为Read A0; 0xa3为Read A2;0xb1为Read B0; 0xb3为Read B2;
            SerBuf[2] = add;                   //  Device internal register 设备内部地址  地址96,97(对应0x60,ox61)读取温度
            SerBuf[3] = count;                      // 读取的个数设定
            iss_transmit(SerBuf, 4, port);
            //iss_recieve(SerBuf, 1);          //  读的位数，目前一次性读1位
            iss_recieve(SerBuf, count, port);           //  读的位数，目前一次性读51位
            StringBuilder sb = new StringBuilder(20);
            for (int i = 0; i < count; i++)      //   i 的长度应该为iss_recieve里的read_bytes的长度
            {
                string dddd = string.Empty;
                //每八个字节换行
                if ((i + 1) % 8 == 0 && i != 0) dddd = "\r\n";
                sb = sb.Append(string.Format("{0:X2} {1}", SerBuf[i], dddd));
            }


            #region 密码上锁
            //密码保存，都不变
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7E;                   // Device internal register 设备内部地址
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x00;                   // The data bytes
            iss_transmit(SerBuf, 5, port);
            iss_recieve(SerBuf, 1, port);
            return sb.ToString();
            #endregion

        }
        /// <summary>
        /// 读取低字节数据
        /// </summary>
        /// <param name="SerBuf"></param>
        /// <param name="port"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public string MyI2C_ReadLowByte(byte[] SerBuf, SerialPort port,byte count)
        {
            //int val, pins;
            StringBuilder sb = new StringBuilder();
            SerBuf[0] = (byte)IssCmds.I2C_AD1;  //  Read/Write 1 byte addressed devices (the majority of devices will use this one)
            SerBuf[1] = 0xa3;                   //  I2C address of EEPROM for reading，I2C数据区块，0xa1为Read A0; 0xa3为Read A2;0xb1为Read B0; 0xb3为Read B2;
            SerBuf[2] = 0x00;                   //  Device internal register 设备内部地址   A2区第126为地址为7E,改密码区
            SerBuf[3] = count;                 // 读取的个数设定
            //写入前后数组无变化
            iss_transmit(SerBuf, 4,port);
            //iss_recieve(SerBuf, 1);           //  读的位数，目前一次性读1位
            iss_recieve(SerBuf, count,port);           //  读的位数，目前一次性读51位
                                                     //  iss_recieve(SerBuf, 10);           //  读的位数，目前一次性读4位


            for (int i = 0; i < count; i++)      //   i 的长度应该为iss_recieve里的read_bytes的长度
            {
                string dddd = string.Empty;
                if ((i + 1) % 8 == 0 && i != 0) dddd = "\r\n";
                sb = sb.Append(string.Format("{0:X2} {1}", SerBuf[i], dddd));
            }
            return sb.ToString();
        }
        /// <summary>
        /// 检测端口初始化是否成功
        /// </summary>
        /// <param name="port"></param>
        /// <param name="SerBuf"></param>
        /// <returns></returns>
        public string IsPortReady(SerialPort port, byte[] SerBuf)
        {
            StringBuilder sb = new StringBuilder();
            SerBuf[0] = 0x5A;                            // USB_ISS Module Command   USB_ISS模块命令
            SerBuf[1] = (byte)IssCmds.ISS_VER;           // Get Revision
            this.iss_transmit(SerBuf, 2, port);
            this.iss_recieve(SerBuf, 3, port);                              // ID, Rev, Mode

            if (SerBuf[0] == 7)  // USB_ISS module ID
            {
                byte USBISS_found = SerBuf[1];                                           // and set the usb-iss found indicator   并设置了usb-iss发现指示器
                sb = sb.Append(string.Format("myFound USB-ISS Version {0}\r\n", USBISS_found));  //print the software version on screen
                SerBuf[0] = 0x5A;                            // USB_ISS Module Command
                SerBuf[1] = (byte)IssCmds.GET_SER_NUM;       // Get Revision
                this.iss_transmit(SerBuf, 2, port);
                //不是引用变量，为何输入参数数组会改变
                this.iss_recieve(SerBuf, 8, port);
                string temp1 = "";
                for (int x = 0; x < 8; x++) temp1 += char.ConvertFromUtf32(SerBuf[x]);
                sb.Append("mySerial # " + temp1 + "\r\n");
                return sb.ToString();
            }
            else return null;
        }
    }
}
