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
                    Port.PortName = cmbCom.SelectedItem.ToString().Trim();
                    Port.PortName = "COM3";
                    Port.Parity = 0;
                    Port.BaudRate = 19200;
                    Port.StopBits = StopBits.Two;
                    Port.DataBits = 8;
                    Port.ReadTimeout = 100;
                    Port.WriteTimeout = 100;
                    if (Port.IsOpen == false)
                        Port.Open();
                    string msg = Tb.IsPortReady(Port, SerBuf);
                    if ( msg== null)
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
            if (Port.IsOpen == true)
            {
                Port.Close();
                Port.Dispose();
            }
           
        }

        private void tbStartAdd_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter&&!string.IsNullOrEmpty(this.tbStartAdd.Text.Trim()))
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

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
