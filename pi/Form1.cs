using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pi
{
    public partial class Form1 : Form
    {

        private UdpClient udpcSend;


        public Form1()
        {
            InitializeComponent();
        }


        private void SendMessage(object obj)
        {
            string message = (string)obj;
            byte[] sendbytes = Encoding.Unicode.GetBytes(message);

            // 匿名发送
            udpcSend = new UdpClient(0);             // 自动分配本地IPv4地址
            IPEndPoint remoteIpep = new IPEndPoint(
                IPAddress.Parse(this.textBox1.Text), 8848); // 发送到的IP地址和端口号

            udpcSend.Send(sendbytes, sendbytes.Length, remoteIpep);
            udpcSend.Close();
            Thread.Sleep(2);
            ResetTextBox("成功...");

            if (message == "getpic") {
                UpdatePicture();
            }
        }
        public void DownloadFile(string URL, string filename, System.Windows.Forms.ProgressBar prog, System.Windows.Forms.Label label1)
        {
            float percent = 0;
            try
            {
                System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
                Myrq.Timeout = 5;
                System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
                if (prog != null)
                {
                    prog.Maximum = (int)totalBytes;
                }
                System.IO.Stream st = myrp.GetResponseStream();
                System.IO.Stream so = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                long totalDownloadedByte = 0;
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, (int)by.Length);
                while (osize > 0)
                {
                    totalDownloadedByte = osize + totalDownloadedByte;
                    System.Windows.Forms.Application.DoEvents();
                    so.Write(by, 0, osize);
                    if (prog != null)
                    {
                        prog.Value = (int)totalDownloadedByte;
                    }
                    osize = st.Read(by, 0, (int)by.Length);

                    percent = (float)totalDownloadedByte / (float)totalBytes * 100;
                    label1.Text = "当前下载进度" + percent.ToString() + "%";
                    System.Windows.Forms.Application.DoEvents(); //必须加注这句代码，否则label1将因为循环执行太快而来不及显示信息
                }
                so.Close();
                st.Close();
            }
            catch (System.Exception)
            {
                this.richTextBox1.AppendText("获取图片失败\n");
            }
        }
    

        private void button1_Click(object sender, EventArgs e)
        {

            String commond = "getpic";
 
            Thread thrSend = new Thread(SendMessage);
            thrSend.Start(commond);
            this.richTextBox1.Text = "获取图片中... \r\n请等待\r\n";


            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String commond = "reboot";

            Thread thrSend = new Thread(SendMessage);
            thrSend.Start(commond);
            this.richTextBox1.Text = "设备重启中...\r\n";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            String commond = "seq";

            if (this.radioButton2.Checked == true){
                commond = "sync";
            } else if (this.radioButton3.Checked == true) {
                commond = this.textBox2.Text;
            }

            Thread thrSend = new Thread(SendMessage);
            thrSend.Start(commond);
            this.richTextBox1.Text = "发送GPIO模式...\r\n";
        }

        delegate void ResetTextBoxDelegate(String text);
        private void ResetTextBox(String text)
        {
            if (this.richTextBox1.InvokeRequired)
            {
                ResetTextBoxDelegate resetTextBoxDelegate = ResetTextBox;
                this.richTextBox1.Invoke(resetTextBoxDelegate, new object[] { text});
            }
            else
            {
                this.richTextBox1.Text = text;
            }
        }



        delegate void UpdatePictureDelegate();
        private void UpdatePicture()
        {
            if (this.pictureBox1.InvokeRequired)
            {
                UpdatePictureDelegate resetTextBoxDelegate = UpdatePicture;
                this.pictureBox1.Invoke(resetTextBoxDelegate, new object[] { });
            }
            else
            {
                DownloadFile("http://" + this.textBox1.Text + "/1.jpg", @"1.jpg", this.progressBar1, this.label3);
                //this.pictureBox1.Image = Image.FromFile(@"1.jpg");
            }
        }

    }
}
