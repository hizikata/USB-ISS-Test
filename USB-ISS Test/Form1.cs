using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;          // added to include serial ports
using System.IO;


enum ISS_mode { 
    IO_MODE =       0x00,
    IO_CHANGE =     0x10,
    I2C_S_20KHZ =   0x20, 		// Software I2C (bit-bashed) modes
    I2C_S_50KHZ =   0x30,
	I2C_S_100KHZ =  0x40,
	I2C_S_400KHZ =  0x50,
	I2C_H_100KHZ =  0x60,		// Hardware I2C peripheral modes
	I2C_H_400KHZ =  0x70,
	I2C_H_1000KHZ = 0x80,
	SPI_MODE =      0x90,
	SERIAL =        0x01,				
};


enum IssCmds {
    ISS_VER = 1, 			// returns version num, 1 byte
    ISS_MODE,				// returns ACK, NACK, 1 byte
    GET_SER_NUM,

    I2C_SGL = 0x53,		    // 0x53 Read/Write single byte for non-registered devices  
    I2C_AD0,				// 0x54 Read/Write multiple bytes for devices without internal address register 
    I2C_AD1,				// 0x55 Read/Write multiple bytes for 1 byte addressed devices  
    I2C_AD2,				// 0x56 Read/Write multiple bytes for 2 byte addressed devices 
    I2C_DIRECT,				// 0x57 Direct control of I2C start, stop, read, write.  
    ISS_CMD = 0x5A,		    // 0x5A 
    SPI_IO = 0x61,			// 0x61 SPI I/O
    SERIAL_IO,              // 0x62
    SETPINS,				// 0x63 [SETPINS] [pin states]  
    GETPINS,				// 0x64 
    GETAD,					// 0x65 [GETAD] [pin to convert]   
};

// I2C DIRECT commands
enum I2Cdirect
{
    I2CSRP = 0x00,			// Start/Stop Codes - 0x01=start, 0x02=restart, 0x03=stop, 0x04=nack  
    I2CSTART,				// send start sequence  
    I2CRESTART,				// send restart sequence   
    I2CSTOP,				// send stop sequence   
    I2CNACK,				// send NACK after next read  
    I2CREAD = 0x20,		    // 0x20-0x2f, reads 1-16 bytes
    I2CWRITE = 0x30,		// 0x30-0x3f, writes next 1-16 bytes
};
// return from I2C_DIRECT is:  从I2C_DIRECT返回:
// [(ACK] [Read Cnt] [Data1] Data2] ... [DataN]
// or
// [(NACK] [Reason]
enum Reason
{
    DEVICE = 0x01,				// no ack from device			
    BUF_OVRFLOW,				// buffer overflow (>60)    
    RD_OVERFLOW,				// no room in buffer to read data  
    WR_UNDERFLOW,				// not enough data provided  
};


namespace USB_ISS_Test
{
    public partial class Form1 : Form
    {
        #region Fields
        byte ReadByte = 64;
        #endregion
        static SerialPort USB_ISS;
        //int Mode = 0xff, IOtypeOld, IOtypeNew;
        //int Mode = 0xff;
        byte USBISS_found = 0;
        byte[] SerBuf = new byte[200];
        //Int32 counter = 0;
        //byte[] myI2CBuf = new byte[200];    //新增加，2018-2-8
        //Int32 myTimerCNT = 0;  //新增加，2018-2-8
        
        public Form1()
        {
            InitializeComponent();
            USB_ISS = new SerialPort();

            foreach (string s in SerialPort.GetPortNames())        // Get a list of available serial port names.
            {
                USBISS_comboBox.Items.Add(s);                      // places each "COMx" name into combobox
            }
            USBISS_comboBox.Text = "Select COM Port";              // printed only at first execution in the combobox
            //Mode_comboBox.Text = "Select Mode";
            //Mode_comboBox.Items.Add("IO_Mode");                     // 0
            //Mode_comboBox.Items.Add("I2C_S_20KHZ+IO");              // 1
            //Mode_comboBox.Items.Add("I2C_S_20KHZ+Serial");          // 2
            //Mode_comboBox.Items.Add("I2C_S_50KHZ+IO");              // 3
            //Mode_comboBox.Items.Add("I2C_S_50KHZ+Serial");          // 4
            //Mode_comboBox.Items.Add("I2C_S_100KHZ+IO");             // 5
            //Mode_comboBox.Items.Add("I2C_S_100KHZ+Serial");         // 6
            //Mode_comboBox.Items.Add("I2C_S_400KHZ+IO");             // 7
            //Mode_comboBox.Items.Add("I2C_S_400KHZ+Serial");         // 8
            //Mode_comboBox.Items.Add("I2C_H_100KHZ+IO");             // 9
            //Mode_comboBox.Items.Add("I2C_H_100KHZ+Serial");         // 10
            //Mode_comboBox.Items.Add("I2C_H_400KHZ+IO");             // 11
            //Mode_comboBox.Items.Add("I2C_H_400KHZ+Serial");         // 12
            //Mode_comboBox.Items.Add("I2C_H_1000KHZ+IO");            // 13
            //Mode_comboBox.Items.Add("I2C_H_1000KHZ+Serial");        // 14
            //Mode_comboBox.Items.Add("SPI_Mode");                    // 15
            //Mode_comboBox.Items.Add("IO+Serial");                   // 16

            //Mode_comboBox.Enabled = false;

           // doI2C_IO();  //新增加， 2018-2-8

        }
        
        //原来程序OK，保存好， 2018-2-8

        /*
    private void USBISS_comboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        int x;

        USB_ISS.Close();                            // close any existing handle  关闭任何现有的处理
        USB_ISS.PortName = USBISS_comboBox.Text;    // retrieves "COMx" from selection in combo box
        USB_ISS.Parity = 0;
        USB_ISS.BaudRate = 19200;
        USB_ISS.StopBits = StopBits.Two;
        USB_ISS.DataBits = 8;
        USB_ISS.ReadTimeout = 500;
        USB_ISS.WriteTimeout = 500;
        USB_ISS.Open();

        SerBuf[0] = 0x5A;                            // USB_ISS Module Command   USB_ISS模块命令
        SerBuf[1] = (byte)IssCmds.ISS_VER;           // Get Revision
        iss_transmit(2);
        iss_recieve(3);                              // ID, Rev, Mode

        if (SerBuf[0] == 7)  // USB_ISS module ID
        {
            USBISS_found = SerBuf[1];                                           // and set the usb-iss found indicator  并设置了usb-iss发现指示器
            USBISStextBox.Text = string.Format("Found USB-ISS Version {0}\r\n", USBISS_found);  //print the software version on screen
            SerBuf[0] = 0x5A;                            // USB_ISS Module Command
            SerBuf[1] = (byte)IssCmds.GET_SER_NUM;       // Get Revision
            iss_transmit(2);
            iss_recieve(8);
            string temp1 = "";
            for (x = 0; x < 8; x++) temp1 += char.ConvertFromUtf32(SerBuf[x]);
            USBISStextBox.Text += "Serial #" + temp1;

            Mode_comboBox.Enabled = true;
            timer1.Enabled = true;
        }
    }

    */

        //新修改， 2018-2-8
        private void USBISS_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int x;

                USB_ISS.Close();                            // close any existing handle    关闭任何现有的处理
                USB_ISS.PortName = USBISS_comboBox.Text;    // retrieves "COMx" from selection in combo box
                USB_ISS.Parity = 0;
                USB_ISS.BaudRate = 19200;
                USB_ISS.StopBits = StopBits.Two;
                USB_ISS.DataBits = 8;
                USB_ISS.ReadTimeout = 100;
                USB_ISS.WriteTimeout = 100;
                USB_ISS.Open();

