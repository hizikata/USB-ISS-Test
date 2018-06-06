using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace ConsoleApp1
{
    class Program
    {
        static byte[] SerBuf = new byte[100];
        static SerialPort Port = new SerialPort();

        static void Main(string[] args)
        {
            double temp, vcc, txPower, rxPower, bais;
            StringBuilder sb = new StringBuilder(100);
            sb.Append("0x");
            short cache = 0;
            ushort uCache = 0;
            //List<string> data = new List<string>() { "E4", "30", "7D", "C0", "01", "EF", "94", "AE", "00", "16" };
            List<short> data = new List<short>() { 0xE4, 0x30, 0x7D, 0xC0, 0x01, 0xEF, 0x94, 0xAE, 0x00, 0x16 };
            cache = DigitTransform(data[0], data[1]);
            temp = (double)cache / 256;
            Console.WriteLine("当前温度:" + temp + "℃");
            //Vcc
            ushort u1, u2;
            u1 = Convert.ToUInt16(data[2]);
            u2 = Convert.ToUInt16(data[3]);
            uCache = UDigitTransform(u1,u2);
            vcc = (double)uCache / 10000;
            Console.WriteLine("Vcc:" + vcc.ToString() + "V");
            //Bais
            uCache = UDigitTransform((ushort)data[4], (ushort)data[5]);
            bais = (double)uCache  /500;
            Console.WriteLine("Bais:" + bais.ToString() + "mA");
            //TxPower
            uCache = UDigitTransform((ushort)data[6], (ushort)data[7]);
            txPower = (double)uCache / 10000; //mW
            txPower = Math.Log10(txPower) * 10;
            Console.WriteLine("TxPower:" + txPower + "dBm");

            //RxPower
            uCache = UDigitTransform((ushort)data[8], (ushort)data[9]);
            rxPower = (double)uCache / 10000; //mW
            rxPower = Math.Log10(rxPower) * 10;
            Console.WriteLine("RxPower:" + rxPower + "dBm");
            Console.ReadKey();
            return;
            try
            {
                Port.PortName = "COM3";
                Port.Parity = 0;
                Port.BaudRate = 19200;
                Port.StopBits = StopBits.Two;
                Port.DataBits = 8;
                Port.ReadTimeout = 100;
                Port.WriteTimeout = 100;
                Port.Open();
                TransmitBase tb = new TransmitBase();
                string msg = string.Empty;
                Console.Write("输入读取的数据的地址>>");
                byte add = Convert.ToByte(Console.ReadLine());
                Console.Write("输入要读取的字节数:>>");
                byte count = Convert.ToByte(Console.ReadLine());
                //msg = tb.MyI2C_ReadLowByte(SerBuf, Port, 64);
                //msg = tb.MyI2C_ReadA2HByte(SerBuf, Port,add,count);
                msg = tb.MyI2C_ReadA2HByte(SerBuf, Port, add, count);

                //string[] StrArray = SerialPort.GetPortNames();
                //foreach (var item in StrArray)
                //{
                //    Console.WriteLine(item);
                //}
                Console.WriteLine(msg);
                Port.Close();
                Console.WriteLine("press any key to exit");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        static short DigitTransform(short msb, short lsb)
        {
            short num = (short)(msb * 256 + lsb);
            return num;
        }
        static ushort UDigitTransform(ushort msb, ushort lsb)
        {
            ushort num = (ushort)((msb * 256) + lsb);
            return num;
        }
    }
}
