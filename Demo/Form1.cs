using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.IO;

namespace Demo
{
    public partial class myForm : Form
    {

        private string btn_sender = "";
        private const int read_num = 99;
        private const int read_num2 = 60;

        private Boolean received = false;

        public myForm()
        {
            InitializeComponent();
        }

        private void myForm_Load(object sender, EventArgs e)
        {
            comboBoxPortName.SelectedIndex = 0;
            comboBoxBaudRate.SelectedIndex = 6;
            comboBoxParity.SelectedIndex = 1;
            comboBoxDataBits.SelectedIndex = 0;
            comboBoxStopBits.SelectedIndex = 0;
            combo3.SelectedIndex = 1;
            TxtMachAdd.Text = "1";
            btnClose.Enabled = false;
            btnOpen.Enabled = true;
            btnsend.Enabled = false;
            btnread.Enabled = false;
            btnswitch.Enabled = false;
            btnget.Enabled = false;
            btnmulsend.Enabled = false;

            textBoxInformation.Text = "系统初始化成功!\r\n";
        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            String myParity;
            String myStopBits;

            myParity = comboBoxParity.SelectedItem.ToString();
            myStopBits = comboBoxStopBits.SelectedItem.ToString();

            //设置端口号
            mySerialPort.PortName = comboBoxPortName.SelectedItem.ToString();
            //设置波特率
            mySerialPort.BaudRate = Convert.ToInt32(comboBoxBaudRate.SelectedItem);

            //设置校验位
            switch (myParity)
            {
                case "None":
                    mySerialPort.Parity = Parity.None;
                    break;
                case "Even":
                    mySerialPort.Parity = Parity.Even;
                    break;
                case "Odd":
                    mySerialPort.Parity = Parity.Odd;
                    break;
                default:
                    mySerialPort.Parity = Parity.None;
                    break;
            }

            //设置数据位
            mySerialPort.DataBits = Convert.ToInt32(comboBoxDataBits.SelectedItem);

            //设置停止位
            switch (myStopBits)
            {
                case "1":
                    mySerialPort.StopBits = StopBits.One;
                    break;
                case "2":
                    mySerialPort.StopBits = StopBits.Two;
                    break;
                default:
                    mySerialPort.StopBits = StopBits.One;
                    break;

            }
            //采用ASCII编码方式
            mySerialPort.Encoding = Encoding.ASCII;
            //接收到一个字符就触发接收事件
            mySerialPort.ReceivedBytesThreshold = 1;

            //试图打开指定串口
            try
            {
                if (mySerialPort.IsOpen == false)
                {
                    mySerialPort.Open();        //打开串口
                    btnClose.Enabled = true;    //关闭按键失能
                    btnOpen.Enabled = false;    //打开按键失能
                    btnsend.Enabled = true;
                    btnread.Enabled = true;
                    btnswitch.Enabled = true;
                    btnget.Enabled = true;
                    btnmulsend.Enabled = true;

                    comboBoxBaudRate.Enabled = false;
                    comboBoxDataBits.Enabled = false;
                    comboBoxParity.Enabled = false;
                    comboBoxPortName.Enabled = false;
                    comboBoxStopBits.Enabled = false;

                    textBoxInformation.AppendText("串口已打开\r\n");
                    lbl1.Text = "串口已开启，可以进行通讯";
                    lbl2.Text = "串口已开启，可以进行通讯";
                    lbl3.Text = "串口已开启，可以进行通讯";
                    lbl4.Text = "串口已开启，可以进行通讯";
                    lbl5.Text = "串口已开启，可以进行通讯";
                    textBoxInformation.ScrollToCaret();

                }
            }
            //打开异常，输出提示信息
            catch
            {
                MessageBox.Show("串口异常，无法打开", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (mySerialPort.IsOpen == true)
            {
                mySerialPort.Close();       //关闭串口
                btnClose.Enabled = false;
                btnOpen.Enabled = true;
                btnsend.Enabled = false;
                btnread.Enabled = false;
                btnswitch.Enabled = false;
                btnget.Enabled = false;
                btnmulsend.Enabled = false;

                comboBoxBaudRate.Enabled = true;
                comboBoxDataBits.Enabled = true;
                comboBoxParity.Enabled = true;
                comboBoxPortName.Enabled = true;
                comboBoxStopBits.Enabled = true;

                textBoxInformation.AppendText("串口已关闭\r\n");
                lbl1.Text = "串口未开启，不能进行通讯";
                lbl2.Text = "串口未开启，不能进行通讯";
                lbl3.Text = "串口未开启，不能进行通讯";
                lbl4.Text = "串口未开启，不能进行通讯";
                lbl5.Text = "串口未开启，不能进行通讯";
                textBoxInformation.ScrollToCaret();
            }
        }

        private void mySerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] b = new byte[mySerialPort.BytesToRead];//定义byte数组,serialPort读取数据               

            mySerialPort.Read(b, 0, b.Length);

            string str = "";
            for (int i = 0; i < b.Length; i++)
            {
                str += string.Format("{0:X2} ", b[i]);
            }

            //在拥有此控件的基础窗口句柄的线程上执行委托Invoke(Delegate)
            //即在控件textBoxInformation的父窗口form中执行委托.
            textBoxInformation.Invoke
            (
                new MethodInvoker
                (
                    delegate
                    {
                     //   textBoxInformation.AppendText(str);
                     //   textBoxInformation.AppendText("\r\n");

                        switch (btn_sender)
                        {
                            case "写入":
                                txtRcv1.AppendText(str);
                                lbl1.Text = "写单路寄存器通讯正常";
                                break;
                            case "读取":
                                txtRcv2.AppendText(str);
                                lbl2.Text = "读取寄存器通讯正常";
                                break;
                            case "遥控":
                                txtRcv3.AppendText(str);
                                lbl3.Text = "写入开关通讯正常";
                                break;
                            case "获取":
                                txtRcv4.AppendText(str);
                                lbl4.Text = "获取开关通讯正常";
                                break;
                            case "多路写入":
                                txtRcv5.AppendText(str);
                                lbl5.Text = "多路写入通讯正常";
                                break;
                        }
                    }
                )
             );

            str = null;

            received = true;
        }


