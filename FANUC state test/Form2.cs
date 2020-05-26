using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FANUC_state_test
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        NetworkStream telnetStream_A;
        TcpClient telnet_A;
        Thread t;
        bool telnet_receiving = false;
        string Telnet_out;
        delegate void Display(string s);
        string IP = "120.0.0.1";
        int PORT = 23;

        private void button1_Click(object sender, EventArgs e)
        {
            IP = textBox1.Text;
            PORT = Convert.ToInt16(textBox2.Text);

            Telnet_Connect(IP, PORT);
        }

        private void Telnet_Connect(string ip, int port)
        {
            try
            {
                richTextBox1.Text = "";

                telnet_A = new TcpClient();
                telnet_A.SendTimeout = 1000;
                telnet_A.ReceiveTimeout = 1000;
                telnet_A.Connect(IP, PORT);

                if (telnet_A.Connected)
                {
                    telnetStream_A = telnet_A.GetStream();

                    //開啟執行緒做接收
                    telnet_receiving = true;
                    t = new Thread(Telnet_Read);
                    t.Start();
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowMessage(string buf)
        {
            richTextBox1.AppendText(buf);
        }

        //telnet 讀資料
        private void Telnet_Read()
        {
            try
            {
                while (telnet_receiving)
                {
                    if (telnetStream_A.DataAvailable)
                    {
                        byte[] bytes = new byte[telnet_A.ReceiveBufferSize];
                        int numBytesRead = telnetStream_A.Read(bytes, 0, (int)telnet_A.ReceiveBufferSize);
                        Array.Resize(ref bytes, numBytesRead);

                        Telnet_out = Encoding.ASCII.GetString(bytes);

                        //委派顯示    
                        //Display d = new Display(ShowMessage);
                        //this.Invoke(d, new Object[] { Telnet_out });
                    }

                    Display d = new Display(ShowMessage);
                    this.Invoke(d, new Object[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")+"___Telnet OK..."+Environment.NewLine });

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            richTextBox1.Text = "";
            telnet_receiving = false;
        }
    }
}
