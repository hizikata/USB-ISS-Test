using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using I2CIO_Test.Model;
using I2CIO_Test.Methods;

namespace I2CIO_Test.View
{
    /// <summary>
    /// FrmMain.xaml 的交互逻辑
    /// </summary>
    public partial class FrmMain : Window
    {
        #region Properties
        readonly string[] WaveLength = { "1270", "1310", "1490" };
        LowTempPara ParaModel = new LowTempPara();
        //设备列表
        //Keithley Keith;
        HP8153A Hp8153A ;
        HP8156A Hp8156A;
        //MP2100A Mp2100A ;
        //I2C参数
        SerialPort Port = new SerialPort();
        TransmitBase Tb = new TransmitBase();
        byte[] SerBuf = new byte[200];
        #endregion
        /// <summary>
        /// Main 构造函数
        /// </summary>
        #region Constructors
        public FrmMain()
        {
            InitializeComponent();
            
        }
        #endregion

        #region Initialize
        //串口初始化

        #endregion

        #region Settings

        #endregion

        #region Parameters

        #endregion

        #region Test
        private void btnCom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbCom.SelectedIndex == -1)
                {
                    MessageBox.Show("请选择COM口", "系统提示");
                }
                else
                {
                    if (Port.IsOpen == true)
                        Port.Close();
                    Port.PortName = cmbCom.SelectedItem.ToString().Trim();
                    Port.Parity = 0;
                    Port.BaudRate = 19200;
                    Port.StopBits = StopBits.Two;
                    Port.DataBits = 8;
                    Port.ReadTimeout = 100;
                    Port.WriteTimeout = 100;

                    Port.Open();
                    string msg = Tb.IsPortReady(Port, SerBuf) + DateTime.Now.ToShortTimeString();
                    if (msg == null)
                        tbDisplay.Text = "串口初始化失败";
                    else
                    {
                        tbDisplay.Text = msg;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示");
            }
        }


        #endregion
        #region WindowEvents
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //设备初始化
                
                Hp8153A = new HP8153A("12");
                Hp8156A = new HP8156A("28");
                //Keith = new Keithley("24");
                //Mp2100A = new MP2100A("22");

                string[] strCom = SerialPort.GetPortNames();
                cmbCom.ItemsSource = strCom;
                cmbAttWaveLength.ItemsSource = WaveLength;
                cmbMeterWaveLength.ItemsSource = WaveLength;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示");
            }
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //关闭GPIB 通信
            //关闭串口通信
            if (Port.IsOpen == true)
            {
                Port.Close();
                Port.Dispose();
            }
        }
        #endregion

        #region GPIBOperation

        #endregion

        #region I2C_Operation
        /// <summary>
        /// 通过I2C获取温度，Vcc,Bais,TxPower,RxPower
        /// </summary>
        void GetParas()
        {
            if (Port == null || Port.IsOpen == false)
            {
                MessageBox.Show("请先初始化COM口", "系统提示");
            }
            else
            {
                //温度计算 96，97
                double temp, vcc, txPower, rxPower, bais;
                short cache = 0;
                ushort ucache = 0;
                List<byte> data = Tb.MyI2C_ReadA2HByte(SerBuf, Port, 96, 10);
                cache = DigitTransform(data[0], data[1]);
                temp = (double)cache / 256;
                ParaModel.Temperature = temp.ToString() + "℃";
                //Vcc 98，99
                ucache = UDigitTransform(data[2], data[3]);
                vcc = (double)ucache / 10000; //V
                ParaModel.Vcc = vcc.ToString() + "V";

                //Bais 100,101
                ucache = UDigitTransform(data[4], data[5]);
                bais = (double)ucache / 500;
                ParaModel.Bais = bais.ToString() + "mA";
                //TxPower 102,103
                ucache = UDigitTransform(data[6], data[7]);
                txPower = (double)ucache / 10000; //mW
                ParaModel.TxPower = (Math.Log10(txPower) * 10).ToString() + "dBm";
                //RxPower 104,105
                ucache = UDigitTransform(data[8], data[9]);
                rxPower = (double)ucache / 10000; //mW
                ParaModel.RxPower = (Math.Log10(rxPower) * 10).ToString() + "dBm";
            }
        }
        void GetRxPower(string attenuation)
        {

            if (Port == null || Port.IsOpen == false)
            {
                MessageBox.Show("请先初始化COM口", "系统提示");
                return;
            }
            else
            {
                ushort ucache;
                double rxPower;
                List<byte> data = new List<byte>();
                data = Tb.MyI2C_ReadA2HByte(SerBuf, Port, 104, 2);
                ucache = UDigitTransform(data[0], data[1]);
                rxPower = (double)ucache / 10000;
                ParaModel.RxPower = (Math.Log10(rxPower) * 10).ToString() + "dBm";

            }

        }
        /// <summary>
        /// 获取short数据
        /// </summary>
        /// <param name="msb">高八位</param>
        /// <param name="lsb">低八位</param>
        /// <returns></returns>
        short DigitTransform(byte msb, byte lsb)
        {
            short num = (short)((short)msb * 256 + (short)lsb);
            return num;
        }
        /// <summary>
        /// 获取ushort 数据
        /// </summary>
        /// <param name="msb">高八位</param>
        /// <param name="lsb">低八位</param>
        /// <returns></returns>
        ushort UDigitTransform(byte msb, byte lsb)
        {
            ushort num = (ushort)((ushort)msb * 256 + (ushort)lsb);
            return num;
        }

        #endregion

        private void btnGetValue_Click(object sender, RoutedEventArgs e)
        {
            if (Port == null || Port.IsOpen == false)
            {
                MessageBox.Show("请先初始化COM口", "系统提示");
                
            }
            else
            {
                Hp8156A.SetAtt("10");
                this.GetParas();
                tbTemp.Text = ParaModel.Temperature;
                tbBais.Text = ParaModel.Bais;
                tbRx1.Text = ParaModel.RxPower;
                Hp8156A.SetAtt("19");
                tbRx2.Text = ParaModel.RxPower;
                Hp8156A.SetAtt("28");
                tbRx3.Text = ParaModel.RxPower;
                //tbER.Text= Mp2100A.GetER();
                //tbCrossing.Text = Mp2100A.GetCrossing();
                

            }
        }

        private void btnMeterSet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Hp8153A.Initialize();
                Hp8153A.SetWaveLength(cmbMeterWaveLength.SelectedItem.ToString());
                Hp8153A.SetCalibration(tbMeterCal.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示");
            }
        }

        private void btnAttSet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Hp8156A.SetWaveLength(cmbAttWaveLength.SelectedItem.ToString());
                Hp8156A.SetAtt(tbAttAtt.Text.Trim());
                Hp8156A.SetCal(tbAttCal.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示");
            }
            
        }
    }
}
