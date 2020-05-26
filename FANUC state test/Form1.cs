using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YCM;
using System.Threading;
using System.IO;

namespace FANUC_state_test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //button1_Click(null, null);

        }


        private void button1_Click(object sender, EventArgs e)
        {
            if(backgroundWorker1.IsBusy)
            {

            }
            else
            {
                a = 1;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        int a = 0;
        short a1 = 0,b=0;
        string prostartno;
        Focas1.ODBERR err = new Focas1.ODBERR();
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            ushort cnc = 0;
            Focas1.IODBPMC fPMC = new Focas1.IODBPMC();            
            char[] F1_bit = new char[8];
            while (a == 1)
            {
                int aa = Focas1.cnc_allclibhndl3(textBox1.Text, 8193, 1, out cnc);
                aa= Focas1.cnc_rdetherinfo(cnc, out a1, out b);
                if (aa == 0)
                {
                    while (a == 1)
                    {
                        try
                        {
                            short ret = Focas1.pmc_rdpmcrng(cnc, 1, 0, 0, 1, 10, fPMC);
                            Get_Operation_Mode(cnc);
                            prostartno = fPMC.cdata[0].ToString();//如果數值F0 = 224 or 208 => START鍵啟動 OR FEED HOLD暫停
                        }
                        catch
                        { }
                        backgroundWorker1.ReportProgress(0);
                        Thread.Sleep(50);
                    }
                }
                else if(aa==-17)
                {
                    //Focas1.ODBPMCERR pmc_err = new Focas1.ODBPMCERR();
                    Focas1.cnc_getdtailerr(cnc, err);
                    //Focas1.pmc_getdtailerr(cnc, pmc_err);
                    DMMLog(err.err_no.ToString() + "  " + err.err_dtno.ToString(),textBox1);
                }
                else 
                {
                    Thread.Sleep(5000);
                }
            }
        }

        string als = "";
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (prostartno != als)
            {
                switch (prostartno)
                {
                    case "224":
                        textBox2.Text += (prostartno + " " + System_Operation_Modal + Environment.NewLine);
                        //DMMLog("----" + als + " " + System_Operation_Modal + "  [[" + textBox1.Text + "]]", textBox1);
                        break;
                    case "64":
                        textBox2.Text += (prostartno + " " + System_Operation_Modal + Environment.NewLine);
                        //DMMLog("----" + als + " " + System_Operation_Modal + "  [[" + textBox1.Text + "]]", textBox1);
                        break;
                    case "192":
                        textBox2.Text += (prostartno + " " + System_Operation_Modal + Environment.NewLine);
                        //DMMLog("----" + als + " " + System_Operation_Modal + "  [[" + textBox1.Text + "]]", textBox1);
                        break;
                    default:
                        textBox2.Text += (prostartno + " e" + " " + System_Operation_Modal + Environment.NewLine);
                        //DMMLog("----" + als + " " + System_Operation_Modal + "e  [[" + textBox1.Text + "]]", textBox1);
                        break;

                }
                als = prostartno;               
            }
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        string System_Operation_Modal = "";
        Focas1.ODBST cncStatus = new Focas1.ODBST();
        public void Get_Operation_Mode(ushort cncHandle_s)
        {
            //取得模式是否EDIT

            int ret = Focas1.cnc_statinfo(cncHandle_s, cncStatus);
            short Mode = cncStatus.aut;

            switch (Mode)
            {
                case 0:
                    System_Operation_Modal = "MDI";
                    break;
                case 1:
                    System_Operation_Modal = "MEM";
                    break;
                case 9:
                    System_Operation_Modal = "REF";
                    break;
                case 10:
                    System_Operation_Modal = "REF";
                    break;
                case 3:
                    System_Operation_Modal = "EDIT";
                    break;
                case 4:
                    System_Operation_Modal = "HND";
                    break;
                case 5:
                    System_Operation_Modal = "JOG";
                    break;
                default:
                    break;
            }
            //取得模式是否EDIT
            //0 : MDI 
            //1 : MEMory 
            //2 : **** 
            //3 : EDIT 
            //4 : HaNDle 
            //5 : JOG 
            //6 : Teach in JOG 
            //7 : Teach in HaNDle 
            //8 : INC·feed 
            //9 : REFerence 
            //10 : ReMoTe 
        }


        private void button2_Click(object sender, EventArgs e)
        {
            a = 0;
            textBox2.Text = "";
            als = "";
        }

        static object lockLog = new object();
        //Write Log 
        public static void DMMLog(string txt,TextBox tb)
        {
            //今日日期
            DateTime Date = DateTime.Now;
            string TodyTime = Date.ToString("yyyy-MM-dd HH:mm:ss");
            string Tody = Date.ToString("yyyy-MM-dd");

            //檢查此路徑有無資料夾
            if (!Directory.Exists(System.Environment.CurrentDirectory + "\\Log"+tb.Text))
            {
                //新增資料夾
                Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\Log" + tb.Text);
            }
            lock (lockLog)
            {
                //把內容寫到目的檔案，若檔案存在則附加在原本內容之後(換行)
                File.AppendAllText(System.Environment.CurrentDirectory + "\\Log" + tb.Text + "\\Log" + Tody + ".txt", "\r\n" + TodyTime + "：" + txt);
            }
        }
    }
}