                SerBuf[0] = 0x5A;                            // USB_ISS Module Command   USB_ISS模块命令
                SerBuf[1] = (byte)IssCmds.ISS_VER;           // Get Revision
                iss_transmit(SerBuf, 2);
                iss_recieve(SerBuf, 3);                              // ID, Rev, Mode

                if (SerBuf[0] == 7)  // USB_ISS module ID
                {
                    USBISS_found = SerBuf[1];                                           // and set the usb-iss found indicator   并设置了usb-iss发现指示器
                    USBISStextBox.Text = string.Format("myFound USB-ISS Version {0}\r\n", USBISS_found);  //print the software version on screen
                    SerBuf[0] = 0x5A;                            // USB_ISS Module Command
                    SerBuf[1] = (byte)IssCmds.GET_SER_NUM;       // Get Revision
                    iss_transmit(SerBuf, 2);
                    //不是引用变量，为何输入参数数组会改变
                    iss_recieve(SerBuf, 8);
                    string temp1 = "";
                    for (x = 0; x < 8; x++) temp1 += char.ConvertFromUtf32(SerBuf[x]);
                    USBISStextBox.Text += "mySerial # " + temp1 + "\r\n";

                    // Mode_comboBox.Enabled = true;
                    // timer1.Enabled = true;
                    // MyI2C_WriteTest();
                    // MyI2C_Read(); 


                    //doI2C_IO();

                    // //MyI2C_Read();

                    //doI2C_IOStep1();
                    //doI2C_IOStep2();
                    //doI2C_IOStep3();

                    //  //MyI2C_WriteTest();

                    //  //MyI2C_Read();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示");
            }
            
        }
        /// <summary>
        /// 将指定byte数组数据写入设备中
        /// </summary>
        /// <param name="serBuf">要写入的byte 数组</param>
        /// <param name="write_bytes">数组的长度</param>
        private void iss_transmit(byte[] serBuf, byte write_bytes)
        {
            try
            {
                USB_ISS.Write(serBuf, 0, write_bytes);      // writes specified amount of SerBuf out on COM port
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// 从串口的输入缓冲区读取字节数据，写入指定的字节数组
        /// </summary>
        /// <param name="serBuf">要写入数据的byte数组</param>
        /// <param name="read_bytes">要读取的字节数</param>
        private void iss_recieve(byte[] serBuf, byte read_bytes)
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
                    SerBuf[x] = 0x88;
                }
            }
            //？？
            byte[] serBufss = serBuf;
        }
  
        /*   //原来程序OK，保存好
         private void timer1_Tick_1(object sender, EventArgs e)
         {
             switch (Mode)
             {
                 case 0:
                     //  doIO();
                     break;

                 case 1:         // I2C + I/O modes
                 case 3:
                 case 5:
                 case 7:
                 case 9:
                 case 11:
                 case 13:
                  //   doI2C_IO();
                     break;

                 case 2:         // I2C + Serial modes
                 case 4:
                 case 6:
                 case 8:
                 case 10:
                 case 12:
                 case 14:
                   //  doI2C_Ser();
                     break;
                 case 15:
                   //  doSPI();
                     break;
                 case 16:
                   //  doIO_Ser();
                     break;
             }

         }
         */


        //新修改，2018-2-8
         private void timer1_Tick_1(object sender, EventArgs e)
         {
             //myTimerCNT++;

             //if (myTimerCNT == 50)   //开始读数的等待时间
             //{
             //    //doI2C_IO();
             //    MyI2C_Read();   //自定义读取Transceiver函数
             //}
             //switch (Mode)
             //{
             //    case 0:              
             //         //  doIO(); 
             //         break;
             //    case 1:   // I2C + I/O modes
             //    case 3:
             //    case 5:
             //    case 7:
             //    case 9:
             //    case 11:
             //    case 13:
             //        //  doI2C_IO();
             //       {
             //         //  doI2C_IO();
             //           Mode = 0;
             //       }
             //       break;
             //    case 2:         // I2C + Serial modes
             //    case 4:
             //    case 6:
             //    case 8:
             //    case 10:
             //    case 12:
             //    case 14:
             //      //  doI2C_Ser();
             //        break;
             //    case 15:
             //      //  doSPI();
             //        break;
             //    case 16:
             //      //  doIO_Ser();
             //        break;
             //}
         }
     
        //private void doIO_Ser()
        //{
        //    byte cnt, x;
        //    SerBuf[0] = (byte)IssCmds.SERIAL_IO;
        //    SerBuf[1] = 0x00;
        //    SerBuf[2] = 0x55;
        //    SerBuf[3] = (byte)counter++; ;
        //    iss_transmit(4);
        //    USBISStextBox.Text = string.Format("Writing to Serial Port {0:X02} {1:X02} {2:X02}\r\n", SerBuf[1], SerBuf[2], SerBuf[3]);
        //    iss_recieve(3);
        //    USBISStextBox.Text += string.Format("Read from Serial Port [{0:X02}{1:X02}{2:X02}]", SerBuf[0], SerBuf[1], SerBuf[2]);
        //    cnt = SerBuf[2];
        //    if (cnt > 0)
        //    {
        //        iss_recieve(cnt);
        //        for (x = 0; x < cnt; x++)
        //        {
        //            USBISStextBox.Text += string.Format(" {0:X02}", SerBuf[x]);
        //        }
        //    }
        //    USBISStextBox.Text += string.Format("\r\n");
        //    if (IOtypeNew != IOtypeOld)
        //    {
        //        SerBuf[0] = 0x5A;                            // USB_ISS Module Command
        //        SerBuf[1] = (byte)IssCmds.ISS_MODE;          // Set Mode
        //        SerBuf[2] = (byte)ISS_mode.IO_CHANGE;
        //        IOtypeOld = IOtypeNew;
        //        SerBuf[3] = (byte)IOtypeNew;                 // input mode
        //        iss_transmit(4);
        //        iss_recieve(1);

        //        SerBuf[0] = (byte)IssCmds.SETPINS;           // set any outputs
        //        SerBuf[1] = 0;

        //        if (comboBox3.SelectedIndex == 1) SerBuf[1] |= 0x04;
        //        if (comboBox4.SelectedIndex == 1) SerBuf[1] |= 0x08;
        //        iss_transmit(4);
        //        iss_recieve(1);
        //    }
        //    else
        //    {
        //        int val, pins;
        //        SerBuf[0] = (byte)IssCmds.GETPINS;          // Get Inputs/Outputs
        //        iss_transmit(1);
        //        iss_recieve(1);
        //        pins = SerBuf[0];
        //        if ((IOtypeOld & 0x30) == 0x30)
        //        {
        //            SerBuf[0] = (byte)IssCmds.GETAD;
        //            SerBuf[1] = 3;                          // chnl 3
        //            iss_transmit(2);
        //            iss_recieve(2);
        //            val = (SerBuf[0] << 8) + SerBuf[1];
        //            USBISStextBox.Text += string.Format("Chnl3 = {0}\r\n", val);
        //        }
        //        else
        //        {
        //            if ((pins & 0x04) == 0x04) USBISStextBox.Text += string.Format("Chnl3 = High\r\n");
        //            else USBISStextBox.Text += string.Format("Chnl3 = Low\r\n");
        //        }
        //        if ((IOtypeOld & 0xc0) == 0xc0)
        //        {
        //            SerBuf[0] = (byte)IssCmds.GETAD;
        //            SerBuf[1] = 4;                          // chnl 4
        //            iss_transmit(2);
        //            iss_recieve(2);
        //            val = (SerBuf[0] << 8) + SerBuf[1];
        //            USBISStextBox.Text += string.Format("Chnl4 = {0}\r\n", val);
        //        }
        //        else
        //        {
        //            if ((pins & 0x08) == 0x08) USBISStextBox.Text += string.Format("Chnl4 = High\r\n");
        //            else USBISStextBox.Text += string.Format("Chnl4 = Low\r\n");
        //        }
        //    }
        //}

        private void doI2C_Ser()
        {
        //    byte cnt, x;

        //    SerBuf[0] = (byte)IssCmds.I2C_DIRECT;
        //    SerBuf[1] = (byte)I2Cdirect.I2CSTART;
        //    SerBuf[2] = (byte)I2Cdirect.I2CWRITE+2;       // write 2+1=3 bytes
        //    SerBuf[3] = 0xa0;
        //    SerBuf[4] = 0x00;
        //    SerBuf[5] = 0x00;
        //    SerBuf[6] = (byte)I2Cdirect.I2CRESTART;
        //    SerBuf[7] = (byte)I2Cdirect.I2CWRITE+0;         // write 0+1=1 bytes
        //    SerBuf[8] = 0xa1;
        //    SerBuf[9] = (byte)I2Cdirect.I2CREAD + 2;        // read 2+1=3 bytes
        //    SerBuf[10] = (byte)I2Cdirect.I2CNACK;
        //    SerBuf[11] = (byte)I2Cdirect.I2CREAD + 0;        // read 0+1=1 byte
        //    SerBuf[12] = (byte)I2Cdirect.I2CSTOP;
        //    SerBuf[13] = (byte)I2Cdirect.I2CSTART;
        //    SerBuf[14] = (byte)I2Cdirect.I2CWRITE + 6;       // write 6+1=7 bytes
        //    SerBuf[15] = 0xa0;
        //    SerBuf[16] = 0x00;
        //    SerBuf[17] = 0x00;
        //    SerBuf[18] = (byte)(counter >> 24);
        //    SerBuf[19] = (byte)(counter >> 16);
        //    SerBuf[20] = (byte)(counter >> 8);
        //    SerBuf[21] = (byte)(counter >> 0);
        //    SerBuf[22] = (byte)I2Cdirect.I2CSTOP;
        //    iss_transmit(23);
        //    iss_recieve(2);
        //    if (SerBuf[0] == 0xff && SerBuf[1] == 4)
        //    {
        //        iss_recieve(4);
        //        USBISStextBox.Text = string.Format("Reading I2C EEPROM type 24LC256 {0:X02}{1:X02}{2:X02}{3:X02}\r\n", SerBuf[0], SerBuf[1], SerBuf[2], SerBuf[3]);  //print the software version on screen
        //    }
        //    else
        //    {
        //        USBISStextBox.Text = string.Format("Error Reading I2C EEPROM type 24LC256\r\n");  //print the software version on screen
        //    }
        //    USBISStextBox.Text += string.Format("Writing to I2C EEPROM type 24LC256 {0:X02}{1:X02}{2:X02}{3:X02}\r\n", SerBuf[18], SerBuf[19], SerBuf[20], SerBuf[21]);  //print the software version on screen

        //    counter++;

        //    SerBuf[0] = (byte)IssCmds.SERIAL_IO;
        //    SerBuf[1] = 0x00;
        //    SerBuf[2] = 0x55;
        //    SerBuf[3] = (byte)counter; ;
        //    iss_transmit(4);
        //    USBISStextBox.Text += string.Format("Writing to Serial Port {0:X02} {1:X02} {2:X02}\r\n", SerBuf[1], SerBuf[2], SerBuf[3] );
        //    iss_recieve(3);
        //    USBISStextBox.Text += string.Format("Read from Serial Port [{0:X02}{1:X02}{2:X02}]", SerBuf[0], SerBuf[1], SerBuf[2]);
        //    cnt = SerBuf[2];
        //    if(cnt>0) 
        //      {
        //        iss_recieve(cnt);
        //        for(x=0; x<cnt; x++) {
        //            USBISStextBox.Text += string.Format(" {0:X02}", SerBuf[x]);
        //        }
        //    }

        }


        //该程序读取A0/A2、B0/B2读取数据
        private void MyI2C_Read()
        {
            //int val, pins;
            StringBuilder sb = new StringBuilder();
            SerBuf[0] = (byte)IssCmds.I2C_AD1;  //  Read/Write 1 byte addressed devices (the majority of devices will use this one)
            SerBuf[1] = 0xa3;                   //  I2C address of EEPROM for reading，I2C数据区块，0xa1为Read A0; 0xa3为Read A2;0xb1为Read B0; 0xb3为Read B2;
            SerBuf[2] = 0x00;                   //  Device internal register 设备内部地址   A2区第126为地址为7E,改密码区
            SerBuf[3] = ReadByte;                 // 读取的个数设定
            //写入前后数组无变化
            iss_transmit(SerBuf, 4);
            //iss_recieve(SerBuf, 1);           //  读的位数，目前一次性读1位
            iss_recieve(SerBuf, ReadByte);           //  读的位数，目前一次性读51位
           //  iss_recieve(SerBuf, 10);           //  读的位数，目前一次性读4位


            for (int i = 0; i < ReadByte; i++)      //   i 的长度应该为iss_recieve里的read_bytes的长度
            {
                string dddd = string.Empty;
                if ((i + 1) % 8 == 0 && i != 0) dddd = "\r\n";
                sb = sb.Append(string.Format("{0:X2} {1}", SerBuf[i], dddd));            
            }
            USBISStextBox.Text = sb.ToString();
           
        }


        ////该程序写入A0/A2、B0/B2数据，写入数据时，必须将模块a2 126为写 1 ，其他才可以动作，写完后再将a2 126为写 0，数据才保存！！		
        /// <summary>
        /// //----------------------------Step6:write A2 Data------
            // Set:DDM a2 126 1    		//密码写1
            // Set:DDM a2 0~95 Date    	//0h~5Fh写固定信息，CHA Device："A2"、CHB Device："B2"
            // Set:DDM a2 126 0    		//密码写0、存储设定值
            // Read:DDM a2 0 96			//读取0h~5Fh地址区数据是否写对
        /// </summary>
        private void MyI2C_WriteTest()      //写入程序
        {
       
            //密码写入，都不变
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7E;                   // Device internal register 设备内部地址(a2 126位）
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x01;                   // The data bytes
            iss_transmit(SerBuf, 5);
            iss_recieve(SerBuf, 1);
            //USBISStextBox.Text += string.Format("write 126 1:{0:X02} \r\n", SerBuf[0]);

            //写入内容，根据地址与数据进行写入
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing:   0xa0为Write A0; 0xa2为Write A2;0xb0为Write B0; 0xb2为Write B2;
            SerBuf[2] = 0x00;                   // Device internal register 设备内部地址   写入时地址
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x36;                   // The data bytes
            iss_transmit(SerBuf, 5);
            iss_recieve(SerBuf, 1);
            //USBISStextBox.Text += string.Format("write 00 2:{0:X02} \r\n", SerBuf[0]);

            //密码保存，都不变
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7E;                   // Device internal register 设备内部地址
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x00;                   // The data bytes
            iss_transmit(SerBuf, 5);
            iss_recieve(SerBuf, 1);
            USBISStextBox.Text += string.Format("write 126 0:{0:X02} \r\n", SerBuf[0]);
        }

        private void MyI2C_ReadFlashTimes()      //读取模块烧写次数，剩下的次数为7减一下。
        {
            //密码写入，都不变
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7E;                   // Device internal register 设备内部地址(a2 126位）
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x01;                   // The data bytes
            iss_transmit(SerBuf, 5);
            iss_recieve(SerBuf, 1);
            //USBISStextBox.Text += string.Format("write 126 1:{0:X02} \r\n", SerBuf[0]);

            //选择 Table-configure
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7f;                   // Device internal register 设备内部地址(a2 127位 写1）
            SerBuf[3] = 0x01;                   // Number of data bytes 字节数设置为1
            SerBuf[4] = 0x01;                   // The data bytes  字节值设置为1 解锁
            iss_transmit(SerBuf, 5);
            iss_recieve(SerBuf, 1);

            //读到A2 197位,为烧写次数
            SerBuf[0] = (byte)IssCmds.I2C_AD1;  //  Read/Write 1 byte addressed devices (the majority of devices will use this one)
            SerBuf[1] = 0xa3;                   //  I2C address of EEPROM for reading，I2C数据区块，0xa1为Read A0; 0xa3为Read A2;0xb1为Read B0; 0xb3为Read B2;
            SerBuf[2] = 0xc5;                   //  Device internal register 设备内部地址   A2区第197为地址为C5,烧录次数读取
            SerBuf[3] = 1;                      // 读取的个数设定
            iss_transmit(SerBuf, 4);
            //iss_recieve(SerBuf, 1);          //  读的位数，目前一次性读1位
            iss_recieve(SerBuf, 1);           //  读的位数，目前一次性读51位
            USBISStextBox.Text += string.Format("烧写次数为：{0:X02} \r\n", SerBuf[0]);

            //密码保存，都不变
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7E;                   // Device internal register 设备内部地址
            SerBuf[3] = 0x01;                   // Number of data bytes 字节数设置为1
            SerBuf[4] = 0x00;                   // The data bytes 字节数值设置为0  上锁
            iss_transmit(SerBuf, 5);
            iss_recieve(SerBuf, 1);
            //USBISStextBox.Text += string.Format("write 126 0:{0:X02} \r\n", SerBuf[0]);
        }

        private void MyI2C_ReadLUT()      //读取模块烧写次数，剩下的次数为7减一下。
        {
            //密码写入，都不变
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7E;                   // Device internal register 设备内部地址(a2 126位）
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x01;                   // The data bytes
            iss_transmit(SerBuf, 5);
            iss_recieve(SerBuf, 1);
            //USBISStextBox.Text += string.Format("write 126 1:{0:X02} \r\n", SerBuf[0]);

            //选择 Table-configure
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7f;                   // Device internal register 设备内部地址(a2 127位 写1）
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x01;                   // The data bytes
            iss_transmit(SerBuf, 5);
            iss_recieve(SerBuf, 1);

            //读到A2 176位,读取两个字节的高低位温度，温度值 =((HEX2DEC(A2)*187.4/65536-53.7)+40)/2，单位度 
            SerBuf[0] = (byte)IssCmds.I2C_AD1;  //  Read/Write 1 byte addressed devices (the majority of devices will use this one)
            SerBuf[1] = 0xa3;                   //  I2C address of EEPROM for reading，I2C数据区块，0xa1为Read A0; 0xa3为Read A2;0xb1为Read B0; 0xb3为Read B2;
            SerBuf[2] = 0xB0;                   //  Device internal register 设备内部地址   A2区第176为地址为B0,读取温度
            SerBuf[3] = 2;                      // 读取的个数设定
            iss_transmit(SerBuf, 4);
            //iss_recieve(SerBuf, 1);          //  读的位数，目前一次性读1位
            iss_recieve(SerBuf, 2);           //  读的位数，目前一次性读51位
            USBISStextBox.Text += string.Format("模块温度为：{0:X02} {1:X02} \r\n", SerBuf[0],SerBuf[1]);

            //选择 Table-Ch(ChA(0)、ChB(1))
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7f;                   // Device internal register 设备内部地址(a2 127位 写2+0为CHA,写2+1为CHB）
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x02;                   // The data bytes   写2+0为CHA,写2+1为CHB
            iss_transmit(SerBuf, 5);
            iss_recieve(SerBuf, 1);

            //读到LUT 128位,  72个数
            SerBuf[0] = (byte)IssCmds.I2C_AD1;  //  Read/Write 1 byte addressed devices (the majority of devices will use this one)
            SerBuf[1] = 0xa3;                   //  I2C address of EEPROM for reading，I2C数据区块，0xa1为Read A0; 0xa3为Read A2;0xb1为Read B0; 0xb3为Read B2;
            SerBuf[2] = 0x80;                   //  Device internal register 设备内部地址   A2区第128为地址为80
            SerBuf[3] = 32;                 // 读取的个数设定
            iss_transmit(SerBuf, 4);
            iss_recieve(SerBuf, 32);           //  读的位数，目前一次性读51位

            for (int i = 0; i < 32; i++)      //   i 的长度应该为iss_recieve里的read_bytes的长度
            {
                string dddd = string.Empty;
                if ((i + 1) % 32 == 0 && i != 0) dddd = "\r\n";
                USBISStextBox.Text += string.Format("{0:X02} {1}", SerBuf[i], dddd);
            }

            SerBuf[0] = (byte)IssCmds.I2C_AD1;  //  Read/Write 1 byte addressed devices (the majority of devices will use this one)
            SerBuf[1] = 0xa3;                   //  I2C address of EEPROM for reading，I2C数据区块，0xa1为Read A0; 0xa3为Read A2;0xb1为Read B0; 0xb3为Read B2;
            SerBuf[2] = 0xA0;                   //  Device internal register 设备内部地址 
            SerBuf[3] = 40;                 // 读取的个数设定
            iss_transmit(SerBuf, 4);
            iss_recieve(SerBuf, 40);           //  读的位数，目前一次性读51位

            for (int i = 0; i < 40; i++)      //   i 的长度应该为iss_recieve里的read_bytes的长度
            {
                string dddd = string.Empty;
                if ((i + 1) % 40 == 0 && i != 0) dddd = "\r\n";
                USBISStextBox.Text += string.Format("{0:X02} {1}", SerBuf[i], dddd);
            }
            

            //密码保存，都不变
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7E;                   // Device internal register 设备内部地址
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x00;                   // The data bytes
            iss_transmit(SerBuf, 5);
            iss_recieve(SerBuf, 1);
            //USBISStextBox.Text += string.Format("write 126 0:{0:X02} \r\n", SerBuf[0]);
        }

        private void MyI2C_ReadPower()      //读取模块Modulation，温度 & Modulation。
        {
            //密码写入，都不变
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7E;                   // Device internal register 设备内部地址(a2 126位）
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x01;                   // The data bytes
            iss_transmit(SerBuf, 5);
            iss_recieve(SerBuf, 1);
            //USBISStextBox.Text += string.Format("write 126 1:{0:X02} \r\n", SerBuf[0]);

            //选择 Table-configure
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7f;                   // Device internal register 设备内部地址(a2 127位 写1）
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x01;                   // The data bytes
            iss_transmit(SerBuf, 5);
            iss_recieve(SerBuf, 1);

            //读到A2 176位,读取两个字节的高低位温度，温度值 =((HEX2DEC(A2)*187.4/65536-53.7)+40)/2，单位度 
            SerBuf[0] = (byte)IssCmds.I2C_AD1;  //  Read/Write 1 byte addressed devices (the majority of devices will use this one)
            SerBuf[1] = 0xa3;                   //  I2C address of EEPROM for reading，I2C数据区块，0xa1为Read A0; 0xa3为Read A2;0xb1为Read B0; 0xb3为Read B2;
            SerBuf[2] = 0xb0;                   //  Device internal register 设备内部地址   A2区第176为地址为B0,读取温度
            SerBuf[3] = 2;                      // 读取的个数设定
            iss_transmit(SerBuf, 4);
            //iss_recieve(SerBuf, 1);          //  读的位数，目前一次性读1位
            iss_recieve(SerBuf, 2);           //  读的位数，目前一次性读51位
            USBISStextBox.Text += string.Format("模块温度为：{0:X02} {1:X02} \r\n", SerBuf[0], SerBuf[1]);

            ////选择 Table-Ch(ChA(0)、ChB(1))
            //SerBuf[0] = (byte)IssCmds.I2C_AD1;
            //SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            //SerBuf[2] = 0x7f;                   // Device internal register 设备内部地址(a2 127位 写2+0为CHA,写2+1为CHB）
            //SerBuf[3] = 0x01;                   // Number of data bytes
            //SerBuf[4] = 0x02;                   // The data bytes   写2+0为CHA,写2+1为CHB
            //iss_transmit(SerBuf, 5);
            //iss_recieve(SerBuf, 1);

            //读到Power 102位,  2个数
            SerBuf[0] = (byte)IssCmds.I2C_AD1;  //  Read/Write 1 byte addressed devices (the majority of devices will use this one)
            SerBuf[1] = 0xa3;                   //  I2C address of EEPROM for reading，I2C数据区块，0xa1为Read A0; 0xa3为Read A2;0xb1为Read B0; 0xb3为Read B2;
            SerBuf[2] = 0xcf;                   //  Device internal register 设备内部地址     A4为CHA；CF为CHB
            SerBuf[3] = 3;                 // 读取的个数设定
            iss_transmit(SerBuf, 4);
            iss_recieve(SerBuf, 3);           //  读的位数，目前一次性读51位
            USBISStextBox.Text += string.Format("功率值为：{0:X02} {1:X02} {2:X02} \r\n", SerBuf[0], SerBuf[1], SerBuf[2]);

           
            //密码保存，都不变
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7E;                   // Device internal register 设备内部地址
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x00;                   // The data bytes
            iss_transmit(SerBuf, 5);
            iss_recieve(SerBuf, 1);
            //USBISStextBox.Text += string.Format("write 126 0:{0:X02} \r\n", SerBuf[0]);
        }

        private void MyI2C_ReadTuneParameter()      //读取调整参数，Power、ER、Crossing。
        {
            //密码写入，都不变
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7E;                   // Device internal register 设备内部地址(a2 126位）
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x01;                   // The data bytes
            iss_transmit(SerBuf, 5);
            iss_recieve(SerBuf, 1);
            //USBISStextBox.Text += string.Format("write 126 1:{0:X02} \r\n", SerBuf[0]);

            //选择 Table-configure
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7f;                   // Device internal register 设备内部地址(a2 127位 写1）
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x01;                   // The data bytes
            iss_transmit(SerBuf, 5);
            iss_recieve(SerBuf, 1);

            //读到A2 168位,为Power值
            SerBuf[0] = (byte)IssCmds.I2C_AD1;  //  Read/Write 1 byte addressed devices (the majority of devices will use this one)
            SerBuf[1] = 0xa3;                   //  I2C address of EEPROM for reading，I2C数据区块，0xa1为Read A0; 0xa3为Read A2;0xb1为Read B0; 0xb3为Read B2;
            SerBuf[2] = 0xA8;                   //  Device internal register 设备内部地址   A2区第168为地址为A8,为Power值
            SerBuf[3] = 1;                      // 读取的个数设定
            iss_transmit(SerBuf, 4);
            //iss_recieve(SerBuf, 1);          //  读的位数，目前一次性读1位
            iss_recieve(SerBuf, 1);           //  读的位数，目前一次性读51位
            USBISStextBox.Text += string.Format("Power值：{0:X02} \r\n", SerBuf[0]);

            //读到A2 164位,为ER值
            SerBuf[0] = (byte)IssCmds.I2C_AD1;  //  Read/Write 1 byte addressed devices (the majority of devices will use this one)
            SerBuf[1] = 0xa3;                   //  I2C address of EEPROM for reading，I2C数据区块，0xa1为Read A0; 0xa3为Read A2;0xb1为Read B0; 0xb3为Read B2;
            SerBuf[2] = 0xA4;                   //  Device internal register 设备内部地址   A2区第164为地址为A4,为ER值
            SerBuf[3] = 1;                      // 读取的个数设定
            iss_transmit(SerBuf, 4);
            //iss_recieve(SerBuf, 1);          //  读的位数，目前一次性读1位
            iss_recieve(SerBuf, 1);           //  读的位数，目前一次性读51位
            USBISStextBox.Text += string.Format("ER值：{0:X02} \r\n", SerBuf[0]);

            //读到A2 162位,为Crossing值
            SerBuf[0] = (byte)IssCmds.I2C_AD1;  //  Read/Write 1 byte addressed devices (the majority of devices will use this one)
            SerBuf[1] = 0xa3;                   //  I2C address of EEPROM for reading，I2C数据区块，0xa1为Read A0; 0xa3为Read A2;0xb1为Read B0; 0xb3为Read B2;
            SerBuf[2] = 0xA2;                   //  Device internal register 设备内部地址   A2区第162为地址为A2,为Crossing值
            SerBuf[3] = 1;                      // 读取的个数设定
            iss_transmit(SerBuf, 4);
            //iss_recieve(SerBuf, 1);          //  读的位数，目前一次性读1位
            iss_recieve(SerBuf, 1);           //  读的位数，目前一次性读51位
            USBISStextBox.Text += string.Format("Crossing值：{0:X02} \r\n", SerBuf[0]);

            //密码保存，都不变
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 只能变更 0xa2区域126位;
            SerBuf[2] = 0x7E;                   // Device internal register 设备内部地址
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x00;                   // The data bytes
            iss_transmit(SerBuf, 5);
            iss_recieve(SerBuf, 1);
            //USBISStextBox.Text += string.Format("write 126 0:{0:X02} \r\n", SerBuf[0]);
        }

      //  private void doI2C_IO()
       // {
        //    int val, pins;

            //SerBuf[0] = (byte)IssCmds.I2C_AD2;
            //SerBuf[1] = 0xa1;                   //  I2C address of EEPROM for reading
            //SerBuf[2] = 0x00;                   // address
            //SerBuf[3] = 0x00;
            //SerBuf[4] = 0x04;                   // dummy bytes
            //iss_transmit(SerBuf, 5);
            //iss_recieve(SerBuf, 4);
            //USBISStextBox.Text = string.Format("Reading I2C EEPROM type 24LC256 {0:X02}{1:X02}{2:X02}{3:X02}\r\n", SerBuf[0], SerBuf[1], SerBuf[2], SerBuf[3]);  //print the software version on screen

        //    SerBuf[0] = (byte)IssCmds.I2C_AD0;
        //    SerBuf[1] = 0xa0;                   //  I2C address of EEPROM for writing
        //    SerBuf[2] = 0x06;                   // number of bytes coming     未来的字节数
        //    SerBuf[3] = 0x00;                   // addressH
        //    SerBuf[4] = 0x00;                   // addressL
        //    SerBuf[5] = (byte)(counter >> 24);
        //    SerBuf[6] = (byte)(counter >> 16);
        //    SerBuf[7] = (byte)(counter >> 8);
        //    SerBuf[8] = (byte)(counter >> 0);
        //    iss_transmit(9);

        //    USBISStextBox.Text += string.Format("Writing to I2C EEPROM type 24LC256 {0:X02}{1:X02}{2:X02}{3:X02}\r\n", SerBuf[5], SerBuf[6], SerBuf[7], SerBuf[8]);  //print the software version on screen
        //    iss_recieve(1);
        //    counter++;

        //    if (IOtypeNew != IOtypeOld)
        //    {
        //        if (((IOtypeNew ^ IOtypeOld) & 0xaa) > 0)         // mode changed from Output to Input or Analog or viceversa  模式由输出变为输入或模拟或viceversa
        //        {
        //            SerBuf[0] = 0x5A;                            // USB_ISS Module Command
        //            SerBuf[1] = (byte)IssCmds.ISS_MODE;          // Set Mode
        //            SerBuf[2] = (byte)ISS_mode.IO_CHANGE;
        //            SerBuf[3] = (byte)IOtypeNew;                    // input mode
        //            iss_transmit(4);
        //            iss_recieve(2);
        //        }
        //        else                                             // only outH<-->outL changed
        //        {
        //            SerBuf[0] = (byte)IssCmds.SETPINS;           // set any outputs
        //            SerBuf[1] = 0;

        //            if ((IOtypeNew & 0x01) == 0x01) SerBuf[1] |= 0x01;
        //            if ((IOtypeNew & 0x04) == 0x04) SerBuf[1] |= 0x02;
        //            if ((IOtypeNew & 0x10) == 0x10) SerBuf[1] |= 0x04;
        //            if ((IOtypeNew & 0x40) == 0x40) SerBuf[1] |= 0x08;
        //            iss_transmit(2);
        //            iss_recieve(1);
        //        }
        //    }
        //    IOtypeOld = IOtypeNew;

        //    SerBuf[0] = (byte)IssCmds.GETPINS;          // Get Inputs/Outputs
        //    iss_transmit(1);
        //    iss_recieve(1);
        //    pins = SerBuf[0];
        //    if ((IOtypeOld & 0x03) == 3)
        //    {
        //        SerBuf[0] = (byte)IssCmds.GETAD;
        //        SerBuf[1] = 1;                          // chnl 1
        //        iss_transmit(2);
        //        iss_recieve(2);
        //        val = (SerBuf[0] << 8) + SerBuf[1];
        //        USBISStextBox.Text += string.Format("Chnl1 = {0}\r\n", val);
        //    }
        //    else
        //    {
        //        if ((pins & 0x01) == 0x01) USBISStextBox.Text += string.Format("Chnl1 = High\r\n");
        //        else USBISStextBox.Text += string.Format("Chnl1 = Low\r\n");
        //    }
        //    if ((IOtypeOld & 0x0c) == 0x0c)
        //    {
        //        SerBuf[0] = (byte)IssCmds.GETAD;
        //        SerBuf[1] = 2;                          // chnl 2
        //        iss_transmit(2);
        //        iss_recieve(2);
        //        val = (SerBuf[0] << 8) + SerBuf[1];
        //        USBISStextBox.Text += string.Format("Chnl2 = {0}\r\n", val);
        //    }
        //    else
        //    {
        //        if ((pins & 0x02) == 0x02) USBISStextBox.Text += string.Format("Chnl2 = High\r\n");
        //        else USBISStextBox.Text += string.Format("Chnl2 = Low\r\n");
        //    }
       // }



        private void doI2C_IOStep1()
        {
            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 0xa0为Write A0;0xa1为Read A0; 0xa2为Write A2;0xa3为Read A2;
            SerBuf[2] = 0x7E;                   // Device internal register 设备内部地址
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x01;                   // The data bytes
            iss_transmit(SerBuf, 5);     
            iss_recieve(SerBuf, 1);
            USBISStextBox.Text += string.Format("write 126 1:{0:X02} \r\n", SerBuf[0]);

        }

        private void doI2C_IOStep2()
        {

            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 0xa0为Write A0;0xa1为Read A0; 0xa2为Write A2;0xa3为Read A2;
            SerBuf[2] = 0x00;                   // Device internal register 设备内部地址
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x64;                   // The data bytes
            iss_transmit(SerBuf, 5);
            iss_recieve(SerBuf, 1);
            USBISStextBox.Text += string.Format("write 00 2:{0:X02} \r\n", SerBuf[0]);

        }
        
        private void doI2C_IOStep3()
        {

            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa2;                   // I2C address of EEPROM for writing: 0xa0为Write A0;0xa1为Read A0; 0xa2为Write A2;0xa3为Read A2;
            SerBuf[2] = 0x7E;                   // Device internal register 设备内部地址
            SerBuf[3] = 0x01;                   // Number of data bytes
            SerBuf[4] = 0x00;                   // The data bytes
            iss_transmit(SerBuf, 5);
            iss_recieve(SerBuf, 1);
            USBISStextBox.Text += string.Format("write 126 0:{0:X02} \r\n", SerBuf[0]);
        }




        //新修改，2018-3-10，写入数据
        private void doI2C_IO()
        {
         //   int val, pins;

            //SerBuf[0] = (byte)IssCmds.I2C_AD0;
            //SerBuf[1] = 0xa1;                   //  I2C address of EEPROM for reading
            //SerBuf[2] = 0x00;                   // address
            //SerBuf[3] = 0x00;
            //SerBuf[4] = 0x04;                   // dummy bytes
            //iss_transmit(SerBuf, 5);
            //iss_recieve(SerBuf, 4);
            //USBISStextBox.Text = string.Format("Reading I2C EEPROM type 24LC256 {0:X02}{1:X02}{2:X02}{3:X02}\r\n", SerBuf[0], SerBuf[1], SerBuf[2], SerBuf[3]);  //print the software version on screen



            //SerBuf[0] = (byte)IssCmds.I2C_AD0;
            //SerBuf[1] = 0xa0;                   //  I2C address of EEPROM for writing
            //SerBuf[2] = 0x06;                   // number of bytes coming     未来的字节数
            //SerBuf[3] = 0x00;                   // addressH
            //SerBuf[4] = 0x00;                   // addressL
            //SerBuf[5] = (byte)(counter >> 24);
            //SerBuf[6] = (byte)(counter >> 16);
            //SerBuf[7] = (byte)(counter >> 8);
            //SerBuf[8] = (byte)(counter >> 0);
            //iss_transmit(SerBuf, 9);


            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa0;                   //  I2C address of EEPROM for writing
            SerBuf[2] = 0x7E;                   // number of bytes coming     未来的字节数
            SerBuf[3] = 0x01;                   // addressH

            //SerBuf[4] = 0x02;                   // addressH

            //SerBuf[4] = 0x08;                   // addressH
            //SerBuf[5] = 0x08;                   // addressH
            //SerBuf[6] = 0x08;                   // addressH
            //SerBuf[7] = 0x08;                   // addressH


            iss_transmit(SerBuf, 4);


           // System.Threading.Thread.Sleep(5000);


            SerBuf[0] = (byte)IssCmds.I2C_AD1;
            SerBuf[1] = 0xa0;                   //  I2C address of EEPROM for writing
            SerBuf[2] = 0x02;                   // number of bytes coming     未来的字节数

            SerBuf[3] = 0xA5;                   // addressH
          //  SerBuf[4] = 0xCD;                   // addressH

            //  SerBuf[3] = 0xAC;                   // addressH
            //SerBuf[4] = 0x00;                   // addressL
            //SerBuf[5] = (byte)(counter >> 24);
            //SerBuf[6] = (byte)(counter >> 16);
            //SerBuf[7] = (byte)(counter >> 8);
            //SerBuf[8] = (byte)(counter >> 0);

            //SerBuf[4] = 0x08;                   // addressH
            //SerBuf[5] = 0x08;                   // addressH
            //SerBuf[6] = 0x08;                   // addressH
            //SerBuf[7] = 0x08;                   // addressH


            iss_transmit(SerBuf, 4);


          //  System.Threading.Thread.Sleep(5000);



            //SerBuf[0] = (byte)IssCmds.I2C_AD1;
            //SerBuf[1] = 0xa0;                   //  I2C address of EEPROM for writing
            //SerBuf[2] = 0x7E;                   // number of bytes coming     未来的字节数
            //SerBuf[3] = 0x00;                   // addressH

            ////SerBuf[4] = 0x08;                   // addressH
            ////SerBuf[5] = 0x08;                   // addressH
            ////SerBuf[6] = 0x08;                   // addressH
            ////SerBuf[7] = 0x08;                   // addressH


            //iss_transmit(SerBuf, 4);


           //  System.Threading.Thread.Sleep(5000);




        //   USBISStextBox.Text += string.Format("Writing to I2C EEPROM type 24LC256 {0:X02}{1:X02}{2:X02}{3:X02}\r\n", SerBuf[0], SerBuf[1], SerBuf[2], SerBuf[3]);  //print the software version on screen
       //      iss_recieve(SerBuf, 1);
          //   counter++;


            //if (IOtypeNew != IOtypeOld)
            //{
            //    if (((IOtypeNew ^ IOtypeOld) & 0xaa) > 0)         // mode changed from Output to Input or Analog or viceversa  模式由输出变为输入或模拟或viceversa
            //    {
            //        SerBuf[0] = 0x5A;                            // USB_ISS Module Command
            //        SerBuf[1] = (byte)IssCmds.ISS_MODE;          // Set Mode
            //        SerBuf[2] = (byte)ISS_mode.IO_CHANGE;
            //        SerBuf[3] = (byte)IOtypeNew;                    // input mode
            //        iss_transmit(SerBuf, 4);
            //        iss_recieve(SerBuf, 2);
            //    }
            //    else                                             // only outH<-->outL changed
            //    {
            //        SerBuf[0] = (byte)IssCmds.SETPINS;           // set any outputs
            //        SerBuf[1] = 0;

            //        if ((IOtypeNew & 0x01) == 0x01) SerBuf[1] |= 0x01;
            //        if ((IOtypeNew & 0x04) == 0x04) SerBuf[1] |= 0x02;
            //        if ((IOtypeNew & 0x10) == 0x10) SerBuf[1] |= 0x04;
            //        if ((IOtypeNew & 0x40) == 0x40) SerBuf[1] |= 0x08;
            //        iss_transmit(SerBuf, 2);
            //        iss_recieve(SerBuf, 1);
            //    }
            //}
            //IOtypeOld = IOtypeNew;

            //SerBuf[0] = (byte)IssCmds.GETPINS;          // Get Inputs/Outputs
            //iss_transmit(SerBuf, 1);
            //iss_recieve(SerBuf, 1);
            //pins = SerBuf[0];
            //if ((IOtypeOld & 0x03) == 3)
            //{
            //    SerBuf[0] = (byte)IssCmds.GETAD;
            //    SerBuf[1] = 1;                          // chnl 1
            //    iss_transmit(SerBuf, 2);
            //    iss_recieve(SerBuf, 2);
            //    val = (SerBuf[0] << 8) + SerBuf[1];
            //    USBISStextBox.Text += string.Format("Chnl1 = {0}\r\n", val);
            //}
            //else
            //{
            //    if ((pins & 0x01) == 0x01) USBISStextBox.Text += string.Format("Chnl1 = High\r\n");
            //    else USBISStextBox.Text += string.Format("Chnl1 = Low\r\n");
            //}
            //if ((IOtypeOld & 0x0c) == 0x0c)
            //{
            //    SerBuf[0] = (byte)IssCmds.GETAD;
            //    SerBuf[1] = 2;                          // chnl 2
            //    iss_transmit(SerBuf, 2);
            //    iss_recieve(SerBuf, 2);
            //    val = (SerBuf[0] << 8) + SerBuf[1];
            //    USBISStextBox.Text += string.Format("Chnl2 = {0}\r\n", val);
            //}
            //else
            //{
            //    if ((pins & 0x02) == 0x02) USBISStextBox.Text += string.Format("Chnl2 = High\r\n");
            //    else USBISStextBox.Text += string.Format("Chnl2 = Low\r\n");
            //}
        }




        private void doIO()
        {
        //    if (IOtypeNew != IOtypeOld)
        //    {
        //        SerBuf[0] = 0x5A;                            // USB_ISS Module Command
        //        SerBuf[1] = (byte)IssCmds.ISS_MODE;          // Set Mode
        //        SerBuf[2] = (byte)ISS_mode.IO_MODE;
        //        IOtypeOld = IOtypeNew;
        //        SerBuf[3] = (byte)IOtypeNew;                 // input mode
        //        iss_transmit(4);
        //        iss_recieve(2);

        //        SerBuf[0] = (byte)IssCmds.SETPINS;           // set any outputs
        //        SerBuf[1] = 0;

        //        if (comboBox1.SelectedIndex == 1) SerBuf[1] |= 0x01;
        //        if (comboBox2.SelectedIndex == 1) SerBuf[1] |= 0x02;
        //        if (comboBox3.SelectedIndex == 1) SerBuf[1] |= 0x04;
        //        if (comboBox4.SelectedIndex == 1) SerBuf[1] |= 0x08;
        //        iss_transmit(4);
        //        iss_recieve(2);
        //    }
        //    else
        //    {
        //        int val, pins;
        //        SerBuf[0] = (byte)IssCmds.GETPINS;          // Get Inputs/Outputs
        //        iss_transmit(1);
        //        iss_recieve(1);
        //        pins = SerBuf[0];
        //        if ((IOtypeOld&0x03) == 3)
        //        {
        //            SerBuf[0] = (byte)IssCmds.GETAD;
        //            SerBuf[1] = 1;                          // chnl 1
        //            iss_transmit(2);
        //            iss_recieve(2);
        //            val = (SerBuf[0] << 8) + SerBuf[1];
        //            USBISStextBox.Text = string.Format("Chnl1 = {0}\r\n", val);
        //        }
        //        else
        //        {
        //            if ((pins & 0x01)==0x01) USBISStextBox.Text = string.Format("Chnl1 = High\r\n");
        //            else USBISStextBox.Text = string.Format("Chnl1 = Low\r\n");
        //        }
        //        if ((IOtypeOld & 0x0c) == 0x0c)
        //        {
        //            SerBuf[0] = (byte)IssCmds.GETAD;
        //            SerBuf[1] = 2;                          // chnl 2
        //            iss_transmit(2);
        //            iss_recieve(2);
        //            val = (SerBuf[0] << 8) + SerBuf[1];
        //            USBISStextBox.Text += string.Format("Chnl2 = {0}\r\n", val);
        //        }
        //        else
        //        {
        //            if ((pins & 0x02) == 0x02) USBISStextBox.Text += string.Format("Chnl2 = High\r\n");
        //            else USBISStextBox.Text += string.Format("Chnl2 = Low\r\n");
        //        }
        //        if ((IOtypeOld & 0x30) == 0x30)
        //        {
        //            SerBuf[0] = (byte)IssCmds.GETAD;
        //            SerBuf[1] = 3;                          // chnl 3
        //            iss_transmit(2);
        //            iss_recieve(2);
        //            val = (SerBuf[0] << 8) + SerBuf[1];
        //            USBISStextBox.Text += string.Format("Chnl3 = {0}\r\n", val);
        //        }
        //        else
        //        {
        //            if ((pins & 0x04) == 0x04) USBISStextBox.Text += string.Format("Chnl3 = High\r\n");
        //            else USBISStextBox.Text += string.Format("Chnl3 = Low\r\n");
        //        }
        //        if ((IOtypeOld & 0xc0) == 0xc0)
        //        {
        //            SerBuf[0] = (byte)IssCmds.GETAD;
        //            SerBuf[1] = 4;                          // chnl 4
        //            iss_transmit(2);
        //            iss_recieve(2);
        //            val = (SerBuf[0] << 8) + SerBuf[1];
        //            USBISStextBox.Text += string.Format("Chnl4 = {0}\r\n", val);
        //        }
        //        else
        //        {
        //            if ((pins & 0x08) == 0x08) USBISStextBox.Text += string.Format("Chnl4 = High\r\n");
        //            else USBISStextBox.Text += string.Format("Chnl4 = Low\r\n");
        //        }
        //    }
        }

        //private void doSPI()
        //{
        //    SerBuf[0] = (byte)IssCmds.SPI_IO;
        //    SerBuf[1] = 0x02;                   // write to SPI RAM
        //    SerBuf[2] = 0x00;                   // address
        //    SerBuf[3] = 0x00;
        //    SerBuf[4] = (byte)(counter >> 24);
        //    SerBuf[5] = (byte)(counter >> 16);
        //    SerBuf[6] = (byte)(counter >> 8);
        //    SerBuf[7] = (byte)(counter >> 0);
        //    iss_transmit(8);
        //    USBISStextBox.Text = string.Format("Writing to SPI RAM type 23K256 {0:X02}{1:X02}{2:X02}{3:X02}\r\n", SerBuf[4], SerBuf[5], SerBuf[6], SerBuf[7]);  //print the software version on screen
        //    iss_recieve(8);

        //    SerBuf[0] = (byte)IssCmds.SPI_IO;
        //    SerBuf[1] = 0x03;                   // read SPI RAM
        //    SerBuf[2] = 0x00;                   // address
        //    SerBuf[3] = 0x00;
        //    SerBuf[4] = 0x00;                   // dummy bytes
        //    SerBuf[5] = 0x00;
        //    SerBuf[6] = 0x00;
        //    SerBuf[7] = 0x00;
        //    iss_transmit(8);
        //    iss_recieve(8);
        //    USBISStextBox.Text += string.Format("Reading SPI RAM type 23K256 {0:X02}{1:X02}{2:X02}{3:X02}", SerBuf[4], SerBuf[5], SerBuf[6], SerBuf[7]);  //print the software version on screen
        //    counter++;
        //}

        private void Mode_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //timer1.Enabled = false;
            //Mode = Mode_comboBox.SelectedIndex;
            //switch(Mode) {
            //    case 0:
            //        comboBox1.Enabled = true;
            //        comboBox1.SelectedIndex = 2;
            //        comboBox2.Enabled = true;
            //        comboBox2.SelectedIndex = 2;
            //        comboBox3.Enabled = true;
            //        comboBox3.SelectedIndex = 2;
            //        comboBox4.Enabled = true;
            //        comboBox4.SelectedIndex = 2;

            //        SerBuf[0] = 0x5A;                            // USB_ISS Module Command
            //        SerBuf[1] = (byte)IssCmds.ISS_MODE;          // Set Mode
            //        SerBuf[2] = (byte)ISS_mode.IO_MODE;
            //        GetIOtype();
            //        IOtypeOld = IOtypeNew;
            //        SerBuf[3] = (byte)IOtypeNew;                    // input mode
            //        iss_transmit(SerBuf, 4);
            //        iss_recieve(SerBuf,2);
            //        break;

            //    case 1:                                          // I2C + I/O modes
            //        InitI2c(0x20);
            //        break;
            //    case 3:
            //        InitI2c(0x30);
            //        break;
            //    case 5:
            //        InitI2c(0x40);
            //        break;
            //    case 7:
            //        InitI2c(0x50);
            //        break;
            //    case 9:
            //        InitI2c(0x60);
            //        break;
            //    case 11:
            //        InitI2c(0x70);
            //        break;
            //    case 13:
            //        InitI2c(0x80);
            //        break;

            //    case 2:                                          // I2C + I/O modes
            //        InitI2cSer(0x21);
            //        break;
            //    case 4:
            //        InitI2cSer(0x31);
            //        break;
            //    case 6:
            //        InitI2cSer(0x41);
            //        break;
            //    case 8:
            //        InitI2cSer(0x51);
            //        break;
            //    case 10:
            //        InitI2cSer(0x61);
            //        break;
            //    case 12:
            //        InitI2cSer(0x71);
            //        break;
            //    case 14:
            //        InitI2cSer(0x81);
            //        break;

            //    case 15:
            //        comboBox1.Enabled = false;
            //        comboBox2.Enabled = false;
            //        comboBox3.Enabled = false;
            //        comboBox4.Enabled = false;

            //        SerBuf[0] = 0x5A;                            // USB_ISS Module Command
            //        SerBuf[1] = (byte)IssCmds.ISS_MODE;          // Set Mode
            //        SerBuf[2] = (byte)ISS_mode.SPI_MODE;
            //        SerBuf[3] = 254;                               // clk div
            //        iss_transmit(4);
            //        iss_recieve(2);

            //        SerBuf[0] = (byte)IssCmds.SPI_IO;
            //        SerBuf[1] = 0x01;                                 // SPI RAM, write status Reg
            //        SerBuf[2] = 0x41;
            //        iss_transmit(3);
            //        iss_recieve(3);
            //        timer1.Interval = 10;
            //        break;
              
            //    case 16:
            //        comboBox1.Enabled = false;
            //        comboBox1.SelectedIndex = 2;
            //        comboBox2.Enabled = false;
            //        comboBox2.SelectedIndex = 2;
            //        comboBox3.Enabled = true;
            //        comboBox3.SelectedIndex = 2;
            //        comboBox4.Enabled = true;
            //        comboBox4.SelectedIndex = 2;
            //        SerBuf[0] = 0x5A;                           // USB_ISS Module Command
            //        SerBuf[1] = (byte)IssCmds.ISS_MODE;         // Set Mode
            //        SerBuf[2] = (byte)ISS_mode.SERIAL;          // Serial+I/O
            //        SerBuf[3] = 0;
            //        SerBuf[4] = 155;                            // 155 is 19.2k baud
            //        GetIOtype();
            //        IOtypeOld = IOtypeNew;
            //        SerBuf[5] = (byte)IOtypeNew;                // input mode
            //        iss_transmit(6);
            //        iss_recieve(2);
            //        timer1.Interval = 500;
            //        break;
            //}
            //timer1.Enabled = true;

        }

        private void InitI2cSer(byte mode)
        {
            //comboBox1.Enabled = false;
            //comboBox1.SelectedIndex = 2;
            //comboBox2.Enabled = false;
            //comboBox2.SelectedIndex = 2;
            //comboBox3.Enabled = false;
            //comboBox3.SelectedIndex = 2;
            //comboBox4.Enabled = false;
            //comboBox4.SelectedIndex = 2;
            //SerBuf[0] = 0x5A;                           // USB_ISS Module Command
            //SerBuf[1] = (byte)IssCmds.ISS_MODE;         // Set Mode
            //SerBuf[2] = mode;                           // I2C_S_20KHZ etc.
            //SerBuf[3] = 0;
            //SerBuf[4] = 1;                            // 155 is 19.2k baud
            //iss_transmit(5);
            //iss_recieve(2);
            //timer1.Interval = 500;
        }
        
        private void InitI2c(byte mode)
        {
            //comboBox1.Enabled = true;
            //comboBox1.SelectedIndex = 2;
            //comboBox2.Enabled = true;
            //comboBox2.SelectedIndex = 2;
            //comboBox3.Enabled = false;
            //comboBox3.SelectedIndex = 2;
            //comboBox4.Enabled = false;
            //comboBox4.SelectedIndex = 2;
            //SerBuf[0] = 0x5A;                           // USB_ISS Module Command
            //SerBuf[1] = (byte)IssCmds.ISS_MODE;         // Set Mode
            //SerBuf[2] = mode;                           // I2C_S_20KHZ etc.
            //GetIOtype();
            //IOtypeOld = IOtypeNew;
            //SerBuf[3] = (byte)IOtypeNew;                    // input mode
            //iss_transmit(4);
            //iss_recieve(2);
            //timer1.Interval = 500;
        }

        //private void GetIOtype()
        //{
        //    int mode = comboBox1.SelectedIndex;
        //    mode |= (comboBox2.SelectedIndex << 2);
        //    mode |= (comboBox3.SelectedIndex << 4);
        //    mode |= (comboBox4.SelectedIndex << 6);
        //    IOtypeNew = mode;
        //}

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        //    GetIOtype();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        //    GetIOtype();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
        //    GetIOtype();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
        //    GetIOtype();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                MyI2C_Read();
                //MyI2C_ReadFlashTimes();       //读已被烧写的次数
                //MyI2C_ReadTuneParameter();      //读TUNE参数Power（直接转16进制）、ER（直接转16进制）、Crossing（*8后再转16进制）
                // MyI2C_ReadLUT();
                //MyI2C_ReadPower();

                //doI2C_IOStep1();
                //doI2C_IOStep2();
                //doI2C_IOStep3();

                //MyI2C_WriteTest();

                //MyI2C_Read();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示");
            }
            finally
            {
             
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

}
