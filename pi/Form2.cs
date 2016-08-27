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
using System.IO;

namespace pi
{
    public partial class Form2 : Form
    {
        public static bool aa = true;
        private UdpClient udpcSend;

        public Form2()
        {
            InitializeComponent();
            timer1 = new System.Windows.Forms.Timer() { };
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 3800;
        }

        private void 退出EToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SendMessage(object obj)
        {
            string message = (string)obj;
            byte[] sendbytes = Encoding.Unicode.GetBytes(message);

            // 匿名发送
            udpcSend = new UdpClient(0);             // 自动分配本地IPv4地址
            IPEndPoint remoteIpep = new IPEndPoint(
                IPAddress.Parse(this.textBox1.Text), 8849); // 发送到的IP地址和端口号

            udpcSend.Send(sendbytes, sendbytes.Length, remoteIpep);
            udpcSend.Close();
            if (message == "a")
            {
                UpdatePicture();
            }
        }



        public bool Ping(string ip)
        {
            System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
            System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions();
            options.DontFragment = true;
            string data = "Test Data!";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 1000; // Timeout 时间，单位：毫秒  
            System.Net.NetworkInformation.PingReply reply = p.Send(ip, timeout, buffer, options);
            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                return true;
            else
                return false;
        }  

        public int DownloadFile(string URL, string filename, System.Windows.Forms.ToolStripProgressBar prog, System.Windows.Forms.ToolStripLabel label1)
        {
            this.toolStripProgressBar1.Visible = true;
            float percent = 0;
            System.Net.HttpWebRequest Myrq;
            System.Net.HttpWebResponse myrp;
            try
            {
                Myrq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
                Myrq.Timeout = 2000;
                myrp = (System.Net.HttpWebResponse)Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
                if (prog != null)
                {
                    prog.Maximum = (int)totalBytes;
                }
                System.IO.Stream st = myrp.GetResponseStream();
                System.IO.Stream so = new System.IO.FileStream(filename, System.IO.FileMode.OpenOrCreate);
                long totalDownloadedByte = 0;
                byte[] by = new byte[102400];
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
                    label1.Text = "当前下载进度" + percent.ToString("F2") + "%";
                    System.Windows.Forms.Application.DoEvents(); //必须加注这句代码，否则label1将因为循环执行太快而来不及显示信息
                }
                so.Close();
                st.Close();
 
                return 0;
            }
            catch (System.Exception)
            {
                return -1;
            }
        }

        private void ResetTextBox(String text)
        {
            toolStripStatusLabel1.Text = text;
        }

        private void UpdatePicture()
        {
            if (this.pictureBox1.InvokeRequired)
            {
                UpdatePictureDelegate resetTextBoxDelegate = UpdatePicture;
                this.pictureBox1.Invoke(resetTextBoxDelegate, new object[] { });
            }
            else
            {
                this.toolStripStatusLabel1.Text = "获取中";
                int ret = DownloadFile("http://" + this.textBox1.Text + "/1.jpg", @"1.jpg", this.toolStripProgressBar1, this.toolStripStatusLabel1);
                if (ret == 0)
                {
                    
                    FileStream fs = File.OpenRead(@"1.jpg");
                    this.pictureBox1.Image = Image.FromStream(fs);
                    fs.Close();
                    this.toolStripStatusLabel1.Text = "获取成功";
                    this.toolStripProgressBar1.Visible = false;
                }
                else
                {
   
                    this.toolStripStatusLabel1.Text = "获取失败";
                }
            }
        }

        delegate void UpdatePictureDelegate();

        private void button1_Click(object sender, EventArgs e)
        {
            String commond = "aaaa";

            Thread thrSend = new Thread(SendMessage);
            thrSend.Start(commond);
            this.toolStripStatusLabel1.Text = "向上运动一个单位";
            this.toolStripProgressBar1.Value = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String commond = "aaaaa";

            Thread thrSend = new Thread(SendMessage);
            thrSend.Start(commond);
            this.toolStripStatusLabel1.Text = "向下运动一个单位";
            this.toolStripProgressBar1.Value = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            String commond = "aa";

            Thread thrSend = new Thread(SendMessage);
            thrSend.Start(commond);
            this.toolStripStatusLabel1.Text = "向左运动一个单位";
            this.toolStripProgressBar1.Value = 0;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            String commond = "aaa";

            Thread thrSend = new Thread(SendMessage);
            thrSend.Start(commond);
            this.toolStripStatusLabel1.Text = "向右运动一个单位";
            this.toolStripProgressBar1.Value = 0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (button5.Text== "开始获取图像")
            {
                timer1.Enabled = true;
                button5.Text = "停止获取图像";
                this.toolStripStatusLabel1.Text = "就绪";
            }
            else
            {
                timer1.Enabled = false;
                button5.Text = "开始获取图像";
                this.toolStripStatusLabel1.Text = "就绪";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            String commond = "a";

            Thread thrSend = new Thread(SendMessage);
            thrSend.Start(commond);
           
        }

        private void button6_Click(object sender, EventArgs e)
        {
            String sourcePath, targetPath;
            bool isrewrite=true;
            saveFileDialog1.Filter = "图片文件(*.jpg)|*.jpg";
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = "picture" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") +".jpg";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                targetPath = saveFileDialog1.FileName.ToString();
                sourcePath = @"1.jpg";

                System.IO.File.Copy(sourcePath, targetPath, isrewrite);
            }
        }

        private void 保存图片pToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String sourcePath, targetPath;
            bool isrewrite = true;
            saveFileDialog1.Filter = "图片文件(*.jpg)|*.jpg";
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = "picture" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".jpg";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                targetPath = saveFileDialog1.FileName.ToString();
                sourcePath = @"1.jpg";

                System.IO.File.Copy(sourcePath, targetPath, isrewrite);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                // 是否 Ping 的通  
                if (this.Ping(this.textBox1.Text))
                {
                    MessageBox.Show(this,"可以连接到该地址！","正确连接", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1); 
                }
                else
                    MessageBox.Show("无法连接到该地址，请检查：\n1.计算机的网络连接是否正常；\n2.树莓派是否正确开启并连接至网络；\n3.是否填写了正确的IP地址","无法连接",MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            }
            catch (Exception)
            {
                 MessageBox.Show("无法连接到该地址，请检查您计算机的软件配置","无法连接",MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            }  
        }

        private void button8_Click(object sender, EventArgs e)
        {
            String commond = textBox2.Text;

            Thread thrSend = new Thread(SendMessage);
            thrSend.Start(commond);
            this.toolStripStatusLabel1.Text = "发送自定义命令";
            this.toolStripProgressBar1.Value = 0;
        }

        private void 关于AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "机器鱼控制端程序\n\r版权所有 翻版必究\n\r(C)2016 国创项目组", "关于", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }
    }
}
