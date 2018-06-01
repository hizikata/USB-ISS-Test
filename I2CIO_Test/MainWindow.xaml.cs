using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using I2CIO_Test.Methods;

namespace I2CIO_Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fileds
        /// <summary>
        /// 通信COM口
        /// </summary>
        SerialPort Port = new SerialPort();
        TransmitBase Tb = new TransmitBase();
        string[] StrReadAdd = { "A2H", "A0H" };
        byte[] SerBuf = new byte[200];
        #region Keithley
        byte[] InstrAdd = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 30 };
        string KeyVol, KeyCur;
        #endregion
        #region VISAPara
        static int DefRM, Status;
        static int Vi = 0;
        #endregion
        #endregion
        /// <summary>
        /// 主窗体构造函数
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] strCom = SerialPort.GetPortNames();
            cmbCom.ItemsSource = strCom;
            cmbReadAdd.ItemsSource = StrReadAdd;
            this.btnClose.IsEnabled = false;
            cmbInstrumentAdd.ItemsSource = InstrAdd;
        }

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
                    string msg = Tb.IsPortReady(Port, SerBuf)+DateTime.Now.ToShortTimeString();
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

        private void btnRead_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Port == null || Port.IsOpen == false)
                {
                    MessageBox.Show("请先初始化COM口!", "系统提示");
                }
                else if (cmbReadAdd.SelectedIndex == -1 || string.IsNullOrEmpty(tbStartAdd.Text.Trim())
                    || string.IsNullOrEmpty(tbReadCount.Text.Trim()))
                {
                    MessageBox.Show("信息填写不完整！", "系统提示");
                }
                else
                {
                    byte startAdd, readCount;
                    switch (cmbReadAdd.SelectedItem.ToString())
                    {
                        case "A0H":
                            startAdd = Convert.ToByte(tbStartAdd.Text.Trim());
                            readCount = Convert.ToByte(tbReadCount.Text.Trim());
                            tbDisplay.Text = Tb.MyI2C_ReadA0HByte(SerBuf, Port, startAdd, readCount);
                            break;
                        case "A2H":
                            startAdd = Convert.ToByte(tbStartAdd.Text.Trim());
                            readCount = Convert.ToByte(tbReadCount.Text.Trim());
                            tbDisplay.Text = Tb.MyI2C_ReadA2HByte(SerBuf, Port, startAdd, readCount);
                            break;
                        default:
                            break;
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //关闭GPIB 通信
            if (DefRM != 0)
                visa32.viClose(DefRM);
            if (Vi != 0)
                visa32.viClose(Vi);
            //关闭串口通信
            if (Port.IsOpen == true)
            {
                Port.Close();
                Port.Dispose();
            }

        }

        private void tbStartAdd_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(this.tbStartAdd.Text.Trim()))
            {
                this.tbReadCount.Focus();
            }
        }

        private void tbReadCount_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(this.tbReadCount.Text.Trim()))
            {
                this.btnRead.Focus();
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            KeyVol = tbVoltage.Text.Trim();
            KeyCur = tbCurrent.Text.Trim();
            //设置电压电流
            if (!string.IsNullOrEmpty(KeyVol) && !string.IsNullOrEmpty(KeyCur))
            {
                if (Convert.ToSingle(KeyVol) > 5 || Convert.ToSingle(KeyCur) > 0.6)
                {
                    MessageBox.Show("电流或电压设置过大", "系统提示");
                    return;
                }
                Status = visa32.viPrintf(Vi, ":SOUR:FUNC VOLT\n");
                Status = visa32.viPrintf(Vi, ":SOUR:VOLT:MODE FIX\n");
                Status = visa32.viPrintf(Vi, ":SOUR:VOLT:RANG 10\n");
                Status = visa32.viPrintf(Vi, string.Format(":SOUR:VOLT:LEV {0}\n", KeyVol));
                Status = visa32.viPrintf(Vi, ":SENS:FUNC \"CURR\"\n");

                Status = visa32.viPrintf(Vi, string.Format(":SENS:CURR:PROT {0}\n", KeyCur));
                //设置测量量程
                Status = visa32.viPrintf(Vi, ":SENS:CURR:RANG:AUTO ON\n");

                CheckStatus(Vi, Status);
                //打开设备电源
                Status = visa32.viPrintf(Vi, ":OUTP ON\n");
                Status = visa32.viPrintf(Vi, ":READ?\n");
                Status = visa32.viRead(Vi, out string result, 100);
                CheckStatus(Vi, Status);
                tbKeithInfo.Text = result;
                this.btnClose.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("请设置电流电压", "系统提示");
            }
        }

        private void cmbInstrumentAdd_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (cmbInstrumentAdd.SelectedIndex != -1)
            {
                Status = visa32.viOpenDefaultRM(out DefRM);
                try
                {
                    string add = cmbInstrumentAdd.SelectedItem.ToString().Trim();
                    Status = visa32.viOpen(DefRM, string.Format(@"GPIB0::{0}::INSTR", add),
                        visa32.VI_NO_LOCK, visa32.VI_TMO_IMMEDIATE, out Vi);
                    CheckStatus(Vi, Status);
                    //获取设备信息
                    Status = visa32.viPrintf(Vi, "*RST\n");
                    Status = visa32.viPrintf(Vi, "*IDN?\n");
                    CheckStatus(Vi, Status);
                    Status = visa32.viRead(Vi, out string result, 100);
                    CheckStatus(Vi, Status);
                    tbKeithInfo.Text = result;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "系统提示");
                }
            }

        }

        private void btnSendCommand_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string msg = string.Format("{0}\n", tbCommand.Text.Trim());
                Status = visa32.viPrintf(Vi, msg);
                CheckStatus(Vi, Status);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统设置");
            }
        }

        private void btnReadCommand_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string msg = string.Format("{0}\n", tbCommand.Text.Trim());
                Status = visa32.viRead(Vi, out string result, 100);
                CheckStatus(Vi, Status);
                tbCommonResult.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统设置");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Status = visa32.viPrintf(Vi, ":OUTP OFF\n");
            CheckStatus(Vi, Status);
        }
        #region VISAMethods
        private void CheckStatus(int vi, int status)
        {
            if (status < visa32.VI_SUCCESS)
            {
                StringBuilder err = new StringBuilder(256);
                visa32.viStatusDesc(vi, status, err);
                throw new Exception(err.ToString());
            }
        }
        #endregion
    }
}
