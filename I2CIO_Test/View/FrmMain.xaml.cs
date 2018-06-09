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
using System.Threading;

namespace I2CIO_Test.View
{
    /// <summary>
    /// FrmMain.xaml 的交互逻辑
    /// </summary>
    public partial class FrmMain : Window
    {
        #region Properties
        readonly string[] WaveLength = { "1270", "1310", "1490" };
        byte[] InstrAdd = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 30 };
        LowTempPara ParaModel = new LowTempPara();
        //设备列表
        //Keithley Keith;
        HP8153A Hp8153A;
        HP8156A Hp8156A;
        MP2100A Mp2100A;
        //I2C参数
        SerialPort Port = new SerialPort();
        TransmitBase Tb = new TransmitBase();
        byte[] SerBuf = new byte[200];
        #endregion

        #region Constructors
        /// <summary>
        /// Main 构造函数
        /// </summary>
        public FrmMain()
        {
            InitializeComponent();

        }
        #endregion
        #region WindowEvents
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {


                string[] strCom = SerialPort.GetPortNames();
                cmbCom.ItemsSource = strCom;
                //波长选项
                cmbAttWaveLength.ItemsSource = WaveLength;
                cmbMeterWaveLength.ItemsSource = WaveLength;
                //地址选项
                cmbAttAdd.ItemsSource = InstrAdd;
                cmbAttAdd.SelectedIndex = 21;
                cmbBertAdd.ItemsSource = InstrAdd;
                cmbBertAdd.SelectedIndex = 1;
                cmbMeterAdd.ItemsSource = InstrAdd;
                cmbMeterAdd.SelectedIndex = 4;
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
        #region ControsEvents


        #endregion

        #region TableControl
        #region Settings

        #endregion

        #region ParaSet

        #endregion

        #region Test

        //光功率计初始化
        private void btnMeterSet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbMeterAdd.SelectedIndex != -1)
                {
                    if (Hp8153A != null)
                        Hp8153A.Dispose();
                    Hp8153A = new HP8153A(cmbMeterAdd.SelectedItem.ToString().Trim());
                    Hp8153A.Initialize();
                    MessageBox.Show(Hp8153A.GetIdn(), "系统提示");
                }
                else
                {
                    MessageBox.Show("请选择地址", "系统提示");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示");
            }
        }
        //光衰减器初始化
        private void btnAttSet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbAttAdd.SelectedIndex != -1)
                {
                    if (Hp8156A != null)
                        Hp8156A.Dispose();
                    Hp8156A = new HP8156A(cmbAttAdd.SelectedItem.ToString().Trim());
                    MessageBox.Show(Hp8156A.GetIdn(),"系统提示");
                }
                else
                {
                    MessageBox.Show("请选择设备地址", "系统提示");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示");
            }

        }
        //眼图仪初始化
        private void btnBertSet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbBertAdd.SelectedIndex != -1)
                {
                    if (Mp2100A != null)
                        Mp2100A.Dispose();
                    Mp2100A = new MP2100A(cmbBertAdd.SelectedItem.ToString().Trim());
                    MessageBox.Show(Mp2100A.GetIdn(), "系统提示");
                }
                else
                {
                    MessageBox.Show("请先设置设备地址", "系统提示");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示");
            }
        }
        //COM初始化
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
        //读取设备数据
        private void btnGetValue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Hp8153A == null || Hp8156A == null || Mp2100A == null)
                {
                    MessageBox.Show("请将所有设备初始化", "系统提示");
                    return;
                }
                if (Port == null || Port.IsOpen == false)
                {
                    MessageBox.Show("请先初始化COM口", "系统提示");

                }
                else
                {
                    Hp8156A.SetAtt("10");
                    //I2C获取数据
                    this.GetParas();
                    //温度
                    tbTemp.Text = ParaModel.Temperature;
                    //Bais
                    tbBais.Text = ParaModel.Bais;
                    //RX@10
                    tbRx1.Text = ParaModel.RxPower;
                    //读取HP8153光功率计功率
                    tbTxPower.Text = Hp8153A.ReadData();
                    
                    Thread.Sleep(200);
                    //RX@19
                    Hp8156A.SetAtt("19");
                    Thread.Sleep(200);
                    this.GetRxPower();
                    tbRx2.Text = ParaModel.RxPower;
                    //Rx@28
                    Hp8156A.SetAtt("28");
                    Thread.Sleep(200);
                    tbRx3.Text = ParaModel.RxPower;
                    //ER
                    tbER.Text = Mp2100A.GetER();
                    Thread.Sleep(200);
                    //Crossing
                    tbCrossing.Text = Mp2100A.GetCrossing();


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统设置");
            }
        }
        #endregion
        #endregion



        #region I2CMethods
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
                ParaModel.RxPower = (Math.Log10(rxPower) * 10).ToString() + "dB";
            }
        }
        void GetRxPower()
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
                ParaModel.RxPower = (Math.Log10(rxPower) * 10).ToString() + "dB";

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

        private void cmbMeterWaveLength_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Hp8153A == null)
            {
                MessageBox.Show("请先初始化光功率计", "系统设置");
                return;
            }
            if(cmbMeterWaveLength.SelectedIndex!=-1)
            {
                Hp8153A.SetWaveLength(cmbMeterWaveLength.SelectedItem.ToString());
            }
        }

        private void tbMeterCal_KeyUp(object sender, KeyEventArgs e)
        {
            if (Hp8153A == null)
            {
                MessageBox.Show("请先初始化光功率计", "系统设置");
                return;
            }
            if (e.Key == Key.Enter && this.tbMeterCal.Text.Trim() != string.Empty)
            {
                Hp8153A.SetCalibration(this.tbMeterCal.Text.Trim());
            }
        }

        private void cmbAttWaveLength_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Hp8156A == null)
            {
                MessageBox.Show("请先初始化衰减器", "系统设置");
                return;
            }
            if (cmbAttWaveLength.SelectedIndex == -1)
            {
                Hp8156A.SetWaveLength(cmbAttWaveLength.SelectedItem.ToString());
            }
        }

        private void tbAttCal_KeyUp(object sender, KeyEventArgs e)
        {

        }
    }
}