        public static void formatstring(string strinput, int length, out string stroutput, out Boolean Valid)
        {
            stroutput = "";
            Valid = true;
            byte temp;

            if  ((strinput.Length <= length) & (strinput.Length > 0))
            {
                for (int i = 0; i < strinput.Length; i++)
                {
                    try
                    {
                        temp = Convert.ToByte(strinput[i].ToString(), 16);
                    }

                    catch
                    {
                        Valid = false;
                        stroutput = "";
                        break;
                    }

                    stroutput += strinput[i];
                }

                if (Valid & (strinput.Length < length))
                {
                    for (int j = 0; j < length - strinput.Length; j++)
                    {
                        stroutput = "0" + stroutput;
                    }
                }
            }

            else
            {
                Valid = false;
                stroutput = "";
            }
        }


        private void btnsend_Click(object sender, EventArgs e)
        {
            btn_sender = ((Button)sender).Text.ToString();
            byte[] defByte = new byte[6];
            txtRcv1.Text = "";

            //设备号
            string str1x = TxtMachAdd.Text.ToString();
            string str1 = "";
            Boolean Macvalid1;

            formatstring(str1x, 2, out str1, out Macvalid1);

            if (!Macvalid1)
            {
                MessageBox.Show("设备地址数值不符合规范，最多输入2位十六进制数", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;

            }

            byte[] numbyte1 = this.mysendb(str1);
            defByte[0] = numbyte1[0];

            //功能码 - 06
            string fun_str1 = "06";
            byte[] fun_numbyte1 = this.mysendb(fun_str1);
            defByte[1] = fun_numbyte1[0];

            //地址
            string str2x = txtadd1.Text.ToString();
            string str2 = "";
            Boolean addrvalid1;

            formatstring(str2x, 4, out str2, out addrvalid1);

            if (!addrvalid1)
            {
                MessageBox.Show("寄存器地址数值不符合规范，最多输入4位十六进制数", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // textBoxInformation.AppendText(str2);

            byte[] numbyte2 = this.mysendb(str2);
            defByte[2] = numbyte2[0];
            defByte[3] = numbyte2[1];

            //写入的数据
            string str3x = TxtValue1.Text.ToString();
            string str3 = "";
            Boolean valuevalid1;

            formatstring(str3x, 4, out str3, out valuevalid1);

            if (!valuevalid1)
            {
                MessageBox.Show("输入数值不符合规范，最多输入4位十六进制数", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            byte[] numbyte3 = this.mysendb(str3);
            defByte[4] = numbyte3[0];
            defByte[5] = numbyte3[1];

            //计算CRC
            byte crch = 0;
            byte crcl = 0;

            CalculateCRC(defByte, defByte.Length, out crch, out crcl);

            // MOVE给新的数组
            byte[] rebyte = new byte[defByte.Length + 2];

            for (int i = 0; i < defByte.Length; i++)
            {
                rebyte[i] = defByte[i];
            }

            rebyte[6] = crcl;
            rebyte[7] = crch;

            received = false; 
            this.mySerialPort.Write(rebyte, 0, rebyte.Length); // 发送 

            //起动计时器
            timer1.Enabled = true;

            textBoxInformation.AppendText("发送成功\r\n");


            //将发送数据显示在TEXTBOX中
            txtsend1.Text = "";

            string strSend = "";

            for (int i = 0; i < rebyte.Length; i++)
            {
                strSend += string.Format("{0:X2} ", rebyte[i]);
            }

            this.txtsend1.Text = strSend;

            strSend = null;
        }

        private void btnread_Click(object sender, EventArgs e)
        {
            btn_sender = ((Button)sender).Text.ToString();
            byte[] defByte = new byte[6];
            txtRcv2.Text = "";

            //设备号
            string str1x_03 = TxtMachAdd.Text.ToString();
            string str1_03 = "";
            Boolean Macvalid1_03;

            formatstring(str1x_03, 2, out str1_03, out Macvalid1_03);

            if (!Macvalid1_03)
            {
                MessageBox.Show("设备地址数值不符合规范，最多输入2位十六进制数", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            byte[] numbyte1_03 = this.mysendb(str1_03);
            defByte[0] = numbyte1_03[0];

            //功能码 - 03
            string fun_str1_03 = "03";
            byte[] fun_numbyte1_03 = this.mysendb(fun_str1_03);
            defByte[1] = fun_numbyte1_03[0];

            //起始地址
            string str2x_03 = txtadd2.Text.ToString();
            string str2_03 = "";
            Boolean addrvalid1_03;

            formatstring(str2x_03, 4, out str2_03, out addrvalid1_03);

            if (!addrvalid1_03)
            {
                MessageBox.Show("寄存器地址数值不符合规范，最多输入4位十六进制数", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            byte[] numbyte2_03 = this.mysendb(str2_03);
            defByte[2] = numbyte2_03[0];
            defByte[3] = numbyte2_03[1];

            // 数据数量（长度）

            string str_num_03 = this.txtnum2.Text.ToString();
            Boolean num_valid_03 = true;
            try
            {
                //验证是否是有效的数字
                int num_03 = Convert.ToInt16(str_num_03);
                Boolean bo_num = (Convert.ToInt16(str_num_03) > read_num);
            }

            catch
            {
                num_valid_03 = false;
            }


            if (!num_valid_03)
            {
                MessageBox.Show("输入数量不符合规范，请重新输入十进制数字", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //数量过多，大于read_num = 99;            
            if ((Convert.ToInt16(str_num_03) > read_num) | (Convert.ToInt16(str_num_03) == 0))
                num_valid_03 = false;


            if (!num_valid_03)
            {
                MessageBox.Show("输入数量过大或为0，请重新输入十进制数字", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //    textBoxInformation.AppendText(Convert.ToInt16(str_num_03).ToString());
            string str_num2_03 = Convert.ToString(Convert.ToInt16(str_num_03), 16);
            //     textBoxInformation.AppendText(str_num2_03);

            string str3_03 = "";
            Boolean value_valid_03;

            formatstring(str_num2_03, 4, out str3_03, out value_valid_03);

            if (value_valid_03)
            {
                byte[] numbyte3_03 = this.mysendb(str3_03);
                defByte[4] = numbyte3_03[0];
                defByte[5] = numbyte3_03[1];

                //计算CRC
                byte crch = 0;
                byte crcl = 0;

                CalculateCRC(defByte, defByte.Length, out crch, out crcl);

                // MOVE给新的数组
                byte[] rebyte = new byte[defByte.Length + 2];

                for (int i = 0; i < defByte.Length; i++)
                {
                    rebyte[i] = defByte[i];
                }

                rebyte[6] = crcl;
                rebyte[7] = crch;

                received = false; 
                this.mySerialPort.Write(rebyte, 0, rebyte.Length); // 发送 
                //起动计时器
                timer1.Enabled = true;

                textBoxInformation.AppendText("读取成功\r\n");

                //将发送数据显示在TEXTBOX中
                txtsend2.Text = "";

                string strSend = "";

                for (int i = 0; i < rebyte.Length; i++)
                {
                    strSend += string.Format("{0:X2} ", rebyte[i]);
                }

                this.txtsend2.Text = strSend;

                strSend = null;
            }
        }

        private void btnswitch_Click(object sender, EventArgs e)
        {
            btn_sender = ((Button)sender).Text.ToString();
            byte[] defByte = new byte[6];
            txtRcv3.Text = "";
            lbl3_a1.Text = "";
            lbl3_a2.Text = "";
            lbl3_a3.Text = "";

            //设备号
            string str1x_05 = TxtMachAdd.Text.ToString();
            string str1_05 = "";
            Boolean Macvalid1_05;

            formatstring(str1x_05, 2, out str1_05, out Macvalid1_05);

            if (!Macvalid1_05)
            {
                MessageBox.Show("设备地址数值不符合规范，最多输入2位十六进制数", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            byte[] numbyte1_05 = this.mysendb(str1_05);
            defByte[0] = numbyte1_05[0];

            //功能码 - 05
            string fun_str1_05 = "05";
            byte[] fun_numbyte1_05 = this.mysendb(fun_str1_05);
            defByte[1] = fun_numbyte1_05[0];

            //地址
            string str2x_05 = txtadd3.Text.ToString();
            string str2_05 = "";
            Boolean addrvalid1_05;

            formatstring(str2x_05, 4, out str2_05, out addrvalid1_05);

            if (!addrvalid1_05)
            {
                MessageBox.Show("寄存器地址数值不符合规范，最多输入4位十六进制数", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            byte[] numbyte2_05 = this.mysendb(str2_05);
            defByte[2] = numbyte2_05[0];
            defByte[3] = numbyte2_05[1];

            string lbl_1 = "";
            string lbl_2 = "";
            string lbl_3 = "";

            address_switch(str2_05, out lbl_1, out lbl_2, out lbl_3);

            lbl3_a1.Text = lbl_1;
            lbl3_a2.Text = lbl_2;
            lbl3_a3.Text = lbl_3;

            //选择开关 ”合 - 1“ 。”分 - 0“
            string switch_05;
            string str3_05 = "";

            switch_05 = combo3.SelectedItem.ToString();

            switch (switch_05)
            {
                case "合 - 1":
                    str3_05 = "FF00";
                    break;
                case "分 - 0":
                    str3_05 = "0000";
                    break;
            }

            byte[] numbyte3_05 = this.mysendb(str3_05);
            defByte[4] = numbyte3_05[0];
            defByte[5] = numbyte3_05[1];

            //计算CRC
            byte crch = 0;
            byte crcl = 0;

            CalculateCRC(defByte, defByte.Length, out crch, out crcl);

            // MOVE给新的数组
            byte[] rebyte = new byte[defByte.Length + 2];

            for (int i = 0; i < defByte.Length; i++)
            {
                rebyte[i] = defByte[i];
            }

            rebyte[6] = crcl;
            rebyte[7] = crch;

            received = false; 
            this.mySerialPort.Write(rebyte, 0, rebyte.Length); // 发送 
            //起动计时器
            timer1.Enabled = true;

            textBoxInformation.AppendText("发送成功\r\n");

            //将发送数据显示在TEXTBOX中
            txtsend3.Text = "";

            string strSend = "";

            for (int i = 0; i < rebyte.Length; i++)
            {
                strSend += string.Format("{0:X2} ", rebyte[i]);
            }

            this.txtsend3.Text = strSend;

            strSend = null;
        }

        private void btnget_Click(object sender, EventArgs e)
        {
            btn_sender = ((Button)sender).Text.ToString();
            byte[] defByte = new byte[6];
            txtRcv4.Text = "";

            //设备号
            string str1x_01 = TxtMachAdd.Text.ToString();
            string str1_01 = "";
            Boolean Macvalid1_01;

            formatstring(str1x_01, 2, out str1_01, out Macvalid1_01);

            if (!Macvalid1_01)
            {
                MessageBox.Show("寄存器地址数值不符合规范，最多输入4位十六进制数", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            byte[] numbyte1_01 = this.mysendb(str1_01);
            defByte[0] = numbyte1_01[0];
            //功能码 - 01
            string fun_str1_01 = "01";
            byte[] fun_numbyte1_01 = this.mysendb(fun_str1_01);
            defByte[1] = fun_numbyte1_01[0];

            //起始地址
            string str2x_01 = txtadd4.Text.ToString();
            string str2_01 = "";
            Boolean addrvalid1_01;

            formatstring(str2x_01, 4, out str2_01, out addrvalid1_01);

            if (!addrvalid1_01)
            {
                MessageBox.Show("寄存器地址数值不符合规范，最多输入4位十六进制数", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            byte[] numbyte2_01 = this.mysendb(str2_01);
            defByte[2] = numbyte2_01[0];
            defByte[3] = numbyte2_01[1];

            string lbl_1 = "";
            string lbl_2 = "";
            string lbl_3 = "";

            address_switch(str2_01, out lbl_1, out lbl_2, out lbl_3);

            lbl4_a1.Text = lbl_1;
            lbl4_a2.Text = lbl_2;
            lbl4_a3.Text = lbl_3;

            // 数据数量（长度）
            string str_num_01 = this.txtnum4.Text.ToString();
            Boolean num_valid_01 = true;
            try
            {
                //验证是否是有效的数字
                int num_01 = Convert.ToInt16(str_num_01);
                Boolean bo_num = (Convert.ToInt16(str_num_01) > read_num);
            }

            catch
            {
                num_valid_01 = false;
            }

            if (!num_valid_01)
            {
                MessageBox.Show("输入数量不符合规范，请重新输入十进制数字", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //数量过多，大于read_num = 99;
            if ((Convert.ToInt16(str_num_01) > read_num) | (Convert.ToInt16(str_num_01) == 0))
                num_valid_01 = false;

            if (!num_valid_01)
            {
                MessageBox.Show("输入数量过大或为0，请重新输入十进制数字", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //    textBoxInformation.AppendText(Convert.ToInt16(str_num_01).ToString());
            string str_num2_01 = Convert.ToString(Convert.ToInt16(str_num_01), 16);
            //     textBoxInformation.AppendText(str_num2_01);

            string str3_01 = "";
            Boolean value_valid_01;

            formatstring(str_num2_01, 4, out str3_01, out value_valid_01);

            if (value_valid_01)
            {
                byte[] numbyte3_01 = this.mysendb(str3_01);
                defByte[4] = numbyte3_01[0];
                defByte[5] = numbyte3_01[1];

                //计算CRC
                byte crch = 0;
                byte crcl = 0;

                CalculateCRC(defByte, defByte.Length, out crch, out crcl);

                // MOVE给新的数组
                byte[] rebyte = new byte[defByte.Length + 2];

                for (int i = 0; i < defByte.Length; i++)
                {
                    rebyte[i] = defByte[i];
                }

                rebyte[6] = crcl;
                rebyte[7] = crch;

                received = false;
                this.mySerialPort.Write(rebyte, 0, rebyte.Length); // 发送 
                //起动计时器
                timer1.Enabled = true;

                textBoxInformation.AppendText("读取成功\r\n");

                //将发送数据显示在TEXTBOX中
                txtsend4.Text = "";

                string strSend = "";

                for (int i = 0; i < rebyte.Length; i++)
                {
                    strSend += string.Format("{0:X2} ", rebyte[i]);
                }

                this.txtsend4.Text = strSend;

                strSend = null;
            }
        }

        private void btnmulsend_Click(object sender, EventArgs e)
        {
            btn_sender = ((Button)sender).Text.ToString();
            txtRcv5.Text = "";

            //设备号
            string str1x_10 = TxtMachAdd.Text.ToString();
            string str1_10 = "";
            Boolean Macvalid1_10;

            formatstring(str1x_10, 2, out str1_10, out Macvalid1_10);

            if (!Macvalid1_10)
            {
                MessageBox.Show("设备地址数值不符合规范，最多输入2位十六进制数", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //功能码 - 10
            string fun_str1_10 = "10";

            //起始地址
            string str2x_10 = txtadd5.Text.ToString();
            string str2_10 = "";
            Boolean addrvalid1_10;

            formatstring(str2x_10, 4, out str2_10, out addrvalid1_10);

            if (!addrvalid1_10)
            {
                MessageBox.Show("寄存器地址数值不符合规范，最多输入4位十六进制数", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 数据数量（长度） - 2 byte
            string str_num_10 = this.txtnum5.Text.ToString();
            Boolean num_valid_10 = true;

            try
            {
                //验证是否是有效的数字
                int num_10_x = Convert.ToInt16(str_num_10);
                Boolean bo_num = (num_10_x > read_num2);
            }

            catch
            {
                num_valid_10 = false;
            }

            if (!num_valid_10)
            {
                MessageBox.Show("输入数量不符合规范，请重新输入十进制数字", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //数量过多，大于read_num2 = 60;

            if ((Convert.ToInt16(str_num_10) > read_num2) | (Convert.ToInt16(str_num_10) == 0))
                num_valid_10 = false;

            if (!num_valid_10)
            {
                MessageBox.Show("输入数量太大或为0，最多输入60，请重新输入", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int num_10 = Convert.ToInt16(str_num_10);

            byte[] defByte = new byte[7 + 2 * num_10];

            byte[] numbyte1_10 = this.mysendb(str1_10);
            defByte[0] = numbyte1_10[0];

            byte[] fun_numbyte1_10 = this.mysendb(fun_str1_10);
            defByte[1] = fun_numbyte1_10[0];

            byte[] numbyte2_10 = this.mysendb(str2_10);
            defByte[2] = numbyte2_10[0];
            defByte[3] = numbyte2_10[1];

            string str_num2_10 = Convert.ToString(num_10, 16);

            string str3_10 = "";
            Boolean value_valid_10;

            formatstring(str_num2_10, 4, out str3_10, out value_valid_10);

            byte[] numbyte3_10 = this.mysendb(str3_10);
            defByte[4] = numbyte3_10[0];
            defByte[5] = numbyte3_10[1];

            //字节长度 - 1 byte

            string str_num3_10 = Convert.ToString(num_10 * 2, 16);

            string strx_10 = "";
            Boolean value_valid_x_10;

            formatstring(str_num3_10, 2, out strx_10, out value_valid_x_10);

            if (value_valid_x_10)
            {
                byte[] numbyte4_10 = this.mysendb(strx_10);
                defByte[6] = numbyte4_10[0];

                //读取 数据 num_10 个

                string valueinput = txtvalue5.Text.ToString();
                string[] inputtext = valueinput.Split('/');

               // textBoxInformation.AppendText(inputtext.Length.ToString());
               // textBoxInformation.AppendText("-----\r\n");

                if (num_10 > Convert.ToInt16(inputtext.Length.ToString()))
                {
                    MessageBox.Show("输入的值的数量过少，请重新输入！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Boolean all_value_valid_10 = true;

                for (int i = 0; i < num_10; i++)
                {
                    string str = inputtext[num_10 - i - 1];
                    string str_10 = "";

                    formatstring(str, 4, out str_10, out all_value_valid_10);

                    //    textBoxInformation.AppendText(str_10);
                    //    textBoxInformation.AppendText("-----\r\n");

                    if (all_value_valid_10)
                    {
                        byte[] numbyte5_10 = this.mysendb(str_10);
                        defByte[6 + (2 * num_10 - 1) - 2 * i] = numbyte5_10[0];

                        //        textBoxInformation.AppendText(defByte[6 + (2 * num_10 - 1) - 2 * i].ToString());
                        //        textBoxInformation.AppendText("-----\r\n");

                        defByte[6 + (2 * num_10 - 1) - 2 * i + 1] = numbyte5_10[1];

                        //        textBoxInformation.AppendText(defByte[6 + (2 * num_10 - 1) - 2 * i + 1].ToString());
                        //        textBoxInformation.AppendText("-----\r\n");

                    }
                    else
                        break;
                }

                if (!all_value_valid_10)
                {

                    MessageBox.Show("输入数值不符合规范，每个最多输入4位十六进制数", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //计算CRC
                byte crch = 0;
                byte crcl = 0;

                CalculateCRC(defByte, defByte.Length, out crch, out crcl);

                // MOVE给新的数组
                byte[] rebyte = new byte[defByte.Length + 2];

                for (int i = 0; i < defByte.Length; i++)
                {
                    rebyte[i] = defByte[i];
                }

                rebyte[2 * num_10 + 7] = crcl;
                rebyte[2 * num_10 + 8] = crch;

                received = false; 
                this.mySerialPort.Write(rebyte, 0, rebyte.Length); // 发送 
                //起动计时器
                timer1.Enabled = true;

                textBoxInformation.AppendText("发送成功\r\n");

                //将发送数据显示在TEXTBOX中
                txtsend5.Text = "";

                string strSend = "";

                for (int i = 0; i < rebyte.Length; i++)
                {
                    strSend += string.Format("{0:X2} ", rebyte[i]);
                }

                this.txtsend5.Text = strSend;

                strSend = null;
            }

        }

        public void address_switch(string addr,out string lbl1,out string lbl2, out string lbl3)
        {
            int aa = Convert.ToInt32(addr, 16);
            if ((aa >= Convert.ToInt32("0000", 16)) && (aa <= Convert.ToInt32("3fff", 16)))
            {
                lbl1 = "M0";
                lbl2 = "M" + (aa / 10).ToString() + "0";
                lbl3 = "+" + (aa % 10).ToString();

            }
            else
                if ((aa >= Convert.ToInt32("4000", 16)) && (aa <= Convert.ToInt32("47ff", 16)))
                {
                    lbl1 = "X0";
                    aa -= Convert.ToInt32("4000", 16);
                    lbl2 = "X" + (aa / 8).ToString() + "0";
                    lbl3 = "+" + (aa % 8).ToString();
                }
                else
                    if ((aa >= Convert.ToInt32("4800", 16)) && (aa <= Convert.ToInt32("4fff", 16)))
                    {
                        lbl1 = "Y0";
                        aa -= Convert.ToInt32("4800", 16);
                        lbl2 = "Y" + (aa / 8).ToString() + "0";
                        lbl3 = "+" + (aa % 8).ToString();
                    }
                    else
                        if ((aa >= Convert.ToInt32("5000", 16)) && (aa <= Convert.ToInt32("5fff", 16)))
                        {
                            lbl1 = "S0";
                            aa -= Convert.ToInt32("5000", 16);
                            lbl2 = "S" + (aa / 10).ToString() + "0";
                            lbl3 = "+" + (aa % 10).ToString();
                        }
                        else
                            if ((aa >= Convert.ToInt32("6000", 16)) && (aa <= Convert.ToInt32("63ff", 16)))
                            {
                                lbl1 = "M800";
                                aa -= Convert.ToInt32("6000", 16);
                                lbl2 = "M8" + (aa / 10).ToString() + "0";
                                lbl3 = "+" + (aa % 10).ToString();
                            }
                            else
                                if ((aa >= Convert.ToInt32("6400", 16)) && (aa <= Convert.ToInt32("6bff", 16)))
                                {
                                    lbl1 = "T0";
                                    aa -= Convert.ToInt32("6400", 16);
                                    lbl2 = "T" + (aa / 10).ToString() + "0";
                                    lbl3 = "+" + (aa % 10).ToString();
                                }
                                else
                                    if ((aa >= Convert.ToInt32("6c00", 16)) && (aa <= Convert.ToInt32("73ff", 16)))
                                    {
                                        lbl1 = "C0";
                                        aa -= Convert.ToInt32("6c00", 16);
                                        lbl2 = "C" + (aa / 10).ToString() + "0";
                                        lbl3 = "+" + (aa % 10).ToString();
                                    }
                                    else
                                    {
                                        lbl1 = "null";
                                        lbl2 = "null";
                                        lbl3 = "null";
                                    }
        }
        

        //打包方法，可以将字符串转成byte[] 
        public byte[] mysendb(string s)
        {
            string temps = delspace(s);

            if (temps.Length % 2 != 0)
            {
                temps = "0" + temps;
            }

            byte[] tempb = new byte[50];
            int j = 0;

            for (int i = 0; i < temps.Length; i = i + 2, j++)
            {
                tempb[j] = Convert.ToByte(temps.Substring(i, 2), 16);
            }

            byte[] send = new byte[j];
            Array.Copy(tempb, send, j);
            return send;
        }

        //除去空格
        public string delspace(string putin)
        {
            string putout = "";

            for (int i = 0; i < putin.Length; i++)
            {
                if (putin[i] != ' ')
                    putout += putin[i];
            }

            return putout;
        }

        public static void CalculateCRC(byte[] pByte, int nNumberOfBytes, out ushort pChecksum)
        {
            int nBit;
            ushort nShiftedBit;
            pChecksum = 0xFFFF;

            for (int nByte = 0; nByte < nNumberOfBytes; nByte++)
            {
                pChecksum ^= pByte[nByte];
                for (nBit = 0; nBit < 8; nBit++)
                {
                    if ((pChecksum & 0x1) == 1)
                    {
                        nShiftedBit = 1;
                    }
                    else
                    {
                        nShiftedBit = 0;
                    }
                    pChecksum >>= 1;
                    if (nShiftedBit != 0)
                    {
                        pChecksum ^= 0xA001;
                    }
                }
            }
        }

        public static void CalculateCRC(byte[] pByte, int nNumberOfBytes, out byte hi, out byte lo)
        {
            ushort sum;
            CalculateCRC(pByte, nNumberOfBytes, out sum);
            lo = (byte)(sum & 0xFF);
            hi = (byte)((sum & 0xFF00) >> 8);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!received)
            {
                switch (btn_sender)
                {
                    case "写入":
                        lbl1.Text = "写单路寄存器通讯超时，请检查设置";
                        break;
                    case "读取":
                        lbl2.Text = "读取寄存器通讯超时，请检查设置";
                        break;
                    case "遥控":
                        lbl3.Text = "写入开关通讯超时，请检查设置";
                        break;
                    case "获取":
                        lbl4.Text = "获取开关通讯超时，请检查设置";
                        break;
                    case "多路写入":
                        lbl5.Text = "多路写入通讯超时，请检查设置";
                        break;
                }
            }

     //       textBoxInformation.AppendText("hah");
            timer1.Enabled = false;
        }

    }
}
