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
                byte count =Convert.ToByte( Console.ReadLine());
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
    }
}
