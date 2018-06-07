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
using System.Windows.Shapes;
using I2CIO_Test.Model;

namespace I2CIO_Test.View
{
    /// <summary>
    /// FrmMain.xaml 的交互逻辑
    /// </summary>
    public partial class FrmMain : Window
    {
        #region Properties
        Keithley Keith = new Keithley("24");
        HP8153A Hp8153A = new HP8153A("12");
        HP8156A Hp8156A = new HP8156A("28");
        #endregion
        /// <summary>
        /// Main 构造函数
        /// </summary>
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            string msg=Keith.Identification();
            MessageBox.Show(msg, "系统提示");
        }

        private void btnTest2_Click(object sender, RoutedEventArgs e)
        {
            string msg = Hp8153A.Identification();
            MessageBox.Show(msg, "系统提示");
        }

        private void btnTest3_Click(object sender, RoutedEventArgs e)
        {
            string msg = Hp8156A.Identification();
            MessageBox.Show(msg, "系统提示");
        }
    }
}
