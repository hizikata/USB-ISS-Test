using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace I2CIO_Test.Model
{
    /// <summary>
    /// 低温数据模型
    /// </summary>
    public class LowTempPara
    {
        /// <summary>
        /// SN
        /// </summary>
        public string SN { get; set; }
        /// <summary>
        /// TxPower 光功率计获取
        /// </summary>
        public string TxPower { get; set; }
        /// <summary>
        /// ER
        /// </summary>
        public string ER { get; set; }
        /// <summary>
        /// Crossing
        /// </summary>
        public string Crossing { get; set; }
        /// <summary>
        /// Sen
        /// </summary>
        public string Sensitive { get; set; }
        /// <summary>
        /// RxPower
        /// </summary>
        public string RxPower { get; set; }
        /// <summary>
        /// 温度
        /// </summary>
        public string Temperature { get; set; }
        /// <summary>
        /// Bais
        /// </summary>
        public string Bais { get; set; }
        /// <summary>
        /// Vcc
        /// </summary>
        public string Vcc { get; set; }
    }
}
